using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anda : MonoBehaviour
{
    [SerializeField] Rigidbody AndaRB;
    [SerializeField] float Magnitude;

    // Start is called before the first frame update
    void Start()
    {
        AndaRB.AddForce(transform.forward * Magnitude, ForceMode.Impulse);
       
        Destroy(gameObject, 4f);
    }
}
