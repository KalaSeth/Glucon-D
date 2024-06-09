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

    /// <summary>
    /// 
    /// </summary>
    private NetworkVariable<bool> isReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    int Clintid;
    /// <summary>
    /// Total Client Count in current session.
    /// </summary>
    private NetworkVariable<int> Ccount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    /// <summary>
    /// Current Halal Index
    /// </summary>
    private NetworkVariable<int> CurrentCast = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] GameObject PlayerPrefab;
    Button RD1Button;

    public override void OnNetworkSpawn()
    {
        Clintid = (int)NetworkManager.Singleton.LocalClientId;

        isReady.OnValueChanged += (bool previousValue, bool newValue) =>
        {

        };

        RD1Button = GameObject.Find("ReadyP1").GetComponent<Button>();
        RD1Button.onClick.AddListener(() =>
        {
            if (!IsOwner) return;
            LoadLevelServerRpc();
        });
        AssignPlayerCast();
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
    #region PlayerID Check
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
        Ccount.Value = NetworkManager.ConnectedClients.Count;
        CurrentCast.Value = Random.Range(0,Ccount.Value);
        MakeHalal();
        Debug.Log(OwnerClientId + " CurrentCast " + CurrentCast + " PlayerCast " + PlayerCast);
        
    }
    private void MakeHalal()
    {
        if (CurrentCast.Value == (int)OwnerClientId)
        {
            PlayerCast.Value = 1;
        }
    }
    /// <summary>
    /// Assign a Hallal amog Murgi and sync it across server.
    /// </summary>
    private void AssignPlayerCast()
    {
       if (IsServer)
        {
            PlayeridCheckClientRpc();
        }
        else
        {
            PlayeridCheckClientRpc();
        }
    }
    #endregion
    
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
