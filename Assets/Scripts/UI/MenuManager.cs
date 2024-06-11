using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Unity.Netcode;

public class MenuManager : NetworkBehaviour
{
    public static MenuManager instance;

    [SerializeField] public GameObject WaitinginJoin;
    [SerializeField] public GameObject LoadingSomething;

    [SerializeField] public GameObject ReadyButton;
    [SerializeField] public GameObject SpawnButton;

    [SerializeField] public GameObject MainMenu;
    [SerializeField] public GameObject OnlineMenu;

    [SerializeField] public GameObject ImgPanel;
    [SerializeField] public GameObject HostPanel;
    [SerializeField] public GameObject JoinPanel;

    [SerializeField] public GameObject LobbyPanel;

    [SerializeField] public Text LobbyPlayers;
    public int LobbyCount;



    public void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        LobbyPlayers.text = LobbyCount.ToString();

    }

    public void MainMenuState()
    {
        WaitinginJoin.SetActive(false);
        LoadingSomething.SetActive(false);

        ReadyButton.SetActive(false);
        SpawnButton.SetActive(true);

        MainMenu.SetActive(true);
        OnlineMenu.SetActive(false);

        ImgPanel.SetActive(true);
        HostPanel.SetActive(false);
        JoinPanel.SetActive(false);

        LobbyPanel.SetActive(false);

    }

    public void ErrorOnJoin()
    {
        LoadingSomething.SetActive(false);
    }

    public void GotIn()
    {
        WaitinginJoin.SetActive(false);
        LoadingSomething.SetActive(false);

        ReadyButton.SetActive(false);
        SpawnButton.SetActive(true);

        MainMenu.SetActive(false);
        OnlineMenu.SetActive(false);

        ImgPanel.SetActive(false);
        HostPanel.SetActive(false);
        JoinPanel.SetActive(false);

        LobbyPanel.SetActive(false);
    }

    public void Aboutus()
    {
        Application.OpenURL("https://zherblast.com");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
