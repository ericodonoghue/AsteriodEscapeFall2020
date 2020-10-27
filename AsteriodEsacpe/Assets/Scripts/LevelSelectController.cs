using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    int currentLevel = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int LoadCurrentLevel()
    {
        // Data type returned is "object", so depending upon data type it may be necessary to cast
        //currentLevel = (int)this.playerInputManager.GetPlayerConfigurationValue("CurrentLevel");
        return currentLevel;
    }

    public void SaveMySettings()
    {
        // Data being stored must be [SERIALIZABLE], all native types (string, int, float, etc.) are.
        // Check PlayerInputManager for example of making your type (class, struct, enum) serializable.
        this.playerInputManager.SetPlayerConfigurationValue("CurrentLevel", this.currentLevel);

         // Values are auto-loaded, but the actual file should be saved after making changes
        this.playerInputManager.SavePlayerConfiguration();
    }
}
