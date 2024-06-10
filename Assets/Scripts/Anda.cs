using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking;
using Unity.Netcode;

public class Anda : NetworkBehaviour
{
    [SerializeField] Rigidbody AndaRB;
    [SerializeField] float Magnitude;

    // Start is called before the first frame update
    public void Start()
    {
        AndaRB.AddForce(transform.forward * Magnitude, ForceMode.Impulse);
    }

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
        if (IsServer)
        {
            DespawnAndDestroyServerRpc();
        }
    }
}
