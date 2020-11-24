using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    public bool isPaused = false;

    private GameObject pauseMenu;

    private YouDiedControl youDied;
    private YouWinControl youWin;
    private GameObject youWonMenu;

    // Start is called before the first frame update
    void Start()
    {
        
        youDied = Camera.main.GetComponent<YouDiedControl>();
        youWin = Camera.main.GetComponent<YouWinControl>();
        SetPauseMenuDeactive();
    }

    // Update is called once per frame
    void Update()
    {
        if (!youWin.won && !youDied.isDead)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    SetPauseMenuActive();
                }
                else
                {
                    SetPauseMenuDeactive();
                }
            }          
        }
    }

    private void Awake()
    {
        youWonMenu = GameObject.FindGameObjectWithTag("YouWonMenu");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
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
        Cursor.lockState = CursorLockMode.Locked;
    }
}
