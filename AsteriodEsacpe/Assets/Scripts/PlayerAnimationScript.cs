using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{

    private PauseControl pauseControl;

    //MW: Converted code to use PlayerInputManager for button mapping
    //MW: private SettingsButtonControl settingsButtonControl;

    private AvatarAccounting avatarAccounting;
    private PlayerInputManager playerInputManager;


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
        //MW: GameObject g = GameObject.FindGameObjectWithTag("SettingsMenu");
        //MW: settingsButtonControl = g.GetComponent<SettingsButtonControl>();
        
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();

        // Map a pair of event handlers (OnX_Pressed, OnX_Released) for every supported command (MoveUp, MoveDown, etc.)
        this.EnablePlayerInputEvents();
    }

    #region PlayerInput Events

    private void EnablePlayerInputEvents()
    {
        // Map a pair of event handlers (OnX_Pressed, OnX_Released) for every supported command (MoveUp, MoveDown, etc.)
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveUp, MoveUp_Pressed, MoveUp_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveDown, MoveDown_Pressed, MoveDown_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveLeft, MoveLeft_Pressed, MoveLeft_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveRight, MoveRight_Pressed, MoveRight_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveForward, MoveForward_Pressed, MoveForward_Released);
        this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveReverse, MoveReverse_Pressed, MoveReverse_Released);
    }
    private void OnDisable()
    {
        // Clear event handler mappings
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveUp, MoveUp_Pressed, MoveUp_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveDown, MoveDown_Pressed, MoveDown_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveLeft, MoveLeft_Pressed, MoveLeft_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveRight, MoveRight_Pressed, MoveRight_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveForward, MoveForward_Pressed, MoveForward_Released);
        this.playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveReverse, MoveReverse_Pressed, MoveReverse_Released);
    }

    private void MoveUp_Pressed()
    {
        this.StartAnimation();
    }
    private void MoveUp_Released()
    {
        this.StopAnimation();
    }

    private void MoveDown_Pressed()
    {
        this.StartAnimation();
    }
    private void MoveDown_Released()
    {
        this.StopAnimation();
    }

    private void MoveLeft_Pressed()
    {
        this.StartAnimation();
    }
    private void MoveLeft_Released()
    {
        this.StopAnimation();
    }

    private void MoveRight_Pressed()
    {
        this.StartAnimation();
    }
    private void MoveRight_Released()
    {
        this.StopAnimation();
    }

    private void MoveForward_Pressed()
    {
        this.StartAnimation();
    }
    private void MoveForward_Released()
    {
        this.StopAnimation();
    }

    private void MoveReverse_Pressed()
    {
        this.StartAnimation();
    }
    private void MoveReverse_Released()
    {
        this.StopAnimation();
    }

    #endregion PlayerInput Events

    #region Private Methods

    private void StartAnimation()
    {
        // Only fire jets if there's air in the tanks
        if (avatarAccounting.JetsCanFire)
        {
            if (!anim.isPlaying)
            {
                anim.Play();
            }
        }
    }

    private void StopAnimation()
    {
        // Only fire jets if there's air in the tanks
        if (avatarAccounting.JetsCanFire)
        {
            anim.Stop();
        }
    }

    #endregion Private Methods
}



// MW: Deprecated method and moved code into separate event handlers by input type (MoveUp, etc.)
// Update is called once per frame
//void Update()
//{
//    // Only fire jets if there's air in the tanks
//    if (avatarAccounting.JetsCanFire)
//    {
//        if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
//        {
//            if (!anim.isPlaying)
//            {
//                anim.Play();
//            }
//        }
//        if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
//        {
//            if (!anim.isPlaying)
//            {
//                anim.Play();
//            }
//        }
//        if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
//        {
//            if (!anim.isPlaying)
//            {
//                anim.Play();
//            }
//        }
//        if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
//        {
//            if (!anim.isPlaying)
//            {
//                anim.Play();
//            }
//        }
//        if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
//        {
//            if (!anim.isPlaying)
//            {
//                anim.Play();
//            }
//        }
//        if (Input.GetKeyDown(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
//        {
//            if (!anim.isPlaying)
//            {
//                anim.Play();
//            }
//        }
//        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("right")))
//        {
//            anim.Stop();
//        }
//        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("left")))
//        {
//            anim.Stop();
//        }
//        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("down")))
//        {
//            anim.Stop();
//        }
//        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("up")))
//        {
//            anim.Stop();
//        }
//        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("forward")))
//        {
//            anim.Stop();
//        }
//        if (Input.GetKeyUp(settingsButtonControl.GetKeyCodeMappedToDirection("reverse")))
//        {
//            anim.Stop();
//        }
//    }
//}
