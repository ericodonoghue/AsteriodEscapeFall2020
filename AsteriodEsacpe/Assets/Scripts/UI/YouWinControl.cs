using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinControl : MonoBehaviour
{
    private GameObject youWonMenu;

    // Start is called before the first frame update
    void Start()
    {
        youWonMenu = GameObject.FindGameObjectWithTag("YouWonMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: you win stuff
        
        SetYouWinMenuActive();
    }

    public void SetYouWinMenuActive()
    {
        Time.timeScale = 0;
        Debug.Log("You Win");
        Cursor.lockState = CursorLockMode.Confined;
        youWonMenu.SetActive(true);
    }
}
