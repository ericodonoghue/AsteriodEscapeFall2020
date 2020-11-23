using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWonMenuScript : MonoBehaviour
{
    private YouWinControl control;

    public string sceneToLoad = "LevelTwoScene";


    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<YouWinControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LevelSelectPressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void NextLevelPressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneToLoad);
    }
}
