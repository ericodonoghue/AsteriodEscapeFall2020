using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* Using members of this class from another game object is accomplished by including the following code in the calling module:

    // Define a local (unshared) variable to hold a reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;

    private void Start()
    {
      // Get a reference to the AvatarAccounting component of Main Camera
      AvatarAccounting avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
    }   

    // Sample method call:
    avatarAccounting.FireJet(JetType.MainThruster);

*/


/* Technical Documentation (some is from actual research, some is made up or fudged for simplicity)

2x 100 cu ft tank @3500 psi = 6000 litres (normal use in space is 


Normal Respiration Rate (in space):
    60 Litres\hour or 1 L\minute (this is actually 50 per hour, but 60 makes the math easier)
    1 Litres\minute = 16.66 milliletre\second


Heart Rate:
    Normal bpm (50) = normal respiration 
    Terrified bpm (150) = increased respiration 100% (this is just a guess)


Thruster\Jet Usage:
  Primary Jet Usage: Power is scaled from 1-10 by the calling function (how fast do you want to go?), for ONE second.
  The oxygen consumption tracking function is called every second the jet is burning, thus the power level and jet(s) used
  can change moment to moment.

  Directional Thruster Usage: Power is scaled from 0.1 to 2.0

  Consumption = Power*100 = L\second  

*/


//
// Enums used for parameter arguments to public methods
//
public enum JetType { MainThruster, AttitudeJetLeft, AttitudeJetRight, AttitudeJetUp, AttitudeJetDown }
public enum InjuryType { WallStrikeGlancingBlow, WallStrikeDirect, WallStrikeNearMiss, SharpObject, SharpObjectNearMiss, EnemyAttack, EnemyAttackNearMiss }
public enum CalmingType { ControlBreathing, Rest, MedicalInducement }


public class AvatarAccounting : MonoBehaviour
{
  #region Private Variables

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // Local variables for internal tracking and processing
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  private bool buffPresentExtraTank = false;              // place holder for future addition of buffs like "Extra Tank"
  private bool buffPresentExtraSuitIntegrity = false;     // place holder for future addition of buffs
  private int nextUpdate = 1;                             // Used to force certain updates to occur every second, not every frame
  private int tickCountPerMillisecond = 0;                // Used to calculate fractional time costs

  // Heart rate (pulse) increases respiration, aka Oxygen burn rate (see calculation for respiration)
  private float currentHeartRatePerMinute = 65f;          // Player current pulse (permissible range = base to max)

  // How much Oxygen per second is respiration burning?
  private float currentRespirationRatePerMinute = 40.0f;  // Litterally, breathes per minute (permissible range = base to max)
  private float currentRespirationCostPerSecond = 30.0f;  // MilliLiters per second (16.667 ml\s is normal)

  // How much is suit damage costing?
  private float currentSuitIntegrity = 10.0f;             // Base integrity, does not change with damage (starts at 10, but could be buffed to max)
  ///////private float currentSuitIntegrityDamage = 0.0f; // Percentage of damage to ability to retain air
  private float currentSuitIntegrityCostPerSecond = 0.0f; // Milliliters per second leaking

  // Jets are currently burning...
  private float currentJetBurnRate = 0.0f;
  private Dictionary<JetType, float> currentJetBurnRateSpecificJet = new Dictionary<JetType, float>();

  // Overall Oxygen burn rate, and Oxygen remaining
  private float currentOxygenTankContent = 0.0f;

  // Current Blood Oxygen Level (this is character health determinant: 80% = brain death)
  private float currentBloodOxygenPercent = 95.0f;

  // The principal way the player "dies" in this game is to blackout from lack of oxygen, this flag indicates that this has happened
  private bool playerBlackout = false;

  #endregion Private Variables


  #region Public Field Variables

  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // Values for resource management (settable in Inspector for tweeking - values below are guesses)
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  [Header("Set in Inspector")]
  public float baseHeartRatePerMinute = 50;  // Default value
  public float minHeartRatePerMinute = 50;   // Normal pulse, can't go lower
  public float maxHeartRatePerMinute = 200;  // Abnormal pulse, can't go higher (not doing heart attacks in this game)

  public float baseRespirationRatePerMinute = 20.0f;  // Default value
  public float minRespirationRatePerMinute = 10.0f;   // Normal respiration is 16-20 breathes, but we're in space, in a mine, and lost, so 20
  public float maxRespirationRatePerMinute = 50.0f;   // Hard to imagine being able to physically inhale\exhale more than 50 times a minute

