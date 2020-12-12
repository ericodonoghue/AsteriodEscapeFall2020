using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* Update Notes:
 
    Call SetChallengeLevel() to set the player's selected "level", or to "Override" their selected level
        NOTE: Override is passed a percentage, that should range from 1 to maybe 1.5, which would result in O2
        costs of 150%.

    Call examples:
        // default setting (other modes are the same, but with different ChallengeMode argument)
        this.avatarAccounting.ChallengeMode(ChallengeMode.TooYoungToDie);

        // Override for "challenge" levels within the overall challenge level (this keeps the setting)
        // This stores the current ChallengeMode so it can return to it later, and sets values to 110%
        this.avatarAccounting.ChallengeMode(ChallengeMode.OverrideOn, 1.1f);

        // This restores ChallengeMode to the value stored by the call with ChallengeMode.OverrideOn
        this.avatarAccounting.ChallengeMode(ChallengeMode.OverrideOff);

*/


/* Using members of this class from another game object is accomplished by including the following code in the calling module:

    // Define a local (unshared) variable to hold a reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;

    private void Start()
    {
      // Get a reference to the AvatarAccounting component of Main Camera
      this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
    }   



    //
    // Sample method calls:
    //

    // Fire a specific jet. NOTE: the jet will continue burning oxygen until you call TerminateJet() on that same jet
    USES enum JetType { MainThruster, AttitudeJetLeft, AttitudeJetRight, AttitudeJetUp, AttitudeJetDown }
    avatarAccounting.FireJet(JetType.MainThruster);       // on KeyDown
    avatarAccounting.TerminateJet(JetType.MainThruster);  // on KeyUp
    avatarAccounting.TerminateAllJets();                  // Called with out of air, but could be used externally to shut all off

    // Normal oxygen refill
    USES enum OxygenTankRefillAmount { FivePercent, TenPercent, FullSingleTank, FillBothTanks }  
    avatarAccounting.AddOxygen(OxygenTankRefillAmount.FivePercent);   // Called repeatedly to simulate slow filling
    avatarAccounting.AddOxygen(OxygenTankRefillAmount.FillBothTanks); // Called once to simulate replacing tanks

    // Adds an extra tank buff called a "Pony Bottle" that will disappear at expiration (empty)
    avatarAccounting.AddOxygenExtraTank();

    // Add damage to the avatar's spacesuit
    USES enum InjuryType { WallStrikeGlancingBlow, WallStrikeDirect, WallStrikeNearMiss, SharpObject, SharpObjectNearMiss, EnemyAttack, EnemyAttackNearMiss }
    avatarAccounting.AddInjury(InjuryType injuryType, float velocity);

    // Just what it says, O2 is low, find a "gas" station
    USES enum OxygenTankRefillAmount { FivePercent, TenPercent, FullSingleTank, FillBothTanks }
    avatarAccounting.AddOxygen(OxygenTankRefillAmount oxygenTankRefillAmount)

    // Adds a pony bottle, once it's used up, it should be removed (has no further affect here)
    avatarAccounting.AddOxygenExtraTank()



    //
    // Public Properties (read only)
    //

    // Monitor this enum property to know when the avatar blacks out (as good as dead)
    avatarAccounting.PlayerFailState

    // Values used to drive UX gauges, etc.
    avatarAccounting.CurrentBloodOxygenPercent
    avatarAccounting.CurrentHeartRatePerMinute
    avatarAccounting.CurrentJetBurnRatePerSecond
    avatarAccounting.CurrentOxygenAllTanksContent
    avatarAccounting.CurrentOxygenBurnRatePerSecond
    avatarAccounting.CurrentRespirationRatePerMinute
    avatarAccounting.CurrentSuitIntegrityInPercentage

    // Secondary properties used internally, but available for detail
    avatarAccounting.CurrentOxygenPonyBottleContent
    avatarAccounting.CurrentOxygenTankContent
    avatarAccounting.CurrentRespirationBurnRatePerSecond
    avatarAccounting.CurrentSuitIntegrityDamageCostPerSecond


    //
    // UNDER CONSTRUCTION
    //

    avatarAccounting.RepairInjury(float suitIntegrityIncrease) { this.RepairInjury(suitIntegrityIncrease, false); }
    avatarAccounting.RepairInjury(float suitIntegrityIncrease, bool allowIncreaseOverBase)
    avatarAccounting.CalmDown(CalmingType calmingType)

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


#region Public Enums

//
// Enums used for parameter arguments to public methods
//
public enum JetType { MainThruster, AttitudeJetLeft, AttitudeJetRight, AttitudeJetUp, AttitudeJetDown, AttitideJetReverse }
public enum InjuryType { WallStrikeGlancingBlow, WallStrikeDirect, WallStrikeNearMiss, SharpObject, SharpObjectNearMiss, EnemyAttack, EnemyAttackNearMiss }
public enum CalmingType { ControlBreathing, Rest, MedicalInducement }
public enum OxygenTankRefillAmount { FivePercent, TenPercent, FullSingleTank, FillBothTanks }
public enum RepairInjuryAmount { OnePercent, FivePercent, TenPercent }
public enum Gauges { OxygenTank, OxygenTankExtra, HeartBeat, BloodOxygen, OxygenBurnRate }
public enum ChallengeMode { TooYoungToDie, BringThePain, GaspingForAir, OverrideOn, OverrideOff }
public enum PlayerFailStates { StillKicking, Hypoxemia, Hypothermia, BluntForceTrauma }

#endregion Public Enums


public class AvatarAccounting : MonoBehaviour
{
    #region Private Variables

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Local variables for internal tracking and processing
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Overall Oxygen burn rate, and Oxygen remaining
    private float currentOxygenTankContent = 0.0f;
    private float currentOxygenPonyBottleContent = 0.0f;    // Extra tank "buff"  

    private bool buffPresentExtraTank = false;              // place holder for future addition of buffs like "Extra Tank"
    private bool buffPresentExtraSuitIntegrity = false;     // place holder for future addition of buffs
    private int nextUpdate = 1;                             // Used to force certain updates to occur every second, not every frame
    private float fixedUpdateRate = 50f;
    private float lastTimeJetsFired = 0f;                   // Keep track of how long it's been, this supports the auto-heal when not using jets
    private float lastTimeSuitWasDamaged = 0f;              // Keep track of how long it's been, this prevents repeated damage from same hit
    private float timeIntervalForAvatarStabilization = 5f;  // Limit stabilization cost to every five seconds incase they spam the button
    private float timeAvatarLastStabilized = 0f;            // Keep track of how long it's been, this prevents repeated cost from spamming
    private bool jetsCostsOxygen = true;
    private bool suitDamageCostsOxygen = true;
    private ChallengeMode curentChallengeMode = ChallengeMode.TooYoungToDie;
    private float mostRecentImpactWholeDamage = 0f;

    // Heart rate (pulse) increases respiration, aka Oxygen burn rate (see calculation for respiration)
    private float currentHeartRatePerMinute = 65f;          // Player current pulse (permissible range = base to max)

    // How much Oxygen per second is respiration burning?
    private float currentRespirationRatePerMinute = 20.0f;  // Litterally, breathes per minute (permissible range = min to max)
    private float currentRespirationBurnRatePerSecond = 1.0f;   // How much oxygen does the current respiration rate cost

    private float currentGeneralSuitLeakBurnRatePerMinute = 0.0f; // General leak, used by difficulty level system

    // How much is suit damage costing?
    private float currentSuitIntegrityInPercentage = 100.0f;    // Base integrity, does not change with damage (starts at 10, but could be buffed to max)
    private float currentSuitIntegrityCostPerSecond = 0.0f;     // Liters per second leaking (usually small enought to call millilitres)

    // Jets are currently burning...
    private float currentJetBurnRate = 0.0f;
    private Dictionary<JetType, float> currentJetBurnRateSpecificJet = new Dictionary<JetType, float>();

    // Current Blood Oxygen Level (this is character health determinant: 80% = brain death)
    private float currentBloodOxygenPercent = 95.0f;

    // The principal way the player "dies" in this game is to blackout from lack of oxygen, but
    // they can also freeze to death or die due to brutal impacts.  This field is used to hold
    // the current state of the Avatar.
    PlayerFailStates playerFailState = PlayerFailStates.StillKicking;

    #endregion Private Variables


    #region Public Field Variables

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Values for resource management (settable in Inspector for tweeking - values below are guesses)
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //[Header("Set in Inspector")]
    private float baseHeartRatePerMinute = 50;  // Default value
    private float minHeartRatePerMinute = 50;   // Normal pulse, can't go lower
    private float maxHeartRatePerMinute = 200;  // Abnormal pulse, can't go higher (not doing heart attacks in this game)

    private float baseRespirationRatePerMinute = 20.0f;  // Default value
    private float minRespirationRatePerMinute = 20.0f;   // Normal respiration is 16-20 breathes, but we're in space, in a mine, and lost, so 20
    private float maxRespirationRatePerMinute = 50.0f;   // Hard to imagine being able to physically inhale\exhale more than 50 times a minute

    private float baseRespirationBurnRatePerMinute = 60.0f;  // Default value
    private float minRespirationBurnRatePerMinute = 0.0f;    // Normal respiration cost (actually 50, but 60 makes the math simpler, and we're in space)
    private float maxRespirationBurnRatePerMinute = 120.0f;  // Hard to imagine being able to physically inhale\exhale this much, but it's a game

    private float baseGeneralSuitLeakBurnRatePerMinute = 00.0f;  // These values are used by "Challenge Mode" to establish a general leak instead of ramping up respiration
    private float minGeneralSuitLeakBurnRatePerMinute = 0.0f;
    private float maxGeneralSuitLeakBurnRatePerMinute = 0.0f;

    private float baseSuitIntegrityPercentage = 100.0f;  // Default value
    private float minSuitIntegrityPercentage = 0.0f;     // Suit destroyed (no oxygen containment)
    private float maxSuitIntegrityPercentage = 100.0f;   // Suit starts at 100%

    private float minJetBurnRatePerSecond = 0.0f;
    private float maxJetBurnRatePerSecond = 1000.0f;

    private float baseOxygenBurnRatePerMinute = 0.0f;    // Default value
    private float minOxygenBurnRatePerMinute = 0.0f;     // Represents normal respiration rate
    private float maxOxygenBurnRatePerMinute = 1000.0f;  // Cry havok!  And let loose the dogs of war!

    private float baseOxygenTankContent = 6000.0f;       // Default value, full tank
    private float minOxygenTankContent = 0.0f;           // Out of air...
    private float maxOxygenTankContent = 6000.0f;        // This is actually days worth of air...without using jets
    private float minOxygenPonyBottle = 0.0f;            // This is a buff that expires, goes away at 0...
    private float maxOxygenPonyBottle = 1000.0f;         // Default value.  This is a buff that expires, can only have one at a time

    private float baseBloodOxygenPercent = 95.0f;        // Default
    private float minBloodOxygenPercent = 80.0f;         // Min level (brain death occurs below this point)
    private float maxBloodOxygenPercent = 115.0f;        // High level

    // How much Oxygen per second does respiration burn?
    private float oxygenCostRespirationHeatbeatBase = 1.0f;        // Multipler of base burn rate = How much does it cost to breath with normal pulse?
    private float oxygenCostRespirationHeatbeatPlus20bpm = 1.3f;   // How much does it cost to breath with normal pulse + 20bpm?
    private float oxygenCostRespirationHeatbeatPlus40bpm = 1.7f;   // How much does it cost to breath with normal pulse + 40bpm?
    private float oxygenCostRespirationHeatbeatPlus60bpm = 2.0f;   // How much does it cost to breath with normal pulse + 60bpm?
    private float oxygenCostRespirationHeatbeatPlus80bpm = 2.5f;   // How much does it cost to breath with normal pulse + 80bpm?
    private float oxygenCostRespirationHeatbeatPlus100bpm = 3.0f;  // How much does it cost to breath with normal pulse + 100bpm?

    // How does damage to the spacesuit affect oxygen use?
    // Suit Integrity cost is calculated using the closest value, multiplied against actual damage (see calc)
    private float oxygenCostPerMinuteSuitIntegrityMinus10Percent = 100.0f;   // This much damage caused by a minor leak
    private float oxygenCostPerMinuteSuitIntegrityMinus20Percent = 150.0f;   // Getting sloppy bub
    private float oxygenCostPerMinuteSuitIntegrityMinus30Percent = 225.0f;   // Dying is starting to sound like an option
    private float oxygenCostPerMinuteSuitIntegrityMinus40Percent = 325.0f;   // WTF dude, stop running into walls
    private float oxygenCostPerMinuteSuitIntegrityMinus50Percent = 450.0f;   // Ok, you are just hitting walls to see what happens, right?
    private float oxygenCostPerMinuteSuitIntegrityMinus60Percent = 600.0f;   // Doh!
    private float oxygenCostPerMinuteSuitIntegrityMinus70Percent = 725.0f;   // This isn't fun anymore...
    private float oxygenCostPerMinuteSuitIntegrityMinus80Percent = 900.0f;   // That next step is a loser
    private float oxygenCostPerMinuteSuitIntegrityMinus90Percent = 1000.0f;  // Wow!  You're fucked!

    // How does using the jets affect oxygen use?
    private float oxygenCostPerSecondUsingMainThruster = 50;                     // Litres per second - it adds up quick!
    private float oxygenCostPerSecondUsingAttitudeJet = 50;                      // 50% power used by attitude thrusters

    // What happens when the player is "injured"?
    private float injuryEffectWallStrikeGlancingBlow_SuitIntegrityDamage = 5.0f; // Up to 5% Suit Integrity loss (see calculation)
    private float injuryEffectWallStrikeGlancingBlow_HeartRateIncrease = 30.0f;  // Up to 30 bpm pulse increase (see calculation)
    private float injuryEffectWallStrikeDirect_SuitIntegrityDamage = 10.0f;      // Up to 10% Suit Integrity loss (see calculation)
    private float injuryEffectWallStrikeDirect_HeartRateIncrease = 10.0f;        // Up to 40 bpm pulse increase (see calculation)
    private float injuryEffectWallStrikeNearMiss_HeartRateIncrease = 20.0f;      // Up to 20 bpm pulse increase (see calculation)
    private float injuryEffectSharpObject_SuitIntegrityDamage = 30.0f;           // Up to 30% Suit Integrity loss (see calculation)
    private float injuryEffectSharpObject_HeartRateIncrease = 70.0f;             // Up to 70 bpm pulse increase (see calculation)
    private float injuryEffectSharpObjectNearMiss_HeartRateIncrease = 40.0f;     // Up to 40 bpm pulse increase (see calculation)
    private float injuryEffectEnemyAttack_SuitIntegrityDamage = 50.0f;           // Up to 50% Suit Integrity loss (see calculation)
    private float injuryEffectEnemyAttack_HeartRateIncrease = 120.0f;            // Up to 20 bpm pulse increase (see calculation)
    private float injuryEffectEnemyAttackNearMiss_HeartRateIncrease = 80.0f;     // Up to 80 bpm pulse increase (see calculation)

    // How does not having oxygen affect the human brain?  Hypoxemia - low blood oxygen, which leads to brain and tissue death
    private float oxygenTankEmptyHypoxemiaDamagePerSecond = 0.5f;                // Percent of blood oxygen lost per second when the O2 runs out
    private float oxygenTankEmptyRespirationRatePerMinute = 0.0f;                // Can't breathe if you have nothing left in the tank

    // Tank refill and buff values
    private float oxygenTankRefillSingleTank = 0.5f;                            // 50% of MAX oxygen to restore (will be cut off at max when set)
    private float oxygenTankRefillBothTanks = 1.0f;                             // 100% of MAX oxygen to restore (will be cut off at max when set)

    // Auto repair costs
    private float oxygenCostPerSecondAutoRepairingSuit = 50.0f;                 // Litres per second - it adds up quick!

    // Forced Avatar stabilization costs
    private float oxygenCostStabilizeAvatar = 500f;


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

    #endregion Public Field Variables


    #region Event Handlers

    // Start is called before the first frame update
    private void Start()
    {
        // Indicate that the player character is alive (for now)
        this.PlayerFailState = PlayerFailStates.StillKicking;

        this.InitializeLocals();

        // Initialize jet cost tracking
        this.TerminateAllJets();

        this.nextUpdate = Mathf.FloorToInt(Time.time);
        this.lastTimeJetsFired = Time.time;
        this.lastTimeSuitWasDamaged = Time.time;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        this.ProcessCurrentOxygenBurn();

        this.SetGaugeValues();
    }

    private void Update()
    {
        // If jets are burning, set tracker so auto-heal will not work (while jets are burning)
        if (this.CurrentJetBurnRatePerSecond > 0) this.lastTimeJetsFired = Time.time;


        // Provide for Update calls once per second for time-based UX requirements
        if (Time.time >= this.nextUpdate)
        {
            this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
            this.UpdateEverySecond();
        }
    }

    #endregion Event Handlers


    #region Private Methods

    private void InitializeLocals()
    {
        // Set internal tracking vars to "base" (default) values
        this.CurrentHeartRatePerMinute = this.baseHeartRatePerMinute;
        this.CurrentRespirationRatePerMinute = this.baseRespirationRatePerMinute;
        this.CurrentRespirationBurnRatePerSecond = this.baseRespirationBurnRatePerMinute;
        this.CurrentJetBurnRatePerSecond = this.minJetBurnRatePerSecond;
        this.CurrentSuitIntegrityInPercentage = this.baseSuitIntegrityPercentage;
        this.CurrentOxygenTankContent = this.baseOxygenTankContent;
        this.CurrentBloodOxygenPercent = this.baseBloodOxygenPercent;
        this.CurrentGeneralSuitLeakBurnRatePerMinute = this.baseGeneralSuitLeakBurnRatePerMinute;
    }

    // Calculates oxygen used by jets, suit damage, anything by actually breathing (handled elsewhere)
    // and "uses" the current call's worth.  
    // Should be called by FixedUpdate, 50 times per second
    private void ProcessCurrentOxygenBurn()
    {
        // Once the oxygen tank is empty, Sp02 or "Blood Oxygen Level" decreaces until you black out and eventually die of hypoxemia
        if (this.CurrentOxygenAllTanksContent == 0f)
        {
            this.CurrentBloodOxygenPercent -= (this.oxygenTankEmptyHypoxemiaDamagePerSecond * (1f / fixedUpdateRate) * 2);
        }
        else
        {
            // Update current respiration cost
            this.CalculateRespirationBurnRate();


            // Update current suit integrity oxygen loss
            this.CalculateSuitIntegrityBurnRate();


            // Consume oxygen for current consumption rate (1/50th of one second's worth)
            this.UseOxygen(this.CurrentOxygenBurnRatePerSecond / fixedUpdateRate);
        }
    }

    // Calculates oxygen used by respiration
    private void CalculateRespirationBurnRate()
    {
        // Normal Respiration Rate(in space):
        // 60 Litres\hour or 1 L\minute(this is actually 50 per hour, but 60 makes the math easier)
        // 1 Litres\minute = 16.66 milliletre\second

        float heartRateOverrage = (this.CurrentHeartRatePerMinute - this.baseHeartRatePerMinute);
        float heartRateOverrageMultiplier = this.oxygenCostRespirationHeatbeatBase;


        // Calculate Respiration Cost by Pulse:
        // Normal bpm(50) = normal respiration
        // Terrified bpm(150) = increased respiration 300 % (this is just a guess)
        if (heartRateOverrage > 0)
        {
            if (heartRateOverrage >= 100) heartRateOverrageMultiplier = this.oxygenCostRespirationHeatbeatPlus100bpm;
            else if (heartRateOverrage >= 80) heartRateOverrageMultiplier = this.oxygenCostRespirationHeatbeatPlus80bpm;
            else if (heartRateOverrage >= 60) heartRateOverrageMultiplier = this.oxygenCostRespirationHeatbeatPlus60bpm;
            else if (heartRateOverrage >= 40) heartRateOverrageMultiplier = this.oxygenCostRespirationHeatbeatPlus40bpm;
            else if (heartRateOverrage >= 20) heartRateOverrageMultiplier = this.oxygenCostRespirationHeatbeatPlus20bpm;
        }


        // Set the current RATE (breaths per minute) based on pulse (for now)
        this.CurrentRespirationRatePerMinute = (this.baseRespirationRatePerMinute * heartRateOverrageMultiplier);


        // How much Oxygen is consumed PER SECOND by the current respiration RATE?
        this.CurrentRespirationBurnRatePerSecond =
            (this.baseRespirationBurnRatePerMinute * heartRateOverrageMultiplier) / 60;
    }

    private void CalculateSuitIntegrityBurnRate()
    {
        float currentOxygenCostRate = 0f;
        float suitIntegrityLossInPercentage = (this.baseSuitIntegrityPercentage - this.CurrentSuitIntegrityInPercentage);


        // If no damage, or integrity is up due to buff presense, set cost to 0;
        if (suitIntegrityLossInPercentage <= 0) this.currentSuitIntegrityCostPerSecond = 0f;

        // If suit is damaged, calculate the cost
        else
        {
            if (suitIntegrityLossInPercentage >= 10)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus10Percent;
            else if (suitIntegrityLossInPercentage >= 20)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus20Percent;
            else if (suitIntegrityLossInPercentage >= 30)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus30Percent;
            else if (suitIntegrityLossInPercentage >= 40)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus40Percent;
            else if (suitIntegrityLossInPercentage >= 50)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus50Percent;
            else if (suitIntegrityLossInPercentage >= 60)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus60Percent;
            else if (suitIntegrityLossInPercentage >= 70)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus70Percent;
            else if (suitIntegrityLossInPercentage >= 80)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus80Percent;
            else if (suitIntegrityLossInPercentage >= 90)
                currentOxygenCostRate = oxygenCostPerMinuteSuitIntegrityMinus90Percent;

            // Use the constants for "per minute" burn rate, calculate "per second"
            this.currentSuitIntegrityCostPerSecond = (currentOxygenCostRate / 60);
        }
    }

    // Update is called once per SECOND
    private void UpdateEverySecond() // Every second (not every frame)
    {
        // Process DOT, Buff\Debuff Expiration, etc.

        // Auto repair if actually damaged (less than 100%)
        if (this.CurrentSuitIntegrityInPercentage < 100.0f) this.AutoRepair();
    }

    private void AutoRepair()
    {
        // Only allow autorepair if the O2 tanks are not empty
        if (this.CurrentOxygenAllTanksContent > 0)
        {
            // Only repair after one or more seconds of no jet activity
            if ((Time.time - this.lastTimeJetsFired) >= 1)
            {
                // Repairs cost a little air
                this.UseOxygen(this.oxygenCostPerSecondAutoRepairingSuit);

                // Repair 5% - actually 5% of the suit's current integrity.  See RepairInjury() for details.
                this.RepairInjury(RepairInjuryAmount.OnePercent);
            }
        }
    }

    #endregion Private Methods


    #region Public Properties

    #region Public "BurnRate" Properties (actual O2 consumption rates)

    public float CurrentRespirationBurnRatePerSecond
    {
        // Read-Only, calculated inline
        get
        {
            return this.currentRespirationBurnRatePerSecond;
        }
        private set
        {
            if (value > this.maxRespirationBurnRatePerMinute)
                this.currentRespirationBurnRatePerSecond = this.maxRespirationBurnRatePerMinute;
            else if (value <= this.minRespirationBurnRatePerMinute)
                this.currentRespirationBurnRatePerSecond = this.minRespirationBurnRatePerMinute;
            else
                this.currentRespirationBurnRatePerSecond = value;
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

    public float CurrentOxygenBurnRatePerSecond
    {
        // Read-Only, calculated inline
        get
        {
            return
              (
                  this.CurrentRespirationBurnRatePerSecond

                // If suit integrity loss is set up to cost oxygen, apply it now  
                + this.CurrentSuitIntegrityDamageCostPerSecond

                // If Jets are set up to cost oxygen, apply it now  
                + this.CurrentJetBurnRatePerSecond

                // If there is a "General" leak running, apply it now
                + this.CurrentGeneralSuitLeakBurnRatePerSecond
              );
        }
    }

    public float CurrentGeneralSuitLeakBurnRatePerMinute
    {
        get { return this.currentGeneralSuitLeakBurnRatePerMinute; }
        private set
        {
            if (value > this.maxGeneralSuitLeakBurnRatePerMinute)
                this.currentGeneralSuitLeakBurnRatePerMinute = this.maxGeneralSuitLeakBurnRatePerMinute;
            else if (value < this.minGeneralSuitLeakBurnRatePerMinute)
                this.currentGeneralSuitLeakBurnRatePerMinute = this.minGeneralSuitLeakBurnRatePerMinute;
            else
                this.currentGeneralSuitLeakBurnRatePerMinute = value;
        }
    }

    public float CurrentGeneralSuitLeakBurnRatePerSecond
    {
        get { return this.CurrentGeneralSuitLeakBurnRatePerMinute / 60f; }
    }

    #endregion Public "BurnRate" Properties (actual O2 consumption rates)

    #region Public "Rate" Properties used in calculations

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

    #endregion Public "Rate" Properties used in calculations

    /// <summary>
    /// Now that we have a mode where jets may or may not use O2, this method has the
    /// final word on whether jets work, instead of peppering code with checking multiple variables
    /// </summary>
    public bool JetsCanFire
    {
        get
        {
            bool result = false;

            // If current player challenge mode does not use O2 for jets, then of course they can fire
            if (!this.jetsCostsOxygen)
                result = true;

            // If jets are using O2, then make sure there's air in the tank, and the avatar isn't blacked out
            else if ((this.CurrentOxygenAllTanksContent != 0) && (this.PlayerFailState == PlayerFailStates.StillKicking))
                result = true;

            return result;
        }
    }

    public float CurrentOxygenAllTanksContent
    {
        get { return this.CurrentOxygenTankContent + this.CurrentOxygenPonyBottleContent; }
    }

    public float CurrentOxygenTankContent
    {
        get { return this.currentOxygenTankContent; }
        private set
        {
            if (value > this.maxOxygenTankContent)
                this.currentOxygenTankContent = this.maxOxygenTankContent;
            else if (value <= this.minOxygenTankContent)
                this.currentOxygenTankContent = this.minOxygenTankContent;
            else
                this.currentOxygenTankContent = value;
        }
    }

    public float CurrentOxygenPonyBottleContent
    {
        get
        {
            if (this.buffPresentExtraTank) return this.currentOxygenPonyBottleContent;
            else return 0f;
        }
        private set
        {
            // When tank is empty, expire the buff and remove the extra tank
            if (value <= 0f)
            {
                this.buffPresentExtraTank = false;
                this.currentOxygenPonyBottleContent = 0f;
            }

            // Can only set this value if the buff is present (unless wiping out, as above, then who cares)
            else if (this.buffPresentExtraTank)

                // Picking up another pony bottle resets pony bottle tank to full, but does not add a bottle
                // if incoming value is less than currnent, that's ok - this check prevents "adding" O2 this way
                if ((value == this.maxOxygenPonyBottle) || (value < this.currentOxygenPonyBottleContent))
                    this.currentOxygenPonyBottleContent = value;
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
                this.PlayerFailState = PlayerFailStates.Hypoxemia;
            }
            else
                this.currentBloodOxygenPercent = value;


            // Sp02 (Blood Oxygen Level) also impacts heartrate

            // TODO: Speed up or slow down?



        }
    }

    //TODO: Fix how setting this value with a buff works
    public float CurrentSuitIntegrityInPercentage
    {
        get { return this.currentSuitIntegrityInPercentage; }
        private set
        {

            // TODO: Integrity can be higher than max if (this.buffPresentExtraSuitIntegrity == true)
            //       BUT IT CANNOT BE ADDED TO!  Once the integrity drops below the normal max, this buff
            //       drops off (set this.buffPresentExtraSuitIntegrity = false), and allow adding again


            if (value > this.maxSuitIntegrityPercentage)
                this.currentSuitIntegrityInPercentage = this.maxSuitIntegrityPercentage;
            else if (value < this.minSuitIntegrityPercentage)
                this.currentSuitIntegrityInPercentage = this.minSuitIntegrityPercentage;
            else
                this.currentSuitIntegrityInPercentage = value;

            // When damage reaches 0, player dies
            if (this.currentSuitIntegrityInPercentage < 1)
            {
                // If player died because of their most recent impact, decide whether it was the
                // impact (more than 10% in one go), or their suit is just damaged enough that
                // they finally froze to death.
                if (this.mostRecentImpactWholeDamage >= 10.0f)
                    this.PlayerFailState = PlayerFailStates.BluntForceTrauma;
                else
                    this.PlayerFailState = PlayerFailStates.Hypothermia;
            }
        }
    }

    public float CurrentSuitIntegrityDamageCostPerSecond
    {
        // Read-Only
        get
        {
            return (this.suitDamageCostsOxygen ? this.currentSuitIntegrityCostPerSecond : 0);
        }
    }

    public PlayerFailStates PlayerFailState
    {
        get { return this.playerFailState; }
        private set
        {
            playerFailState = value;

            // Ideally, this would raise an event indicating "Game Over", but I believe this value will have to be monitored externally
        }
    }

    public string PlayerFailStateDescription
    {
        get
        {
            string result = string.Empty;

            switch (this.PlayerFailState)
            {
                case PlayerFailStates.StillKicking:
                    result = "Our intrepid hero still lives.  How boring.";
                    break;
                case PlayerFailStates.Hypoxemia:
                    result = "Blacked out from insufficient Blood Oxygen (SpO2)";
                    break;
                case PlayerFailStates.Hypothermia:
                    result = "You died from hypothermia becuase your suit was too badly damaged";
                    break;
                case PlayerFailStates.BluntForceTrauma:
                    result = "You died from blunt force trauma because you hit too many walls or objects";
                    break;
            }

            return result;
        }
    }

    #endregion Public Properties


    #region Public Methods

    #region Public Methods - Jets

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
        // Only worry about O2 if jets are set to burn oxy
        if (this.JetsCanFire)
        {
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
    }

    public void TerminateJet(JetType jetType) { this.TerminateJet(jetType, 0, false); }
    public void TerminateJet(JetType jetType, float powerLevel, bool overburn)
    {
        // Only shut off oxy burn if the jets are set to burn oxy
        if (this.jetsCostsOxygen)
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
    }

    public void TerminateAllJets()
    {
        // Store cost for display
        this.CurrentJetBurnRatePerSecond = 0f;

        // Store cost for tracking and termination (on button up, etc.)
        this.currentJetBurnRateSpecificJet[JetType.MainThruster] = 0f;
        this.currentJetBurnRateSpecificJet[JetType.AttitideJetReverse] = 0f;
        this.currentJetBurnRateSpecificJet[JetType.AttitudeJetDown] = 0f;
        this.currentJetBurnRateSpecificJet[JetType.AttitudeJetLeft] = 0f;
        this.currentJetBurnRateSpecificJet[JetType.AttitudeJetRight] = 0f;
        this.currentJetBurnRateSpecificJet[JetType.AttitudeJetUp] = 0f;
    }

    public void FireAllJetsToStabilizeAvatar()
    {
        // Only costs oxy if jets are using oxygen
        if (this.JetsCanFire)
        {
            // Limit stabilization cost to every five seconds incase they spam the button
            if ((Time.time - this.timeAvatarLastStabilized) >= this.timeIntervalForAvatarStabilization)
            {
                this.UseOxygen(oxygenCostStabilizeAvatar);
                this.timeAvatarLastStabilized = Time.time;
            }
        }
    }

    #endregion Public Methods - Jets


    #region Public Methods - Oxygen

    // Decreaces stored oxygen, or terminates normal usage if tanks are empty
    public void UseOxygen(float howMuch)
    {
        // No, you cannot add oxygen this way
        if (howMuch <= 0) return;


        // Take from the pony bottle first, if applicable.  Any remainder is passed on to the main tank
        if (this.buffPresentExtraTank)
        {
            float howMuchRemovedFromPonyBottle = howMuch;

            // more o2 being used than remaining in the pony bottle, adjust to leave some to take from main tanks
            if (howMuch > this.CurrentOxygenPonyBottleContent)
                howMuch = howMuch - this.CurrentOxygenPonyBottleContent;

            this.CurrentOxygenPonyBottleContent -= howMuchRemovedFromPonyBottle;
        }


        // if there is still a debt to pay, take it from the main tanks (amount does not matter, property will only go to 0)
        if (howMuch > 0) this.CurrentOxygenTankContent -= howMuch;


        // If tanks are all empty, discontinue normal O2 burning operations (player is now blacking out)
        if (this.CurrentOxygenAllTanksContent == 0f)
        {
            // Can't breathe with no air in the tank
            this.CurrentRespirationRatePerMinute = this.oxygenTankEmptyRespirationRatePerMinute;

            // Reset all Jets to inoperative
            this.TerminateAllJets();


            // TODO: Stop leaking air from suit injuries (no air left to give)


            // Set heart rate to min, not breathing
            this.CurrentHeartRatePerMinute = this.minHeartRatePerMinute;
        }
    }

    // Allows for normal refill (allowIncreaseOverBase = false) or buffs (allowIncreaseOverBase = true)
    public void AddOxygen(OxygenTankRefillAmount oxygenTankRefillAmount)
    {
        float percentToFillTanks = 0.0f;


        switch (oxygenTankRefillAmount)
        {
            case OxygenTankRefillAmount.FivePercent:
                // This amount is divided by fixedUpdateRate sos it can be called by FixedUpdate (called 50 times per second) for smooth delivery
                percentToFillTanks = 0.05f / fixedUpdateRate;
                break;
            case OxygenTankRefillAmount.TenPercent:
                // This amount is divided by fixedUpdateRate sos it can be called by FixedUpdate (called 50 times per second) for smooth delivery
                percentToFillTanks = 0.1f / fixedUpdateRate;
                break;
            case OxygenTankRefillAmount.FullSingleTank:
                percentToFillTanks = this.oxygenTankRefillSingleTank;
                break;
            case OxygenTankRefillAmount.FillBothTanks:
                percentToFillTanks = this.oxygenTankRefillBothTanks;
                break;
        }

        // Normal refill operation, just add to tank (up to max - constrained in property setter)
        this.CurrentOxygenTankContent += (this.maxOxygenTankContent * percentToFillTanks);
    }

    public void AddOxygenExtraTank()
    {
        // If extra tank is already present, cannot add another
        if (!this.buffPresentExtraTank)
        {
            this.buffPresentExtraTank = true;
            this.currentOxygenPonyBottleContent = this.maxOxygenPonyBottle;
        }
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

    #endregion Public Methods - Oxygen

    #endregion Public Methods





    // YOU ARE HERE

    public void SetPlayerChallengeMode(ChallengeMode challengeMode, float overrideChallengeModeByPercentage = 0f)
    {
        // If turning off Override, just re-set to original ChallengeMode
        if (challengeMode == ChallengeMode.OverrideOff)
            challengeMode = this.curentChallengeMode;

        // Capture given challengeMode to allow Override to return to normal after (see above)
        else if (challengeMode != ChallengeMode.OverrideOn)
            this.curentChallengeMode = challengeMode;


        // Set base values
        switch (challengeMode)
        {
            case ChallengeMode.TooYoungToDie:

                // This constant O2 burn comes from a bad regulartor or other permanent leak in the O2 system
                this.baseGeneralSuitLeakBurnRatePerMinute = 1200f;
                this.minGeneralSuitLeakBurnRatePerMinute = 0f;
                this.maxGeneralSuitLeakBurnRatePerMinute = 1200f;
                this.CurrentGeneralSuitLeakBurnRatePerMinute = this.baseGeneralSuitLeakBurnRatePerMinute;

                // Oxygen tanks max content
                this.baseOxygenTankContent = 6000.0f;
                this.maxOxygenTankContent = 6000.0f;
                this.maxOxygenPonyBottle = 2000.0f;

                // How does using the jets affect oxygen use?
                this.jetsCostsOxygen = false;
                this.oxygenCostPerSecondUsingMainThruster = 0f;
                this.oxygenCostPerSecondUsingAttitudeJet = 0f;

                // Forced Avatar stabilization costs
                this.oxygenCostStabilizeAvatar = 250f;

                this.suitDamageCostsOxygen = false;

                // Eliminate the "blackout" scene for this challenge level
                this.oxygenTankEmptyHypoxemiaDamagePerSecond = 15.0f;

                // Should also reduce the amount of suit damage on impact, but that requires actual code

                // Turn on\off "Glancing Blow" and "Near Miss" metrics?

                break;

            case ChallengeMode.BringThePain:

                // This constant O2 burn comes from a bad regulartor or other permanent leak in the O2 system
                this.baseGeneralSuitLeakBurnRatePerMinute = 800f;
                this.minGeneralSuitLeakBurnRatePerMinute = 0f;
                this.maxGeneralSuitLeakBurnRatePerMinute = 800f;
                this.CurrentGeneralSuitLeakBurnRatePerMinute = this.baseGeneralSuitLeakBurnRatePerMinute;

                // Oxygen tanks max content
                this.baseOxygenTankContent = 6000.0f;
                this.maxOxygenTankContent = 6000.0f;
                this.maxOxygenPonyBottle = 1500.0f;

                // How does using the jets affect oxygen use?
                this.jetsCostsOxygen = true;
                this.oxygenCostPerSecondUsingMainThruster = 50.0f;
                this.oxygenCostPerSecondUsingAttitudeJet = 50.0f;

                // Forced Avatar stabilization costs
                this.oxygenCostStabilizeAvatar = 500f;

                this.suitDamageCostsOxygen = true;

                // Make the "blackout" scene last 5 seconds
                this.oxygenTankEmptyHypoxemiaDamagePerSecond = 3.0f;

                break;

            case ChallengeMode.GaspingForAir:

                // This constant O2 burn comes from a bad regulartor or other permanent leak in the O2 system
                this.baseGeneralSuitLeakBurnRatePerMinute = 500f;
                this.minGeneralSuitLeakBurnRatePerMinute = 0f;
                this.maxGeneralSuitLeakBurnRatePerMinute = 500f;
                this.CurrentGeneralSuitLeakBurnRatePerMinute = this.baseGeneralSuitLeakBurnRatePerMinute;

                // Oxygen tanks max content
                this.baseOxygenTankContent = 6000.0f;
                this.maxOxygenTankContent = 6000.0f;
                this.maxOxygenPonyBottle = 1000.0f;

                // How does using the jets affect oxygen use?
                this.jetsCostsOxygen = true;
                this.oxygenCostPerSecondUsingMainThruster = 100.0f;
                this.oxygenCostPerSecondUsingAttitudeJet = 100.0f;

                // Forced Avatar stabilization costs
                this.oxygenCostStabilizeAvatar = 1000f;

                this.suitDamageCostsOxygen = true;

                // Make the "blackout" scene last 5 seconds
                this.oxygenTankEmptyHypoxemiaDamagePerSecond = 3.0f;

                break;

            case ChallengeMode.OverrideOn:

                // This constant O2 burn comes from a bad regulartor or other permanent leak in the O2 system
                this.CurrentGeneralSuitLeakBurnRatePerMinute *= overrideChallengeModeByPercentage;

                this.baseOxygenTankContent = 6000.0f;
                this.maxOxygenTankContent = 6000.0f;

                this.maxOxygenPonyBottle = ((this.maxOxygenPonyBottle * overrideChallengeModeByPercentage) - this.maxOxygenPonyBottle);
                

                // How does using the jets affect oxygen use?
                this.oxygenCostPerSecondUsingMainThruster *= overrideChallengeModeByPercentage;
                this.oxygenCostPerSecondUsingAttitudeJet *= overrideChallengeModeByPercentage;

                // Forced Avatar stabilization costs
                this.oxygenCostStabilizeAvatar *= overrideChallengeModeByPercentage;

                break;
        }


        // Update local properties to behave in accordance with new values
        this.InitializeLocals();
    }


    #region Public Methods - Space Suit Maintenance

    // Add damage to the player's suit, or just scare the shit out of them and increase their heartrate and respiration
    public void AddInjury(InjuryType injuryType) { this.AddInjury(injuryType, 0);  }
    public void AddInjury(InjuryType injuryType, float velocity, float angle)
    {
        //// Calculate damage using velocity, angle - this does not account for sharp objects, just walls, etc.
        //float impactDamagePercentage = velocity * angle; // maybe some actual math here...
        //this.AddInjury(injuryType, impactDamagePercentage);
    }
    public void AddInjury(float damage) 
    {
        float newSuitIntegrityDamage = 0.0f;
        float newHeartRateIncrease = damage / 10;//??
        float callTime = Time.time;
        float mostRecentImpactWholeDamage = damage;

        if (damage > 0f) newSuitIntegrityDamage = damage;
        float actualDamage = this.currentSuitIntegrityInPercentage * (newSuitIntegrityDamage / 100);

        if (actualDamage > 0)
        {
            this.lastTimeSuitWasDamaged = callTime;
            this.CurrentSuitIntegrityInPercentage -= actualDamage;
            this.CurrentSuitIntegrityInPercentage -= damage / 4;//modifier for difficulty? 5, 4, 3
            this.currentHeartRatePerMinute += newHeartRateIncrease;
            //calm down after time?
        }
    }
    public void AddInjury(InjuryType injuryType, float damagePercentageOverride)
    {
        float newSuitIntegrityDamage = 0.0f;
        float newHeartRateIncrease = 0.0f;
        float callTime = Time.time;

        if ((callTime - this.lastTimeSuitWasDamaged) >= 1)
        {

            switch (injuryType)
            {
                // Get "base" damage occurring from injury
                case InjuryType.WallStrikeGlancingBlow:
                    newSuitIntegrityDamage = this.injuryEffectWallStrikeGlancingBlow_SuitIntegrityDamage;
                    newHeartRateIncrease = this.injuryEffectWallStrikeGlancingBlow_HeartRateIncrease;
                    break;
                case InjuryType.WallStrikeDirect:
                    newSuitIntegrityDamage = this.injuryEffectWallStrikeDirect_SuitIntegrityDamage;
                    newHeartRateIncrease = this.injuryEffectWallStrikeDirect_HeartRateIncrease;
                    break;
                case InjuryType.WallStrikeNearMiss:
                    newSuitIntegrityDamage = 0.0f;
                    newHeartRateIncrease = this.injuryEffectWallStrikeNearMiss_HeartRateIncrease;
                    break;
                case InjuryType.SharpObject:
                    newSuitIntegrityDamage = this.injuryEffectSharpObject_SuitIntegrityDamage;
                    newHeartRateIncrease = this.injuryEffectSharpObject_HeartRateIncrease;
                    break;
                case InjuryType.SharpObjectNearMiss:
                    newSuitIntegrityDamage = 0.0f;
                    newHeartRateIncrease = this.injuryEffectSharpObjectNearMiss_HeartRateIncrease;
                    break;
                case InjuryType.EnemyAttack:
                    newSuitIntegrityDamage = this.injuryEffectEnemyAttack_SuitIntegrityDamage;
                    newHeartRateIncrease = this.injuryEffectEnemyAttack_HeartRateIncrease;
                    break;
                case InjuryType.EnemyAttackNearMiss:
                    newSuitIntegrityDamage = 0.0f;
                    newHeartRateIncrease = this.injuryEffectEnemyAttackNearMiss_HeartRateIncrease;
                    break;
                default:
                    break;
            }


            // Override calculated suit integrity damage if damage value was given
            if (damagePercentageOverride > 0f) newSuitIntegrityDamage = damagePercentageOverride;


            // TODO: Is this correct?


            // Suit damage is in percent of current integrity (can't take away 70% of something that only has 30%,
            // but you can take 70% OF THAT 30% since the suit is waving around it is harder to damage what is left)
            float actualDamage = this.currentSuitIntegrityInPercentage * (newSuitIntegrityDamage / 100);

            if (actualDamage > 0)
            {
                // Update the last time the suit took damage to right now!
                this.lastTimeSuitWasDamaged = callTime;

                // Apply the actual damage
                this.CurrentSuitIntegrityInPercentage -= actualDamage;
            }



            // TODO: YOU ARE HERE!!!

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


            // Alter actual damage based on the angle and velocity of impact
            //this.CurrentSuitIntegrity += newSuitIntegrityDamage;


            //// Emotional damage is also altered based on angle and speed becuase a "near miss" is much worse at high speed
        }
    }

    // Allows for normal repairs (allowIncreaseOverBase = false) or buffs (allowIncreaseOverBase = true)
    public void RepairInjury(RepairInjuryAmount repairInjuryAmount)
    {
        float repairPercentage = 0f;


        switch (repairInjuryAmount)
        {
            case RepairInjuryAmount.OnePercent:
                repairPercentage = 0.01f;
                break;
            case RepairInjuryAmount.FivePercent:
                repairPercentage = 0.05f;
                break;
            case RepairInjuryAmount.TenPercent:
                repairPercentage = 0.1f;
                break;
        }

        // Like damage, suit repairs are done in percent of current integrity(can't improve something by 70% if
        // it is already at 90%, and you can't repair something that is flapping around in the breeze without
        // difficulty.  Repair percentage is applied the same way damage applies: you can add 5 % or 10 % the
        // current integrity of the suit(10 % of 30 % current integrity = 3 % increase\repair).
        float actualRepair = this.currentSuitIntegrityInPercentage * repairPercentage;

        if ((this.CurrentSuitIntegrityInPercentage + actualRepair) <= this.maxSuitIntegrityPercentage)
            this.CurrentSuitIntegrityInPercentage += actualRepair;
        else
            this.CurrentSuitIntegrityInPercentage += actualRepair;
    }

    #endregion Public Methods - Space Suit Maintenance


    #region Gauge Management

    // OxygenTank Gauge: Assign a Gauge "pointer" in the Inspector to rotate around
    public GameObject GaugePointer_OxygenTank;
    public float GaugePointerMin_OxygenTank;
    public float GaugePointerMax_OxygenTank;

    // OxygenTankExtra Gauge: Assign a Gauge "pointer" in the Inspector to rotate around
    public GameObject GaugePointer_OxygenTankExtra;
    public float GaugePointerMin_OxygenTankExtra;
    public float GaugePointerMax_OxygenTankExtra;

    // OxygenTankExtra Gauge: Assign a Gauge "pointer" in the Inspector to rotate around
    public GameObject GaugePointer_OxygenBurnRate;
    public float GaugePointerMin_OxygenBurnRate;
    public float GaugePointerMax_OxygenBurnRate;

    // OxygenTankExtra Gauge: Assign a Gauge "pointer" in the Inspector to rotate around
    public GameObject GaugePointer_BloodOxygen;
    public float GaugePointerMin_BloodOxygen;
    public float GaugePointerMax_BloodOxygen;

    // HeartBeat Gauge: Assign a Gauge "pointer" in the Inspector to rotate around
    public GameObject HeartBeatGauge;
    public float HeartBeatGaugeMinBPM;
    public float HeartBeatGaugeMaxBPM;

    private void SetGaugeValue(GameObject gaugePointer, float value, float valueMin, float valueMax, float gaugePointerMin, float gaugePointerMax)
    {
        if (gaugePointer != null)
        {
            // Needle cannot move past min\max values
            if (value < valueMin) value = valueMin;
            if (value > valueMax) value = valueMax;

            // Calculate the "range" (max-min) for the value and the pointer
            float valueRange = (valueMax - valueMin);
            float pointerRange = (gaugePointerMax - gaugePointerMin);

            // Calculate the point in "range" where target "value" belongs (in percentage)
            float valuePercentageOfRange = (value / valueRange);

            // Assign the pointer's "rotation" to the percentage of the pointerRange that matches the value percentage
            float newPointerRotation = ((pointerRange * valuePercentageOfRange) + gaugePointerMin);
            gaugePointer.transform.localRotation = Quaternion.Euler(0, newPointerRotation, 0);

            print(this.CurrentOxygenTankContent);
        }
    }
    
    private void SetGaugeValues()
    {
        SetGaugeValue(
                  GaugePointer_OxygenTank
                , this.CurrentOxygenTankContent
                , this.minOxygenTankContent
                , this.maxOxygenTankContent
                , GaugePointerMin_OxygenTank
                , GaugePointerMax_OxygenTank
            );

        SetGaugeValue(
                  GaugePointer_OxygenTankExtra
                , this.CurrentOxygenPonyBottleContent
                , this.minOxygenPonyBottle
                , this.maxOxygenPonyBottle
                , GaugePointerMin_OxygenTankExtra
                , GaugePointerMax_OxygenTankExtra
            );

        SetGaugeValue(
                  GaugePointer_OxygenBurnRate
                , this.CurrentOxygenBurnRatePerSecond
                , this.minOxygenBurnRatePerMinute
                , this.maxOxygenBurnRatePerMinute
                , this.GaugePointerMin_OxygenBurnRate
                , this.GaugePointerMax_OxygenBurnRate
            );

        SetGaugeValue(
                  GaugePointer_BloodOxygen
                , this.CurrentBloodOxygenPercent
                , this.minBloodOxygenPercent
                , this.maxBloodOxygenPercent
                , this.GaugePointerMin_BloodOxygen
                , this.GaugePointerMax_BloodOxygen
            );


        // Set Heartbeat...

    }

    #endregion Gauge Management
}



/* Keep, just in case
    ///// <summary>
    ///// Like MonoBehavior.Invoke("FunctionName", 2f); but can include params. Usage:
    ///// AvatarAccounting.RunLater( ()=> FunctionName(true, Vector.one, "or whatever parameters you want"), 2f);
    ///// </summary>
    ///// <remarks>
    ///// Plagiarised from https://answers.unity.com/questions/897095/workaround-for-using-invoke-for-methods-with-param.html
    ///// </remarks>
    //private void RunLater(System.Action method, float waitSeconds)
    //{
    //    if (waitSeconds < 0 || method == null) return;
    //    this.StartCoroutine(this.RunLaterCoroutine(method, waitSeconds));
    //}
    //private IEnumerator RunLaterCoroutine(System.Action method, float waitSeconds)
    //{
    //    yield return new WaitForSeconds(waitSeconds);
    //    method();
    //}

 */
