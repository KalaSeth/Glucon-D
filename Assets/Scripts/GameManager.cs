using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int DeviceType; //  0 PC, 1 Android, 2 web,

    [SerializeField] string[] PlayerNameList;
    public string PlayerName;

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
        RandomPlayerNameGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RandomPlayerNameGenerator()
    {
        string name = (PlayerNameList[Random.Range(0, 6)] + "_" + Random.Range(2, 99)).ToString();
        PlayerName = name;
    }
}
