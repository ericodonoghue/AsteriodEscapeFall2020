using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.LowLevel;


/* Player Input Mapping UI:

The UI form for setting player controls should allow the user to select from two drop down lists or radio
button sets for
    Control Type: Choose from “Keyboard Only”, “Keyboard and Mouse”, or “Gamepad”
        NOTE: It may not be possible to do a Keyboard Only mapping with the way we are doing the camera
        this would require altering that code to use the arrow keys (probably, or allow the user to choose)
        and would change the way the camera is now working.
    Control Map: Choose from “Mapping Template A”, “.. B”, “.. C”, or “Custom”
Depending on their selection above, you can either just call one of the 9 matching “Preset” methods defined
below in this script, or you will need to collect input to allow them to build a custom control map.  Only
list the Presets that match the "Control Type" selection (do not list the Gamepad Presets for keyboard users).

The best way to see how the mappings are actually assembled and stored is to look at the first (and so far,
the only) “Preset” method, SetControlMappingKeyboardOnlyPresetA().  The Preset methods can be called to
simply load a hard-coded “Preset” set of control mappings.  Get the names of all of the Presets (they aren’t
coded, but there are method stubs for them).  Call the appropriate method if the player chooses a Preset.
    NOTE: If you want to design some Presets, go ahead and fill in the methods following the example above, but
    check with me before checking anything in to GitHub to make sure we don’t end up with merge conflicts – I
    am still working on this script.

For a “custom” mapping, the user needs to provide a name (or leave it with some default value that you supply,
then using a grid or simple list they can set the controls they want to change.  Allow the player to choose a
“Preset” template first to populate the custom mapping form.  When they change to “Custom”, don’t clear
anything, but allow them to start with a map that is mostly close to what they are after.

To support this custom mapping ability, I’ve added new events that you can hook up handlers for, that will
tell you which key or button the user is pressing:
    OnKeyDown(KeyCode key)
    OnKeyUp(KeyCode key)

    OnMouseButtonDown(int mouseButton)
    OnMouseButtonUp(int mouseButton)

    OnGamepadButtonDown(string gamepadButton)
    OnGamepadButtonUp(string gamepadButton)

NOTE: I haven’t done anything with Gamepad, yet, but the events and some other support stuff is in place for
coding against so things are mostly ready for when I find the time to get to it.

Please take a look at the enum I use for mapping controls called PlayerInput.  It lists all of the possible
player actions, including several that we may or may not implement.  I just put them there in case.  The UI
should provide this list to allow buttons\keys etc. to be assigned.  For now, ignore the functions we are not
currently doing (zoom camera, etc.) and I haven't figured out how to assign mouse movement or gamepad sticks
to movement or camera just yet, those are forthcoming.

Anyway, to create a mapping for use\storage, check out the code in the Preset method above, you will see it
is really simple.  Call this.playerInputManager.SetCustomControlMapping with the player selected values to
import them to the PlayerInputManager.

Finally, call this.playerInputManager.SavePlayerConfiguration(); to save the user configuration stored in the
input manager.


There is a good example of using the events in my UnitTest script PlayerSettingsUnitTest, I commented out
everything that no longer applies.  Also, there is some testing code that demonstrates how things are used
in this script (PlayerInputManager) in the #region “Unit Testing Code”, and I built a vanilla scene that is
used for testing called “PlayerSettingsTest.”  Open and run the scene and it shows which button or key is
pressed.

Let me know if you have any questions or need me to change\add anything.

*/


/* Player Configuration Sample Usage

    class YourClass
    {
        private PlayerInputManager playerInputManager;
        private string myStoredStringValue;

        void Start()
        {
            this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
        }

        void LoadMySettings()
        {
            // Data type returned is "object", so depending upon data type it may be necessary to cast
            this.myStoredStringValue = this.playerInputManager.GetPlayerConfigurationValue("MyStringName");
        }

        void SaveMySettings()
        {
            // Data being stored must be [SERIALIZABLE], all native types (string, int, float, etc.) are.
            // Check PlayerInputManager for example of making your type (class, struct, enum) serializable.
            this.playerInputManager.SetPlayerConfigurationValue("MyStringName", this.myStoredStringValue)

            // Values are auto-loaded, but the actual file should be saved after making changes
            this.playerInputManager.SavePlayerConfiguration();
        }
    }
 */


/* Player Controls Example Usage:
 
  // NOTE: All of this code could\probably should be moved to the actual player object's script
  // That way, the OnEnable and OnDisable methods can manage event handler assignments and prevent memory
  // leaks and other tragedies when the player object is disabled, destroyed (as in a game reset).
  public class PlayerMovement: MonoBehavior
  {
    // Local references to central objects (held by main camera)
    private AvatarAccounting avatarAccounting;
    private PlayerInputManager playerInputManager;

    void Start()
    {
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    }

    void OnEnable()
    {
        // Map a pair of event handlers (OnX_Pressed, OnX_Released) for every supported command (MoveUp, MoveDown, etc.)
        playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveUp, OnMoveUp_Pressed, OnMoveUp_Released);
    }
  
    void OnDisable()
    {
        // Map a pair of event handlers (OnX_Pressed, OnX_Released) for every supported command (MoveUp, MoveDown, etc.)
        playerInputManager.UnassignPlayerInputEventHandler(PlayerInput.MoveUp, OnMoveUp_Pressed, OnMoveUp_Released);
    }

    void MoveUp_Pressed()
    {
        avatarAccounting.FireJet(JetType.AttitudeJetUp);
       
        // Execute avatar movement code (increase movement UP)
    }
  
    void MoveUp_Released()
    {
        avatarAccounting.TerminateJet(JetType.AttitudeJetUp);
       
        // Execute avatar movement code (stop movement UP)
    }

 */


