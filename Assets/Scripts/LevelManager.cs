using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking;
using Unity.Netcode;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager instance;

    int ControlType; // 0 PC, 1 Android, 2 web

    [SerializeField] GameObject DpadParent;
    public FloatingJoystick Dpad;
    public FloatingJoystick CamDpad;

    public bool LevelStarted;
    /// <summary>
    /// 0 if Chicken won, 1 if Butcher won
    /// </summary>
    public int WhoWon;
    public bool LevelOver;
    float LevelTimer;

    [SerializeField] GameObject PlayerBucket;
    
    [SerializeField] public Transform[] SpawnLocations;

    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        ControlType = GameManager.instance.DeviceType;
        if (ControlType == 0)
        {
            DpadParent.SetActive(false);
        }
        else if (ControlType == 1)
        {
            DpadParent.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.Confined;
        // Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
