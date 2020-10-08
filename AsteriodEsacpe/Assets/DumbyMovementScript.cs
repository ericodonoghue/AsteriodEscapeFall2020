using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbyMovementScript : MonoBehaviour
{
    public GameObject player;
    public Rigidbody playerRB;
    public Vector3 force;
    float vX = 0.0f;
    float vY = 0.0f;
    float vZ = 0.0f;

    float thrust = 0.001f;//10.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = GetComponent<Rigidbody>();
        force = new Vector3(0f,0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        //D and S do not function; W and A do not in other configuration
        //check for neither / both
        if (Input.GetKey(KeyCode.D))
        {
            //force.x = 1;
            vX += thrust;
        }
        //else if (!Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)))
        //    force.x = 0;
        if (Input.GetKey(KeyCode.A))
        {
            //force.x = -1;
            vX -= thrust;
        }
        //else if (!Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)))
        //    force.x = 0;
        if (Input.GetKey(KeyCode.S))
        {
            //force.y = -1;
            vY -=thrust;
        }
        //else if (!Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W)))
        //    force.y = 0;
        if (Input.GetKey(KeyCode.W))
        {
           // force.y = 1;
            vY += thrust;
        }
        //else if (!Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W)))
        //    force.y = 0;
        if (Input.GetMouseButton(0))
        {
            //force.z = 1;
            vZ += thrust;
        }
        //else if (!Input.GetMouseButton(0))
        //{
        //    force.z = 0;
        //}

        //mouse aiming stuff
    }

    void FixedUpdate()
    {
        Vector3 playerPos = player.transform.position;

        playerPos.x += vX;
        playerPos.y += vY;
        playerPos.z += vZ;

        player.transform.position = playerPos;

        //playerRB.AddRelativeForce(force * thrust);
    }
}