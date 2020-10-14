using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbyMovementScript : MonoBehaviour
{
    public GameObject player;
    public Rigidbody playerRB;
    public Vector3 force;
    public Vector3 rotate;
    public PlayerCollisionO2 CollOxScript;
    readonly float thrust = 10f;
    readonly float turnSpeed = 0.5f;
    public float vertRot = 0f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        CollOxScript = player.GetComponent<PlayerCollisionO2>();
        playerRB = GetComponent<Rigidbody>();
        force = new Vector3(0f, 0f, 0f);
        rotate = new Vector3(0f, 0f, 0f);
    }

    // Called before Start is called for all objects
    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetForceVector();
        RotateWithKeys();
        //RotatePlayer();
        ResetForcesButton();
    }

    // Called 50 times per second
    void FixedUpdate()
    {
        ApplyForce();
        ApplyTorque();
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
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float angleY = AngleBetweenTwoPointsY(positionOnScreen, mouseOnScreen);
        //float angleX = AngleBetweenTwoPointsX(positionOnScreen, mouseOnScreen);
        //playerRB.AddRelativeTorque(new Vector3(0f, angle, 0f));

        //Quaternion playerRot = player.transform.rotation;
        //player.transform.rotation = Quaternion.Euler(new Vector3(playerRot.x, angleY, playerRot.z));
        player.transform.rotation = Quaternion.Euler(new Vector3(vertRot, angleY, 0f));
        //playerRB.AddRelativeTorque(vertRot, 0f, 0f);
    }

    /** Sets the force vector based on WASD input */
    void SetForceVector()
    {
        float fuelRateValue = 1f;

        if (CollOxScript.fuel > 0)
        {
            // Key down
            if (Input.GetKeyDown(KeyCode.D))
            {
                force.x = 1;
                CollOxScript.fuelRate += fuelRateValue;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                force.x = -1;
                CollOxScript.fuelRate += fuelRateValue;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                force.y = -1;
                CollOxScript.fuelRate += fuelRateValue;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                force.y = 1;
                CollOxScript.fuelRate += fuelRateValue;
            }
            if (Input.GetMouseButtonDown(0))
            {
                force.z = 1;
                CollOxScript.fuelRate += fuelRateValue;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                force.z = 1;
                CollOxScript.fuelRate += fuelRateValue;
            }
            // Key up
            if (Input.GetKeyUp(KeyCode.D))
            {
                force.x = 0;
                CollOxScript.fuelRate -= fuelRateValue;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                force.x = 0;
                CollOxScript.fuelRate -= fuelRateValue;
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                force.y = 0;
                CollOxScript.fuelRate -= fuelRateValue;
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                force.y = 0;
                CollOxScript.fuelRate -= fuelRateValue;
            }
            if (Input.GetMouseButtonUp(0))
            {
                force.z = 0;
                CollOxScript.fuelRate -= fuelRateValue;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                force.z = 0;
                CollOxScript.fuelRate -= fuelRateValue;
            }
        }

        if (CollOxScript.fuel <= 0)
        {
            CollOxScript.fuel = 0;
            CollOxScript.fuelRate = 0;
        }

        //reset button - should be smoother and automatic
        if (Input.GetMouseButtonDown(1))
        {
            if (player.transform.rotation.x != 0f && player.transform.rotation.y != 0f && player.transform.rotation.z != 0f && CollOxScript.fuel >= 5)
            {
                Quaternion initialRot = player.transform.rotation;
                player.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                CollOxScript.fuel -= 5;
                CollOxScript.fuelRate = 0;
                CollOxScript.hasCollided = false;
                vertRot = 0;
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
        float angle =  Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;//not working except top-left corner
        //Debug.Log("vertical angle: " + angle);
        return angle;
    }

    void RotateWithKeys()
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


    void ResetForcesButton()
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
