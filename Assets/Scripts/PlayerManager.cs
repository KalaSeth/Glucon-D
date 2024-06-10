using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    /// <summary>
    /// Class of player O for Murgi, 1 for Butcher
    /// </summary>
    [SerializeField] private NetworkVariable<int> PlayerCast = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    public int Playerclass;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField] private NetworkVariable<bool> isReady = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    int Clintid;
    private int ClientCount;
    int Index;
    /// <summary>
    /// Total Client Count in current session.
    /// </summary>
    [SerializeField] private NetworkVariable<int> Ccount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    /// <summary>
    /// Current Halal Index
    /// </summary>
    [SerializeField] public NetworkVariable<int> CurrentCast = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    [SerializeField] GameObject PlayerPrefab;
    Button RD1Button;
    Button AtankButton;

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

        AtankButton = GameObject.Find("Atank").GetComponent<Button>();
        AtankButton.onClick.AddListener(() =>
        {
            //AssignPlayerCast();
        });
        
    }
    // Update is called once per frame
    void Update()
    {
        
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
        Index = (int)OwnerClientId;
        
        GameObject newPlayer = Instantiate(PlayerPrefab, LevelManager.instance.SpawnLocations[Index].transform.position, LevelManager.instance.SpawnLocations[Index].rotation); 
        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        
        // NetworkManager.SceneManager.LoadScene("Level",LoadSceneMode.Single);
    }


}
