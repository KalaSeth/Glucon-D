using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerReadinessHandler : NetworkBehaviour
{
    public Button readyButton;
    public UnityEvent OnPlayerReady = new UnityEvent();

    private bool isReady = false;
    private bool isCountdownRunning = false;
    private LevelManager levelManager;

    public override void OnNetworkSpawn()
    {
        readyButton = readyButton = MenuManager.instance.SpawnButton.GetComponent<Button>();

        if (IsOwner)
        {
            readyButton.onClick.AddListener (() =>
            {
                Debug.Log("Pressed");
                PressReadyButton();
            });
        }

        // Find the LevelManager in the scene
        levelManager = FindObjectOfType<LevelManager>();
        if (IsServer)
        {
            levelManager.AddPlayerServerRpc(NetworkObjectId);
        }
    }

    public void PressReadyButton()
    {
        if (!isReady)
        {
            isReady = true;
            Debug.Log("Ready");
            NotifyReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyReadyServerRpc()
    {
        levelManager.OnPlayerReady();
    }

    public void SetCountdownRunning(bool value)
    {
        isCountdownRunning = value;

        Debug.Log("Counting");

        if (isCountdownRunning == true)
        {
            Debug.Log("Counting xx");
            if (MenuManager.instance.SpawnButton.GetComponent<Button>().interactable == false)
            {
                Debug.Log("Counting xxx");
                MenuManager.instance.SpawnButton.GetComponent<Button>().interactable = true;
            }
        }
    }
}

