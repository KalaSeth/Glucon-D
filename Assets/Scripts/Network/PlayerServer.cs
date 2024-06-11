using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerServer : NetworkBehaviour
{
    public LevelManager levelManager;

    [ServerRpc]
    public void PlayerReadyServerRpc()
    {
        levelManager.StartCountdown();
    }
}
