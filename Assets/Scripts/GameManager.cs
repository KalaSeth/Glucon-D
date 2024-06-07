using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int DeviceType; //  0 PC, 1 Android, 2 web,

    public void Awake()
    {
        instance = this;
        
          if (SystemInfo.deviceType == UnityEngine.DeviceType.Desktop)
          {
              DeviceType = 0;
          }else if (SystemInfo.deviceType == UnityEngine.DeviceType.Handheld)
          {
              DeviceType = 1;
          }
          else if(SystemInfo.deviceType == UnityEngine.DeviceType.Console)
          {
              DeviceType = 2;
          }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
