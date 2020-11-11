using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    // Monitor this boolean property to know when the avatar blacks out (as good as dead)
    avatarAccounting.PlayerBlackout

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
public enum JetType { MainThruster, AttitudeJetLeft, AttitudeJetRight, AttitudeJetUp, AttitudeJetDown }
public enum InjuryType { WallStrikeGlancingBlow, WallStrikeDirect, WallStrikeNearMiss, SharpObject, SharpObjectNearMiss, EnemyAttack, EnemyAttackNearMiss }
public enum CalmingType { ControlBreathing, Rest, MedicalInducement }
public enum OxygenTankRefillAmount { FivePercent, TenPercent, FullSingleTank, FillBothTanks }

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
    //private int nextUpdate = 1;                             // Used to force certain updates to occur every second, not every frame

    // Heart rate (pulse) increases respiration, aka Oxygen burn rate (see calculation for respiration)
    private float currentHeartRatePerMinute = 65f;          // Player current pulse (permissible range = base to max)

    // How much Oxygen per second is respiration burning?
    private float currentRespirationRatePerMinute = 20.0f;  // Litterally, breathes per minute (permissible range = min to max)
    private float currentRespirationBurnRatePerSecond = 1.0f;   // How much oxygen does the current respiration rate cost

    // How much is suit damage costing?
    private float currentSuitIntegrityInPercentage = 100.0f;    // Base integrity, does not change with damage (starts at 10, but could be buffed to max)
    private float currentSuitIntegrityCostPerSecond = 0.0f;     // Liters per second leaking (usually small enought to call millilitres)

    // Jets are currently burning...
    private float currentJetBurnRate = 0.0f;
    private Dictionary<JetType, float> currentJetBurnRateSpecificJet = new Dictionary<JetType, float>();

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

    public float baseRespirationBurnRatePerMinute = 60.0f;  // Default value
    public float minRespirationBurnRatePerMinute = 0.0f;    // Normal respiration cost (actually 50, but 60 makes the math simpler, and we're in space)
    public float maxRespirationBurnRatePerMinute = 180.0f;  // Hard to imagine being able to physically inhale\exhale this much, but it's a game

    public float baseSuitIntegrityPercentage = 100.0f;  // Default value
    public float minSuitIntegrityPercentage = 0.0f;     // Suit destroyed (no oxygen containment)
    public float maxSuitIntegrityPercentage = 150.0f;   // Suit starts at 100, but with buffs...

    public float minJetBurnRatePerSecond = 0.0f;
    public float maxJetBurnRatePerSecond = 2000.0f;

    public float baseOxygenBurnRatePerSecond = 20.0f;   // Default value
    public float minOxygenBurnRatePerSecond = 20.0f;    // Represents normal respiration rate
    public float maxOxygenBurnRatePerSecond = 2000.0f;  // Cry havok!  And let loose the dogs of war!

    public float baseOxygenTankContent = 6000.0f;       // Default value, full tank
    public float minOxygenTankContent = 0.0f;           // Out of air...
    public float maxOxygenTankContent = 6000.0f;        // This is actually days worth of air...without using jets
    public float fullOxygenPonyBottle = 1000.0f;        // This is a buff that expires, can only have one at a time

    public float baseBloodOxygenPercent = 95.0f;        // Default
    public float minBloodOxygenPercent = 80.0f;         // Min level (brain death occurs below this point)
    public float maxBloodOxygenPercent = 98.0f;         // High level

    // How much Oxygen per second does respiration burn?
    public float oxygenCostRespirationHeatbeatBase = 1.0f;        // Multipler of base burn rate = How much does it cost to breath with normal pulse?
    public float oxygenCostRespirationHeatbeatPlus20bpm = 1.3f;   // How much does it cost to breath with normal pulse + 20bpm?
    public float oxygenCostRespirationHeatbeatPlus40bpm = 1.7f;   // How much does it cost to breath with normal pulse + 40bpm?
    public float oxygenCostRespirationHeatbeatPlus60bpm = 2.0f;   // How much does it cost to breath with normal pulse + 60bpm?
    public float oxygenCostRespirationHeatbeatPlus80bpm = 2.5f;   // How much does it cost to breath with normal pulse + 80bpm?
    public float oxygenCostRespirationHeatbeatPlus100bpm = 3.0f;  // How much does it cost to breath with normal pulse + 100bpm?

    // How does damage to the spacesuit affect oxygen use?
    // Suit Integrity cost is calculated using the closest value, multiplied against actual damage (see calc)
    public float oxygenCostPerMinuteSuitIntegrityMinus10Percent = 200.0f;   // This much damage caused by a minor leak
    public float oxygenCostPerMinuteSuitIntegrityMinus20Percent = 300.0f;   // Getting sloppy bub
    public float oxygenCostPerMinuteSuitIntegrityMinus30Percent = 450.0f;   // Dying is starting to sound like an option
    public float oxygenCostPerMinuteSuitIntegrityMinus40Percent = 650.0f;   // WTF dude, stop running into walls
    public float oxygenCostPerMinuteSuitIntegrityMinus50Percent = 900.0f;   // Ok, you are just hitting walls to see what happens, right?
    public float oxygenCostPerMinuteSuitIntegrityMinus60Percent = 1200.0f;  // Doh!
    public float oxygenCostPerMinuteSuitIntegrityMinus70Percent = 1550.0f;  // This isn't fun anymore...
    public float oxygenCostPerMinuteSuitIntegrityMinus80Percent = 1800.0f;  // That next step is a loser
    public float oxygenCostPerMinuteSuitIntegrityMinus90Percent = 2000.0f;  // Wow!  You're fucked!

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
    public float injuryEffectSharpObject_HeartRateIncrease = 70.0f;             // Up to 70 bpm pulse increase (see calculation)
    public float injuryEffectSharpObjectNearMiss_HeartRateIncrease = 40.0f;     // Up to 40 bpm pulse increase (see calculation)
    public float injuryEffectEnemyAttack_SuitIntegrityDamage = 50.0f;           // Up to 50% Suit Integrity loss (see calculation)
    public float injuryEffectEnemyAttack_HeartRateIncrease = 120.0f;            // Up to 20 bpm pulse increase (see calculation)
    public float injuryEffectEnemyAttackNearMiss_HeartRateIncrease = 80.0f;     // Up to 80 bpm pulse increase (see calculation)

    // How does not having oxygen affect the human brain?  Hypoxemia - low blood oxygen, which leads to brain and tissue death
    public float oxygenTankEmptyHypoxemiaDamagePerSecond = 0.5f;                // Percent of blood oxygen lost per second when the O2 runs out
    public float oxygenTankEmptyRespirationRatePerMinute = 0.0f;                // Can't breathe if you have nothing left in the tank

    // Tank refill and buff values
    public float oxygenTankRefillSingleTank = 0.5f;                            // Percent of MAX oxygen to restore (will be cut off at max when set)
    public float oxygenTankRefillBothTanks = 1.0f;                             // Percent of MAX oxygen to restore (will be cut off at max when set)


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

        // Set internal tracking vars to "base" (default) values
        this.CurrentHeartRatePerMinute = this.baseHeartRatePerMinute;
        this.CurrentRespirationRatePerMinute = this.baseRespirationRatePerMinute;
        this.CurrentRespirationBurnRatePerSecond = this.baseRespirationBurnRatePerMinute;
        this.CurrentJetBurnRatePerSecond = this.minJetBurnRatePerSecond;
        this.CurrentSuitIntegrityInPercentage = this.baseSuitIntegrityPercentage;
        this.CurrentOxygenTankContent = this.baseOxygenTankContent;
        this.CurrentBloodOxygenPercent = this.baseBloodOxygenPercent;

        // Initialize jet cost tracking
        this.TerminateAllJets();

        //this.nextUpdate = Mathf.FloorToInt(Time.time);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        this.ProcessCurrentOxygenBurn();
    }

    #endregion Event Handlers


    #region Private Methods

    // Calculates oxygen used by jets, suit damage, anything by actually breathing (handled elsewhere)
    // and "uses" one frames worth.
    // Should be called by FixedUpdate, every frame
    private void ProcessCurrentOxygenBurn()
    {
        // This should use actual frame rate, but meh
        float currentFrameRate = 50f;


        // Once the oxygen tank is empty, Sp02 or "Blood Oxygen Level" decreaces until you black out and eventually die of hypoxemia
        if (this.CurrentOxygenAllTanksContent == 0f)
        {
            this.CurrentBloodOxygenPercent -= (this.oxygenTankEmptyHypoxemiaDamagePerSecond * (1f / currentFrameRate) * 2);
        }
        else
        {
            // Update current respiration cost
            this.CalculateRespirationBurnRate();


            // Update current suit integrity oxygen loss
            this.CalculateSuitIntegrityBurnRate();


            // Consume oxygen for current consumption rate (1/50th per frame)
            this.UseOxygen(this.CurrentOxygenBurnRatePerSecond * (1f / currentFrameRate));
        }
    }

    // Calculates oxygen used by respiration, and "uses" one seconds worth.
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
        float suitIntegrityLossInPercentage = (this.CurrentSuitIntegrityInPercentage - this.baseSuitIntegrityPercentage);


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


            this.currentSuitIntegrityCostPerSecond = currentOxygenCostRate;
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
            return (this.currentRespirationBurnRatePerSecond / 60);
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
                + this.CurrentSuitIntegrityDamageCostPerSecond
                + this.CurrentJetBurnRatePerSecond
              );
        }
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
            if (value <= 0)
            {
                this.buffPresentExtraTank = false;
                this.currentOxygenPonyBottleContent = 0f;
            }

            // Can only set this value if the buff is present (unless wiping out, as above, then who cares)
            else if (this.buffPresentExtraTank)

                // Can't "refil" a pony bottle, they are used until empty and discarded
                if ((value <= this.fullOxygenPonyBottle) && (value <= this.currentOxygenPonyBottleContent))

                    // But value can change as air is used (lower value only)
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
                this.PlayerBlackout = true;
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
        }
    }

    public float CurrentSuitIntegrityDamageCostPerSecond
    {
        // Read-Only
        get
        {
            return this.currentSuitIntegrityCostPerSecond;
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
        // No juice, no jet
        if (this.CurrentOxygenAllTanksContent == 0f) return;

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
                percentToFillTanks = 0.05f;
                break;
            case OxygenTankRefillAmount.TenPercent:
                percentToFillTanks = 0.1f / 50;
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
            this.currentOxygenPonyBottleContent = this.fullOxygenPonyBottle;
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

    #region Public Methods - Space Suit Maintenance

    // Add damage to the player's suit, or just scare the shit out of them and increase their heartrate and respiration
    public void AddInjury(InjuryType injuryType, float velocity = 0)
    {
        float newSuitIntegrityDamage = 0.0f;
        float newHeartRateIncrease = 0.0f;

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


        // TODO: Is this correct?


        // Suit damage is in percent of current integrity (can't take away 70% of something that only has 30%,
        // but you can take 70% of the 30% since the suit is waving around it is harder to damage what is left)
        float actualDamage = this.currentSuitIntegrityInPercentage * (newSuitIntegrityDamage / 100);


        // WTF are the suit integrity numbers?  Change names to spell it the fuck out:


        this.currentSuitIntegrityInPercentage += actualDamage;



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

    // Allows for normal repairs (allowIncreaseOverBase = false) or buffs (allowIncreaseOverBase = true)
    public void RepairInjury(float suitIntegrityIncrease) { this.RepairInjury(suitIntegrityIncrease, false); }
    public void RepairInjury(float suitIntegrityIncrease, bool allowIncreaseOverBase)
    {
    }

    #endregion Public Methods - Space Suit Maintenance
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

    //private void Update()
    //{
    //    // Provide for Update calls once per second for time-based UX requirements
    //    if (Time.time >= this.nextUpdate)
    //    {
    //        this.nextUpdate = Mathf.FloorToInt(Time.time) + 1;
    //        this.UpdateEverySecond();
    //    }
    //}

    //// Update is called once per second
    //private void UpdateEverySecond() // Every second (not every frame)
    //{
    //    // Process DOT, Buff\Debuff Expiration, etc.
    //}

 */
