﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2TankMovement : MonoBehaviour
{
    GameObject PoneyBottleParentObj;

    GameObject[] ponyBottles;

    public float intialV = 50.0f;
    public float intialT = 30.0f;

    public Vector3 rotate = new Vector3(1,1,1);
    public Vector3 force = new Vector3(1,1,1);

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        PoneyBottleParentObj = GameObject.FindGameObjectWithTag("PonyBottleParent");
        ponyBottles = new GameObject[PoneyBottleParentObj.transform.childCount];

        for (int i = 0; i < ponyBottles.Length; ++i)
        {
            ponyBottles[i] = PoneyBottleParentObj.transform.GetChild(i).gameObject;
        }

        /*foreach (GameObject bottle in ponyBottles)
        {
            Rigidbody bottleRB = bottle.GetComponent<Rigidbody>();
            bottleRB.AddForce(force * intialV);
            bottleRB.AddTorque(rotate * intialT);
        }*/

        // code for if attach script to each object

        this.rb = this.GetComponent<Rigidbody>();

        rb.addForce(force * initialV);
        rb.addForce(rotate * initialT);

    }

    // Update is called once per frame
    void Update()
    {
/*       foreach (GameObject bottle in ponyBottles)
       {
            Rigidbody bottleRB = bottle.GetComponent<Rigidbody>();
            bottleRB.AddForce(force * intialV);
            bottleRB.AddTorque(rotate * intialT);
       }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*foreach (GameObject bottle in ponyBottles)
        {
            Rigidbody bottleRB = bottle.GetComponent<Rigidbody>();
            bottleRB.AddForce(force * intialV);
            bottleRB.AddTorque(rotate * intialT);
        }*/

        // todo: get the normal force and apply it to the rigidbody 
        
        
    }
}
