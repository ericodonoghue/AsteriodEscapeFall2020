using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectScript : MonoBehaviour
{

    // TODO: add in code that grays out buttons if they have not beat the previous level
    private LevelSelectController controller;

    private GameObject progress;
    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        this.controller = Camera.main.GetComponent<LevelSelectController>();
        this.progress = GameObject.FindGameObjectWithTag("InProgressScreen");
        progress.SetActive(false);
        currentLevel = 1; // controller.LoadCurrentLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackButton ()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Back2Button()
    {
        progress.SetActive(false);
    }

    public void SelectLevel1 ()
    {
        // TODO: change to load level 1
        if (currentLevel >= 1)
        {
            SceneManager.LoadScene("LevelOneScene");
        }
        else
        {
            progress.SetActive(true);
        }
    }
    
    public void SelectLevel2 ()
    {
        if (currentLevel >= 2)
        {
            SceneManager.LoadScene("Level2Scene");
        }
        else
        {
            progress.SetActive(true);
        }
    }
    public void SelectLevel3()
    {
        if (currentLevel >= 3)
        {
            SceneManager.LoadScene("Level3Scene");
        }
        else
        {
            progress.SetActive(true);
        }
    }
    public void SelectLevel4()
    {
        if (currentLevel >= 4)
        {
            SceneManager.LoadScene("Level4Scene");
        }
        else
        {
            progress.SetActive(true);
        }
    }
    public void SelectLevel5()
    {
        if (currentLevel >= 5)
        {
            SceneManager.LoadScene("Level5Scene");
        }
        else
        {
            progress.SetActive(true);
        }
    }

}