/* Player Input object notes:
 *      settings for all possile player actions (Move Left\Right\Up\Down, Turn Camera, Stabilize, etc.)
 *      settings for 2-3 Preset mapping sets
 *      settings for using keyboard or controller (if controler is possible, 2-3 more mappings needed)
 *          
 *      Events\properties are generalized values like "MoveUp", not tied to specific key\button pressed:
 *              MoveUp, MoveDown, MoveLeft, MoveRight, MoveForward, MoveReverse
 *              CameraUp, CameraDown, CameraLeft, CameraRight
 *              CameraZoom?
 *              Switch First\Third person?  Is this possible?  Difficult?
 *              Stabilize (are are we making this automatic?)
 *
 * Should be accompanied by UX popup forms
 *      Main Menu (Start, Resume, Control Settings, Quit) - Eric is working on this
 *      Control Settings (Controller, Keyboard, Mouse setup) - I will take a crack at this
 *          Allow player to choose from presets, or edit individual values (in a table, nnthing fancy)
 *
 * InputSystem is new, and must be installed.  Follow instructions to set project to use BOTH new and old
 * https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Installation.html
 * 
 * Gamepad Input Links:
 * https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/api/UnityEngine.InputSystem.Gamepad.html
 * https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/manual/Gamepad.html
 * https://docs.unity3d.com/2020.1/Documentation/ScriptReference/ControllerColliderHit-moveDirection.html
 * https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
 * 
 */


#region Player Input Support Types

public enum PlayerInput
{
    Interact               // Start/Cancel O2 refill at refilling stations, etc.
    , MoveUp               // Use to assign to individual keys
    , MoveDown
    , MoveLeft
    , MoveRight
    , MoveForward          // Primary Jet - costs more to use, but moves much faster than other jets
    , MoveReverse          // Reverse jet should be small like the other directional jets
    
    , StabilizeAvatar
    , PauseGame            // Are these separate?
    , GameMenu

    , None                 // Default for variables of type PlayerInput

    // Pipe dreams
    , MovementControls     // Use to assign to gamepad stick
    , CameraControls       // Use to assign camera controls to mouse or gamepad stick
}

public enum CameraControlsType {  Mouse, GamepadStick }

public enum PlayerInputType
{
    KeyboardOnly
   , KeyboardAndMouse
   , Gamepad
}

public enum InputAction { Pressed, Released }

/// <summary>
/// NOTE: There is a MouseButton enum in UnityEngine.InputSystem.LowLevel, but I need one that maps
/// to the event model using int 1,2,3 for left, middle, right buttons, so I created this that can
/// be converted from int to get the right button label.
/// </summary>
public enum MouseButtons {  LMB = 0, RMB = 1, MMB = 2 }

public enum PlayerInputMonitoring { MonitorGameInputsAndCallMenu, MonitorCallMenuOnly, MonitorForMapping, None }

public interface MappedControl
{
    object Control { get; set; }
    int CompareTo(object obj);
}

[Serializable]
public class KeyboardInputMapping : MappedControl, IComparable
{
    private KeyCode keyCode = KeyCode.None;  // Default value, should be set to appropriate value upon instantiation

    public KeyboardInputMapping(KeyCode keyCode)
    {
        this.keyCode = keyCode;
    }

    public KeyCode MappingKeyCode
    {
        get { return this.keyCode; }
        set { this.Control = (KeyCode)value; }
    }

    public object Control
    {
        get { return this.keyCode; }
        set { if (value is KeyCode) this.keyCode = (KeyCode)value; }
    }

    public int CompareTo(object obj)
    {
        int result = -1; // Not equal

        if (obj is KeyboardInputMapping)
            if ((obj as KeyboardInputMapping).keyCode == this.keyCode) result = 0; //Equal

        return result;
    }
}

[Serializable]
public class MouseInputMapping : MappedControl, IComparable
{
    private MouseButtons mouseButton;

    public MouseInputMapping(MouseButtons mouseButton)
    {
        this.mouseButton = mouseButton;
    }

    public MouseButtons MappingMouseButton
    {
        get { return this.mouseButton; }
        set { this.Control = (MouseButtons)value; }
    }

    public object Control
    {
        get { return this.mouseButton; }
        set { if (value is MouseButtons) this.mouseButton = (MouseButtons)value; }
    }

    public int CompareTo(object obj)
    {
        int result = -1; // Not equal

        if (obj is MouseInputMapping)
            if ((obj as MouseInputMapping).mouseButton == this.mouseButton) result = 0; //Equal

        return result;
    }
}

[Serializable]
public class GamepadInputMapping : MappedControl, IComparable
{
    private GamepadButton gamepadButton;

    public GamepadInputMapping(GamepadButton gamepadButton)
    {
        this.gamepadButton = gamepadButton;
    }

    public GamepadButton MappingGamepadButton
    {
        get { return this.gamepadButton; }
        set { this.Control = (GamepadButton)value; }
    }

    public object Control
    {
        get { return this.gamepadButton; }
        set { if (value is GamepadButton) this.gamepadButton = (GamepadButton)value; }
    }

    public int CompareTo(object obj)
    {
        int result = -1; // Not equal

        if (obj is GamepadInputMapping)
            if ((obj as GamepadInputMapping).gamepadButton == this.gamepadButton) result = 0; //Equal

        return result;
    }
}

#endregion Player Input Support Types


#region Event Juggling
// Borrowed and heavily modified the code in this region from: https://stackoverflow.com/questions/987050/array-of-events-in-c

public delegate void PlayerInputEventDelegate();

class PlayerInputEventElement
{
    protected event PlayerInputEventDelegate eventDelegate;

    public void Dispatch()
    {
        if (eventDelegate != null) eventDelegate();
    }

    public static PlayerInputEventElement operator +(PlayerInputEventElement eventElement, PlayerInputEventDelegate eventDelegate)
    {
        eventElement.eventDelegate += eventDelegate;
        return eventElement;
    }

    public static PlayerInputEventElement operator -(PlayerInputEventElement eventElement, PlayerInputEventDelegate eventDelegate)
    {
        eventElement.eventDelegate -= eventDelegate;
        return eventElement;
    }
}

class PlayerInputEventSet
{
    public PlayerInputEventElement OnInput_Pressed;
    public PlayerInputEventElement OnInput_Released;
}

#endregion Event Juggling


#region Public Event Delegates

public delegate void KeyDown(KeyCode keyCode);
public delegate void KeyUp(KeyCode keyCode);

public delegate void MouseButtonDown(MouseButtons mouseButton);
public delegate void MouseButtonUp(MouseButtons mouseButton);

public delegate void GamepadButtonDown(GamepadButton gamepadButton);
public delegate void GamepadButtonUp(GamepadButton gamepadButton);

#endregion


#region Player Configuration File Support Types

/// <summary>
/// Serializable class used to store player settings in a text file
/// </summary>
[Serializable]
public class PlayerConfigurationData
{
    // Current player input type (keyboard, mouse, gamepad)
    public PlayerInputType SelectedPlayerInputType;

