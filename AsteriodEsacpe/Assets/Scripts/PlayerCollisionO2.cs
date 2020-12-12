using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;


public class PlayerCollisionO2 : MonoBehaviour
{
    // Local reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;
    public GameObject player;
    public Rigidbody playerRB;
    // Set in inspector
    public AudioSource wallCollisionAudio1;
    public AudioSource wallCollisionAudio2;

    private int collisions = 0;

    private bool inRefuelRange;
    private bool fillTanks;

    private float cdValue;

    private ScoreTracker scoreTracker;

    //
    public UnifiedPlayerMovement unifiedPlayerMovement;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the AvatarAccounting component of Main Camera
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        player = GameObject.FindGameObjectWithTag("Player");
        //MW: CollOxScript = player.GetComponent<PlayerCollisionO2>();
        playerRB = GetComponent<Rigidbody>();
        inRefuelRange = false;
        fillTanks = false;
        // TODO: get values of variables from save data once implemented
        //wallCollision = GetComponent<AudioSource>();

        TextMeshProUGUI t = GameObject.FindGameObjectWithTag("PressF").GetComponent<TextMeshProUGUI>();
        t.text = "";

        scoreTracker = GameObject.FindGameObjectWithTag("ScoreTracker").GetComponent<ScoreTracker>();

        //
        unifiedPlayerMovement = player.GetComponent<UnifiedPlayerMovement>();
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

        ///*
        switch (collided.tag)
        {
            case "Cave":
                scoreTracker.collisionsCount++;
                //avatarAccounting.AddInjury(InjuryType.WallStrikeDirect);
               // UnityEngine.Debug.Log("In CollisionO2 Script");

                if (collisions % 2 == 0)
                {
                    wallCollisionAudio1.Play();
                }
                else
                {
                    wallCollisionAudio2.Play();
                }
                collisions++;

                //just get contact [0]
                /*
                for (int i = 0; i < c.contactCount; i++)
                {
                    ContactPoint contact = c.GetContact(i);
                    //UnityEngine.Debug.Log("total contacts: " + c.contacts.Length);
                    UnityEngine.Debug.Log("position: " + contact.point.x + ", " + contact.point.y + ", " + contact.point.z);
                    UnityEngine.Debug.Log("normal: " + contact.normal.x + ", " + contact.normal.y + ", " + contact.normal.z);
                    UnityEngine.Debug.DrawLine(contact.point, contact.normal, Color.white);
                }
                */

                ContactPoint cPoint = c.GetContact(0);
                Vector3 cNorm= cPoint.normal;
                Vector3 vel = playerRB.velocity;//.normalized;
                Vector3 damage;
                //UnityEngine.Debug.Log("normals: " + cNorm.x + "x, " + cNorm.y +"y, " + cNorm.z + "z");
                //UnityEngine.Debug.Log("velocities: " + vel.x + "x, " + vel.y + "y, " + vel.z + "z");
                damage.x = cNorm.x * vel.x;
                damage.y = cNorm.y * vel.y;
                damage.z = cNorm.z * vel.z;
                //UnityEngine.Debug.Log("damage: " + damage.x + "x, " + damage.y + "y, " + damage.z + "z");

                UnityEngine.Debug.Log("collision found: ");

                UnityEngine.Debug.Log("adding damage: ");
                UnityEngine.Debug.Log("pre-damage" + avatarAccounting.CurrentSuitIntegrityInPercentage);
                avatarAccounting.AddInjury(damage.magnitude);
                UnityEngine.Debug.Log("post-damage" + avatarAccounting.CurrentSuitIntegrityInPercentage);

                //
                if (damage.magnitude > 5)
                {
                    unifiedPlayerMovement.spinOutTime = Time.time + 1;
                    unifiedPlayerMovement.spinOut = true;
                    unifiedPlayerMovement.recoverySpeed = 20f;
                }
                //
                //UnityEngine.Debug.Log(avatarAccounting.damageModifier);
                //TODO: add impulse to player depending on speed
                break;

            case "Cave_GlancingBlow":
                avatarAccounting.AddInjury(InjuryType.WallStrikeGlancingBlow);
                break;
        //*/
            // MW 11-12: No need for these until\if we get back to these scenarios
            //case "SharpObject":
            //    avatarAccounting.AddInjury(InjuryType.SharpObject);
            //    break;
            //case "SharpObject_NearMiss":
            //    avatarAccounting.AddInjury(InjuryType.SharpObjectNearMiss);
            //    break;
            //case "Monster":
            //    avatarAccounting.AddInjury(InjuryType.EnemyAttack);
            //    break;
            //case "Monster_NearMiss":
            //    avatarAccounting.AddInjury(InjuryType.EnemyAttackNearMiss);
            //    break;
            //case "AirTank":
            //    avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks);
            //    Destroy(collided);
            //    break;
            //case "AirTank_Single":
            //    avatarAccounting.AddOxygen(OxygenTankRefillAmount.FullSingleTank);
            //    Destroy(collided);
            //    break;
            //case "AirTank_Double":
            //    avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks);
            //    Destroy(collided);
            //    break;
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        GameObject collided = c.gameObject;


        if (collided.tag == "AirTank_PonyBottle")
        {
            avatarAccounting.AddOxygenExtraTank();//chack
            Destroy(collided);
        }
        if (collided.tag == "RefuelStation")
        {
            inRefuelRange = true;

            TextMeshProUGUI t = GameObject.FindGameObjectWithTag("PressF").GetComponent<TextMeshProUGUI>();
            t.text = "Press F to refill oxygen tanks";
            //StartCoroutine(StartCountdown());
        }
    }

    private void OnTriggerExit(Collider c)
    {
        GameObject collided = c.gameObject;
        if (collided.tag == "RefuelStation")
        {
            TextMeshProUGUI t = GameObject.FindGameObjectWithTag("PressF").GetComponent<TextMeshProUGUI>();
            t.text = "";
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