using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class YouDiedMenuScript : MonoBehaviour
{
    private YouDiedControl control;

    // Start is called before the first frame update
    void Start()
    {
        control = Camera.main.GetComponent<YouDiedControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenuPressed()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartPressed()
    {
        SceneManager.LoadScene("CollisionTestingScene");
    }
}
