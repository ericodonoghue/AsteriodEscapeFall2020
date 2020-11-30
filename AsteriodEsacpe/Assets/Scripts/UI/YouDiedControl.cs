using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YouDiedControl : MonoBehaviour
{
    private AvatarAccounting avatarAccounting;

    public bool isDead = false;

    private GameObject youDiedMenu;
    private GameObject redPanel;


    // Start is called before the first frame update
    void Start()
    {
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        youDiedMenu = GameObject.FindGameObjectWithTag("YouDiedMenu");
        redPanel = GameObject.FindGameObjectWithTag("RedPanel");

        SetYouDiedeMenuDeactive();
    }

    // Update is called once per frame
    void Update()
    {
        if (avatarAccounting.PlayerFailState != PlayerFailStates.StillKicking)
        {
            isDead = true;
        }

        if (isDead)
        {
            SetYouDiedeMenuActive(avatarAccounting.PlayerFailStateDescription);
            Cursor.lockState = CursorLockMode.Confined;
        }
        
    }

    public void SetYouDiedeMenuActive(string deathText)
    {
        //Time.timeScale = 0;
        youDiedMenu.SetActive(true);
        redPanel.SetActive(true);
        Cursor.visible = true;

        TextMeshProUGUI deathLabel = GameObject.FindGameObjectWithTag("DeathScreenText").GetComponent<TextMeshProUGUI>();
        deathLabel.text = deathText;
    }

    public void SetYouDiedeMenuDeactive()
    {
        youDiedMenu.SetActive(false);
        redPanel.SetActive(false);
    }
}
