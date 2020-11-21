using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2TankMovement : MonoBehaviour
{
    //GameObject PoneyBottleParentObj;

    //GameObject[] ponyBottles;

    public float initialV = 100.0f;
    public float v = 1;
    public float torque = 30.0f;

    public Vector3 rotate = new Vector3(1,1,1);
    public float force = 50f;

    public float dir = 1;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        /*
        PoneyBottleParentObj = GameObject.FindGameObjectWithTag("PonyBottleParent");
        ponyBottles = new GameObject[PoneyBottleParentObj.transform.childCount];

        for (int i = 0; i < ponyBottles.Length; ++i)
        {
            ponyBottles[i] = PoneyBottleParentObj.transform.GetChild(i).gameObject;
        }
       
        foreach (GameObject bottle in ponyBottles)
        {
            Rigidbody bottleRB = bottle.GetComponent<Rigidbody>();
            bottleRB.AddForce(force * intialV);
            bottleRB.AddTorque(rotate * intialT);
        }*/

        rb = GetComponent<Rigidbody>();
        Vector3 forceVector = new Vector3(Random.Range(-2f, 2f) * initialV, Random.Range(-2f, 2f) * initialV, 0);
        rb.AddForce(forceVector);
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
        Vector3 forceVector = new Vector3(rb.velocity.x * 0.01f * dir, rb.velocity.z * 0.01f * dir, rb.velocity.z * 0f * dir);
        rb.AddForce(forceVector);
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*foreach (GameObject bottle in ponyBottles)
        {
            Rigidbody bottleRB = bottle.GetComponent<Rigidbody>();
            bottleRB.AddForce(force * intialV);
            bottleRB.AddTorque(rotate * intialT);
        var forceVec : Vector3 = -target_.rigidbody.velocity.normalized * explosionStrength;
        target_.rigidbody.AddForce(forceVec,ForceMode.Acceleration);
        }*/

        //Vector3 forceVector = new Vector3(rb.velocity.x * -50, rb.velocity.y * -100f, 0f);
        Vector3 forceVector = new Vector3(Random.Range(-2f, 2f) * initialV, Random.Range(-2f, 2f) * initialV, 0);

        Vector3 f = rb.velocity.normalized * force;
        rb.AddForce(f);
        rb.velocity = f;
        dir = -dir;
    }
}
