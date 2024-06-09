using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    /// <summary>
    /// Class of player O for Murgi, 1 for Butcher
    /// </summary>
    private NetworkVariable<int> PlayerCast = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public int Playerclass;

    private NetworkVariable<bool> isReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Write Client ID

    [SerializeField] GameObject PlayerPrefab;
    Button RD1Button;

    public override void OnNetworkSpawn()
    {
       // Update THIS 
       // Clintid = NetworkManager.Singleton.LocalClientId;

        isReady.OnValueChanged += (bool previousValue, bool newValue) =>
        {

        };

        RD1Button = GameObject.Find("ReadyP1").GetComponent<Button>();
        RD1Button.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            LoadLevelServerRpc();
        });

        PlayeridCheckServerRpc();
    }
    // Update is called once per frame
    void Update()
    {
        // LoadLevelClientRpc();
    }

    public void OnClickReady()
    {
        isReady.Value = !isReady.Value;
        return;
    }
    /// <summary>
    /// Check and assign Player Class to each client
    /// </summary>
    [ServerRpc]
    private void PlayeridCheckServerRpc()
    {
        PlayeridCheckClientRpc();
    }
    [ClientRpc]
    private void PlayeridCheckClientRpc()
    {
        PlayerIDCheck();
    }
    private void PlayerIDCheck()
    {
        // Update tHIS
        //int Ccount = NetworkManager.Cl
        if (((int)OwnerClientId) == 1)
        {
            PlayerCast.Value = 1;
        }
        Debug.Log(OwnerClientId + " PlayerCast" + PlayerCast);
    }
    /// <summary>
    /// Switch to gameplay for all clients
    /// </summary>
    public void OnClickSwitchTOGame()
    {
        //LoadLevelServerRpc();
    }

    [ServerRpc]
    private void LoadLevelServerRpc()
    {   
        Playerclass = PlayerCast.Value;
        Debug.Log("Switching to Level"); Debug.LogError("Spawn");
        GameObject newPlayer = Instantiate(PlayerPrefab, LevelManager.instance.SpawnLocations[Random.Range(0, LevelManager.instance.SpawnLocations.Length)].transform.position, LevelManager.instance.SpawnLocations[Random.Range(0, LevelManager.instance.SpawnLocations.Length)].rotation);

        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        

        // NetworkManager.SceneManager.LoadScene("Level",LoadSceneMode.Single);
    }


}
