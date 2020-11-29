using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PauseControl : MonoBehaviour
{
    public bool isPaused = false;
    public bool isListening = true;

    private GameObject pauseMenu;
    private YouDiedControl youDied;
    private YouWinControl youWin;
    private PlayerInputManager playerInputManager;


    // Start is called before the first frame update
    void Start()
    {
        youDied = Camera.main.GetComponent<YouDiedControl>();
        youWin = Camera.main.GetComponent<YouWinControl>();
        SetPauseMenuInactive();
    }

    private void Awake()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();


        if (isPaused)
        {
            SetPauseMenuActive();
        }
        else
        {
            SetPauseMenuInactive();
        }


        // Set up event handlers for Player Input Manager to monitor for menu commands
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.PauseGame, PauseGame_Pressed, PauseGame_Released);
    }

    private void OnDisable()
    {
        // Tear down event handlers for Player Input Manager to monitor for menu commands
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.PauseGame, PauseGame_Pressed, PauseGame_Released);
    }

    public void SetPauseMenuActive()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.MonitorCallMenuOnly;
    }

    public void SetPauseMenuInactive()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.MonitorGameInputsAndCallMenu;
    }


    #region PlayerInput Events

    private void PauseGame_Pressed() {}
    private void PauseGame_Released()
    {
        // If the player uses the currently assigned "pause" key on the settings menu
        // to reassign it, this code will resume gameplay, but not close the settings screen
        // This flag allows the settings screen to tell the pause system to stop listening
        // while it is busy assigning keys
        if (this.isListening)
        {
            if (!youWin.won && !youDied.isDead)
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    SetPauseMenuActive();
                }
                else
                {
                    SetPauseMenuInactive();
                }
            }
        }
    }

    #endregion PlayerInput Events
}


// Decprecated (actaully just relocated) code
//// Update is called once per frame
//void Update()
//{
//    if (!youWin.won && !youDied.isDead)
//    {
//        if (Input.GetKeyDown(KeyCode.BackQuote))
//        {
//            isPaused = !isPaused;
//            if (isPaused)
//            {
//                SetPauseMenuActive();
//            }
//            else
//            {
//                SetPauseMenuDeactive();
//            }
//        }          
//    }
//}
