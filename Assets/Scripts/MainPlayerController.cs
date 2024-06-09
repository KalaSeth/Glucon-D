using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;


public class MainPlayerController : NetworkBehaviour
{
    /// <summary>
    /// 0 for Chicken, 1 for butcher
    /// </summary>
    [SerializeField] public int PlayerClass;
    [SerializeField] public GameObject PlayerObjectClient;

    /// <summary>
    /// 0 PC, 1 Android, 2 web
    /// </summary>
    int ControlType;
    [SerializeField] CinemachineVirtualCamera ChickenVirCam;
    [SerializeField] CinemachineVirtualCamera ButcherVirCam;
    [SerializeField] NavMeshAgent PlayerNavi;

    [SerializeField] GameObject ChickenMesh;
    [SerializeField] GameObject ButcherMesh;

    float HorValue, VarValue;

    FloatingJoystick DPad;
    FloatingJoystick CamDPad;

    [SerializeField] float ChickenHealth;
    [SerializeField] float ButcherHealth;
    float Health;

    [SerializeField] float ChickenSpeed;
    [SerializeField] float ButcherSpeed;
    float Speed;

    // Kudna jaruri hai kya?
    //[SerializeField] float ChickenJumpForce;
    //[SerializeField] float ButcherJumpForce;
    //float JumpForce;

    [SerializeField] float RotSensi;
    float RotSpeedX, RotSpeedY;

    [SerializeField] GameObject CamRoot;
    [SerializeField] GameObject Anada;
    [SerializeField] GameObject AnadaRoot;
    [SerializeField] GameObject Anadabin;

    bool CanShoot;
    float ShootCooldown;
    float ShootTimer;
    [SerializeField] float ChickenShootTimer;
    [SerializeField] float ButcherShootTimer;

    bool LevelStarted;
    bool isDead;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        MeshToggle();
        ShootCooldown = ShootTimer;
        Anadabin = GameObject.Find("AndaBin");

