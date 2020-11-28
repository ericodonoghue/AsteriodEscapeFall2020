using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UnifiedPlayerMovement : MonoBehaviour
{
    #region Private Fields

    // Local reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;
    private PlayerInputManager playerInputManager;
    private SoundManager soundManager;

    #endregion Private Fields


    #region Unity Events

    private void Awake()
    {
        // Get a reference to the script components from Main Camera
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
        this.soundManager = Camera.main.GetComponent<SoundManager>();


        // Map a pair of event handlers (OnX_Pressed, OnX_Released) for every supported command (MoveUp, MoveDown, etc.)
        this.EnablePlayerInputEvents();
    }

    #endregion Unity Events


    #region Configuration

    //private void LoadSettings()
    //{
    //    // Data type returned is "object", so depending upon data type it may be necessary to cast
    //    //this.myStoredStringValue = this.playerInputManager.GetPlayerConfigurationValue("MyStringName");
    //}

    //private void SaveSettings()
    //{
    //    // Data being stored must be [SERIALIZABLE], all native types (string, int, float, etc.) are.
    //    // Check PlayerInputManager for example of making your type (class, struct, enum) serializable.
    //    //this.playerInputManager.SetPlayerConfigurationValue("MyStringName", this.myStoredStringValue)

    //    // Values are auto-loaded, but the actual file should be saved after making changes
    //    this.playerInputManager.SavePlayerConfiguration();
    //}

    #endregion Configuration


    #region Movement2.cs

    #region Fields

    private PauseControl pauseControl;
    //MW: private SettingsButtonControl settingsButtonControl;

    public GameObject player;
    public Rigidbody playerRB;

    public Vector3 force;
    public Vector3 rotate;
    float rotationSpeed = 1f;
    public float forwardThrust = 1f;
    public float vertThrust = 1f;
    public float strafeThrust = 1f;

    readonly float thrust = 10f;
    readonly float turnSpeed = 2f;
    public float vertRot = 0f;
    public bool spinOut = false;
    private float spinOutTime = 0;

    private Vector3 CamRot;

    #endregion Fields


    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = GetComponent<Rigidbody>();
        force = new Vector3(0f, 0f, 0f);
        rotate = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseControl.isPaused)
        {
            //MW: SetForceVector(); - Code moved into action-specific event handlers
            RotatePlayer();
            //MW: ResetForcesButton(); - Code moved into specific event handler
            ResetSpinOut();
        }
    }

    // Called 50 times per second
    void FixedUpdate()
    {
        if (!pauseControl.isPaused)
        {
            ApplyForce();
            ApplyTorque();
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        GameObject collided = c.gameObject;
        spinOut = true;

        // Choose the best tag to use (cave walls are not tagged, but should belong to a tagged parent)
        while (collided.tag == "Untagged")
        {
            if (collided.transform.parent != null)
            {
                collided = collided.transform.parent.gameObject;
                if (collided.tag != "Untagged") break;
            }
            else break;  // no more parents to bother
        }

        switch (collided.tag)
        {
            case "Cave":
                spinOutTime = Time.time + 7;
                //Debug.Log("In Movement2 Script");
                break;
            case "AirTank":

                break;
            case "Cave_GlancingBlow":
                spinOutTime = Time.time + 5;
                break;
                //case "SharpObject":
                //    spinOutTime = Time.time + 10;
                //    break;
                //case "SharpObject_NearMiss":

                //    break;
                //case "Monster":
                //    spinOutTime = Time.time + 20;
                //    break;
                //case "Monster_NearMiss":
                //    spinOutTime = Time.time + 5;
                //    break;
                //case "AirTank_Single":

                //    break;
                //case "AirTank_Double":

                //    break;
                //case "AirTank_PonyBottle":

                //    break;
        }
    }

    #endregion Unity Events

    #region PlayerInput Events

    private void EnablePlayerInputEvents()
    {
        // Map a pair of event handlers (OnX_Pressed, OnX_Released) for every supported command (MoveUp, MoveDown, etc.)
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.Interact, Interact_Pressed, Interact_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveUp, MoveUp_Pressed, MoveUp_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveDown, MoveDown_Pressed, MoveDown_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveLeft, MoveLeft_Pressed, MoveLeft_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveRight, MoveRight_Pressed, MoveRight_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveForward, MoveForward_Pressed, MoveForward_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveReverse, MoveReverse_Pressed, MoveReverse_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.StabilizeAvatar, StabilizeAvatar_Pressed, StabilizeAvatar_Released);
    }
    private void OnDisable()
    {
        // Clear event handler mappings
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.Interact, Interact_Pressed, Interact_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveUp, MoveUp_Pressed, MoveUp_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveDown, MoveDown_Pressed, MoveDown_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveLeft, MoveLeft_Pressed, MoveLeft_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveRight, MoveRight_Pressed, MoveRight_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveForward, MoveForward_Pressed, MoveForward_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveReverse, MoveReverse_Pressed, MoveReverse_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.StabilizeAvatar, StabilizeAvatar_Pressed, StabilizeAvatar_Released);
    }

    private void Interact_Pressed()
    {

    }
    private void Interact_Released()
    {

    }

    private void MoveUp_Pressed()
    {
        if (avatarAccounting.JetsCanFire)
        {
            this.avatarAccounting.FireJet(JetType.AttitudeJetUp);

            force.y = vertThrust;
            this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 1.5f);
        }
    }
    private void MoveUp_Released()
    {
        this.avatarAccounting.TerminateJet(JetType.AttitudeJetUp);

        force.y = 0;
        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
    }

    private void MoveDown_Pressed()
    {
        if (avatarAccounting.JetsCanFire)
        {
            this.avatarAccounting.FireJet(JetType.AttitudeJetDown);

            force.y = -vertThrust;
            this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
        }
    }
    private void MoveDown_Released()
    {
        this.avatarAccounting.TerminateJet(JetType.AttitudeJetDown);

        force.y = 0;
        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
    }

    private void MoveLeft_Pressed()
    {
        if (avatarAccounting.JetsCanFire)
        {
            this.avatarAccounting.FireJet(JetType.AttitudeJetLeft);

            force.x = -strafeThrust;
            this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
        }
    }
    private void MoveLeft_Released()
    {
        this.avatarAccounting.TerminateJet(JetType.AttitudeJetLeft);

        force.x = 0;
        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
    }

    private void MoveRight_Pressed()
    {
        if (avatarAccounting.JetsCanFire)
        {
            this.avatarAccounting.FireJet(JetType.AttitudeJetRight);

            force.x = strafeThrust;
            this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
        }
    }
    private void MoveRight_Released()
    {
        this.avatarAccounting.TerminateJet(JetType.AttitudeJetRight);

        force.x = 0;
        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
    }

    private void MoveForward_Pressed()
    {
        if (avatarAccounting.JetsCanFire)
        {
            avatarAccounting.FireJet(JetType.MainThruster);

            force.z = forwardThrust;
            this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
        }
    }
    private void MoveForward_Released()
    {
        avatarAccounting.TerminateJet(JetType.MainThruster);

        force.z = 0;
        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
    }

    private void MoveReverse_Pressed()
    {
        if (avatarAccounting.JetsCanFire)
        {
            avatarAccounting.FireJet(JetType.AttitideJetReverse);

            force.z = -strafeThrust;
            this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
        }
    }
    private void MoveReverse_Released()
    {
        avatarAccounting.TerminateJet(JetType.AttitideJetReverse);

        force.z = 0;
        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
    }

    private void StabilizeAvatar_Pressed()
    {
        // This operation is expensive
        avatarAccounting.FireAllJetsToStabilizeAvatar();

        // Stabilization code from Movement2.ResetForcesButton()
        // Only fire jets if there's air in the tank
        if (avatarAccounting.JetsCanFire)
        {
            // https://forum.unity.com/threads/reset-rigidbody.39998/
            if (playerRB.isKinematic == false)
            {
                //playerRB.velocity = Vector3.zero;
                playerRB.angularVelocity = Vector3.zero;
                playerRB.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            //playerRB.isKinematic = true;
        }
    }
    private void StabilizeAvatar_Released()
    {
        // Stabilization code from Movement2.ResetForcesButton()
        // Only fire jets if there's air in the tank
        if (avatarAccounting.JetsCanFire)
        {
            playerRB.isKinematic = false;
        }
    }

    // Placeholder: I really have no idea how to handle this, yet
    private void MovementControls_Input()     // Use to assign to gamepad stick
    {

    }

    // Placeholder: I really have no idea how to handle this, yet
    private void CameraControls_input()       // Use to assign camera controls to mouse or gamepad stick
    {

    }

    #endregion PlayerInput Events


    #region Private Methods

    /** Apply's a force to the player based on WASD input */
    void ApplyForce()
    {
        playerRB.AddRelativeForce(force * thrust);
        //playerRB.AddRelativeTorque(vertRot, 0f, 0f);
    }

    void ApplyTorque()
    {
        playerRB.AddRelativeTorque(rotate * turnSpeed);
    }

    /** Rotates the player object according to mouse position on screen */
    void RotatePlayer()
    {
        if (avatarAccounting.CurrentOxygenAllTanksContent != 0)
        {
            //Camera.main.transform.Rotate(lookSensitivity * Time.deltaTime * Input.GetAxis("Mouse X"), lookSensitivity * Time.deltaTime * Input.GetAxis("Mouse Y"), 0);
            if (!spinOut)
            {
                player.transform.rotation = Camera.main.transform.rotation;
            }
            else
            {
                player.transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.rotation, rotationSpeed * Time.deltaTime);

            }

        }

    }

    private void ResetSpinOut()
    {
        if (spinOut)
        {
            rotationSpeed += 1 * Time.deltaTime;
            if (Time.time >= spinOutTime)
            {
                spinOut = false;
                rotationSpeed = 1;
                //playerRB.angularVelocity = Vector3.zero;
                //playerRB.rotation = Quaternion.Euler(0f, 0f, 0f);
                //player.transform.rotation = Quaternion.Slerp
            }

        }
    }

    /** Computes the angles between 2 Vectors */
    float AngleBetweenTwoPointsY(Vector2 a, Vector2 b)
    {
        // definitely not it //float angle = Mathf.Atan2(b.x - a.x, 0) * Mathf.Rad2Deg;
        float angle = Mathf.Atan2(b.x - a.x, b.y - a.y) * Mathf.Rad2Deg;//working fine
        //Debug.Log("horizontal angle: " + angle);
        return angle;
    }

    //added - don't change original
    float AngleBetweenTwoPointsX(Vector2 a, Vector2 b)
    {
        //definitely not it //float angle = Mathf.Atan2(0, b.y - a.y) * Mathf.Rad2Deg;
        float angle = Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;//not working except top-left corner
        //Debug.Log("vertical angle: " + angle);
        return angle;
    }

    #endregion Private Methods

    #endregion Movement2.cs
}


