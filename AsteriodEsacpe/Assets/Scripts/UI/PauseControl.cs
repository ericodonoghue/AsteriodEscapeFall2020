using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public bool isPaused = false;

    private GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            SetPauseMenuActive();
        }
        else
        {
            SetPauseMenuDeactive();
        }
    }

    public void SetPauseMenuActive()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetPauseMenuDeactive()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        
    }
}
