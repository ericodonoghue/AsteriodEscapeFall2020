using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    int currentLevel;
    

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
        object result = this.playerInputManager.GetPlayerConfigurationValue("CurrentLevel");

        // when result is null it is the first time the player has loaded the game so set the current level to one then save 
        if (result == null)
        {
            currentLevel = 1;
            SaveCurrentLevel();
        }
        else
        {
            currentLevel = (int)result;
        }
        return currentLevel;
    }

    public void SaveCurrentLevel()
    {
        // Data being stored must be [SERIALIZABLE], all native types (string, int, float, etc.) are.
        // Check PlayerInputManager for example of making your type (class, struct, enum) serializable.
        this.playerInputManager.SetPlayerConfigurationValue("CurrentLevel", this.currentLevel);

         // Values are auto-loaded, but the actual file should be saved after making changes
        this.playerInputManager.SavePlayerConfiguration();
    }
}
