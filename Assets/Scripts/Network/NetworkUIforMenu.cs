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
    [SerializeField] Toggle IsPublicToggle;
    private bool IsPublicBool;

    // Join Lobby
    [SerializeField] InputField LobbyCodeTextInput;


    private void Update()
    {
        JoinCodeText.text = NetworkRelayConnectionHandler.Instace.GeneratedJoinCode;
        LobbyCodeText.text = NetworkRelayConnectionHandler.Instace.GeneratedLobbyCode;
    }

#region On Click Lobby Actions
    public void OnClickCreateLobby()
    {
        NetworkRelayConnectionHandler.Instace.CreateLobby(LobbyNameTextInput.text.ToString(), (int)MaxPlayer.value, IsPublicBool);
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
#endregion




}