#region Methods deprecated - code moved into action-specific event handlers above

//private void Awake()
//{
//    GameObject g = GameObject.FindGameObjectWithTag("SettingsMenu");
//    settingsButtonControl = g.GetComponent<SettingsButtonControl>();
//}

///** Sets the force vector based on WASD input */
//void SetForceVector()
//{
//    // Only fire jets if there's air in the tanks
//    if (avatarAccounting.CurrentOxygenAllTanksContent != 0 && !avatarAccounting.PlayerBlackout)
//    {
//        {
//            if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
//            {
//                force.x = strafeThrust;
//                this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
//            }
//            if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
//            {
//                force.x = -strafeThrust;
//                this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
//            }
//            if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
//            {
//                force.y = -vertThrust;
//                this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
//            }
//            if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
//            {
//                force.y = vertThrust;
//                this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
//            }
//            if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
//            {
//                force.z = forwardThrust;
//                this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
//            }
//            if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
//            {
//                force.z = -strafeThrust;
//                this.soundManager.SetSoundState(SoundStates.Start, ScaryNoises.Jet, 0.5f);
//            }

//        }


//        //reset button - should be smoother and automatic
//        if (Input.GetMouseButtonDown(1))
//        {
//            if (player.transform.rotation.x != 0f
//            && player.transform.rotation.y != 0f
//            && player.transform.rotation.z != 0f
//            && (avatarAccounting.CurrentOxygenAllTanksContent != 0))
//            {
//                Quaternion initialRot = player.transform.rotation;
//                player.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
//                vertRot = 0;
//            }
//        }
//    }

