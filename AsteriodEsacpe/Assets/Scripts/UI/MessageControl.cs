using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageControl : MonoBehaviour
{
    #region Private Fields

    private GameObject orientationMessage;
    private GameObject urgentMessage;
    private GameMenuControl gameMenuControl;        // used to return (player pressed "Back" button)
    private PlayerInputManager playerInputManager;

    private enum MessageDisplayed { OrientationMessage, UrgentMessage, None }
    private MessageDisplayed currentMessageDisplayed = MessageDisplayed.None;
    private bool calledExternally = false;

    #endregion Private Fields


    #region Unity Events

    private void Awake()
    {
        this.orientationMessage = GameObject.FindGameObjectWithTag("OrientationMessage");
        this.urgentMessage = GameObject.FindGameObjectWithTag("UrgentMessage");

        this.gameMenuControl = Camera.main.GetComponent<GameMenuControl>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();

        this.SetOrientationMessageInactive(true);
        this.SetUrgentMessageInactive();
    }

    #endregion Unity Events


    #region Public Methods

    public void SetOrientationMessageActive()
    {
        // There can be only one...
        if (this.currentMessageDisplayed == MessageDisplayed.UrgentMessage)
            this.SetUrgentMessageInactive();

        // Pause game scene and display the message
        Time.timeScale = 0;
        this.orientationMessage.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        this.playerInputManager.ActivatePlayerInputMonitoring = false;

        // Keep track of which message is actively displayed
        this.currentMessageDisplayed = MessageDisplayed.OrientationMessage;
        this.calledExternally = true;
    }

    public void SetOrientationMessageInactive(bool calledInternally = false)
    {
        Time.timeScale = 1;
        this.orientationMessage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        this.playerInputManager.ActivatePlayerInputMonitoring = true;

        // Return to caller (but not if being called internally)
        if ((!calledInternally) && (this.calledExternally))
            this.gameMenuControl.SetGameMenuActive();
    }

    public void SetUrgentMessageActive()
    {
        Time.timeScale = 0;
        this.urgentMessage.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        this.playerInputManager.ActivatePlayerInputMonitoring = false;
    }

    public void SetUrgentMessageInactive()
    {
        Time.timeScale = 1;
        this.urgentMessage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        this.playerInputManager.ActivatePlayerInputMonitoring = true;
    }

    #endregion Public Methods
}
