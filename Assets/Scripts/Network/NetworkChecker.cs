using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkChecker : MonoBehaviour
{
    public static NetworkChecker instance;

    public bool isOnline;

    public void Awake()
    {
        instance = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        NetCheck();
    }

    void NetCheck()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isOnline = false;
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            isOnline = true;
        }
    }
}
