using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChallengeModeButtonControl : MonoBehaviour
{
    #region Private Fields

    private ChallengeModeControl challengeModeControl;
    private PlayerInputManager playerInputManager;

    #endregion Private Fields


    #region Unity Events

    void Awake()
    {
        this.challengeModeControl = Camera.main.GetComponent<ChallengeModeControl>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        // Initialize button coloring
        this.SetButtonStates();
    }

    #endregion Unity Events


    #region private Methods

    private void SetButtonStates()
    {
        string buttonTag = "";

        // Just turn them all off to start
        this.SetButtonSelectedState(false, "ChallengeMode1Button");
        this.SetButtonSelectedState(false, "ChallengeMode2Button");
        this.SetButtonSelectedState(false, "ChallengeMode3Button");

        // Now figure out which one should be highlighted
        switch (this.playerInputManager.PlayerConfig.PlayerChallengeMode)
        {
            case ChallengeMode.TooYoungToDie:
                buttonTag = "ChallengeMode1Button";
                break;
            case ChallengeMode.BringThePain:
                buttonTag = "ChallengeMode2Button";
                break;
            case ChallengeMode.GaspingForAir:
                buttonTag = "ChallengeMode3Button";
                break;
        }

        // Make it so
        this.SetButtonSelectedState(true, buttonTag);
    }

    private void SetButtonSelectedState(bool active, string buttonTag)
    {
        // Set the text of the button in UI
        Button button = GameObject.FindGameObjectWithTag(buttonTag).GetComponent<Button>();
        ColorBlock colors = button.colors;


        if (active)
        {
            // Set the colors of the button in UI (activate after clicked on, until assignment made)
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
        }
        else
        {
            // Set the colors of the button in UI (deactivate after assignment made)
            colors.normalColor = Color.clear;
            colors.highlightedColor = new Color32(233, 0, 0, 109);
        }


        button.colors = colors;
    }

    #endregion private Methods


    #region Public Scene Event Handlers

    public void BackButtonPressed()
    {
        // return to caller (Game Menu)
        this.challengeModeControl.SetChallengeModeInactive();
    }

    public void ChallengeMode1ButtonPressed()
    {
        this.playerInputManager.PlayerConfig.PlayerChallengeMode = ChallengeMode.TooYoungToDie;
        this.SetButtonStates();
    }

    public void ChallengeMode2ButtonPressed()
    {
        this.playerInputManager.PlayerConfig.PlayerChallengeMode = ChallengeMode.BringThePain;
        this.SetButtonStates();
    }

    public void ChallengeMode3ButtonPressed()
    {
        this.playerInputManager.PlayerConfig.PlayerChallengeMode = ChallengeMode.GaspingForAir;
        this.SetButtonStates();
    }

    #endregion Public Scene Event Handlers
}
