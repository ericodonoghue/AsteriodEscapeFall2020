using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEventScripts : MonoBehaviour
{
    public AudioSource scarySound1;

    private GameObject lights;

    private float cdValue;

    // Start is called before the first frame update
    void Start()
    {
        lights = GameObject.FindGameObjectWithTag("CaveLights");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider c)
    {
        GameObject collided = c.gameObject;

        if (collided.tag == "ScarySound1")
        {
            scarySound1.Play();
        }
        else if (collided.tag == "LightFlicker1")
        {
            lights.SetActive(false);
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown(float countdownValue = 3)
    {
        cdValue = countdownValue;
        while (cdValue > 0)
        {
            UnityEngine.Debug.Log("Countdown: " + cdValue);
            yield return new WaitForSeconds(1.0f);
            cdValue--;
        }
        lights.SetActive(true);
    }

}
