using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelSelectPressed()
    {
        // TODO: change to load the level select screen
        SceneManager.LoadScene("LevelSelectScene");
    }


    public void QuitPressed()
    {
        Application.Quit();
    }
}
