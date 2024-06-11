using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    int Index;
    public bool IsReady { get; private set; }

    [SerializeField] GameObject PlayerPrefab;
    Button RD1Button;
    Button readyButton;

    public override void OnNetworkSpawn()
    {
        RD1Button = GameObject.Find("ReadyP1").GetComponent<Button>();
        RD1Button.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            LoadLevelServerRpc();
        });

        readyButton = GameObject.Find("Atank").GetComponent<Button>();
        readyButton.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            PressReadyButton();
        });
    }

    [ServerRpc]
    private void LoadLevelServerRpc()
    {
        Index = (int)OwnerClientId;

        GameObject newPlayer = Instantiate(PlayerPrefab, GameManager.instance.SpawnLocations[Index].transform.position, GameManager.instance.SpawnLocations[Index].rotation);
        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }

    public void PressReadyButton()
    {
        if (!IsReady)
        {
            IsReady = true;
            GetComponent<PlayerServer>().PlayerReadyServerRpc();
        }
    }
}
