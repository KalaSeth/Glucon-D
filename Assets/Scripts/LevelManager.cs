using UnityEngine;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class LevelManager : NetworkBehaviour
{
    public float countdownDuration;
    private float countdownTimer;
    [SerializeField] private bool isCountdownActive;

    // Start the countdown when all players are ready
    public void StartCountdown()
    {
        if (IsServer)
        {
            // Check if all players are ready
            if (AreAllPlayersReady())
            {
                countdownTimer = countdownDuration;
                isCountdownActive = true;

                CountdownStartClientRpc();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountdownActive)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                isCountdownActive = false;
                DisplayMessageAndScore();
            }
        }
    }

    /// <summary>
    /// Method to check if all players are ready
    /// </summary>
    private bool AreAllPlayersReady()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            var playerObject = client.Value.PlayerObject;
            if (playerObject == null || !playerObject.GetComponent<PlayerManager>().IsReady)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ClientRpc to notify clients to start their countdown timers
    /// </summary>
    [ClientRpc]
    private void CountdownStartClientRpc()
    {
        // Level Start
        isCountdownActive = true;
    }

    /// <summary>
    /// Method to Reset the game and display message and score
    /// </summary>
    private void DisplayMessageAndScore()
    {
        // Level Over
        
    }
}
