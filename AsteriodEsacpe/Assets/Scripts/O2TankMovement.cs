using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2TankMovement : MonoBehaviour
{
    public Rigidbody tankRB;
    readonly float force = 8f;

    // Start is called before the first frame update
    void Start()
    {
        tankRB = GetComponent<Rigidbody>();

        tankRB.AddRelativeForce(new Vector3(Random.Range(0f, 2f) * force, Random.Range(0f, 2f) * force, Random.Range(0, 2f) * force));
    }

    // Update is called once per frame
    void Update()
    {
        tankRB.AddRelativeForce(new Vector3(Random.Range(0f, 2f) * force, Random.Range(0f, 2f) * force, Random.Range(0, 2f) * force));
    }
}
