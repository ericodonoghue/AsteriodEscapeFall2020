using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsControl : MonoBehaviour
{

    private GameObject settingsMenu;


    // Start is called before the first frame update
    void Start()
    {
        settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        this.SetSettingMenuDeactive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSettingMenuActive()
    {
        settingsMenu.SetActive(true);
    }

    public void SetSettingMenuDeactive()
    {
        settingsMenu.SetActive(false);
    }
}
