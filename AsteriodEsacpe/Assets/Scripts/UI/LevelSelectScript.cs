using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelSelectScript : MonoBehaviour
{

    // TODO: add in code that grays out buttons if they have not beat the previous level
    private LevelSelectController controller;

    private GameObject progress;
    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        progress.SetActive(false);

        // disable level buttons and set colors
        { 
            Button b1 = GameObject.FindGameObjectWithTag("Level1Button").GetComponent<Button>();
            Button b2 = GameObject.FindGameObjectWithTag("Level2Button").GetComponent<Button>();
            Button b3 = GameObject.FindGameObjectWithTag("Level3Button").GetComponent<Button>();
            Button b4 = GameObject.FindGameObjectWithTag("Level4Button").GetComponent<Button>();
            Button b5 = GameObject.FindGameObjectWithTag("Level5Button").GetComponent<Button>();

            b1.interactable = false;
            b2.interactable = false;
            b3.interactable = false;
            b4.interactable = false;
            b5.interactable = false;

            ColorBlock colors = b1.colors;
            colors.normalColor = Color.grey;
            b1.colors = colors;

            colors = b2.colors;
            colors.normalColor = Color.grey;
            b2.colors = colors;

            colors = b3.colors;
            colors.normalColor = Color.grey;
            b3.colors = colors;

            colors = b4.colors;
            colors.normalColor = Color.grey;
            b4.colors = colors;

            colors = b5.colors;
            colors.normalColor = Color.grey;
            b5.colors = colors;
        }

        currentLevel = controller.LoadCurrentLevel();
    }

    private void Awake()
    {
        this.controller = Camera.main.GetComponent<LevelSelectController>();
        this.progress = GameObject.FindGameObjectWithTag("InProgressScreen");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLevel >= 1)
        {
            Button b1 = GameObject.FindGameObjectWithTag("Level1Button").GetComponent<Button>();
            b1.interactable = true;

            ColorBlock colors = b1.colors;
            colors.normalColor = Color.white;
            b1.colors = colors;
        }
        if (currentLevel >= 2)
        {
            Button b2 = GameObject.FindGameObjectWithTag("Level2Button").GetComponent<Button>();
            b2.interactable = true;

            ColorBlock colors = b2.colors;
            colors.normalColor = Color.white;
            b2.colors = colors;
        }
        if (currentLevel >= 3)
        {
            Button b3 = GameObject.FindGameObjectWithTag("Level3Button").GetComponent<Button>();
            b3.interactable = true;

            ColorBlock colors = b3.colors;
            colors.normalColor = Color.white;
            b3.colors = colors;
        }
        if (currentLevel >= 4)
        {
            Button b4 = GameObject.FindGameObjectWithTag("Level4Button").GetComponent<Button>();
            b4.interactable = true;

            ColorBlock colors = b4.colors;
            colors.normalColor = Color.white;
            b4.colors = colors;
        }
        if (currentLevel >= 5)
        {
            Button b5 = GameObject.FindGameObjectWithTag("Level5Button").GetComponent<Button>();
            b5.interactable = true;

            ColorBlock colors = b5.colors;
            colors.normalColor = Color.white;
            b5.colors = colors;
        }

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
            SceneManager.LoadScene("LevelTwoScene");
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
            SceneManager.LoadScene("Level3Threecene");
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
            SceneManager.LoadScene("LevelFourScene");
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
            SceneManager.LoadScene("LevelFiveScene");
        }
        else
        {
            progress.SetActive(true);
        }
    }
}