  public float baseSuitIntegrity = 10.0f;             // Default value
  public float minSuitIntegrity = 0.0f;               // Suit destroyed (no oxygen containment)
  public float maxSuitIntegrity = 20.0f;              // Suit starts at 10, but with buffs...

  public float minJetBurnRatePerSecond = 0.0f;
  public float maxJetBurnRatePerSecond = 2000.0f;

  public float baseOxygenBurnRatePerSecond = 20.0f;   // Default value
  public float minOxygenBurnRatePerSecond = 20.0f;    // Represents normal respiration rate
  public float maxOxygenBurnRatePerSecond = 2000.0f;  // Cry havok!  And let loose the dogs of war!

  public float baseOxygenTankContent = 6000.0f;       // Default value, full tank
  public float minOxygenTankContent = 0.0f;           // Out of air...
  public float maxOxygenTankContent = 6000.0f;        // This is actually days worth of air...without using jets
  public float maxOxygenWithExtraTank = 9000.0f;      // This is actually days worth of air...without using jets

  public float baseBloodOxygenPercent = 95.0f;        // Default
  public float minBloodOxygenPercent = 80.0f;         // Min level (brain death occurs below this point)
  public float maxBloodOxygenPercent = 98.0f;         // High level

  // How much Oxygen per second does respiration burn?
  public float oxygenCostRespirationMultiplier = 2.0f;  // How much extra does it cost to breath too fast?

  // How does damage to the spacesuit affect oxygen use?
  // Suit Integrity cost is calculated using the closest value, multiplied against actual damage (see calc)
  public float oxygenCostPerMinuteSuitIntegrityMinus10Percent = 200.0f;   // This much damage causes a minor leak
  public float oxygenCostPerMinuteSuitIntegrityMinus20Percent = 400.0f;   // Getting sloppy bub
  public float oxygenCostPerMinuteSuitIntegrityMinus30Percent = 600.0f;   // Dying is starting to sound like an option
  public float oxygenCostPerMinuteSuitIntegrityMinus40Percent = 1000.0f;  // WTF dude, stop running into walls
  public float oxygenCostPerMinuteSuitIntegrityMinus50Percent = 1500.0f;  // Ok, you are just hitting walls to see what happens, right?
  public float oxygenCostPerMinuteSuitIntegrityMinus60Percent = 2000.0f;  // Doh!
  public float oxygenCostPerMinuteSuitIntegrityMinus70Percent = 2500.0f;  // This isn't fun anymore...
  public float oxygenCostPerMinuteSuitIntegrityMinus80Percent = 3500.0f;  // That next step is a loser
  public float oxygenCostPerMinuteSuitIntegrityMinus90Percent = 4000.0f;  // Wow!  You're fucked!

  // How does using the jets affect oxygen use?
  public float oxygenCostPerSecondUsingMainThruster = 250.0f;             // Litres per second - it adds up quick!
  public float oxygenCostPerSecondUsingAttitudeJet = 50.0f;

  // What happens when the player is "injured"?
  public float injuryEffectWallStrikeGlancingBlow_SuitIntegrityDamage = 5.0f; // Up to 5% Suit Integrity loss (see calculation)
  public float injuryEffectWallStrikeGlancingBlow_HeartRateIncrease = 30.0f;  // Up to 30 bpm pulse increase (see calculation)
  public float injuryEffectWallStrikeDirect_SuitIntegrityDamage = 10.0f;      // Up to 10% Suit Integrity loss (see calculation)
  public float injuryEffectWallStrikeDirect_HeartRateIncrease = 10.0f;        // Up to 40 bpm pulse increase (see calculation)
  public float injuryEffectWallStrikeNearMiss_HeartRateIncrease = 20.0f;      // Up to 20 bpm pulse increase (see calculation)
  public float injuryEffectSharpObject_SuitIntegrityDamage = 30.0f;           // Up to 30% Suit Integrity loss (see calculation)
  public float injuryEffectSharpObject_HeartRateIncrease = 70.0f;             // Up to 20 bpm pulse increase (see calculation)
  public float injuryEffectSharpObjectNearMiss_HeartRateIncrease = 40.0f;     // Up to 40 bpm pulse increase (see calculation)
  public float injuryEffectEnemyAttack_SuitIntegrityDamage = 50.0f;           // Up to 50% Suit Integrity loss (see calculation)
  public float injuryEffectEnemyAttack_HeartRateIncrease = 120.0f;            // Up to 20 bpm pulse increase (see calculation)
  public float injuryEffectEnemyAttackNearMiss_HeartRateIncrease = 80.0f;     // Up to 80 bpm pulse increase (see calculation)

