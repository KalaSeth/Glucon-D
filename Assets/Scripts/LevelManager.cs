using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

public class LevelManager : NetworkBehaviour
{
    public float countdownDuration = 10800;
    private float countdownTimer;
    private bool isCountdownActive = false;
    private int readyPlayerCount = 0;

    // List to keep track of players
    private List<PlayerReadinessHandler> players = new List<PlayerReadinessHandler>();

    private void Start()
    {
        if (IsServer)
        {
            // Register client connection callback
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        var playerReadinessHandler = playerObject.GetComponent<PlayerReadinessHandler>();
        if (playerReadinessHandler != null)
        {
            players.Add(playerReadinessHandler);
            playerReadinessHandler.OnPlayerReady.AddListener(OnPlayerReady);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerServerRpc(ulong playerId)
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId];
        var playerReadinessHandler = playerObject.GetComponent<PlayerReadinessHandler>();
        if (playerReadinessHandler != null)
        {
            players.Add(playerReadinessHandler);
            playerReadinessHandler.OnPlayerReady.AddListener(OnPlayerReady);
        }
    }

    public void OnPlayerReady()
    {
        Debug.Log("Level Readuy");
        readyPlayerCount++;
        if (readyPlayerCount == players.Count)
        {
            Debug.Log("Count Set");
            StartCountdown();

        }
    }

    // Start the countdown when all players are ready
    private void StartCountdown()
    {
        Debug.Log("Starting Timer");
        if (IsServer)
        {
            countdownTimer = countdownDuration;
            isCountdownActive = true;

            // Notify clients to start their countdown timers
            CountdownStartClientRpc();

            // Set IsCountdownRunning to true for all players
            foreach (var player in players)
            {
                player.SetCountdownRunning(true);
                
            }


        }
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (isCountdownActive)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                isCountdownActive = false;
                DisplayMessageAndScore();

                // Set IsCountdownRunning to false for all players
                foreach (var player in players)
                {
                    player.SetCountdownRunning(false);
                }
            }
        }
    }

    public void OnPlayerDied(ulong playerId)
    {
        players.RemoveAll(p => p.NetworkObjectId == playerId); // Remove dead players from the list

        if (players.Count == 1)
        {
            // The last alive player is the winner
            var winner = players[0];
            AnnounceWinner(winner);
        }
    }

    private void AnnounceWinner(PlayerReadinessHandler winner)
    {
        // Announce the winner logic here
        Debug.Log("The winner is: " + winner.NetworkObjectId);

        // Despawn and destroy the winner's object
        winner.GetComponent<PlayerDeathHandler>().Die();
    }

    // ClientRpc to notify clients to start their countdown timers
    [ClientRpc]
    private void CountdownStartClientRpc()
    {
        Debug.Log("Starting Timer RPC Call");
        // Set IsCountdownRunning to true for all players
        foreach (var player in players)
        {
            player.SetCountdownRunning(true);
        }
       
    }

    // Method to display message and score
    private void DisplayMessageAndScore()
    {
        // Display message and score logic here
        Debug.Log("Timer finished! Displaying message and score.");
    }
}
