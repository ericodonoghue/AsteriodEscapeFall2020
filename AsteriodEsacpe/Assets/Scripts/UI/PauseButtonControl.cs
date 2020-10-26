using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonControl : MonoBehaviour
{
    private PauseControl pauseControl;

    // Start is called before the first frame update
    void Start()
    {
        pauseControl = Camera.main.GetComponent<PauseControl>();
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
        // TODO: Implement a settings UI interface
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
