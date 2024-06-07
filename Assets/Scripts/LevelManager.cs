using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    int ControlType; // 0 PC, 1 Android, 2 web

    [SerializeField] GameObject DpadParent;
    public FloatingJoystick Dpad;
    public FloatingJoystick CamDpad;

    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
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
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
