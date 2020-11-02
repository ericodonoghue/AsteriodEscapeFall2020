using UnityEngine;
using UnityEngine.UI;


public class HeadsUpDisplay : MonoBehaviour
{
  // Local reference to the central AvatarAccounting object (held by main camera)
  private AvatarAccounting avatarAccounting;

  private Color warnOxygen;
  private Color regOxygen;


  private void Awake()
  {
    Text valueLabelText = GameObject.Find("OxygenTankValue").GetComponent<Text>();
    regOxygen = valueLabelText.color;
    warnOxygen = new Color(1 - regOxygen.r, 1 - regOxygen.g, regOxygen.b);
  }

  void Start()
  {
    // Get a reference to the AvatarAccounting component of Main Camera
    this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();

    this.UpdateHUD();
  }

  // fifty times per second
  void FixedUpdate()
  {
      // Update global output fields
      this.UpdateHUD();

    //if (oxygen <= 10)
    //{
    //  warnTimerOxygen++;
    //  if (warnTimerOxygen >= 25)
    //  {
    //    if (oxygenDisplay.color == regOxygen)
    //    {
    //      oxygenDisplay.color = warnOxygen;
    //      warnTimerOxygen = 0;
    //    }
    //    else if (oxygenDisplay.color == warnOxygen)
    //    {
    //      oxygenDisplay.color = regOxygen;
    //      warnTimerOxygen = 0;
    //    }
    //  }
    //}
  }

  private void UpdateHUD()
  {
    // Set UX guages
    SetUXData("HeartRate", avatarAccounting.CurrentHeartRatePerMinute, " bpm");
    SetUXData("RespirationRate", avatarAccounting.CurrentRespirationRatePerMinute, " bpm");
    SetUXData("JetBurnRate", avatarAccounting.CurrentJetBurnRatePerSecond, " ml\\s");
    SetUXData("SuitIntegrity", avatarAccounting.CurrentSuitIntegrityInPercentage, " units");
    SetUXData("OxygenBurnRate", avatarAccounting.CurrentOxygenBurnRatePerSecond, " ml\\s");
    SetUXData("OxygenTank", avatarAccounting.CurrentOxygenAllTanksContent, " litres");
    SetUXData("BloodOxygen", avatarAccounting.CurrentBloodOxygenPercent, "%");
  }

  private void SetUXData(string gameObjectNameBase, float value, string tag = "")
  {
    // use gameObjectNameBase to create actual object names
    string valueLabelName = gameObjectNameBase + "Value";
    string valueSliderName = gameObjectNameBase + "Slider";

    // Find a reference to the indicated Text GameObject and use its Text component to assign the given value
    Text valueLabelText = GameObject.Find(valueLabelName).GetComponent<Text>();
    valueLabelText.text = (Mathf.RoundToInt(value)).ToString() + tag;

    // Find a reference to the indicated Slider GameObject
    GameObject valueSliderGameObject = GameObject.Find(valueSliderName);
    Slider valueSlider = valueSliderGameObject.GetComponent<Slider>();
    valueSlider.value = value;
  }
}
