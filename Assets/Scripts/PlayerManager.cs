using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    int Index;

    [SerializeField] GameObject PlayerPrefab;
    Button RD1Button;
    Button AtankButton;

    public override void OnNetworkSpawn()
    {
        RD1Button = GameObject.Find("ReadyP1").GetComponent<Button>();
        RD1Button.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            LoadLevelServerRpc();
        });

        AtankButton = GameObject.Find("Atank").GetComponent<Button>();
        AtankButton.onClick.AddListener(() =>
        {
            //AssignPlayerCast();
        });
    }

    [ServerRpc]
    private void LoadLevelServerRpc()
    {
        Index = (int)OwnerClientId;

        GameObject newPlayer = Instantiate(PlayerPrefab, LevelManager.instance.SpawnLocations[Index].transform.position, LevelManager.instance.SpawnLocations[Index].rotation);
        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}
