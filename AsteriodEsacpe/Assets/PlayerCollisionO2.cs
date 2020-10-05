using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCollisionO2 : MonoBehaviour
{
    public int timer = 0;
    Color warn;
    Color reg;
    public Text oxygenDisplay;
    public Slider oxygenBar;
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

    // Start is called before the first frame update
    void Start()
    {
        // TODO: get values of variables from save data once implemented
    }

    private void Awake()
    {
        reg = oxygenDisplay.color;
        Debug.Log(reg);
        warn = new Color(1 - reg.r, 1 - reg.g, reg.b);
        Debug.Log(warn);
    }

    // Update is called once per frame
    void Update()
    {
        if(oxygen <= 0)
        {
            Destroy(PlayerModelTest);
        }

        oxygenDisplay.text = (Mathf.RoundToInt(oxygen)).ToString();

        //holding breath?

        //if (oxygen <= -60)
        //{
        //    Destroy(PlayerModelTest);
        //}
    }

    void FixedUpdate()
    {
        oxygen -= (oxygenLoseRate * suitDamage) / 50.0f;
        oxygen -= oxygenJetRate / 50.0f;
        fuel -= fuelRate / 50.0f;
        oxygenBar.value = oxygen;

        if (oxygen <= 10)
        {
            int oxygenRound = Mathf.RoundToInt(oxygen);
            if (oxygenRound % 2 == 0)
                oxygenDisplay.color = warn;
            else if (oxygenRound % 2 == 1)
                oxygenDisplay.color = reg;
        }
    

    }

    private void OnCollisionEnter(Collision c)
    {
        GameObject collided = c.gameObject;
        if(collided.tag == "Cave")
        {
            suitDamage++;
            //TODO: add impulse to player depending on speed
        }

        if(collided.tag == "OxygenTank")
        {
            if (oxygen <= 74)
                oxygen += 25;
            else
                oxygen = 99;
        }

        if (collided.tag == "FuelTank")
        {
            if (fuel <= 74)
                fuel += 25;
            else
                fuel = 99;
        }
        if (collided.tag == "Monster")
            Destroy(PlayerModelTest);
    }
}
