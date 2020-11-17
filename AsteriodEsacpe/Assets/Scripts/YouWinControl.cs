using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: you win stuff
        Time.timeScale = 0;
        Debug.Log("You Win");
        SceneManager.LoadScene("LevelSelectScene");
        Cursor.lockState = CursorLockMode.Confined;
    }
}
