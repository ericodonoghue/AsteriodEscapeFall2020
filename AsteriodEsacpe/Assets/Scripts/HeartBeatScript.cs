using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatScript : MonoBehaviour
{

    public AudioSource heartBeat;

    public float intialDelay = 1.0f;

    private float delay = 1.0f;

    private float cdValue;

    // Start is called before the first frame update
    void Start()
    {
        StartCountdown(intialDelay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private IEnumerator StartCountdown(float countdownValue)
    {
        cdValue = countdownValue;
        while (cdValue > 0)
        {
            yield return new WaitForSeconds(0.1f);
            cdValue -= 0.1f;
        }
        heartBeat.Play();

        // TODO: calculate the delay change based on current player heartrate / O2
        StartCountdown(delay);
    }
}
