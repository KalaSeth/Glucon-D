using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class NetworkRelayConnectionHandler : MonoBehaviour
{
    public static NetworkRelayConnectionHandler Instace;

    private string playername;
    private Lobby hostlobby;
    private Lobby joinedlobby;
    private float Heartbeattimer;
    private float LobbyUpdateTimer;
    [SerializeField] private float HeartRate;
    public string GeneratedJoinCode;
    public string GeneratedLobbyCode;

    public int MaxPlayerLimit;
    public bool isLobbyHost;

    private void Awake()
    {
        Instace = this;
    }

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {

        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        PlayernameAssigner();
    }

    private void Update()
    {
        Heartbeater();
        LobbyPollUpdate();
        PlayernameAssigner();
    }

    #region Lobby

    private void PlayernameAssigner()
    {
        playername = GameManager.instance.PlayerName;
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,playername)}

            }
        };
    }

    private async void Heartbeater()
    {
        if (hostlobby != null)
        {
            Heartbeattimer -= Time.deltaTime;
            if (Heartbeattimer <= 0)
            {
                Heartbeattimer = HeartRate;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostlobby.Id);
            }
        }
    }

    private async void LobbyPollUpdate()
    {
        if (joinedlobby != null)
        {
            LobbyUpdateTimer -= Time.deltaTime;
            if (LobbyUpdateTimer <= 0)
            {
                LobbyUpdateTimer = 1.2f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedlobby.Id);
                joinedlobby = lobby;
                 
                if (AuthenticationService.Instance.PlayerId != lobby.HostId)
                {
                    isLobbyHost = false;
                }
                else if(AuthenticationService.Instance.PlayerId == lobby.HostId)
                {
                    isLobbyHost = true;
                }
                
                if (joinedlobby.Data["Startkey"].Value != "0")
                {
                    if (isLobbyHost != true)
                    {
                        JoinRelay(joinedlobby.Data["Startkey"].Value);
                        Debug.Log("Relay Secured, [Exiting Lobby --> Connecting Relay]");
                    }
                    joinedlobby = null;

                    MenuManager.instance.GotIn();
                }

                MenuManager.instance.LobbyCount = lobby.Players.Count;
            }
        }
    }

    public async void CreateLobby(string Lobbyname, int MaxPlayer, bool isPrivate)
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode",new DataObject(DataObject.VisibilityOptions.Public, "Mode 1") },
                    {"Startkey", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(Lobbyname, MaxPlayer, createLobbyOptions);
            MaxPlayerLimit = MaxPlayer;
            hostlobby = lobby;
            joinedlobby = hostlobby;
            PrintPlayerData(hostlobby);

            GeneratedLobbyCode = lobby.LobbyCode;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>()
                {

                },
                Order = new List<QueryOrder>()
                {
                    new QueryOrder(true,QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + "__" + lobby.MaxPlayers + " " + lobby.Players.Count);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbybyCode(string lobbycode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbycode, joinLobbyByCodeOptions);
            joinedlobby = lobby;
            Debug.Log("Joined Lobby");
            PrintPlayerData(lobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void QuickjoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };
            Lobby quickjoinedlobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
            joinedlobby = quickjoinedlobby;
            Debug.Log("Quick Joined Lobby");
            PrintPlayerData(quickjoinedlobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            MenuManager.instance.ErrorOnJoin();
        }
    }

    public async void UpdateLobbyGameMode(string gamemode)
    {
        try
        {
            hostlobby = await Lobbies.Instance.UpdateLobbyAsync(hostlobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gamemode) }
                }
            });

            joinedlobby = hostlobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void MigrateLobbyHost()
    {
        try
        {
            hostlobby = await Lobbies.Instance.UpdateLobbyAsync(hostlobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedlobby.Players[1].Id
            });

            joinedlobby = hostlobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    public async void LeaveLobby()
    {
        try
        {
            if (isLobbyHost == true)
            {
                MigrateLobbyHost();
            }
            
            await LobbyService.Instance.RemovePlayerAsync(joinedlobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedlobby.Id, joinedlobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayerData(string newplayername)
    {
        try
        {
            playername = newplayername;
            await LobbyService.Instance.UpdatePlayerAsync(joinedlobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                { 
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playername) } 
                }
            });
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void PrintPlayerData(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name + " " + lobby.Players.Count);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    public void PrintPlayerList()
    {
        Debug.Log("Players in Lobby " + joinedlobby.Name + " " + joinedlobby.Players.Count);
        Debug.Log("Host = " + joinedlobby.HostId);
        foreach (Player player in joinedlobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }

    public async void StartGameViaRelay()
    {
        if (isLobbyHost)
        {
            try
            {
                string relaycode = await CreateRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedlobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                       {"Startkey",new DataObject(DataObject.VisibilityOptions.Member, relaycode) }
                    }
                });
                joinedlobby = lobby;
                Debug.Log("StartKey " + joinedlobby.Data["Startkey"].Value);
                Debug.Log("RelayCode " + relaycode);
                
            }

            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedlobby.Id);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    #endregion

    #region Relay 
    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayerLimit, "asia-south1");
            GeneratedJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            return GeneratedJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async void JoinRelay(string Joincode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(Joincode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            Debug.Log("Joined Relay");
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void DisconnectRelay()
    {
        try
        {
            if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Disconnected from Relay and closed Netcode connection as Client");
            }
            else if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("Disconnected from Relay and closed Netcode connection as Host");
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            MenuManager.instance.MainMenuState();
        }
    }
    #endregion
}
