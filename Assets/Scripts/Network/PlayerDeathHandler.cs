using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerDeathHandler : NetworkBehaviour
{
    private bool isAlive = true;
    private LevelManager levelManager;

    public override void OnNetworkSpawn()
    {
        // Find the LevelManager in the scene
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void Die()
    {
        if (isAlive)
        {
            isAlive = false;
            NotifyDeathServerRpc();
            NetworkRelayConnectionHandler.Instace.DisconnectRelay();
            Destroy(gameObject);  // Despawn and destroy the player
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyDeathServerRpc()
    {
        levelManager.OnPlayerDied(NetworkObjectId);
    }
}
