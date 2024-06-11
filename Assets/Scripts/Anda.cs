using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking;
using Unity.Netcode;

public class Anda : NetworkBehaviour
{
    [SerializeField] Rigidbody AndaRB;
    [SerializeField] float Magnitude;


    [ServerRpc(RequireOwnership = false)]
    private void DespawnAndDestroyServerRpc()
    {
        if (IsServer)
        {
            NetworkObject.Despawn(true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId == OwnerClientId) return;
        
        if (IsServer)
        {
            DespawnAndDestroyServerRpc();
        }
    }
}