  // How does not having oxygen affect the human brain?  Hypoxemia - low blood oxygen, which leads to brain and tissue death
  public float oxygenTankEmptyHypoxemiaDamagePerSecond = 0.5f;                // Percent of blood oxygen lost per second when the O2 runs out
  public float oxygenTankEmptyRespirationRatePerMinute = 0.0f;                // Can't breathe if you have nothing left in the tank


  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  // "Current" values that are set dynamically, available globally for UX, et al
  ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
  [Header("Set Dynamically")]
  // Values used for UX gauges (should probably be floats, not Text, but printed for now)
  public Text UX_CurrentHeartRatePerMinuteText;
  public Text UX_CurrentRespirationRatePerMinuteText;
  public Text UX_CurrentJetBurnRatePerSecondText;
  public Text UX_CurrentSuitIntegrityText;
  public Text UX_CurrentOxygenBurnRatePerSecondText;
  public Text UX_CurrentOxygenTankContentText;
  public Text UX_CurrentBloodOxygenPercentText;

  // These are the vars that should be used externally
  public float UX_CurrentHeartRatePerMinute;
  public float UX_CurrentRespirationRatePerMinute;
  public float UX_CurrentJetBurnRatePerSecond;
  public float UX_CurrentSuitIntegrity;
  public float UX_CurrentOxygenBurnRatePerSecond;
  public float UX_CurrentOxygenTankContent;
  public float UX_CurrentBloodOxygenPercent;

  #endregion Public Field Variables


  #region Event Handlers

  // Start is called before the first frame update
  private void Start()
  {
    // Indicate that the player character is alive (for now)
    this.PlayerBlackout = false;

    // Used to calculate fractional time costs
    DateTime now = DateTime.Now;
    tickCountPerMillisecond = ((int)((TimeSpan)((now.AddMilliseconds(1)) - now)).Ticks);

    // Set internal tracking vars to "base" (default) values
    this.CurrentHeartRatePerMinute = this.baseHeartRatePerMinute;
    this.CurrentRespirationRatePerMinute = this.CurrentRespirationRatePerMinute;
    this.CurrentJetBurnRatePerSecond = this.minJetBurnRatePerSecond;
    this.CurrentSuitIntegrity = this.baseSuitIntegrity;
    this.CurrentOxygenTankContent = this.baseOxygenTankContent;
    this.CurrentBloodOxygenPercent = this.baseBloodOxygenPercent;

    // Initialize jet cost tracking
    this.TerminateAllJets();

    this.UpdateUXData();

    this.nextUpdate = Mathf.FloorToInt(Time.time);
  }

  private void Update()
  {
    // Provide for Update calls once per second for time-based UX requirements
    if (Time.time >= this.nextUpdate)
    {
      this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
      this.UpdateEverySecond();
    }



    // Test code only
#if DEBUG
    DateTime now = DateTime.Now;

    // Key down
    if (Input.GetKeyDown(KeyCode.Space))
      this.FireJet(JetType.MainThruster);
    if (Input.GetKeyUp(KeyCode.Space))
      this.TerminateJet(JetType.MainThruster);

    if (Input.GetKeyDown(KeyCode.W))
      this.FireJet(JetType.AttitudeJetUp);
    if (Input.GetKeyUp(KeyCode.W))
      this.TerminateJet(JetType.AttitudeJetUp);

    if (Input.GetKeyDown(KeyCode.S))
      this.FireJet(JetType.AttitudeJetDown);
    if (Input.GetKeyUp(KeyCode.S))
      this.TerminateJet(JetType.AttitudeJetDown);

    if (Input.GetKeyDown(KeyCode.A))
      this.FireJet(JetType.AttitudeJetLeft);
    if (Input.GetKeyUp(KeyCode.A))
      this.TerminateJet(JetType.AttitudeJetLeft);

    if (Input.GetKeyDown(KeyCode.D))
      this.FireJet(JetType.AttitudeJetRight);
    if (Input.GetKeyUp(KeyCode.D))
      this.TerminateJet(JetType.AttitudeJetRight);

#endif
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    this.CalculateCurrentOxygenCost();

    // Update global output fields
    this.UpdateUXData();
  }

  // Update is called once per second
  private void UpdateEverySecond() // Every second (not every frame)
  {
    // Update Oxygen usage (subtract oxygen from tank every second, based upon current respiration)
    this.BreathOneSecond();


    // Process DOT, Buff\Debuff Expiration, etc.
  }