//    if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
//    {
//        force.x = 0;
//        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
//    }
//    if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
//    {
//        force.x = 0;
//        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
//    }
//    if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
//    {
//        force.y = 0;
//        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
//    }
//    if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
//    {
//        force.y = 0;
//        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
//    }
//    if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
//    {
//        force.z = 0;
//        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
//    }
//    if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
//    {
//        force.z = 0;
//        this.soundManager.SetSoundState(SoundStates.Stop, ScaryNoises.Jet);
//    }
//}

//void ResetForcesButton()
//{
//    // Only fire jets if there's air in the tank
//    if (avatarAccounting.CurrentOxygenAllTanksContent != 0)
//    {
//        if (Input.GetKeyDown(KeyCode.Tab))
//        {
//            // https://forum.unity.com/threads/reset-rigidbody.39998/
//            if (playerRB.isKinematic == false)
//            {
//                //playerRB.velocity = Vector3.zero;
//                playerRB.angularVelocity = Vector3.zero;
//                playerRB.rotation = Quaternion.Euler(0f, 0f, 0f);
//            }
//            //playerRB.isKinematic = true;
//        }
//        if (Input.GetKeyUp(KeyCode.Tab))
//        {
//            playerRB.isKinematic = false;
//        }
//    }
//}

// Unused?  Only reference found was in DumbyMovement - deprecated code
//void RotateWithKeys()
//{
//    if (avatarAccounting.CurrentOxygenAllTanksContent != 0)
//    {
//        if (Input.GetKeyDown(KeyCode.LeftArrow))
//        {
//            rotate.y = -1f;
//        }
//        if (Input.GetKeyDown(KeyCode.RightArrow))
//        {
//            rotate.y = 1f;
//        }
//        // Key up
//        if (Input.GetKeyUp(KeyCode.LeftArrow))
//            rotate.y = 0f;
//        if (Input.GetKeyUp(KeyCode.RightArrow))
//            rotate.y = 0f;

//        if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            rotate.x = -1f;
//        }
//        if (Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            rotate.x = 1f;
//        }
//        // Key up
//        if (Input.GetKeyUp(KeyCode.UpArrow))
//            rotate.x = 0f;
//        if (Input.GetKeyUp(KeyCode.DownArrow))
//            rotate.x = 0f;
//    }
//}

#endregion Methods deprecated - code moved into action-specific event handlers above