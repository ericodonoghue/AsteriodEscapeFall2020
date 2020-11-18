using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{

    private PauseControl pauseControl;

    private SettingsButtonControl settingsButtonControl;

    private AvatarAccounting avatarAccounting;

    Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the AvatarAccounting component of Main Camera
        pauseControl = Camera.main.GetComponent<PauseControl>();
        anim = GetComponent<Animation>();
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();

    }

    private void Awake()
    {
        GameObject g = GameObject.FindGameObjectWithTag("SettingsMenu");
        settingsButtonControl = g.GetComponent<SettingsButtonControl>();
    }


    // Update is called once per frame
    void Update()
    {
        // Only fire jets if there's air in the tanks
        if (avatarAccounting.CurrentOxygenAllTanksContent != 0)
        {
            //MW: float fuelRateValue = 1f;

            //MW: if (CollOxScript.fuel > 0)
            {
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
                {
                    if (!anim.isPlaying)
                    {
                        anim.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
                {
                    if (!anim.isPlaying)
                    {
                        anim.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
                {
                    if (!anim.isPlaying)
                    {
                        anim.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
                {
                    if (!anim.isPlaying)
                    {
                        anim.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
                {
                    if (!anim.isPlaying)
                    {
                        anim.Play();
                    }
                }
                if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
                {
                    if (!anim.isPlaying)
                    {
                        anim.Play();
                    }
                }
                if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
                {
                    anim.Stop();
                }
                if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
                {
                    anim.Stop();
                }
                if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
                {
                    anim.Stop();
                }
                if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
                {
                    anim.Stop();
                }
                if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
                {
                    anim.Stop();
                }
                if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
                {
                    anim.Stop();
                }

            }

        }
    }
}