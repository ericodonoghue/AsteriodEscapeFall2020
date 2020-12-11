using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonControl : MonoBehaviour
{
    private PauseControl pauseControl;
    private SettingsControl settingsControl;
    private PlayerInputManager playerInputManager;


    public string CurrentLevel = "";

    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
        settingsControl = Camera.main.GetComponent<SettingsControl>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    public void ResumePressed()
    {
        pauseControl.SetPauseMenuInactive();
        Cursor.lockState = CursorLockMode.Locked;
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.MonitorGameInputsAndCallMenu;
    }

    public void SettingsPressed()
    {
        this.pauseControl.SetPauseMenuInactive();
        this.settingsControl.SetSettingMenuActive(SettingsControlCalledBy.PauseMenu);
    }

    public void LevelSelectPressed()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void RestartPressed()
    {
        SceneManager.LoadScene("CurrentLevel");
        this.playerInputManager.ActivePlayerInputMonitoring = PlayerInputMonitoring.MonitorGameInputsAndCallMenu;
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
}
