using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingsControlCalledBy { GameMenu, PauseMenu, None }

public class SettingsControl : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    private SettingsButtonControl settingsButtonControl;
    private GameObject settingsMenu;
    private PauseControl pauseControl;
    private GameMenuControl gameMenuControl;
    private SettingsControlCalledBy settingsCalledBy = SettingsControlCalledBy.None;

    public bool isActive;


    // Start is called before the first frame update
    void Start()
    {
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
        this.settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        this.pauseControl = Camera.main.GetComponent<PauseControl>();
        this.gameMenuControl = Camera.main.GetComponent<GameMenuControl>();
        this.settingsButtonControl = settingsMenu.GetComponent<SettingsButtonControl>();

        this.SetSettingMenuInactive();
    }

    public void SetSettingMenuActive(SettingsControlCalledBy settingsCalledBy)
    {
        // Shut down listening on pause and\or game menus until settings closes
        if (this.pauseControl != null) this.pauseControl.isListening = false;
        if (this.gameMenuControl != null) this.gameMenuControl.isListening = false;

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        this.settingsMenu.SetActive(true);
        this.settingsButtonControl.ActivateInputMonitoring();
        this.isActive = true;
        this.settingsCalledBy = settingsCalledBy;
    }

    public void SetSettingMenuInactive()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        this.settingsMenu.SetActive(false);
        this.settingsButtonControl.DeactivateInputMonitoring();
        this.isActive = false;

        // Reactivate listening on pause and\or game menu until settings closes
        if (this.pauseControl != null) this.pauseControl.isListening = true;
        if (this.gameMenuControl != null) this.gameMenuControl.isListening = true;


        // Finally, reactivate the form that called the SettingsControl
        switch (this.settingsCalledBy)
        {
            case SettingsControlCalledBy.GameMenu:
                if (this.gameMenuControl != null) gameMenuControl.SetGameMenuActive();
                break;
            case SettingsControlCalledBy.PauseMenu:
                if (this.pauseControl != null) pauseControl.SetPauseMenuActive();
                break;
        }
    }
}