    // Current player input mappings (which button moves the avatar up, etc.)
    public Dictionary<PlayerInput, MappedControl> InputMappingTable;

    // Do player control mappings come from a Preset or Custom map?
    public string PlayerInputMappingName = "";

    public Dictionary<string, object> PlayerConfigurationDictionary;

    public ChallengeMode PlayerChallengeMode = ChallengeMode.TooYoungToDie;

    public PlayerConfigurationData()
    {
        // Current player input type (keyboard, mouse, gamepad)
        SelectedPlayerInputType = PlayerInputType.KeyboardOnly;  // default - calue is loaded in RefreshControlMappings()

        // Current player input mappings (which button moves the avatar up, etc.)
        InputMappingTable = new Dictionary<PlayerInput, MappedControl>();

        // Do player control mappings come from a Preset or Custom map?
        PlayerInputMappingName = "";

        // Allow anyone who needs to store config data to store name-value pairs in player config file
        PlayerConfigurationDictionary = new Dictionary<string, object>();
    }
}

#endregion Player Configuration File Support Types


public class PlayerInputManager : MonoBehaviour
{
    #region Private Declarations

    // Local reference to the central AvatarAccounting object (held by main camera)
    private AvatarAccounting avatarAccounting;

    // Dictionary to store event handler delegates such that they can be accessed by type (PlayerInput type)
    private Dictionary<PlayerInput, PlayerInputEventSet> playerInputEventDelegates = new Dictionary<PlayerInput, PlayerInputEventSet>();

    // Serializable object for loading\saving\maintaining and using player config details
    private PlayerConfigurationData playerConfig = new PlayerConfigurationData();

    // Keep a special set of inputs for when game is in a menu (no "in game" actions monitored)
    private Dictionary<PlayerInput, MappedControl> menuInputMappingTable;

    // Path and file name used to load\save player configuration data
    private string playerConfigFileName = "/PlayerConfiguration.xml";
    private string playerConfigFilePath;

    #endregion Private Declarations


    #region Private Properties

    Dictionary<PlayerInput, MappedControl> InputMappingTable
    {
        get
        {
            Dictionary<PlayerInput, MappedControl> result = null;

            switch (this.ActivePlayerInputMonitoring)
            {
                // Only listen for "in-game" inputs when the game is actually being played
                case PlayerInputMonitoring.MonitorGameInputsAndCallMenu:
                    result = this.playerConfig.InputMappingTable;
                    break;

                // If player is on a menu, only listen for menu-related buttons
                case PlayerInputMonitoring.MonitorCallMenuOnly:
                    result = this.menuInputMappingTable;
                    break;

                // This should never be reached, but is the only way to get Unity to quit bitching about the potential null return
                default:
                    result = new Dictionary<PlayerInput, MappedControl>();
                    break;
            }

            return result;
        }
    }

    #endregion Private Properties


    #region Public Fields


    // Keep track of which "mode" PlayerInput is operating.  It can support in-game controls (which
    // listens only for mapped inputs), menu-based control mapping (which listens to any input),
    // and while on menus, it can be used to listen only for menu call buttons (call Game\Pause Menu)
    public PlayerInputMonitoring ActivePlayerInputMonitoring = PlayerInputMonitoring.MonitorGameInputsAndCallMenu;
        // MonitorGameInputsAndCallMenu is used to monitor all mapped keys during gameplay
        // MonitorCallMenuOnly is used for menus to prevent mouse\key\gamepad input other than menu actions
        // MonitorForMapping is used to capture any input, during mapping selected input to a control
        // None is used when you just want PlayerInputManager to just shut up and stop listening


    // Set in Unity Inspector to run in debug mode (warning, this will display output in the host scene)
    public bool DEBUG = false;

    #endregion Public Fields


    #region Public Properties

    public PlayerConfigurationData PlayerConfig
    {
        get { return this.playerConfig; }
        set { this.playerConfig = value; }
    }

    #endregion Public Properties


    #region Unit Testing Code

    string stateOfMoveUp = "RELEASED";
    string stateOfUserInput = "";


    public void OnMoveUp_Pressed()
    {
        stateOfMoveUp = "PRESSED";
    }

    public void OnMoveUp_Released()
    {
        stateOfMoveUp = "RELEASED";
    }

    private void InitUnitTest()
    {
        this.AssignPlayerInputEventHandler(PlayerInput.MoveUp, OnMoveUp_Pressed, OnMoveUp_Released);
        stateOfMoveUp = "RELEASED";
    }

    #endregion Unit Testing Code


    #region Unity Event Handlers

    private void Awake()
    {
        // Get a reference to the script components from Main Camera
        this.avatarAccounting = Camera.main.GetComponent<AvatarAccounting>();

        this.playerConfigFilePath = Application.persistentDataPath + playerConfigFileName;

        this.InitializePlayerInputManager();

        // Set the DEBUG flag in Unity scene to run in debug mode
        if (this.DEBUG) InitUnitTest();
    }

    // Update is called once per frame
    private void Update()
    {
        // Monitor for KeyDown\KeyUp events, only if current controls include keys
        if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardOnly)
        {
            CaptureKeyboardInput();
        }

