using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (avatarAccounting.PlayerBlackout)
        {
            isDead = true;
        }

        if (isDead)
        {
            SetYouDiedeMenuActive();
        }
        else
        {
            SetYouDiedeMenuDeactive();
        }
    }

    public void SetYouDiedeMenuActive()
    {
        Time.timeScale = 0;
        youDiedMenu.SetActive(true);
        redPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void SetYouDiedeMenuDeactive()
    {
        Time.timeScale = 1;
        youDiedMenu.SetActive(false);
        redPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
