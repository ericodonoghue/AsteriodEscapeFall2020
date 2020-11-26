using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonControl : MonoBehaviour
{
    private PauseControl pauseControl;
    private SettingsControl settingsControl;
    private PlayerInputManager playerInputManager;


    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
        settingsControl = Camera.main.GetComponent<SettingsControl>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ResumePressed()
    {
        pauseControl.SetPauseMenuDeactive();
        Cursor.lockState = CursorLockMode.Locked;
        this.playerInputManager.ActivatePlayerInputMonitoring = true;
    }

    public void SettingsPressed()
    {
        settingsControl.SetSettingMenuActive();
    }

    public void LevelSelectPressed()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void RestartPressed()
    {
        SceneManager.LoadScene("LevelOneScene");
        this.playerInputManager.ActivatePlayerInputMonitoring = true;
    }

    public void QuitPressed()
    {
        Application.Quit();
    }
}