        // Monitor for KeyDown\KeyUp events, only if current controls include keys
        // Monitor for Mouse movement\button events, only if current controls include mouse
        else if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardAndMouse)
        {
            CaptureKeyboardInput();
            CaptureMouseInput();
        }

        // Monitor for Gamepad events, only if current controls are set to Gamepad
        else if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.Gamepad)
        {
            CaptureGamepadInput();
        }
    }

    private void OnGUI()
    {
        if (this.ActivePlayerInputMonitoring == PlayerInputMonitoring.MonitorForMapping)
        {
            if ((this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardOnly)
            || (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardAndMouse))
            {
                Event currentEvent = Event.current;

                switch (currentEvent.type)
                {
                    case EventType.KeyDown:
                        if (currentEvent.keyCode != KeyCode.None)
                        {
                            // Invoke assigned event handler(s)
                            this.InvokeOnKeyDown(currentEvent.keyCode);
                        }

                        Event.current.Use();
                        break;
                    case EventType.KeyUp:
                        if (currentEvent.keyCode != KeyCode.None)
                        {
                            // Invoke assigned event handler(s)
                            this.InvokeOnKeyUp(currentEvent.keyCode);
                        }

                        Event.current.Use();
                        break;
                    case EventType.MouseDown:
                        if (currentEvent.button >= 0 && currentEvent.button <= 2)
                        {
                            // Invoke assigned event handler(s)
                            this.InvokeOnMouseButtonDown(currentEvent.button);
                        }

                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        if (currentEvent.button >= 0 && currentEvent.button <= 2)
                        {
                            // Invoke assigned event handler(s)
                            this.InvokeOnMouseButtonUp(currentEvent.button);
                        }

                        Event.current.Use();
                        break;
                }
            }

            //else if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.Gamepad)
            //{

            //    // TODO: Gamepad must be handled differently...

            //    if (Gamepad.current.buttonSouth.isPressed)
            //    {

            //    }

            //    //      Gamepad.current[GamepadButton.Y]
            //    //      Gamepad.current["Y"]
            //    //      Gamepad.current[GamepadButton.Triangle]
            //    //      Gamepad.current["Triangle"]


            //}
        }


        // Set the DEBUG flag in Unity scene to run in debug mode
        if (this.DEBUG)
        {
            GUI.Label(new Rect(375, 0, 125, 150), "MoveUp is: " + stateOfMoveUp);
            GUI.Label(new Rect(375, 100, 125, 150), "User Input: " + stateOfUserInput);
            if (GUI.Button(new Rect(750, 0, 125, 50), "Save Your Game")) this.SavePlayerConfiguration();
            if (GUI.Button(new Rect(750, 100, 125, 50), "Load Your Game")) this.LoadPlayerConfiguration();
            if (GUI.Button(new Rect(750, 200, 125, 50), "Reset Save Data")) this.DeletePlayerConfiguration();
        }
    }

    #endregion Unity Event Handlers


    #region Private Methods

    private void CaptureKeyboardInput()
    {
        // Loop through the mapping table to check the state of all (and ONLY) mapped keys
        foreach (KeyValuePair<PlayerInput, MappedControl> entry in this.InputMappingTable)
        {
            // Only process KEYBOARD mappings (it is possible for multiple types to exist in a map (i.e., Keys and Mouse)
            if (entry.Value is KeyboardInputMapping)
            {
                PlayerInput playerInput = entry.Key;
                KeyCode mappedKeyCode = (KeyCode)((KeyboardInputMapping)entry.Value).Control;

                if (mappedKeyCode != KeyCode.None)
                {
                    // Has the key been pressed?
                    if (Input.GetKeyDown(mappedKeyCode))
                    {
                        // Fire event handler(s) for the identified action
                        this.InvokePlayerInputEventHandler(playerInput, InputAction.Pressed);

                        print(string.Format("'{0}' key was pressed", mappedKeyCode.ToString()));
                    }

                    // Has the key been released?
                    else if (Input.GetKeyUp(mappedKeyCode))
                    {
                        // Fire event handler(s) for the identified action
                        this.InvokePlayerInputEventHandler(playerInput, InputAction.Released);

                        print(string.Format("'{0}' key was released", mappedKeyCode.ToString()));
                    }
                }
            }
        }
    }

    private void CaptureMouseInput()
    {
        // Loop through the mapping table to check the state of all (and ONLY) mapped keys
        foreach (KeyValuePair<PlayerInput, MappedControl> entry in this.InputMappingTable)
        {
            // Only process MOUSE mappings (it is possible for multiple types to exist in a map (i.e., Keys and Mouse)
            if (entry.Value is MouseInputMapping)
            {
                PlayerInput playerInput = entry.Key;
                MouseButtons m2 = ((MouseInputMapping)entry.Value).MappingMouseButton;

                int mappedMouseButton = (int)m2;

                if (mappedMouseButton != -1)
                {
                    // Has the key been pressed?
                    if (Input.GetMouseButtonDown(mappedMouseButton))
                    {
                        // Fire event handler(s) for the identified action
                        this.InvokePlayerInputEventHandler(playerInput, InputAction.Pressed);

                        print(string.Format("Mouse Button '{0}' was clicked", mappedMouseButton.ToString()));
                    }

                    // Has the key been released?
                    else if (Input.GetMouseButtonUp(mappedMouseButton))
                    {
                        // Fire event handler(s) for the identified action
                        this.InvokePlayerInputEventHandler(playerInput, InputAction.Released);

                        print(string.Format("Mouse Button '{0}' was released", mappedMouseButton.ToString()));
                    }
                }
            }
        }
    }

    private void CaptureGamepadInput()
    {
        // Loop through the mapping table to check the state of all (and ONLY) mapped buttons
        foreach (KeyValuePair<PlayerInput, MappedControl> entry in this.InputMappingTable)
        {
            // Only process GAMEPAD mappings (it is possible for multiple types to exist in a map (i.e., Keys and Mouse)
            if (entry.Value is GamepadInputMapping)
            {
                PlayerInput playerInput = entry.Key;
                int mappedGamepadButton = (int)((GamepadInputMapping)entry.Value).Control;

                if (mappedGamepadButton != -1)
                {
                    // All of these check the same button - the enum covers multiple gamepad formats
                    //      Gamepad.current[GamepadButton.Y]
                    //      Gamepad.current["Y"]
                    //      Gamepad.current[GamepadButton.Triangle]
                    //      Gamepad.current["Triangle"]


                    // GamepadButton.X, GamepadButton.RightTrigger, etc.

                    //// Has the key been pressed?
                    ////if (Input.GetMouseButtonDown(mappedGamepadButton))
                    //{
                    //    // Fire event handler(s) for the identified action
                    //    this.DispactchPlayerInputEventHandler(playerInput, InputAction.Pressed);

                    //    print(string.Format("Mouse Button '{0}' was clicked", mappedGamepadButton.ToString()));
                    //}

                    //// Has the key been released?
                    ////else if (Input.GetMouseButtonUp(mappedGamepadButton))
                    //{
                    //    // Fire event handler(s) for the identified action
                    //    this.DispactchPlayerInputEventHandler(playerInput, InputAction.Released);

                    //    print(string.Format("Mouse Button '{0}' was released", mappedGamepadButton.ToString()));
                    //}
                }
            }
        }
    }

    private void InvokePlayerInputEventHandler(PlayerInput playerInput, InputAction inputAction)
    {
        // Only attempt to call function delegates that are actually set
        if (this.playerInputEventDelegates[playerInput] != null)

            // Use inputAction (Pressed or Released) to determine which version of the event handler to call
            if (inputAction == InputAction.Pressed)
            {
                if (this.playerInputEventDelegates[playerInput].OnInput_Pressed != null)
                    this.playerInputEventDelegates[playerInput].OnInput_Pressed.Dispatch();
            }

            else // inputAction == InputAction.Released
            {
                if (this.playerInputEventDelegates[playerInput].OnInput_Released != null)
                    this.playerInputEventDelegates[playerInput].OnInput_Released.Dispatch();
            }
    }

    #endregion Private Methods


    #region Public Events

    public event KeyDown OnKeyDown;
    public event KeyUp OnKeyUp;

    public event MouseButtonDown OnMouseButtonDown;
    public event MouseButtonUp OnMouseButtonUp;

    public event GamepadButtonDown OnGamepadButtonDown;
    public event GamepadButtonUp OnGamepadButtonUp;

    private void InvokeOnKeyDown(KeyCode keyCode)
    {
        // Only invoke if handler is assigned
        if (OnKeyDown != null)

            // Only invoke if current input type allows keyboard input
            if ((this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardOnly)
            || (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardAndMouse))
                OnKeyDown(keyCode);

        // Set debug text (if applicable)
        if (this.DEBUG) stateOfUserInput = keyCode.ToString() + " was PRESSED";
    }

    private void InvokeOnKeyUp(KeyCode keyCode)
    {
        // Only invoke if handler is assigned
        if (OnKeyUp != null)

            // Only invoke if current input type allows keyboard input
            if ((this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardOnly)
            || (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardAndMouse))

                // Invoke assigned event handler(s)
                OnKeyUp(keyCode);

        // Set debug text (if applicable)
        if (this.DEBUG) stateOfUserInput = keyCode.ToString() + " was RELEASED";
    }

    private void InvokeOnMouseButtonDown(int mouseButton)
    {
        // Only invoke if handler is assigned
        if (OnMouseButtonDown != null)

            // Only invoke if current input type allows mouse input
            if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardAndMouse)
            {
                // Invoke assigned event handler(s)
                OnMouseButtonDown((MouseButtons)mouseButton);
            }

        // Set debug text (if applicable)
        if (this.DEBUG) this.stateOfUserInput = "Mouse Button '" + ((MouseButtons)mouseButton).ToString() + "' was PRESSED";
    }

    private void InvokeOnMouseButtonUp(int mouseButton)
    {
        // Only invoke if handler is assigned
        if (OnMouseButtonUp != null)

            // Only invoke if current input type allows mouse input
            if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.KeyboardAndMouse)

                // Invoke assigned event handler(s)
                OnMouseButtonUp((MouseButtons)mouseButton);

        // Set debug text (if applicable)
        if (this.DEBUG) this.stateOfUserInput = "Mouse Button '" + ((MouseButtons)mouseButton).ToString() + "' was RELEASED";
    }

    private void InvokeOnGamepadButtonDown(GamepadButton gamepadButton)
    {
        // Only invoke if handler is assigned
        if (OnGamepadButtonDown != null)

            // Only invoke if current input type allows mouse input
            if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.Gamepad)

                // Invoke assigned event handler(s)
                OnGamepadButtonDown(gamepadButton);
    }

    private void InvokeOnGamepadButtonUp(GamepadButton gamepadButton)
    {
        // Only invoke if handler is assigned
        if (OnGamepadButtonUp != null)

            // Only invoke if current input type allows mouse input
            if (this.playerConfig.SelectedPlayerInputType == PlayerInputType.Gamepad)

                // Invoke assigned event handler(s)
                OnGamepadButtonUp(gamepadButton);
    }

    #endregion Public Events


    #region Public Methods

    /// <summary>
    /// (Re)Load user-set control mappings from user saved settings, or after recent confugration changes in UX.
    /// HINT: Call this method after UX config changes to reset controls currently in use.
    /// </summary>
    public void InitializePlayerInputManager()
    {
        // Set defaults as baseline for any values not loaded from file
        this.SetControlMappingKeyboardOnlyPresetA();

        // If player config has been saved previously, load it
        if (File.Exists(playerConfigFilePath))
            this.LoadPlayerConfiguration();
    }

    /// <summary>
    /// Assigns a pair of event handlers to the PlayerInput type (MoveUp, MoveDown, etc.).  The event
    /// handlers correspond to input from the player, either a control was "pressed" or "released"
    /// </summary>
    /// <param name="playerInput">Indicates which action is handled by the given event handlers</param>
    /// <param name="playerInput_Pressed">OnPressed event handler</param>
    /// <param name="playerInput_Released">OnReleased event handler</param>
    public void AssignPlayerInputEventHandler(PlayerInput playerInput, PlayerInputEventDelegate playerInput_Pressed, PlayerInputEventDelegate playerInput_Released)
    {
        // Make sure the given playerInput type is not already represented the Dictionary, if so raise an internal exception (bad code issue)
        if ((this.playerInputEventDelegates.ContainsKey(playerInput))
        || (playerInput_Pressed == null) || (playerInput_Released == null))
            throw new Exception("Internal Error in call to PlayerInputManager.SetPlayerInputEventHandler().");


        // Create new Event elements (class that holds event delegate so it can be stored in a Dictionary)
        PlayerInputEventElement playerInputEventElement_OnPressed = new PlayerInputEventElement();
        PlayerInputEventElement playerInputEventElement_OnReleased = new PlayerInputEventElement();


        // Assign given event delegates to objects for storage
        playerInputEventElement_OnPressed += playerInput_Pressed;
        playerInputEventElement_OnReleased += playerInput_Released;


        // Create a new Dictionary element (class that holds event delegates in dictionary of all player input events)
        PlayerInputEventSet playerInputEventSet = new PlayerInputEventSet()
        {
            OnInput_Pressed = playerInputEventElement_OnPressed
            ,
            OnInput_Released = playerInputEventElement_OnReleased
        };


        // Add Event element to dictionary
        this.playerInputEventDelegates.Add(playerInput, playerInputEventSet);
    }

    /// <summary>
    /// Removes event handler assignment.  To allow regular, unchecked access to this function, no exceptions
    /// are raised if the designated element does not exist to be unassigned\destroyed.
    /// </summary>
    /// <param name="playerInput">Indicates which event pair should be removed</param>
    public void UnassignPlayerInputEventHandler(PlayerInput playerInput, PlayerInputEventDelegate playerInput_Pressed, PlayerInputEventDelegate playerInput_Released)
    {
        // If no such element in dictionary, just fall out (no error or other response)
        if (this.playerInputEventDelegates.ContainsKey(playerInput))
        {
            PlayerInputEventSet playerInputEventSetToRemove = this.playerInputEventDelegates[playerInput];

            // Unassign given event delegates to prevent memory leaks
            playerInputEventSetToRemove.OnInput_Pressed -= playerInput_Pressed;
            playerInputEventSetToRemove.OnInput_Released -= playerInput_Released;

            // Remove the Event element to dictionary
            this.playerInputEventDelegates.Remove(playerInput);
        }
    }

    public object GetPlayerConfigurationValue(string settingName)
    {
        return 
            (
                (this.playerConfig.PlayerConfigurationDictionary.ContainsKey(settingName))
                ? this.playerConfig.PlayerConfigurationDictionary[settingName]
                : null
            );
    }

    public void SetPlayerConfigurationValue(string settingName, object value)
    {
        if (this.playerConfig.PlayerConfigurationDictionary.ContainsKey(settingName))
            this.playerConfig.PlayerConfigurationDictionary[settingName] = value;
        else
            this.playerConfig.PlayerConfigurationDictionary.Add(settingName, value);
    }

    public void SetCustomControlMapping(PlayerInputType playerInputType, string playerInputMappingName, Dictionary<PlayerInput, MappedControl>  inputMappingTable)
    {
        this.playerConfig.SelectedPlayerInputType = playerInputType;
        this.playerConfig.PlayerInputMappingName = playerInputMappingName;
        this.playerConfig.InputMappingTable = inputMappingTable;
        this.UpdateMenuInputMappingTable();
    }


    private void UpdateMenuInputMappingTable()
    {
        // Create a new dictionary for tracking only inputs assigne to menus
        this.menuInputMappingTable = new Dictionary<PlayerInput, MappedControl>();

        // Loop through the mapping table to find inputs mapped to menus, put them in the alt list
        foreach (KeyValuePair<PlayerInput, MappedControl> entry in this.playerConfig.InputMappingTable)
            if ((entry.Key == PlayerInput.PauseGame) || (entry.Key == PlayerInput.GameMenu))
                this.menuInputMappingTable.Add(entry.Key, entry.Value);
    }

    #region Public Control Mapping "Preset" Methods
    // Preset mappings are available for player to choose a "ready made" mapping without setting every button themselves

    public void SetControlMappingKeyboardOnlyPresetA()
    {
        // Set Preset key mappings
        Dictionary<PlayerInput, MappedControl> inputMappingTable = new Dictionary<PlayerInput, MappedControl>()
                {
                     { PlayerInput.MoveDown, (new KeyboardInputMapping(KeyCode.LeftShift)) }
                    ,{ PlayerInput.MoveUp, (new KeyboardInputMapping(KeyCode.Space)) }
                    ,{ PlayerInput.MoveLeft, (new KeyboardInputMapping(KeyCode.A)) }
                    ,{ PlayerInput.MoveRight, (new KeyboardInputMapping(KeyCode.D)) }
                    ,{ PlayerInput.MoveForward, (new KeyboardInputMapping(KeyCode.W)) }
                    ,{ PlayerInput.MoveReverse, (new KeyboardInputMapping(KeyCode.S)) }

                    ,{ PlayerInput.StabilizeAvatar, (new KeyboardInputMapping(KeyCode.Tab)) }
                    ,{ PlayerInput.Interact, (new KeyboardInputMapping(KeyCode.F)) }
#if UNITY_EDITOR
                    ,{ PlayerInput.PauseGame, (new KeyboardInputMapping(KeyCode.BackQuote)) }
#else
                    ,{ PlayerInput.PauseGame, (new KeyboardInputMapping(KeyCode.Escape)) }
#endif
                    ,{ PlayerInput.GameMenu, (new KeyboardInputMapping(KeyCode.F1)) }
                };

        // TODO: For the time being (until it can be select in UI) default is Keyboard AND Mouse
        this.SetCustomControlMapping(PlayerInputType.KeyboardAndMouse, "Keyboard Preset A", inputMappingTable);
    }

    public void SetControlMappingKeyboardOnlyPresetB()
    {

    }

    public void SetControlMappingKeyboardOnlyPresetC()
    {

    }

    public void SetControlMappingKeyboardAndMousePresetA()
    {
        // Set Preset key mappings
        Dictionary<PlayerInput, MappedControl> inputMappingTable = new Dictionary<PlayerInput, MappedControl>()
                {
                     { PlayerInput.MoveDown, (new MouseInputMapping(MouseButtons.LMB)) }
                    ,{ PlayerInput.MoveUp, (new MouseInputMapping(MouseButtons.RMB)) }
                    ,{ PlayerInput.MoveLeft, (new KeyboardInputMapping(KeyCode.A)) }
                    ,{ PlayerInput.MoveRight, (new KeyboardInputMapping(KeyCode.D)) }
                    ,{ PlayerInput.MoveForward, (new KeyboardInputMapping(KeyCode.W)) }
                    ,{ PlayerInput.MoveReverse, (new KeyboardInputMapping(KeyCode.S)) }

                    ,{ PlayerInput.StabilizeAvatar, (new KeyboardInputMapping(KeyCode.Tab)) }
                    ,{ PlayerInput.Interact, (new KeyboardInputMapping(KeyCode.F)) }
#if UNITY_EDITOR
                    ,{ PlayerInput.PauseGame, (new KeyboardInputMapping(KeyCode.BackQuote)) }
#else
                    ,{ PlayerInput.PauseGame, (new KeyboardInputMapping(KeyCode.Escape)) }
#endif
                    ,{ PlayerInput.GameMenu, (new KeyboardInputMapping(KeyCode.F1)) }
                };

        this.SetCustomControlMapping(PlayerInputType.KeyboardOnly, "Keyboard Preset A", inputMappingTable);
    }

    public void SetControlMappingKeyboardAndMousePresetB()
    {

    }

    public void SetControlMappingKeyboardAndMousePresetC()
    {

    }

    public void SetControlMappingGamepadPresetA()
    {

    }

    public void SetControlMappingGamepadPresetB()
    {

    }

    public void SetControlMappingGamepadPresetC()
    {

    }

    #endregion Public Control Mapping "Preset" Methods


    #region Plagiarized Public Methods - Player settings
    // These functions were borrowed and heavily altered from the original article:
    // https://www.red-gate.com/simple-talk/dotnet/c-programming/saving-game-data-with-unity/

    public void SavePlayerConfiguration()
    {
        if ((this.playerConfigFilePath != null) && (this.playerConfigFilePath != string.Empty))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(this.playerConfigFilePath);


            // Serialize data into file
            bf.Serialize(file, this.playerConfig);
            file.Close();
            Debug.Log("Game data saved!");
        }
    }

    public void LoadPlayerConfiguration()
    {
        if (File.Exists(playerConfigFilePath))
        {
            try
            {
                // Load serialized data from file, right into a deserialized data object
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(playerConfigFilePath, FileMode.Open);
                this.playerConfig = (PlayerConfigurationData)bf.Deserialize(file);
                file.Close();

                Debug.Log("Game data loaded!");
            }
            catch(Exception ex)
            {
                Debug.Log("Game data file is corrupt and could not be loaded!");
            }

            // Update values (whether a file was loaded or not)
            this.UpdateMenuInputMappingTable();
            if (this.avatarAccounting != null) this.avatarAccounting.SetPlayerChallengeMode(this.playerConfig.PlayerChallengeMode);
        }
        else
            Debug.Log("There is no save data!");
    }

    /// <summary>
    /// Wipes out local values, and deletes configuration file (drastic, and should only be done if file is corrupted)
    /// </summary>
    public void DeletePlayerConfiguration()
    {
        if (File.Exists(playerConfigFilePath))
        {
            File.Delete(playerConfigFilePath);
            this.InitializePlayerInputManager();

            Debug.Log("Data reset complete!");
        }
        else
            Debug.Log("No save data to delete.");
    }

    #endregion Plagiarized Public Methods

    #endregion Public Methods
}






