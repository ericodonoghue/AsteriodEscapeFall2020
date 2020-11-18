using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2 : MonoBehaviour
{
    // Local reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;

    private PauseControl pauseControl;

    private SettingsButtonControl settingsButtonControl;

    public GameObject player;
    public Rigidbody playerRB;
    public Vector3 force;
    public Vector3 rotate;
    float rotationSpeed = 1f;
    public float forwardThrust = 1f;
    public float vertThrust = 0.5f;
    public float strafeThrust = 0.5f;
    //MW: public PlayerCollisionO2 CollOxScript;
    readonly float thrust = 10f;
    readonly float turnSpeed = 2f;
    public float vertRot = 0f;
    public bool spinOut = false;
    private float spinOutTime = 0;

    private Vector3 CamRot;


    public AudioSource jetSound;


    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the AvatarAccounting component of Main Camera
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        pauseControl = Camera.main.GetComponent<PauseControl>();
        

        player = GameObject.FindGameObjectWithTag("Player");
        //MW: CollOxScript = player.GetComponent<PlayerCollisionO2>();
        playerRB = GetComponent<Rigidbody>();
        force = new Vector3(0f, 0f, 0f);
        rotate = new Vector3(0f, 0f, 0f);
    }

    // Called before Start is called for all objects
    private void Awake()
    {
        GameObject g = GameObject.FindGameObjectWithTag("SettingsMenu");
        settingsButtonControl = g.GetComponent<SettingsButtonControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseControl.isPaused)
        {
            SetForceVector();
            RotatePlayer();
            ResetForcesButton();
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
            case "SharpObject":
                spinOutTime = Time.time + 10;
                break;
            case "SharpObject_NearMiss":
               
                break;
            case "Monster":
                spinOutTime = Time.time + 20;
                break;
            case "Monster_NearMiss":
                spinOutTime = Time.time + 5;
                break;
            case "AirTank_Single":
                
                break;
            case "AirTank_Double":
                
                break;
            case "AirTank_PonyBottle":
                
                break;
        }
    }

    /** Sets the force vector based on WASD input */
    void SetForceVector()
    {
        // Only fire jets if there's air in the tanks
        if (avatarAccounting.CurrentOxygenAllTanksContent != 0 && !avatarAccounting.PlayerBlackout) 
        {
            //MW: float fuelRateValue = 1f;

            //MW: if (CollOxScript.fuel > 0)
            {
                // Key down
                /*if (Input.GetKeyDown(KeyCode.D))
                {
                    force.x = strafeThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    force.x = -strafeThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    force.y = -vertThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    force.y = vertThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    force.z = forwardThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    force.z = forwardThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    force.z = -strafeThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                }*/
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
                {
                    force.x = strafeThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                    if (!jetSound.isPlaying)
                    {
                        jetSound.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
                {
                    force.x = -strafeThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                    if (!jetSound.isPlaying)
                    {
                        jetSound.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
                {
                    force.y = -vertThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                    if (!jetSound.isPlaying)
                    {
                        jetSound.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
                {
                    force.y = vertThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                    if (!jetSound.isPlaying)
                    {
                        jetSound.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
                {
                    force.z = forwardThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                    if (!jetSound.isPlaying)
                    {
                        jetSound.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
                {
                    force.z = -strafeThrust;
                    //MW: CollOxScript.fuelRate += fuelRateValue;
                    if (!jetSound.isPlaying)
                    {
                        jetSound.Play();
                    }
                }

            }

            

            //MW: 
            //if (CollOxScript.fuel <= 0)
            //{
            //    CollOxScript.fuel = 0;
            //    CollOxScript.fuelRate = 0;
            //}

            //reset button - should be smoother and automatic
            if (Input.GetMouseButtonDown(1))
            {
                if (player.transform.rotation.x != 0f
                && player.transform.rotation.y != 0f
                && player.transform.rotation.z != 0f
                && (avatarAccounting.CurrentOxygenAllTanksContent != 0))
                //MW: && CollOxScript.fuel >= 5)
                {
                    Quaternion initialRot = player.transform.rotation;
                    player.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                    //MW: CollOxScript.fuel -= 5;
                    //MW: CollOxScript.fuelRate = 0;
                    //MW: CollOxScript.hasCollided = false;
                    vertRot = 0;
                }
            }
        }
        // Key up
        /*if (Input.GetKeyUp(KeyCode.D))
        {
            force.x = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            force.x = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            force.y = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            force.y = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }
        if (Input.GetMouseButtonUp(0))
        {
            force.z = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            force.z = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            force.z = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
        }*/
        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
        {
            force.x = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
            jetSound.Stop();
        }
        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
        {
            force.x = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
            jetSound.Stop();
        }
        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
        {
            force.y = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
            jetSound.Stop();
        }
        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
        {
            force.y = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
            jetSound.Stop();
        }
        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
        {
            force.z = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
            jetSound.Stop();
        }
        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
        {
            force.z = 0;
            //MW: CollOxScript.fuelRate -= fuelRateValue;
            jetSound.Stop();
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

    void RotateWithKeys()
    {
        if (avatarAccounting.CurrentOxygenAllTanksContent != 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                rotate.y = -1f;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                rotate.y = 1f;
            }
            // Key up
            if (Input.GetKeyUp(KeyCode.LeftArrow))
                rotate.y = 0f;
            if (Input.GetKeyUp(KeyCode.RightArrow))
                rotate.y = 0f;

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                rotate.x = -1f;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                rotate.x = 1f;
            }
            // Key up
            if (Input.GetKeyUp(KeyCode.UpArrow))
                rotate.x = 0f;
            if (Input.GetKeyUp(KeyCode.DownArrow))
                rotate.x = 0f;
        }
    }

    void ResetForcesButton()
    {
        // Only fire jets if there's air in the tank
        if (avatarAccounting.CurrentOxygenAllTanksContent != 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
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
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                playerRB.isKinematic = false;
            }
        }
    }
    // Code we want to save
    /*D and S do not function; W and A do not in other configuration
    check for neither / both
    if (Input.GetKey(KeyCode.D))
    {
        force.x = 1;
        vX += thrust;
    }
    else if (!Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)))
        force.x = 0;
    if (Input.GetKey(KeyCode.A))
    {
        force.x = -1;
        vX -= thrust;
    }
    else if (!Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)))
        force.x = 0;
    if (Input.GetKey(KeyCode.S))
    {
        force.y = -1;
        vY -=thrust;
    }
    else if (!Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W)))
        force.y = 0;
    if (Input.GetKey(KeyCode.W))
    {
        force.y = 1;
        vY += thrust;
    }
    else if (!Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W)))
        force.y = 0;
    if (Input.GetMouseButton(0))
    {
        force.z = 1;
        vZ += thrust;
    }
    else if (!Input.GetMouseButton(0))
    {
        force.z = 0;
    }*/

    /*Vector3 playerPos = player.transform.position;

        playerPos.x += vX;
        playerPos.y += vY;
        playerPos.z += vZ;

        player.transform.position = playerPos;*/
}