  #endregion Event Handlers


  #region Private Methods

  private void UpdateUXData()
  {
    // Actual pubic values
    this.UX_CurrentHeartRatePerMinute = this.CurrentHeartRatePerMinute;
    this.UX_CurrentRespirationRatePerMinute = this.CurrentRespirationRatePerMinute;
    this.UX_CurrentJetBurnRatePerSecond = this.CurrentJetBurnRatePerSecond;
    this.UX_CurrentSuitIntegrity = this.CurrentSuitIntegrity;
    this.UX_CurrentOxygenBurnRatePerSecond = this.CurrentOxygenBurnRatePerSecond;
    this.UX_CurrentOxygenTankContent = this.CurrentOxygenTankContent;
    this.UX_CurrentBloodOxygenPercent = this.CurrentBloodOxygenPercent;

    // Simple TEXT UX (should be replaced and these variables removed)
    this.UX_CurrentHeartRatePerMinuteText = SetUXData("HeartRateValue", this.CurrentHeartRatePerMinute.ToString() + " bpm");
    this.UX_CurrentRespirationRatePerMinuteText = SetUXData("RespirationRateValue", this.CurrentRespirationRatePerMinute.ToString() + " bpm");
    this.UX_CurrentJetBurnRatePerSecondText = SetUXData("JetBurnRateValue", this.CurrentJetBurnRatePerSecond.ToString() + " ml\\s");
    this.UX_CurrentSuitIntegrityText = SetUXData("SuitIntegrityValue", this.CurrentSuitIntegrity.ToString() + " units");
    this.UX_CurrentOxygenBurnRatePerSecondText = SetUXData("OxygenBurnRateValue", this.CurrentOxygenBurnRatePerSecond.ToString() + " ml\\s");
    this.UX_CurrentOxygenTankContentText = SetUXData("OxygenTankValue", this.CurrentOxygenTankContent.ToString() + " litres");
    this.UX_CurrentBloodOxygenPercentText = SetUXData("BloodOxygenValue", this.CurrentBloodOxygenPercent.ToString() + "%");
  }

  private Text SetUXData(string gameObjectName, string value)
  {
    // Find a reference to the indicated GameObject
    GameObject objectFinder = GameObject.Find(gameObjectName);

    // Get the Text Component of that GameObject
    Text gameObjectText = objectFinder.GetComponent<Text>();

    // Set the starting number of points to 0
    gameObjectText.text = value;

    return gameObjectText;
  }