/* DEPRECATED CODE

    // Event Types and Fields (depreceated)
    #region "Press" Events

    //private delegate void Interact_Pressed();
    //private static event Interact_Pressed OnInteract_Pressed;

    //private delegate void MoveUp_Pressed();
    //private static event MoveUp_Pressed OnMoveUp_Pressed;

    //private delegate void MoveDown_Pressed();
    //private static event MoveDown_Pressed OnMoveDown_Pressed;

    //private delegate void MoveLeft_Pressed();
    //private static event MoveLeft_Pressed OnMoveLeft_Pressed;

    //private delegate void MoveRight_Pressed();
    //private static event MoveRight_Pressed OnMoveRight_Pressed;

    //private delegate void MoveForward_Pressed();
    //private static event MoveForward_Pressed OnMoveForward_Pressed;

    //private delegate void MoveBackward_Pressed();
    //private static event MoveBackward_Pressed OnMoveBackward_Pressed;

    //private delegate void CameraUp_Pressed();
    //private static event CameraUp_Pressed OnCameraUp_Pressed;

    //private delegate void CameraDown_Pressed();
    //private static event CameraDown_Pressed OnCameraDown_Pressed;

    //private delegate void CameraLeft_Pressed();
    //private static event CameraLeft_Pressed OnCameraLeft_Pressed;

    //private delegate void CameraRight_Pressed();
    //private static event CameraRight_Pressed OnCameraRight_Pressed;

    //private delegate void StabilizeAvatar_Pressed();
    //private static event StabilizeAvatar_Pressed OnStabilizeAvatar_Pressed;

    //private delegate void PauseGame_Pressed();
    //private static event PauseGame_Pressed OnPauseGame_Pressed;

    //private delegate void SystemMenu_Pressed();
    //private static event SystemMenu_Pressed OnSystemMenu_Pressed;

    //private delegate void CameraZoomIn_Pressed();
    //private static event CameraZoomIn_Pressed OnCameraZoomIn_Pressed;

    //private delegate void CameraZoomOut_Pressed();
    //private static event CameraZoomOut_Pressed OnCameraZoomOut_Pressed;

    //private delegate void ToggleFirstOrThirdPerson_Pressed();
    //private static event ToggleFirstOrThirdPerson_Pressed OnToggleFirstOrThirdPerson_Pressed;

    #endregion "Press" Events
    #region "Release" Events

    //private delegate void Interact_Released();
    //private static event Interact_Released OnInteract_Released;

    //private delegate void MoveUp_Released();
    //private static event MoveUp_Released OnMoveUp_Released;

    //private delegate void MoveDown_Released();
    //private static event MoveDown_Released OnMoveDown_Released;

    //private delegate void MoveLeft_Released();
    //private static event MoveLeft_Released OnMoveLeft_Released;

    //private delegate void MoveRight_Released();
    //private static event MoveRight_Released OnMoveRight_Released;

    //private delegate void MoveForward_Released();
    //private static event MoveForward_Released OnMoveForward_Released;

    //private delegate void MoveBackward_Released();
    //private static event MoveBackward_Released OnMoveBackward_Released;

    //private delegate void CameraUp_Released();
    //private static event CameraUp_Released OnCameraUp_Released;

    //private delegate void CameraDown_Released();
    //private static event CameraDown_Released OnCameraDown_Released;

    //private delegate void CameraLeft_Released();
    //private static event CameraLeft_Released OnCameraLeft_Released;

    //private delegate void CameraRight_Released();
    //private static event CameraRight_Released OnCameraRight_Released;

    //private delegate void StabilizeAvatar_Released();
    //private static event StabilizeAvatar_Released OnStabilizeAvatar_Released;

    //private delegate void PauseGame_Released();
    //private static event PauseGame_Released OnPauseGame_Released;

    //private delegate void SystemMenu_Released();
    //private static event SystemMenu_Released OnSystemMenu_Released;

    //private delegate void CameraZoomIn_Released();
    //private static event CameraZoomIn_Released OnCameraZoomIn_Released;

    //private delegate void CameraZoomOut_Released();
    //private static event CameraZoomOut_Released OnCameraZoomOut_Released;

    //private delegate void ToggleFirstOrThirdPerson_Released();
    //private static event ToggleFirstOrThirdPerson_Released OnToggleFirstOrThirdPerson_Released;

    #endregion "Release" Events


    private void ProcessPlayerInput(PlayerInput playerInput, InputAction inputAction)
    {
        //switch (playerInput)
        //{
        //    case PlayerInput.Interact:
        //        ExecuteAction_Interact(inputAction);
        //        break;
        //    case PlayerInput.MoveUp:
        //        ExecuteAction_MoveUp(inputAction);
        //        break;
        //    case PlayerInput.MoveDown:
        //        ExecuteAction_MoveDown(inputAction);
        //        break;
        //    case PlayerInput.MoveLeft:
        //        ExecuteAction_MoveLeft(inputAction);
        //        break;
        //    case PlayerInput.MoveRight:
        //        ExecuteAction_MoveRight(inputAction);
        //        break;
        //    case PlayerInput.MoveForward:
        //        ExecuteAction_MoveForward(inputAction);
        //        break;
        //    case PlayerInput.MoveReverse:
        //        ExecuteAction_MoveBackward(inputAction);
        //        break;
        //    case PlayerInput.CameraUp:
        //        ExecuteAction_CameraUp(inputAction);
        //        break;
        //    case PlayerInput.CameraDown:
        //        ExecuteAction_CameraDown(inputAction);
        //        break;
        //    case PlayerInput.CameraLeft:
        //        ExecuteAction_CameraLeft(inputAction);
        //        break;
        //    case PlayerInput.CameraRight:
        //        ExecuteAction_CameraRight(inputAction);
        //        break;
        //    case PlayerInput.StabilizeAvatar:
        //        ExecuteAction_StabilizeAvatar(inputAction);
        //        break;
        //    case PlayerInput.PauseGame:
        //        ExecuteAction_PauseGame(inputAction);
        //        break;
        //    case PlayerInput.GameMenu:
        //        ExecuteAction_SystemMenu(inputAction);
        //        break;

        //    // Nice to Haves
        //    case PlayerInput.CameraZoomIn:
        //        ExecuteAction_CameraZoomIn(inputAction);
        //        break;
        //    case PlayerInput.CameraZoomOut:
        //        ExecuteAction_CameraZoomOut(inputAction);
        //        break;
        //    case PlayerInput.ToggleFirstOrThirdPerson:
        //        ExecuteAction_ToggleFirstOrThirdPerson(inputAction);
        //        break;
        //}
    }

    #region ExecuteAction Private Methods

    //private void ExecuteAction_Interact(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnInteract_Pressed != null) OnInteract_Pressed();
    //    }
    //    else if (OnInteract_Released != null) OnInteract_Released();
    //}

    //private void ExecuteAction_MoveUp(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnMoveUp_Pressed != null) OnMoveUp_Pressed();
    //    }
    //    else if (OnMoveUp_Released != null) OnMoveUp_Released();
    //}

    //private void ExecuteAction_MoveDown(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnMoveDown_Pressed != null) OnMoveDown_Pressed();
    //    }
    //    else if (OnMoveDown_Released != null) OnMoveDown_Released();
    //}

    //private void ExecuteAction_MoveLeft(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnMoveLeft_Pressed != null) OnMoveLeft_Pressed();
    //    }
    //    else if (OnMoveLeft_Released != null) OnMoveLeft_Released();
    //}

    //private void ExecuteAction_MoveRight(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnMoveRight_Pressed != null) OnMoveRight_Pressed();
    //    }
    //    else if (OnMoveRight_Released != null) OnMoveRight_Released();
    //}

    //private void ExecuteAction_MoveForward(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnMoveForward_Pressed != null) OnMoveForward_Pressed();
    //    }
    //    else if (OnMoveForward_Released != null) OnMoveForward_Released();
    //}

    //private void ExecuteAction_MoveBackward(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnMoveBackward_Pressed != null) OnMoveBackward_Pressed();
    //    }
    //    else if (OnMoveBackward_Released != null) OnMoveBackward_Released();
    //}

    //private void ExecuteAction_CameraUp(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnCameraUp_Pressed != null) OnCameraUp_Pressed();
    //    }
    //    else if (OnCameraUp_Released != null) OnCameraUp_Released();
    //}

    //private void ExecuteAction_CameraDown(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnCameraDown_Pressed != null) OnCameraDown_Pressed();
    //    }
    //    else if (OnCameraDown_Released != null) OnCameraDown_Released();
    //}

    //private void ExecuteAction_CameraLeft(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnCameraLeft_Pressed != null) OnCameraLeft_Pressed();
    //    }
    //    else if (OnCameraLeft_Released != null) OnCameraLeft_Released();
    //}

    //private void ExecuteAction_CameraRight(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnCameraRight_Pressed != null) OnCameraRight_Pressed();
    //    }
    //    else if (OnCameraRight_Released != null) OnCameraRight_Released();
    //}

    //private void ExecuteAction_StabilizeAvatar(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnStabilizeAvatar_Pressed != null) OnStabilizeAvatar_Pressed();
    //    }
    //    else if (OnStabilizeAvatar_Released != null) OnStabilizeAvatar_Released();
    //}

    //private void ExecuteAction_PauseGame(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnPauseGame_Pressed != null) OnPauseGame_Pressed();
    //    }
    //    else if (OnPauseGame_Released != null) OnPauseGame_Released();
    //}

    //private void ExecuteAction_SystemMenu(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnSystemMenu_Pressed != null) OnSystemMenu_Pressed();
    //    }
    //    else if (OnSystemMenu_Released != null) OnSystemMenu_Released();
    //}

    //private void ExecuteAction_CameraZoomIn(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnCameraZoomIn_Pressed != null) OnCameraZoomIn_Pressed();
    //    }
    //    else if (OnCameraZoomIn_Released != null) OnCameraZoomIn_Released();
    //}

    //private void ExecuteAction_CameraZoomOut(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnCameraZoomOut_Pressed != null) OnCameraZoomOut_Pressed();
    //    }
    //    else if (OnCameraZoomOut_Released != null) OnCameraZoomOut_Released();
    //}

    //private void ExecuteAction_ToggleFirstOrThirdPerson(InputAction inputAction)
    //{
    //    if (inputAction == InputAction.Pressed)
    //    {
    //        if (OnToggleFirstOrThirdPerson_Pressed != null) OnToggleFirstOrThirdPerson_Pressed();
    //    }
    //    else if (OnToggleFirstOrThirdPerson_Released != null) OnToggleFirstOrThirdPerson_Released();
    //}

    #endregion ExecuteAction Private Methods
 
*/
