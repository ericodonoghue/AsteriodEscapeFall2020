using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsControl : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private SettingsButtonControl settingsButtonControl;
    private GameObject settingsMenu;
    private PauseControl pauseControl;

    public bool isActive;


    // Start is called before the first frame update
    void Start()
    {
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
        this.settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        this.pauseControl = Camera.main.GetComponent<PauseControl>();
        this.settingsButtonControl = settingsMenu.GetComponent<SettingsButtonControl>();

        this.SetSettingMenuDeactive();
        isActive = false;
    }

    public void SetSettingMenuActive()
    {
        // Shut down listening on pause menu until settings closes
        this.pauseControl.isListening = false;

        this.settingsMenu.SetActive(true);
        this.settingsButtonControl.ActivateInputMonitoring();
        isActive = true;
    }

    public void SetSettingMenuDeactive()
    {
        settingsMenu.SetActive(false);
        this.settingsButtonControl.DeactivateInputMonitoring();
        isActive = false;

        // Deactivate General key input monitoring in PlayerInputManager
        this.playerInputManager.ActivateOpenInputMonitoring = false;

        // Reactivate listening on pause menu until settings closes
        this.pauseControl.isListening = true;
    }
}
