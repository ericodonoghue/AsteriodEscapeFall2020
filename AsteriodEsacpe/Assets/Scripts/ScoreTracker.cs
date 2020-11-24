using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{

    public int collisionsCount;
    public float timeToComplete;
    public float oxygenUsed;
    public int nearMissCounter;

    // Start is called before the first frame update
    void Start()
    {
        collisionsCount = 0;
        timeToComplete = 0;
        oxygenUsed = 0;
        nearMissCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        timeToComplete += 1.0f / 50.0f;
    }
}
