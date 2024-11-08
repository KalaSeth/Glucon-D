using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIforMenu : MonoBehaviour
{   
    // Join Relay
    [SerializeField] InputField JoinCodeTextInput;

    // Generated for UI
    [SerializeField] Text JoinCodeText;
    [SerializeField] Text LobbyCodeText;

    //Create Lobby
    [SerializeField] InputField LobbyNameTextInput;
    [SerializeField] Slider MaxPlayer;
    private bool IsPublicBool;

    // Join Lobby
    [SerializeField] InputField LobbyCodeTextInput;


    private void Update()
    {
        JoinCodeText.text = "Join Code : " + NetworkRelayConnectionHandler.Instace.GeneratedJoinCode;
        LobbyCodeText.text = "Join Code : " + NetworkRelayConnectionHandler.Instace.GeneratedLobbyCode;
    }

#region On Click Lobby Actions
    public void OnClickCreateLobby()
    {
        NetworkRelayConnectionHandler.Instace.CreateLobby(GameManager.instance.PlayerName, 10, IsPublicBool);
    }

    public void OnClickListLobbies()
    {
        NetworkRelayConnectionHandler.Instace.ListLobbies();
    }

    public void OnClickJoinLobby()
    {
        NetworkRelayConnectionHandler.Instace.JoinLobbybyCode(LobbyCodeTextInput.text.ToString());
    }

    public void OnClickQuickJoinLobby()
    {
        NetworkRelayConnectionHandler.Instace.QuickjoinLobby();
    }

    public void OnClickLobbyDetails()
    {
        NetworkRelayConnectionHandler.Instace.PrintPlayerList();
    }

    public void OnClickLeaveLobby()
    {
        NetworkRelayConnectionHandler.Instace.LeaveLobby();
    }

    public void OnClickKickPlayer()
    {
        NetworkRelayConnectionHandler.Instace.KickPlayer();
    }

    public void OnClickDeleteLobby()
    {
        NetworkRelayConnectionHandler.Instace.DeleteLobby();
    }

    public void OnClickMigrateHost()
    {
        NetworkRelayConnectionHandler.Instace.MigrateLobbyHost();
    }

    #endregion

#region On Click Relay Actions
    public void OnClickCreateServer()
    {
        NetworkRelayConnectionHandler.Instace.StartGameViaRelay();
    }

    public void OnClickJoinServer()
    {
        NetworkRelayConnectionHandler.Instace.JoinRelay(JoinCodeTextInput.text);
    }

    public void OnClickDisconnectServer()
    {
        NetworkRelayConnectionHandler.Instace.DisconnectRelay();
    }
    #endregion

}
