using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChallengeModeControl : MonoBehaviour
{
    #region Private Fields

    private GameObject challengeMode;
    private GameMenuControl gameMenuControl;        // used to return (player pressed "Back" button)
    private AvatarAccounting avatarAccounting;
    private PlayerInputManager playerInputManager;
    private ChallengeMode initialChallengeMode;

    #endregion Private Fields


    #region Unity Events

    private void Awake()
    {
        challengeMode = GameObject.FindGameObjectWithTag("ChallengeMode");
        this.gameMenuControl = Camera.main.GetComponent<GameMenuControl>();
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();

        this.SetChallengeModeInactive(true);
    }

    #endregion Unity Events


    #region Public Methods

    public void SetChallengeModeActive()
    {
        // Pause game scene and display the message
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        this.challengeMode.SetActive(true);
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.None;

        // Capture initial mode to know whether to save changes or not
        this.initialChallengeMode = this.playerInputManager.PlayerConfig.PlayerChallengeMode;
    }

    public void SetChallengeModeInactive(bool calledInternally = false)
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        this.challengeMode.SetActive(false);

        // If ChallengeMode changed, save it
        if (this.initialChallengeMode != this.playerInputManager.PlayerConfig.PlayerChallengeMode)
        {
            this.playerInputManager.SavePlayerConfiguration();
            this.avatarAccounting.SetPlayerChallengeMode(this.playerInputManager.PlayerConfig.PlayerChallengeMode);
        }

        // Return to caller (but not if being called internally)
        this.gameMenuControl.SetGameMenuActive();
    }

    #endregion Public Methods
}
