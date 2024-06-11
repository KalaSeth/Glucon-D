using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    /// <summary>
    /// Network variable for synchronizing health between server and clients
    /// </summary>
    private NetworkVariable<int> health = new NetworkVariable<int>(100);

    /// <summary>
    /// Initialize health when the player spawns
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 100;
        }

        // Subscribe to health value changes
        health.OnValueChanged += OnHealthChanged;
    }

    /// <summary>
    /// Method called when health value changes
    /// </summary>
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (newHealth <= 0)
        {
            DespawnAndDestroyServerRpc();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.CompareTag("Anda"))
        {
            NetworkObject andaNetworkObject = other.gameObject.GetComponent<NetworkObject>();

            if (andaNetworkObject != null && andaNetworkObject.OwnerClientId != OwnerClientId)
            {
                health.Value -= 10;
            }
        }
    }

    #region Destroy and Despawn
    /// <summary>
    /// Method to despawn and destroy player object
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void DespawnAndDestroyServerRpc()
    {
        if (!IsServer) return;

        NetworkObject.Despawn(true);

        Destroy(gameObject);
    }

    /// <summary>
    /// Unsubscribe from health changes when object is destroyed
    /// </summary>
    public override void OnNetworkDespawn()
    {
        if (health != null)
        {
            health.OnValueChanged -= OnHealthChanged;
        }
    }
    #endregion
}
