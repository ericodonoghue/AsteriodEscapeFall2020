using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectScript : MonoBehaviour
{

    // TODO: add in code that grays out buttons if they have not beat the previous level
    private LevelSelectController controller;
    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        this.controller = Camera.main.GetComponent<LevelSelectController>();
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

    public void SelectLevel1 ()
    {
        // TODO: change to load level 1
        if (currentLevel >= 1)
        {
            SceneManager.LoadScene("LevelOneScene");
        }
    }
    
    public void SelectLevel2 ()
    {
        if (currentLevel >= 2)
        {
            SceneManager.LoadScene("Level2Scene");
        }
    }
    public void SelectLevel3()
    {
        if (currentLevel >= 3)
        {
            SceneManager.LoadScene("Level3Scene");
        }
    }
    public void SelectLevel4()
    {
        if (currentLevel >= 4)
        {
            SceneManager.LoadScene("Level4Scene");
        }
    }
    public void SelectLevel5()
    {
        if (currentLevel >= 5)
        {
            SceneManager.LoadScene("Level5Scene");
        }
    }

}
