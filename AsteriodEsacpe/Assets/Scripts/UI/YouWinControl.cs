using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class YouWinControl : MonoBehaviour
{
    private GameObject youWonMenu;

    private ScoreTracker scoreTracker;

    public bool won;

    public int levelToSave = 2;

    private PlayerInputManager playerInputManager;

    // Start is called before the first frame update
    void Start()
    {
        
        scoreTracker = GameObject.FindGameObjectWithTag("ScoreTracker").GetComponent<ScoreTracker>();
        youWonMenu.SetActive(false);
        won = false;

        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        youWonMenu = GameObject.FindGameObjectWithTag("YouWonMenu");
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: you win stuff

        won = true;
        Debug.Log(won);

        SaveCurrentLevel(levelToSave);

        SetYouWinMenuActive();
    }

    public void SetYouWinMenuActive()
    {
        Time.timeScale = 0;
        Debug.Log("You Win");
        Debug.Log(scoreTracker.timeToComplete);
        Cursor.lockState = CursorLockMode.Confined;
        youWonMenu.SetActive(true);

        TextMeshProUGUI timeLabel = GameObject.FindGameObjectWithTag("TimeLabel").GetComponent<TextMeshProUGUI>();
        timeLabel.text = "Time to complete: " + System.Math.Round(scoreTracker.timeToComplete, 2) + " seconds";

        TextMeshProUGUI collisionsLabel = GameObject.FindGameObjectWithTag("CollisionsLabel").GetComponent<TextMeshProUGUI>();
        collisionsLabel.text = "Collisions: " + scoreTracker.collisionsCount;

        //TextMeshProUGUI oxygenUsed = GameObject.FindGameObjectWithTag("OxygenUsedLabel").GetComponent<TextMeshProUGUI>();
        //oxygenUsed.text = "Oxygen Used: " + scoreTracker.oxygenUsed;

        //TextMeshProUGUI nearMissLabel = GameObject.FindGameObjectWithTag("NearMissLabel").GetComponent<TextMeshProUGUI>();
        //nearMissLabel.text = "Near Misses: " + scoreTracker.oxygenUsed;

        
    }

    public void SaveCurrentLevel(int current)
    {
        // Data being stored must be [SERIALIZABLE], all native types (string, int, float, etc.) are.
        // Check PlayerInputManager for example of making your type (class, struct, enum) serializable.
        this.playerInputManager.SetPlayerConfigurationValue("CurrentLevel", current);

        // Values are auto-loaded, but the actual file should be saved after making changes
        this.playerInputManager.SavePlayerConfiguration();
    }
}
