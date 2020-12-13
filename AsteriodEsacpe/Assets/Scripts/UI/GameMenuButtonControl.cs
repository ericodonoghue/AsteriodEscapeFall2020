using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuButtonControl : MonoBehaviour
{
    #region Private Fields

    private GameMenuControl gameMenuControl;
    private SettingsControl settingsControl;
    private MessageControl messageControl;
    private ChallengeModeControl challengeModeControl;
    private PlayerInputManager playerInputManager;

    #endregion Private Fields


    #region Unity Events

    // Start is called before the first frame update
    void Start()
    {
        this.gameMenuControl = Camera.main.GetComponent<GameMenuControl>();
        this.settingsControl = Camera.main.GetComponent<SettingsControl>();
        this.messageControl = Camera.main.GetComponent<MessageControl>();
        this.challengeModeControl = Camera.main.GetComponent<ChallengeModeControl>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    #endregion Unity Events


    #region Public Scene Event Handlers

    public void StartGamePressed()
    {
        // shut down the Game Menu before loading the scene (or it will remain up)
        this.gameMenuControl.SetGameMenuInactive();
        SceneManager.LoadScene("LevelSelectScene");


        //// shut down the Game Menu before loading the scene (or it will remain up)
        //this.gameMenuControl.SetGameMenuInactive();

        //SceneManager.LoadScene("LevelOneScene");

        //this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.MonitorGameInputsAndCallMenu;
    }

    //public void LevelSelectPressed()
    //{
    //    this.gameMenuControl.SetGameMenuInactive();
    //    SceneManager.LoadScene("LevelSelectScene");
    //}

    public void OrientationPressed()
    {
        this.gameMenuControl.SetGameMenuInactive();
        this.messageControl.SetOrientationMessageActive();
    }

    public void ControlSettingsPressed()
    {
        this.gameMenuControl.SetGameMenuInactive();
        this.settingsControl.SetSettingMenuActive(SettingsControlCalledBy.GameMenu);
    }

    public void ChallengeModePressed()
    {
        this.gameMenuControl.SetGameMenuInactive();
        this.challengeModeControl.SetChallengeModeActive();
    }

    public void QuitPressed()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    #endregion Public Scene Event Handlers
}
