using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class PlayerCollisionO2 : MonoBehaviour
{
    // Local reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;

    // Set in inspector
    public AudioSource wallCollisionAudio1;
    public AudioSource wallCollisionAudio2;

    private int collisions = 0;

    private bool inRefuelRange;
    private bool fillTanks;

    private float cdValue;


    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the AvatarAccounting component of Main Camera
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();

        inRefuelRange = false;
        fillTanks = false;
        // TODO: get values of variables from save data once implemented
        //wallCollision = GetComponent<AudioSource>();

        TextMeshProUGUI t = GameObject.FindGameObjectWithTag("PressF").GetComponent<TextMeshProUGUI>();
        t.text = "";
    }


    private void Update()
    {
        if (inRefuelRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                fillTanks = true;
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                fillTanks = false;
            }
        }
        else
        {
            fillTanks = false;
        }

        
    }

    private void FixedUpdate()
    {
        if (fillTanks)
        {
            avatarAccounting.AddOxygen(OxygenTankRefillAmount.TenPercent);
        }
    }

    private void OnCollisionEnter(Collision c)
    {
        GameObject collided = c.gameObject;


        // Choose the best tag to use (cave walls are not tagged, but should belong to a tagged parent)
        while (collided.tag == "Untagged")
        {
            if (collided.transform.parent != null)
            {
                collided = collided.transform.parent.gameObject;
                if (collided.tag != "Untagged") break;
            }
            else break;  // no more parents to bother
        }


        switch (collided.tag)
        {
            case "Cave":
                avatarAccounting.AddInjury(InjuryType.WallStrikeDirect);

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
                break;
            case "AirTank":
                avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks);
                Destroy(collided);
                break;
            case "Cave_GlancingBlow":
                avatarAccounting.AddInjury(InjuryType.WallStrikeGlancingBlow);
                break;
            case "SharpObject":
                avatarAccounting.AddInjury(InjuryType.SharpObject);
                break;
            case "SharpObject_NearMiss":
                avatarAccounting.AddInjury(InjuryType.SharpObjectNearMiss);
                break;
            case "Monster":
                avatarAccounting.AddInjury(InjuryType.EnemyAttack);
                break;
            case "Monster_NearMiss":
                avatarAccounting.AddInjury(InjuryType.EnemyAttackNearMiss);
                break;
            case "AirTank_Single":
                avatarAccounting.AddOxygen(OxygenTankRefillAmount.FullSingleTank);
                Destroy(collided);
                break;
            case "AirTank_Double":
                avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks);
                Destroy(collided);
                break;
            case "AirTank_PonyBottle":
                avatarAccounting.AddOxygenExtraTank();
                Destroy(collided);
                break;
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        GameObject collided = c.gameObject;

        if (collided.tag == "AirTank")
        {
            avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks);
            Destroy(collided);
        }
        if (collided.tag == "RefuelStation")
        {
            inRefuelRange = true;

            TextMeshProUGUI t = GameObject.FindGameObjectWithTag("PressF").GetComponent<TextMeshProUGUI>();
            t.text = "Press F to refill oxygen tanks";
            StartCoroutine(StartCountdown());

        }
    }

    private void OnTriggerExit(Collider c)
    {
        GameObject collided = c.gameObject;
        if (collided.tag == "RefuelStation")
        {
            inRefuelRange = false;
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
        TextMeshProUGUI t = GameObject.FindGameObjectWithTag("PressF").GetComponent<TextMeshProUGUI>();
        t.text = "";
    }

}