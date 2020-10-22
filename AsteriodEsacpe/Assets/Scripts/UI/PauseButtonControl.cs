using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void QuitPressed()
    {
        Application.Quit();
    }
}
