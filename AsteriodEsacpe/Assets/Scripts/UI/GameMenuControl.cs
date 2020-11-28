using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMenuControl : MonoBehaviour
{
    #region Private Fields

    private GameObject gameMenu;
    private YouDiedControl youDied;
    private YouWinControl youWin;
    private PlayerInputManager playerInputManager;

    #endregion Private Fields


    #region Public Fields

    public bool isPaused = false;
    public bool isListening = true;

    #endregion Public Fields


    #region Unity Events

    private void Awake()
    {
        this.gameMenu = GameObject.FindGameObjectWithTag("GameMenu");
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
        this.youDied = Camera.main.GetComponent<YouDiedControl>();
        this.youWin = Camera.main.GetComponent<YouWinControl>();


        if (this.isPaused)
        {
            this.SetGameMenuActive();
        }
        else
        {
            this.SetGameMenuInactive();
        }


        // Set up event handlers for Player Input Manager to monitor for menu commands
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.GameMenu, GameMenu_Pressed, GameMenu_Released);
    }

    private void OnDisable()
    {
        // Tear down event handlers for Player Input Manager to monitor for menu commands
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.GameMenu, GameMenu_Pressed, GameMenu_Released);
    }

    #endregion Unity Events


    #region Public Methods

    public void SetGameMenuActive()
    {
        Time.timeScale = 0;
        this.gameMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        this.playerInputManager.ActivatePlayerInputMonitoring = false;
    }

    public void SetGameMenuInactive()
    {
        Time.timeScale = 1;
        gameMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        this.playerInputManager.ActivatePlayerInputMonitoring = true;
    }

    #endregion Public Methods


    #region PlayerInput Events

    private void GameMenu_Pressed() { }
    private void GameMenu_Released()
    {
        // If the player uses the currently assigned "Game Menu" key on the settings menu
        // to reassign it, this code will resume gameplay, but not close the settings screen.
        // This flag allows the settings screen to tell the pause system to stop listening
        // while it is busy assigning keys
        if (this.isListening)
        {
            if (!this.youWin.won && !this.youDied.isDead)
            {
                this.isPaused = !this.isPaused;
                if (this.isPaused)
                {
                    this.SetGameMenuActive();
                }
                else
                {
                    this.SetGameMenuInactive();
                }
            }
        }
    }

    #endregion PlayerInput Events
}
