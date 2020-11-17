using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsControl : MonoBehaviour
{

    private GameObject settingsMenu;
    public bool isActive;


    // Start is called before the first frame update
    void Start()
    {
        settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        this.SetSettingMenuDeactive();
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSettingMenuActive()
    {
        settingsMenu.SetActive(true);
        isActive = true;
    }

    public void SetSettingMenuDeactive()
    {
        settingsMenu.SetActive(false);
        isActive = false;
    }
}
