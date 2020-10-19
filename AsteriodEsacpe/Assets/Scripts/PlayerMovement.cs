using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    // Local reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;

    private float resetButtonCost = 1000f;

    [Header("Set in Inspector")]
    public string GameSceneName;


    private void Start()
    {
        // Get a reference to the AvatarAccounting component of Main Camera
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
    }

    private bool leftMouseButtonIsDown = false;
    private void Update()
    {
        // Check to see if the player is dead
        this.AreWeDeadYet();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            avatarAccounting.UseOxygen(resetButtonCost);
        }

        //// If flag and actual button state are not the same, do something about it
        //if (leftMouseButtonIsDown != Input.GetMouseButton(0))
        //{
        //    // Left Mouse Button pressed
        //    if ((!leftMouseButtonIsDown) && (Input.GetMouseButton(0)))
        //    {
        //        avatarAccounting.FireJet(JetType.MainThruster);
        //        leftMouseButtonIsDown = true;
        //    }

        //    // Left Mouse Button released
        //    if ((leftMouseButtonIsDown) && (!Input.GetMouseButton(0)))
        //    {
        //        avatarAccounting.TerminateJet(JetType.MainThruster);
        //        leftMouseButtonIsDown = false;
        //    }

        //}


        if (Input.GetKeyDown(KeyCode.Space))
            avatarAccounting.FireJet(JetType.MainThruster);
        if (Input.GetKeyUp(KeyCode.Space))
            avatarAccounting.TerminateJet(JetType.MainThruster);

        if (Input.GetKeyDown(KeyCode.W))
            avatarAccounting.FireJet(JetType.AttitudeJetUp);
        if (Input.GetKeyUp(KeyCode.W))
            avatarAccounting.TerminateJet(JetType.AttitudeJetUp);

        if (Input.GetKeyDown(KeyCode.S))
            avatarAccounting.FireJet(JetType.AttitudeJetDown);
        if (Input.GetKeyUp(KeyCode.S))
            avatarAccounting.TerminateJet(JetType.AttitudeJetDown);

        if (Input.GetKeyDown(KeyCode.A))
            avatarAccounting.FireJet(JetType.AttitudeJetLeft);
        if (Input.GetKeyUp(KeyCode.A))
            avatarAccounting.TerminateJet(JetType.AttitudeJetLeft);

        if (Input.GetKeyDown(KeyCode.D))
            avatarAccounting.FireJet(JetType.AttitudeJetRight);
        if (Input.GetKeyUp(KeyCode.D))
            avatarAccounting.TerminateJet(JetType.AttitudeJetRight);


        // Key down
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            avatarAccounting.FireJet(JetType.AttitudeJetLeft);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            avatarAccounting.FireJet(JetType.AttitudeJetRight);
        }
        // Key up
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            avatarAccounting.TerminateJet(JetType.AttitudeJetLeft);
        if (Input.GetKeyUp(KeyCode.RightArrow))
            avatarAccounting.TerminateJet(JetType.AttitudeJetRight);

        // Key down
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            avatarAccounting.FireJet(JetType.AttitudeJetDown);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            avatarAccounting.FireJet(JetType.AttitudeJetUp);
        }
        // Key up
        if (Input.GetKeyUp(KeyCode.UpArrow))
            avatarAccounting.TerminateJet(JetType.AttitudeJetUp);
        if (Input.GetKeyUp(KeyCode.DownArrow))
            avatarAccounting.TerminateJet(JetType.AttitudeJetDown);
    }

    private void AreWeDeadYet()
    {
        // For now, just restart on blackout
        if (avatarAccounting.PlayerBlackout)
            SceneManager.LoadScene(GameSceneName);
    }

    //private void OnCollisionEnter(Collision c)
    //{
    //  GameObject collided = c.gameObject;

    //  switch(collided.tag)
    //  {
    //    case "Cave":
    //      avatarAccounting.AddInjury(InjuryType.WallStrikeDirect);
    //      break;
    //    case "Cave_GlancingBlow":
    //      avatarAccounting.AddInjury(InjuryType.WallStrikeGlancingBlow);
    //      break;
    //    case "SharpObject":
    //      avatarAccounting.AddInjury(InjuryType.SharpObject);
    //      break;
    //    case "SharpObject_NearMiss":
    //      avatarAccounting.AddInjury(InjuryType.SharpObjectNearMiss);
    //      break;
    //    case "Monster":
    //      avatarAccounting.AddInjury(InjuryType.EnemyAttack);
    //      break;
    //    case "Monster_NearMiss":
    //      avatarAccounting.AddInjury(InjuryType.EnemyAttackNearMiss);
    //      break;
    //    case "AirTank_Single":
    //      avatarAccounting.AddOxygen(OxygenTankRefillAmount.FullSingleTank);
    //      Destroy(collided);
    //      break;
    //    case "AirTank_Double":
    //      avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks);
    //      Destroy(collided);
    //      break;
    //    case "AirTank_PonyBottle":
    //      avatarAccounting.AddOxygenExtraTank();
    //      Destroy(collided);
    //      break;
    //  }
    //}
}