        if (!IsOwner) return;
        ControlType = GameManager.instance.DeviceType;
        if (ControlType == 1)
        {
            Inputsthings();
        }
        VirtualCamSwitch();
       
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (ShootCooldown <= ShootTimer)
        {
            ShootCooldown -= Time.deltaTime;
            if (ShootCooldown <= 0)
            {
                ShootCooldown = 0;
                CanShoot = true;
            }
        }

        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerSkillCall();
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        PlayerMovement();
    }

    #region Virtual Player Methods
    /// <summary>
    /// Toggle between Butcher mesh an Chicken Mesh
    /// </summary>
    [ServerRpc]
    private void PlayerMeshToggleServerRpc()
    {
        PlayerMeshToggleClientRpc();
    }
    [ClientRpc]
    private void PlayerMeshToggleClientRpc()
    {
        SetPlayerMesh();
    }

    private void SetPlayerMesh()
    {
        PlayerClass = (int)OwnerClientId;//NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerManager>().Playerclass;
        Debug.Log(PlayerClass + " Class " + OwnerClientId);
        if (PlayerClass == 0)
        {
            Debug.Log(PlayerClass + " Murgi");
            ChickenMesh.SetActive(true);
            ButcherMesh.SetActive(false);

        }
        else if (PlayerClass == 1)
        {
            Debug.Log(PlayerClass + " Halal");
            ChickenMesh.SetActive(false);
            ButcherMesh.SetActive(true);
        }
    }

    private void MeshToggle()
    {
        if (IsServer)
        {
            PlayerMeshToggleClientRpc();
        }
        else
        {
            PlayerMeshToggleServerRpc();
        }
    }
    /// <summary>
    /// This method switches the camera, meshm stats and animation of player, depends upon the class.
    /// </summary>
    void VirtualCamSwitch()
    {
        if (PlayerClass == 0)
        {
            ChickenVirCam.gameObject.SetActive(true);
            ButcherVirCam.gameObject.SetActive(false);
            Health = ChickenHealth;

        }
        else if (PlayerClass == 1)
        {
            ChickenVirCam.gameObject.SetActive(false);
            ButcherVirCam.gameObject.SetActive(true);
            Health = ButcherHealth;
        }
    }
    /// <summary>
    /// This method checks the health of player.
    /// </summary>
    void HealthCheckforPlayer()
    {
        if (Health <= 1)
        {
            Health = 0;
            isDead = true;
            PlayerDed();
        }
    }
    /// <summary>
    /// This Method calls the death logic.
    /// </summary>
    void PlayerDed()
    {
        if (PlayerClass == 0)
        {
            MurgaDed();
        }
        else if (PlayerClass == 1)
        {
            ButcherDed();
        }
    }
    /// <summary>
    /// This Method runs the after death logic for Murgi.
    /// </summary>
    void MurgaDed()
    {

    }
    /// <summary>
    /// This Method runs the after death logic for Butcher.
    /// </summary>
    void ButcherDed()
    {

    }
    #endregion


    #region Movement Methods
    void PlayerMovement()
    {
        if (ControlType == 1)   // if Android
        {
            HorValue = DPad.Horizontal;
            VarValue = DPad.Vertical;

            RotSpeedY += CamDPad.Horizontal * RotSensi;
            RotSpeedX += CamDPad.Vertical * -RotSensi;
        }
        else
        {
            HorValue = Input.GetAxis("Horizontal");
            VarValue = Input.GetAxis("Vertical");

            RotSpeedX += Input.GetAxis("Mouse Y") * RotSensi;
            RotSpeedY += Input.GetAxis("Mouse X") * RotSensi;
        }

        // Player Class > 0 Chicken, 1 Butcher
        if (PlayerClass == 0)
        {
            Speed = ChickenSpeed;
            //JumpForce = ChickenJumpForce;
        }
        else if (PlayerClass == 1)
        {
            Speed = ButcherSpeed;
            //JumpForce = ButcherJumpForce;
        }

        PlayerNavi.speed = Speed;

        Vector3 newPos = transform.position + transform.right * HorValue + transform.forward * VarValue;
        PlayerNavi.destination = newPos;

       // transform.Translate(Vector3.forward * Speed * VarValue * Time.deltaTime);
       // transform.Translate(Vector3.right * Speed * HorValue * Time.deltaTime);

        transform.localEulerAngles = new Vector3(0 , RotSpeedY , 0);
    }
    /// <summary>
    /// Configure Dpad
    /// </summary>
    void Inputsthings()
    {
        DPad = LevelManager.instance.Dpad;
        CamDPad = LevelManager.instance.CamDpad;
        Debug.Log(DPad);
    }
    #endregion

    #region Skill Methods
    /// <summary>
    /// Murga and halal ke skill call karta hai, depends on the Player class.
    /// </summary>
    public void PlayerSkillCall()
    {
        Debug.Log("Got Input");
        if (isDead == false)
        {
            // 0 Chicken, 1 Butcher
            if (PlayerClass == 0)
            {
                ShootAndaServerRpc();
            }
            else if (PlayerClass == 1)
            {
                ButherKattaServerRpc();
            }
        }
    }
    /// <summary>
    /// Halal ke skill.
    /// </summary>
    [ServerRpc]
    void ButherKattaServerRpc()
    {
        if (CanShoot == true)
        {
            Debug.Log("ButcherSkill");
            ShootTimer = ButcherShootTimer;
            ShootCooldown = ShootTimer;
            CanShoot = false;
        }
    }
    /// <summary>
    /// Murga ke skill.
    /// </summary>
    [ServerRpc]
    private void ShootAndaServerRpc()
    {
        Debug.Log("Locking");
        if (CanShoot == true)
        {
            Debug.Log("Fire");
            GameObject anda = Instantiate(Anada, AnadaRoot.transform.position, AnadaRoot.transform.rotation, Anadabin.transform);
            anda.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            Destroy(anda, 4);
            ShootTimer = ChickenShootTimer;
            ShootCooldown = ShootTimer;
            CanShoot = false;
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Saw")
        {
            if (PlayerClass == 0)
            {
                Health--; // Make sure this does not match the value of butcher
                HealthCheckforPlayer();
            }
        }

        if (other.gameObject.tag == "Anda")
        {
            if (PlayerClass == 1)
            {
                Health--; // Make sure this does not match the value of Chicken
                HealthCheckforPlayer();
            }
        }
    }

}
