using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using UnityEngine.UI;


public class MainPlayerController : NetworkBehaviour
{
    #region Variables initialization

    public int PlayerClass;
    
    [SerializeField] public GameObject PlayerObjectClient;

    /// <summary>
    /// 0 PC, 1 Android, 2 web
    /// </summary>
    int ControlType;
    [SerializeField] CinemachineVirtualCamera ChickenVirCam;
    [SerializeField] NavMeshAgent PlayerNavi;

    [SerializeField] GameObject ChickenMesh;

    float HorValue, VarValue;

    FloatingJoystick DPad;
    FloatingJoystick CamDPad;

    [SerializeField] float ChickenSpeed;
    float Speed;

    [SerializeField] float RotSensi;
    float RotSpeedX, RotSpeedY;

    Animator PlayerAnimator;
    [SerializeField] GameObject PlayerAnimatorObj;
    [SerializeField] GameObject CamRoot;
    [SerializeField] GameObject Anada;
    [SerializeField] GameObject AnadaRoot;
    [SerializeField] GameObject Anadabin;

    bool CanShoot;
    float ShootCooldown;
    float ShootTimer;
    [SerializeField] float ChickenShootTimer;

    Button ShootButton;
    #endregion

    #region Start
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        ShootCooldown = ShootTimer;
        Anadabin = GameObject.Find("AndaBin");
        PlayerAnimator = PlayerAnimatorObj.GetComponent<Animator>();
        ChickenVirCam.gameObject.SetActive(true);

        if (GameManager.instance.DeviceType == 1)
        { 
            ShootButton = GameObject.Find("AndaMar").GetComponent<Button>();
            ShootButton.onClick.AddListener(() =>
            {
                if (!IsOwner) return;
                PlayerSkillCall();
            });
        }

        if (!IsOwner) return;
        ControlType = GameManager.instance.DeviceType;

        if (ControlType == 1)
        {
            Inputsthings();
        }        
    }
    #endregion

    #region Updates
    // Update is called once per frame
    void Update()
    {
        AndaCooldown();

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
    #endregion

    #region Virtual Player Methods

    /// <summary>
    /// Configure Dpad
    /// </summary>
    void Inputsthings()
    {
        DPad = LevelManager.instance.Dpad;
        CamDPad = LevelManager.instance.CamDpad;
    }

    /// <summary>
    /// Timer for Cooldown
    /// </summary>
    void AndaCooldown()
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
    }
    #endregion

    #region Animation Methods

    /// <summary>
    /// Animates the PLayer movement
    /// </summary>
    void AnimateMovemet()
    {
        if (PlayerNavi.velocity != Vector3.zero)
        {
            PlayerAnimator.SetBool("walk", true);
        }
        else PlayerAnimator.SetBool("walk", false);
    }

    /// <summary>
    /// Animate the player jump
    /// </summary>
    void AnimateJump()
    {

    }
    #endregion

    #region Movement Methods
    /// <summary>
    /// Handels the main Player Movement
    /// </summary>
    void PlayerMovement()
    {
        if (ControlType == 1)   // if Android
        {
            HorValue = DPad.Horizontal;
            VarValue = DPad.Vertical;

            RotSpeedY += CamDPad.Horizontal * RotSensi;
            RotSpeedX += DPad.Vertical * RotSensi;
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
        
        PlayerNavi.speed = Speed;

        Vector3 newPos = transform.position + transform.right * HorValue + transform.forward * VarValue;
        PlayerNavi.destination = newPos;

        transform.localEulerAngles = new Vector3(0 , RotSpeedY , 0);

        AnimateMovemet();
    }
    #endregion

    #region Skill Methods

    /// <summary>
    /// Murga and halal ke skill call karta hai, depends on the Player class.
    /// </summary>
    public void PlayerSkillCall()
    {
        if (CanShoot == true)
        {
            SpawnAndaServerRpc();
            AnimateJump();
        }
    }

    /// <summary>
    /// Murga ke skill
    /// </summary>
    [ServerRpc]
    private void SpawnAndaServerRpc(ServerRpcParams rpcParams = default)
    {
        GameObject andaInstance = Instantiate(Anada, AnadaRoot.transform.position, AnadaRoot.transform.rotation, Anadabin.transform);
        NetworkObject networkObject = andaInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
        ShootTimer = ChickenShootTimer;
        ShootCooldown = ShootTimer;
        CanShoot = false;
    }
    #endregion
}
