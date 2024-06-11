using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    int Index;

    [SerializeField] GameObject PlayerPrefab;
    public Button RD1Button;
    Button readyButton;

    public override void OnNetworkSpawn()
    {
        RD1Button = MenuManager.instance.ReadyButton.GetComponent<Button>();
        RD1Button.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            LoadLevelServerRpc();
        });

        readyButton = MenuManager.instance.SpawnButton.GetComponent<Button>();
        readyButton.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            SpawnPlayers();
        });
    }

    public void SpawnPlayers()
    {
        if (IsOwner)
        {
            LoadLevelServerRpc();
        }
    }

    [ServerRpc]
    private void LoadLevelServerRpc()
    {
        Index = (int)OwnerClientId;

        GameObject newPlayer = Instantiate(PlayerPrefab, GameManager.instance.SpawnLocations[Index].transform.position, GameManager.instance.SpawnLocations[Index].rotation);
        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }

    #region Player is Ready

    #endregion
}
