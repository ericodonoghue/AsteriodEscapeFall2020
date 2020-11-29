using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MessageButtonControl : MonoBehaviour
{
    #region Private Fields

    private MessageControl messageControl;
    private PlayerInputManager playerInputManager;

    #endregion Private Fields


    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        this.messageControl = Camera.main.GetComponent<MessageControl>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    #endregion Unity Events


    #region Public Scene Event Handlers

    public void BackButtonPressed()
    {
        // Deactivate the Orientation message form and return to caller (Game Menu)
        this.messageControl.SetOrientationMessageInactive();
    }

    public void MessagesButtonPressed()
    {
        // Deactivate the Orientation message form
        this.messageControl.SetOrientationMessageInactive(true);

        // Activate the Urgent message popup
        this.messageControl.SetUrgentMessageActive();
    }

    public void CloseMessagesButtonPressed()
    {
        // Deactivate the Urgent message popup
        this.messageControl.SetUrgentMessageInactive();

        // Re-activate the Orientation popup
        this.messageControl.SetOrientationMessageActive();
    }

    #endregion Public Scene Event Handlers
}