  /// <summary>
  /// Like MonoBehavior.Invoke("FunctionName", 2f); but can include params. Usage:
  /// AvatarAccounting.RunLater( ()=> FunctionName(true, Vector.one, "or whatever parameters you want"), 2f);
  /// </summary>
  /// <remarks>
  /// Plagiarised from https://answers.unity.com/questions/897095/workaround-for-using-invoke-for-methods-with-param.html
  /// </remarks>
  private void RunLater(System.Action method, float waitSeconds)
  {
    if (waitSeconds < 0 || method == null) return;
    this.StartCoroutine(this.RunLaterCoroutine(method, waitSeconds));
  }
  private IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds)
  {
    yield return new WaitForSeconds(waitSeconds);
    method();
  }

  private void BreathOneSecond()
  {

    
    // TODO: Calculate how much oxygen to reduce tank by, using current respiration


    // Only breathe if there's air in the tank
    if (this.CurrentOxygenTankContent != 0f) this.CurrentOxygenTankContent -= 1f;
  }

  // Should be called by FixedUpdate, every frame
  private void CalculateCurrentOxygenCost()
  {
    // This should use actual frame rate, but meh
    float currentFrameRate = 50f;

    // Once the oxygen tank is empty, Sp02 or "Blood Oxygen Level" decreaces until you black out and eventually die of hypoxemia
    if (this.CurrentOxygenTankContent == 0f)
    {
      this.CurrentBloodOxygenPercent -= (this.oxygenTankEmptyHypoxemiaDamagePerSecond * (1f / currentFrameRate) * 2);
    }
    else
    {
      // Consume oxygen for current jet consumption rate (1\50th per frame)
      this.CurrentOxygenTankContent -= (this.CurrentJetBurnRatePerSecond * (1f / currentFrameRate));
    }

  }



  // TODO: This proc needs a lot of attention (or to be dropped)
  private void CalculateRespirationCosts()
  {

    // use this.CurrentHeartRatePerMinute to affect respiration somehow
    // Normal bpm(50) = normal respiration
    // Terrified bpm(150) = increased respiration 100 % (this is just a guess)


    // set CurrentRespirationRatePerMinute
    // Normal Respiration Rate(in space):
    // 60 Litres\hour or 1 L\minute(this is actually 50 per hour, but 60 makes the math easier)
    // 1 Litres\minute = 16.66 milliletre\second


    // this.currentRespirationCostPerSecond is replaced with a read only property that does the math


    // this.oxygenCostRespirationMultiplier


    //this.currentRespirationCostPerSecond =
    //  (
    //      this.oxygenCostPerSecond_RespirationBase
    //    * (
    //          1
    //        + this.currentRespirationRateOverrage * this.oxygenCostPerSecond_RespirationMultiplier
    //      )
    //  ) / 60;
  }

  // TODO: This proc needs a lot of attention (or to be dropped)
  // WTF?  Re-think this entire proc
  private void CalculateSuitIntegrityDamage()
  {
    //float currentOxygenCostRate = 0f;

    //// Break it down to avoid 9 if-then statements called when the avatar is dying
    //if (this.currentSuitIntegrityDamage > 0)  // negative values should never occur, and must be avoided in every case
    //{
    //  if (this.currentSuitIntegrityDamage <= 10)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus10Percent;
    //  else if (this.currentSuitIntegrityDamage <= 20)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus20Percent;
    //  else if (this.currentSuitIntegrityDamage <= 30)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus30Percent;
    //  else if (this.currentSuitIntegrityDamage <= 40)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus40Percent;
    //  else if (this.currentSuitIntegrityDamage <= 50)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus50Percent;
    //  else if (this.currentSuitIntegrityDamage <= 60)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus60Percent;
    //  else if (this.currentSuitIntegrityDamage <= 70)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus70Percent;
    //  else if (this.currentSuitIntegrityDamage <= 80)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus80Percent;
    //  else if (this.currentSuitIntegrityDamage <= 90)
    //    currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus90Percent;
    //  else
    //  {
    //    currentOxygenCostRate = this.maxOxygenBurnRatePerSecond;

    //    // THERE IS ALSO THE QUESTION OF NOT BEING ABLE TO ACTUALLY BREATH - THE USAGE RATE\
    //    // DOESN'T REALLY MATTER ONCE YOU CAN'T INHALE ANYWAY...

    //    // SET RESPIRATION RATE TO 0 UNTIL DAMAGE IS REPAIRED OR AVATAR DIES
    //  }
    //}


    //// current cost = adjusted amount of damage per the value captured above
    //// If the current amount of damage is 4.3%, then the 10% damage value is used, but adjusted to
    //// 43% of that value, (43% of 10% = 4.3% cost for 4.3% damage) 
    //this.currentSuitIntegrityCostPerSecond =
    //  (
    //      currentOxygenCostRate
    //    * (this.currentSuitIntegrityDamage / 10)
    //  ) / 60;  // values are in "per minute" ratings, so div 60 to output "per second" value
  }

  #endregion Private Methods


  #region Public Properties

  public float CurrentHeartRatePerMinute
  {
    get { return this.currentHeartRatePerMinute; }
    private set
    {
      if (value > this.maxHeartRatePerMinute)
        this.currentHeartRatePerMinute = this.maxHeartRatePerMinute;
      else if (value < this.minHeartRatePerMinute)
        this.currentHeartRatePerMinute = this.minHeartRatePerMinute;
      else
        this.currentHeartRatePerMinute = value;
    }
  }

  public float CurrentRespirationRatePerMinute
  {
    get { return this.currentRespirationRatePerMinute; }
    private set
    {
      // if out of air, special value applies
      if (value == this.oxygenTankEmptyRespirationRatePerMinute)
        this.currentRespirationRatePerMinute = value;
      else if (value > this.maxRespirationRatePerMinute)
        this.currentRespirationRatePerMinute = this.maxRespirationRatePerMinute;
      else if (value < this.minRespirationRatePerMinute)
        this.currentRespirationRatePerMinute = this.minRespirationRatePerMinute;
      else
        this.currentRespirationRatePerMinute = value;
    }
  }

  public float CurrentRespirationCostPerSecond
  {
    // Read-Only, calculated inline
    get
    {

      // TODO: This is how much respiration occurs per second, not how much oxygen is used

      return (this.currentRespirationRatePerMinute / 60);
    }
  }

  public float CurrentSuitIntegrity
  {
    get { return this.currentSuitIntegrity; }
    private set
    {

      // TODO: Integrity can be higher than max if (this.buffPresentExtraSuitIntegrit == true)
      //       BUT IT CANNOT BE ADDED TO!  Once the integrity drops below the normal max, this buff
      //       drops off (set this.buffPresentExtraSuitIntegrity = false), and allow adding again


      if (value > this.maxSuitIntegrity)
        this.currentSuitIntegrity = this.maxSuitIntegrity;
      else if (value < this.minSuitIntegrity)
        this.currentSuitIntegrity = this.minSuitIntegrity;
      else
        this.currentSuitIntegrity = value;
    }
  }

  public float CurrentSuitIntegrityDamageCostPerSecond
  {
    // Read-Only, calculated inline
    get
    {
      return
        (

          // TODO: Calculate how much air is bleeding off due to suit damage

          this.currentSuitIntegrity
        );
    }
  }

  public float CurrentJetBurnRatePerSecond
  {
    get { return this.currentJetBurnRate; }
    private set
    {
      if (value > this.maxJetBurnRatePerSecond)
        this.currentJetBurnRate = this.maxJetBurnRatePerSecond;
      else if (value < this.minJetBurnRatePerSecond)
        this.currentJetBurnRate = this.minJetBurnRatePerSecond;
      else
        this.currentJetBurnRate = value;
    }
  }

  public float CurrentOxygenTankContent
  {
    get { return this.currentOxygenTankContent; }
    private set
    {

      // TODO: Enable tank buff by adding code using these values.  Reset the flag once extra tank is empty (expired).
      //       this.maxOxygenWithExtraTank
      //       this.buffPresentExtraTank



      if (value > this.maxOxygenTankContent)
        this.currentOxygenTankContent = this.maxOxygenTankContent;
      else if (value <= this.minOxygenTankContent)
      {
        // Tank is empty
        this.currentOxygenTankContent = this.minOxygenTankContent;

        // Can't breathe with no air in the tank
        this.CurrentRespirationRatePerMinute = this.oxygenTankEmptyRespirationRatePerMinute;

        // Reset all Jets to inoperative
        this.TerminateAllJets();



        // TODO: Should probably do something with heart rate, too
        // this.CurrentHeartRatePerMinute = ;

      }
      else
        this.currentOxygenTankContent = value;
    }
  }

  public float CurrentOxygenBurnRatePerSecond
  {
    // Read-Only, calculated inline
    get
    {
      return
        (
            this.CurrentRespirationCostPerSecond
          + this.CurrentSuitIntegrityDamageCostPerSecond
          + this.CurrentJetBurnRatePerSecond
        );
    }
  }

  public float CurrentBloodOxygenPercent
  {
    get { return this.currentBloodOxygenPercent; }
    private set
    {
      if (value > this.maxBloodOxygenPercent)
        this.currentBloodOxygenPercent = this.maxBloodOxygenPercent;
      else if (value < this.minBloodOxygenPercent)
      {
        this.currentBloodOxygenPercent = this.minBloodOxygenPercent;
        this.PlayerBlackout = true;
      }
      else
        this.currentBloodOxygenPercent = value;


      // Sp02 (Blood Oxygen Level) also impacts heartrate

      // TODO: Speed up or slow down?



    }
  }

  public bool PlayerBlackout
  {
    get { return this.playerBlackout; }
    private set
    {
      playerBlackout = value;

      // Ideally, this would raise an event indicating "Game Over", but I believe this value will have to be monitored externally
    }
  }

  #endregion Public Properties


  #region Public Methods

  /// <summary>
  /// FireJet is called every time a jet is fired, every frame the button for that jet is pressed.
  /// </summary>
  /// <remarks>
  /// Jet burn isn't based on fixed time counts like everything else (respiration cost is per second),
  /// but Jets burn for a slice of time as inticated by the beginTime parameter, every time this function is called.
  /// </remarks>
  /// <param>jetType = enum value indicating the type of jet fired.</param>
  /// <param>beginTime = begin time of current burn.</param>
  /// <param>powerLevel = analog power level value for possible future use (ignore for now).</param>
  /// <param>overburn = ability to double jet output - parameter for possible future use (ignore for now).</param>
  /// <returns>New "begin" time used in subsequent calls, as long as the triggering button remains pressed.</returns>
  /// <example>
  ///    // Local variable to track running jets
  ///    float[] beginJetTimes = new float[10];
  ///
  ///    button PRESS event()
  ///    {
  ///      // Button press data should indicate WHICH jet is being fired, jet operation is tracked using the array beginJetTimes[]
  ///      int jetNumberFired = 1; // Set with actual jet number
  ///      JetType jetTypeFired = ((jetNumberFired == 1) ? JetType.MainThruster : JetType.AttitudeJet);
  ///
  ///      // capture time of button down (reset if button released)
  ///      beginJetTimes[jetNumberFired] = Time.fixedTime;
  ///
  ///      // FireJet will return a new "begin time" for subsequent calls to ensure proper fuel costs
  ///      beginJetTimes[jetNumberFired] = avatarAccounting.FireJet(jetTypeFired, beginJetTimes[jetNumberFired]);  // ignore optional params for now
  ///    }
  ///    button RELEASE event()
  ///    {
  ///      // Button press data should indicate WHICH jet is being fired, jet operation is tracked using the array beginJetTimes[]
  ///      int jetNumberFired = 1; // Set with actual jet number
  ///
  ///      // Reset time, jet is no longer burning
  ///      beginJetTimes[jetNumberFired] = 0f;
  ///    }
  /// </example>
  public void FireJet(JetType jetType) { this.FireJet(jetType, 0, false); }
  public void FireJet(JetType jetType, float powerLevel, bool overburn)
  {
    // No juice, no jet
    if (this.CurrentOxygenTankContent == 0f) return;

    // optional param, can be used to set variable power level applied to jets
    if (powerLevel <= 0) powerLevel = 1f;   //100%
    //TODO: else need to adjust incoming value for multiplication below

    // Only fire the jet if it IS NOT already burning
    if (this.currentJetBurnRateSpecificJet[jetType] == 0)
    {
      // Get the cost of jet burn based on the type of jet firing
      float oxygenCostPerSecond =
              (
                  (jetType == JetType.MainThruster)
                ? this.oxygenCostPerSecondUsingMainThruster
                : this.oxygenCostPerSecondUsingAttitudeJet
              ) * powerLevel;

      // Store cost for display
      this.CurrentJetBurnRatePerSecond += oxygenCostPerSecond;

      // Store cost for tracking and termination (on button up, etc.)
      this.currentJetBurnRateSpecificJet[jetType] = oxygenCostPerSecond;
    }





    // TODO: Until these features are implemented, powerLevel and overburn arguments are igored

    // Adjust power level by jetType (main uses 1-10, but attitude jets are more like 0.1 to 1.0)
    // Create an editable value for this - the kind of thing that can be tweaked in testing, or to 
    // increase difficulty, etc. for player level

    // overburn(nice to have: use 2x Oxygen (based on other values) to haul ass away from
    // scary monsters, escape from this stoney hell before dying, etc.)

  }

  public void TerminateJet(JetType jetType) { this.TerminateJet(jetType, 0, false); }
  public void TerminateJet(JetType jetType, float powerLevel, bool overburn)
  {
    // optional param, can be used to set variable power level applied to jets
    if (powerLevel <= 0) powerLevel = 1f;   //100%
    //TODO: else need to adjust incoming value for multiplication below

    // Only terminate the jet if it IS already burning
    if (this.currentJetBurnRateSpecificJet[jetType] != 0)
    {
      // Get the cost of jet burn based on the type of jet firing
      float oxygenCostPerSecond =
            (
                (jetType == JetType.MainThruster)
              ? this.oxygenCostPerSecondUsingMainThruster
              : this.oxygenCostPerSecondUsingAttitudeJet
            ) * powerLevel;

      // Store cost for display
      this.CurrentJetBurnRatePerSecond -= oxygenCostPerSecond;

      // Store cost for tracking and termination (on button up, etc.)
      this.currentJetBurnRateSpecificJet[jetType] = 0f;
    }
  }

  public void TerminateAllJets()
  {
    // Store cost for display
    this.CurrentJetBurnRatePerSecond = 0f;

    // Store cost for tracking and termination (on button up, etc.)
    this.currentJetBurnRateSpecificJet[JetType.MainThruster] = 0f;
    this.currentJetBurnRateSpecificJet[JetType.AttitudeJetDown] = 0f;
    this.currentJetBurnRateSpecificJet[JetType.AttitudeJetLeft] = 0f;
    this.currentJetBurnRateSpecificJet[JetType.AttitudeJetRight] = 0f;
    this.currentJetBurnRateSpecificJet[JetType.AttitudeJetUp] = 0f;
  }

  // Add damage to the player's suit, or just scare the shit out of them and increase their heartrate and respiration
  public void AddInjury(InjuryType injuryType, float velocity)
  {
    float newSuitIntegrityDamage = 0.0f;
    float newHeartRateIncrease = 0.0f;


    //switch (injuryType)
    //{


    // Set up variables for storing damage rates defined in InjuryType enum
    // NOTE: IT may be necessary to calculate the velocity of the avatar at the point of impact
    // by vector, or better, put the math here and add vector parameters to allow encapsulation.


    // Add "near miss" injury types that just increase heart and respiration rates
    // Add increases to heart and respiration rates to all suit injuries


    // Optional param injuryDuration can put a timer on injuries (especially near misses) to remove the injury
    // upon expiration.  This should be an ordered list that is reviewed regularaly elsewhere, and must include
    // the duration, and type\amount of injury so it can be "removed"


    // Velocity: directional force at time of impact(if going 23kph N, but hit NW, velocity is 50 %, right ?)

    // Duration: When impact results in dragging along a wall, etc., how long was the duration?



    //  // Get "base" damage occurring from injury
    //  case InjuryType.WallStrikeGlancingBlow:
    //    newSuitIntegrityDamage = injuryEffectWallStrikeGlancingBlow_SuitIntegrityDamage;
    //    newHeartRateIncrease = injuryEffectWallStrikeGlancingBlow_HeartRateIncrease;
    //    break;
    //  case InjuryType.WallStrikeDirect:
    //    newSuitIntegrityDamage = injuryEffectWallStrikeDirect_SuitIntegrityDamage;
    //    newHeartRateIncrease = injuryEffectWallStrikeDirect_HeartRateIncrease;
    //    break;
    //  case InjuryType.WallStrikeNearMiss:
    //    newSuitIntegrityDamage = 0.0f;
    //    newHeartRateIncrease = injuryEffectWallStrikeNearMiss_HeartRateIncrease;
    //    break;
    //  case InjuryType.SharpObject:
    //    newSuitIntegrityDamage = injuryEffectSharpObject_SuitIntegrityDamage;
    //    newHeartRateIncrease = injuryEffectSharpObject_HeartRateIncrease;
    //    break;
    //  case InjuryType.SharpObjectNearMiss:
    //    newSuitIntegrityDamage = 0.0f;
    //    newHeartRateIncrease = injuryEffectSharpObjectNearMiss_HeartRateIncrease;
    //    break;
    //  case InjuryType.EnemyAttack:
    //    newSuitIntegrityDamage = injuryEffectEnemyAttack_SuitIntegrityDamage;
    //    newHeartRateIncrease = injuryEffectEnemyAttack_HeartRateIncrease;
    //    break;
    //  case InjuryType.EnemyAttackNearMiss:
    //    newSuitIntegrityDamage = 0.0f;
    //    newHeartRateIncrease = injuryEffectEnemyAttackNearMiss_HeartRateIncrease;
    //    break;
    //  default:
    //    break;
    //}



    //// TODO: YOU ARE HERE!!!
    //// Alter actual damage based on the angle and velocity of impact
    //this.currentSuitIntegrityDamage += 25.0f;  // This is in percent of damage to suits ability to hold air


    //// Emotional damage is also altered based on angle and speed becuase a "near miss" is much worse at high speed

  }

  // Allows for normal repairs (allowIncreaseOverBase = false) or buffs (allowIncreaseOverBase = true)
  public void RepairInjury(float suitIntegrityIncrease) { this.RepairInjury(suitIntegrityIncrease, false); }
  public void RepairInjury(float suitIntegrityIncrease, bool allowIncreaseOverBase)
  {
  }

  // Allows for normal refill (allowIncreaseOverBase = false) or buffs (allowIncreaseOverBase = true)
  public void AddOxygen(float oxygenIncrease) { this.AddOxygen(oxygenIncrease, false); }
  public void AddOxygen(float oxygenIncrease, bool increaseOverBase)
  {
    // if placing a buff, set a value that allows tank to hold more than max, until oxygen falls
    // below normal max, at which point the buff "expires", the flag is reset and level is again
    // capped at max

  }

  // Slow breathing and heartrate through rest, meditation or drugs
  public void CalmDown(CalmingType calmingType)
  {
    //switch (calmingType)
    //{
    //  case CalmingType.ControlBreathing:
    //    break;
    //  case CalmingType.MedicalInducement:
    //    break;
    //  case CalmingType.Rest:
    //    break;
    //  default:
    //    break;
    //}
  }

  #endregion Public Methods
}
