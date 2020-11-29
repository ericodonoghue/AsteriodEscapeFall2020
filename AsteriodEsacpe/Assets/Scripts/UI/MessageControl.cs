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
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.None;

        // Keep track of which message is actively displayed
        this.currentMessageDisplayed = MessageDisplayed.OrientationMessage;
        this.calledExternally = true;
    }

    public void SetOrientationMessageInactive(bool calledInternally = false)
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        this.orientationMessage.SetActive(false);

        // Return to caller (but not if being called internally)
        if ((!calledInternally) && (this.calledExternally))
            this.gameMenuControl.SetGameMenuActive();
    }

    public void SetUrgentMessageActive()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        this.urgentMessage.SetActive(true);
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.None;
    }

    public void SetUrgentMessageInactive()
    {
        Time.timeScale = 1;
        this.urgentMessage.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion Public Methods
}
