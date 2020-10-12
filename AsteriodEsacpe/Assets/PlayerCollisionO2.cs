using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisionO2 : MonoBehaviour
{
    public bool hasCollided = false;

    Color warnOxygen;
    Color regOxygen;

    Color warnFuel;
    Color regFuel;

    public Text oxygenDisplay;
    public Slider oxygenBar;
    public Text fuelDisplay;
    public Slider fuelBar;
    public Canvas canvas;
    public GameObject PlayerModelTest;

    public float oxygen = 99.0f;

    public float oxygenLoseRate = 1.0f;

    // TODO: player movemnt script will set this value when jets are being used
    public float oxygenJetRate = 0.0f;

    public float suitDamage = 1.0f;

    // TODO: player movemnt script will set this value when jets are being used
    public float fuel = 99.0f;

    public float fuelRate = 0.0f;

    public int warnTimerOxygen = 0;
    public int warnTimerFuel = 0;

    // Set in inspector
    public AudioSource wallCollisionAudio1;
    public AudioSource wallCollisionAudio2;
    private int collisions = 0;


    public AvatarAccounting avatarScript;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: get values of variables from save data once implemented
        //wallCollision = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        regOxygen = oxygenDisplay.color;
        warnOxygen = new Color(1 - regOxygen.r, 1 - regOxygen.g, regOxygen.b);
        regFuel = fuelDisplay.color;
        warnFuel = new Color(regFuel.r, 1 - regFuel.g, regFuel.b);
    }

    // Update is called once per frame
    void Update()
    {
        if(oxygen <= 0)
        {
            Destroy(PlayerModelTest);
        }
        oxygenDisplay.text = (Mathf.RoundToInt(oxygen)).ToString();
        fuelDisplay.text = (Mathf.RoundToInt(fuel)).ToString();
    }

    //fifty times per second
    void FixedUpdate()
    {
        oxygen -= (oxygenLoseRate * suitDamage) / 50.0f;
        oxygen -= oxygenJetRate / 50.0f;
        fuel -= fuelRate / 50.0f;
        oxygenBar.value = oxygen;
        fuelBar.value = fuel;

        if (oxygen <= 10)
        {
            warnTimerOxygen++;
            if (warnTimerOxygen >= 25)
            {
                if (oxygenDisplay.color == regOxygen)
                {
                    oxygenDisplay.color = warnOxygen;
                    warnTimerOxygen = 0;
                }
                else if(oxygenDisplay.color == warnOxygen)
                {
                    oxygenDisplay.color = regOxygen;
                    warnTimerOxygen = 0;
                }
            }
        }

        if (fuel <= 10)
        {
            warnTimerFuel++;
            if (warnTimerFuel >= 25)
            {
                if (fuelDisplay.color == regFuel)
                {
                    fuelDisplay.color = warnFuel;
                    warnTimerFuel = 0;
                }
                else if(fuelDisplay.color == warnFuel)
                {
                    fuelDisplay.color = regFuel;
                    warnTimerFuel = 0;
                }
            }
        }

    }

    private void OnCollisionEnter(Collision c)
    {
        GameObject collided = c.gameObject;
        if(collided.tag == "Cave")
        {
            hasCollided = true;
            suitDamage++;
            if (collisions % 2 == 0)
            {
                wallCollisionAudio1.Play();
            }
            else
            {
                wallCollisionAudio2.Play();
            }
            collisions++;
            //TODO: add impulse to player depending on speed
        }

        if(collided.tag == "AirTank")
        {
            if (oxygen <= 74)
                oxygen += 25;
            else
                oxygen = 99;

            Destroy(collided);
        }

        if (collided.tag == "FuelTank")
        {
            if (fuel <= 74)
                fuel += 25;
            else
                fuel = 99;
            Destroy(collided);
        }
        if (collided.tag == "Monster")
            Destroy(PlayerModelTest);
    }
}
