using UnityEngine;
using Unity.Networking;
using Unity.Netcode;

public class CameraController : NetworkBehaviour
{
    private void Update()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
        }
    }
}
