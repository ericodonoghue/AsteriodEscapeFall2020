using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerCollisionO2 : MonoBehaviour
{
    public float oxygen = 100.0f;

    public float oxygenLoseRate = 1.0f;

    // TODO: player movemnt script will set this value when jets are being used
    public float oxygenJetRate = 0.0f;

    public float suitDamage = 1.0f;

    // TODO: player movemnt script will set this value when jets are being used
    public float fuel = 100.0f;

    public float fuelRate = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: get values of variables from save data once implemented
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        oxygen -= (oxygenLoseRate * suitDamage) / 50.0f;
        oxygen -= oxygenJetRate / 50.0f;

        fuel -= fuelRate / 50.0f;
    }

    private void OnCollisionEnter(Collision c)
    {
    }
}
