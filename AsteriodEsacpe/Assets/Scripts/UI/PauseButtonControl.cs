using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonControl : MonoBehaviour
{
    private PauseControl pauseControl;
    private SettingsControl settingsControl;

    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
        settingsControl = Camera.main.GetComponent<SettingsControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ResumePressed()
    {
        pauseControl.SetPauseMenuDeactive();
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
        SceneManager.LoadScene("CollisionTestingScene");
    }

    public void QuitPressed()
    {
        Application.Quit();
    }
}
