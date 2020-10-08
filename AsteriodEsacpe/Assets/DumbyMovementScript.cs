using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbyMovementScript : MonoBehaviour
{
    public GameObject player;
    public Rigidbody playerRB;
    public Vector3 force;

    readonly float thrust = 10f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = GetComponent<Rigidbody>();
        force = new Vector3(0f, 0f, 0f);
    }

    // Called before Start is called for all objects
    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetForceVector();
        RotatePlayer();
    }

    // Called 50 times per second
    void FixedUpdate()
    {
        ApplyForce();
    }

    /** Apply's a force to the player based on WASD input */
    void ApplyForce()
    {
        playerRB.AddRelativeForce(force * thrust);
    }
    /** Rotates the player object according to mouse position on screen */
    void RotatePlayer()
    {
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        player.transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
    }

    /** Sets the force vector based on WASD input */
    void SetForceVector()
    {
        // Key down
        if (Input.GetKeyDown(KeyCode.D))
            force.x = 1;
        if (Input.GetKeyDown(KeyCode.A))
            force.x = -1;
        if (Input.GetKeyDown(KeyCode.S))
            force.y = -1;
        if (Input.GetKeyDown(KeyCode.W))
            force.y = 1;
        if (Input.GetMouseButtonDown(0))
            force.z = 1;
        // Key up
        if (Input.GetKeyUp(KeyCode.D))
            force.x = 0;
        if (Input.GetKeyUp(KeyCode.A))
            force.x = 0;
        if (Input.GetKeyUp(KeyCode.S))
            force.y = 0;
        if (Input.GetKeyUp(KeyCode.W))
            force.y = 0;
        if (Input.GetMouseButtonUp(0))
            force.z = 0;
    }

    /** Computes the angles between 2 Vectors */
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(b.x - a.x, b.y - a.y) * Mathf.Rad2Deg;
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
