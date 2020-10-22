using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectScript : MonoBehaviour
{

    // TODO: add in code that grays out buttons if they have not beat the previous level


    // Start is called before the first frame update
    void Start()
    {
        
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
        SceneManager.LoadScene("CollisionTestingScene");
    }
    public void SelectLevel2 ()
    {
        /*if (player.BeatLevel2)
        {
            SceneManager.LoadScene("Level2Scene");
        }*/
    }
    public void SelectLevel3()
    {
        //SceneManager.LoadScene("Level2Scene");
    }
    public void SelectLevel4()
    {
        //SceneManager.LoadScene("Level2Scene");
    }
    public void SelectLevel5()
    {
        //SceneManager.LoadScene("Level2Scene");
    }

}
