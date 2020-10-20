using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
//using UnityEngine.InputSystem;


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
       playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveUp, OnMoveUp_Pressed, OnMoveUp_Released);
    }
  
    void OnDisable()
    {
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


/* 
 * Player Input object notes:
 *      settings for all possile player actions (Move Left\Right\Up\Down, Turn Camera, Stabilize, etc.)
 *      settings for 2-3 prefab mapping sets
 *      settings for using keyboard or controller (if controler is possible, 2-3 more mappings needed)
 *          
 *      Events\properties are generalized values like "MoveUp", not tied to specific key\button pressed:
 *              MoveUp, MoveDown, MoveLeft, MoveRight, MoveForward, MoveBackward
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
    , MoveUp
    , MoveDown
    , MoveLeft
    , MoveRight
    , MoveForward          // Primary Jet - costs more to use, but moves much faster than other jets
    , MoveBackward         // Reverse jet should be small like the other directional jets
    , CameraUp
    , CameraDown
    , CameraLeft
    , CameraRight
    , StabilizeAvatar      // are are we making this automatic?
    , PauseGame
    , SystemMenu

    // Nice to Haves
    , CameraZoomIn
    , CameraZoomOut
    , ToggleFirstOrThirdPerson   // Is this possible? Difficult?
}

public enum PlayerInputType
{
    KeyboardOnly
   , KeyboardAndMouse
   , Gamepad
}

public enum InputAction { Pressed, Released }

[Serializable]
public class InputMapping
{
    public KeyCode MappedKeyCode = KeyCode.None;  // Default value, should be changed upon scene load\UX config update
    //    public MouseInput         // create enum for this, or is there something available?
    //    public GamepadButton      // create enum for this, or is there something available?


    public InputMapping(KeyCode mappedKeyCode)
    {
        this.MappedKeyCode = mappedKeyCode;
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

#region Player Configuration File Support Types

/// <summary>
/// Serializable class used to store player settings in a text file
/// </summary>
[Serializable]
class PlayerConfigurationData
{
    // Current player input type (keyboard, mouse, gamepad)
    public PlayerInputType SelectedPlayerInputType;

    // Current player input mappings (which button moves the avatar up, etc.)
    public Dictionary<PlayerInput, InputMapping> InputMappingTable;

    // Do player control mappings come from a Prefab or Custom map?
    public string PlayerInputMappingPrefabName = "";

    public Dictionary<string, object> PlayerConfigurationDictionary;
}

#endregion Player Configuration File Support Types


public class PlayerInputManager : MonoBehaviour
{
    // Current player input type (keyboard, mouse, gamepad)
    private PlayerInputType selectedPlayerInputType = PlayerInputType.KeyboardOnly;  // default - calue is loaded in RefreshControlMappings()

    // Current player input mappings (which button moves the avatar up, etc.)
    private Dictionary<PlayerInput, InputMapping> inputMappingTable;

    // Do player control mappings come from a Prefab or Custom map?
    private string playerInputMappingPrefabName = "";

    // Allow anyone who needs to store config data to store name-value pairs in player config file
    private Dictionary<string, object> playerConfigurationDictionary = new Dictionary<string, object>();

    // Dictionary to store event handler delegates such that they can be accessed by type (PlayerInput type)
    private Dictionary<PlayerInput, PlayerInputEventSet> playerInputEventDelegates = new Dictionary<PlayerInput, PlayerInputEventSet>();

    // Path and file name used to load\save player configuration data
    private string playerConfigFileName = "/PlayerConfiguration.xml";
    private string playerConfigFilePath;


    #region Unit Testing Code

    string stateOfMoveUp = "RELEASED";

    public void OnMoveUp_Pressed()
    {
        stateOfMoveUp = "PRESSED";
    }

    public void OnMoveUp_Released()
    {
        stateOfMoveUp = "RELEASED";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(375, 300, 125, 50), "MoveUp is: " + stateOfMoveUp);

        if (GUI.Button(new Rect(750, 0, 125, 50), "Save Your Game")) this.SavePlayerConfiguration();
        if (GUI.Button(new Rect(750, 100, 125, 50), "Load Your Game")) this.LoadPlayerConfiguration();
        if (GUI.Button(new Rect(750, 200, 125, 50), "Reset Save Data")) this.DeletePlayerConfiguration();
    }

    private void InitUnitTest()
    {
        this.AssignPlayerInputEventHandler(PlayerInput.MoveUp, OnMoveUp_Pressed, OnMoveUp_Released);
        stateOfMoveUp = "RELEASED";
    }

    #endregion Unit Testing Code


    #region Unity Event Handlers

    // Start is called before the first frame update
    private void Start()
    {
        this.playerConfigFilePath = Application.persistentDataPath + playerConfigFileName;

        this.InitializePlayerInputManager();

#if DEBUG
        InitUnitTest();
#endif
    }

    // Update is called once per frame
    private void Update()
    {
        // Monitor for KeyDown\KeyUp events, only if current controls include keys
        if (this.selectedPlayerInputType == PlayerInputType.KeyboardOnly)
        {
            CaptureKeyboardInput();
        }

        // Monitor for KeyDown\KeyUp events, only if current controls include keys
        // Monitor for Mouse movement\button events, only if current controls include mouse
        else if (this.selectedPlayerInputType == PlayerInputType.KeyboardAndMouse)
        {
            CaptureKeyboardInput();
            CaptureMouseInput();
        }

        // Monitor for Gamepad events, only if current controls are set to Gamepad
        else if (this.selectedPlayerInputType == PlayerInputType.Gamepad)
        {
            CaptureGamepadInput();
        }
    }

    #endregion Unity Event Handlers


    #region Private Methods

    private void CaptureKeyboardInput()
    {
        // Loop through the mapping table to check the state of all (and ONLY) mapped keys
        foreach (KeyValuePair<PlayerInput, InputMapping> entry in this.inputMappingTable)
        {
            PlayerInput playerInput = (PlayerInput)entry.Key;
            InputMapping inputMapping = (InputMapping)entry.Value;

            if (inputMapping.MappedKeyCode != KeyCode.None)
            {
                // Has the key been pressed?
                if (Input.GetKeyDown(inputMapping.MappedKeyCode))
                {
                    // Fire event handler(s) for the identified action
                    this.DispactchPlayerInputEventHandler(playerInput, InputAction.Pressed);

                    print(string.Format("'{0}' key was pressed", inputMapping.MappedKeyCode.ToString()));
                }

                // Has the key been released?
                else if (Input.GetKeyUp(inputMapping.MappedKeyCode))
                {
                    // Fire event handler(s) for the identified action
                    this.DispactchPlayerInputEventHandler(playerInput, InputAction.Released);

                    print(string.Format("'{0}' key was released", inputMapping.MappedKeyCode.ToString()));
                }

            }
        }
    }

    private void CaptureMouseInput()
    {

    }

    private void CaptureGamepadInput()
    {

    }

    private void DispactchPlayerInputEventHandler(PlayerInput playerInput, InputAction inputAction)
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


    #region Public Methods

    /// <summary>
    /// (Re)Load user-set control mappings from user saved settings, or after recent confugration changes in UX.
    /// HINT: Call this method after UX config changes to reset controls currently in use.
    /// </summary>
    public void InitializePlayerInputManager()
    {
        // If player config has been saved previously, load it
        if (File.Exists(playerConfigFilePath))
            this.LoadPlayerConfiguration();

        // If player config file has not yet been created (or was deleted), start with default values
        else
            this.SetControlMappingKeyboardOnlyPrefabA();
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
                (this.playerConfigurationDictionary.ContainsKey(settingName))
                ? this.playerConfigurationDictionary[settingName]
                : null
            );
    }

    public void SetPlayerConfigurationValue(string settingName, object value)
    {
        if (this.playerConfigurationDictionary.ContainsKey(settingName))
            this.playerConfigurationDictionary[settingName] = value;
        else
            this.playerConfigurationDictionary.Add(settingName, value);
    }


    #region Public Control Mapping "Prefab" Methods
    // Prefab mappings are available for player to choose a "ready made" mapping without setting every button themselves

    public void SetControlMappingKeyboardOnlyPrefabA()
    {
        // Set field variables
        this.selectedPlayerInputType = PlayerInputType.KeyboardOnly;
        this.playerInputMappingPrefabName = "Keyboard Prefab A";

        // Set default key mappings
        this.inputMappingTable = new Dictionary<PlayerInput, InputMapping>()
                {
                     { PlayerInput.CameraDown, (new InputMapping(KeyCode.DownArrow)) }
                    ,{ PlayerInput.CameraUp, (new InputMapping(KeyCode.UpArrow)) }
                    ,{ PlayerInput.CameraLeft, (new InputMapping(KeyCode.LeftArrow)) }
                    ,{ PlayerInput.CameraRight, (new InputMapping(KeyCode.RightArrow)) }

                    ,{ PlayerInput.MoveDown, (new InputMapping(KeyCode.S)) }
                    ,{ PlayerInput.MoveUp, (new InputMapping(KeyCode.W)) }
                    ,{ PlayerInput.MoveLeft, (new InputMapping(KeyCode.A)) }
                    ,{ PlayerInput.MoveRight, (new InputMapping(KeyCode.D)) }
                    ,{ PlayerInput.MoveForward, (new InputMapping(KeyCode.Space)) }
                    ,{ PlayerInput.MoveBackward, (new InputMapping(KeyCode.LeftShift)) }

                    ,{ PlayerInput.StabilizeAvatar, (new InputMapping(KeyCode.Tab)) }
                };
    }

    public void SetControlMappingKeyboardOnlyPrefabB()
    {

    }

    public void SetControlMappingKeyboardOnlyPrefabC()
    {

    }

    public void SetControlMappingKeyboardAndMousePrefabA()
    {

    }

    public void SetControlMappingKeyboardAndMousePrefabB()
    {

    }

    public void SetControlMappingKeyboardAndMousePrefabC()
    {

    }

    public void SetControlMappingGamepadPrefabA()
    {

    }

    public void SetControlMappingGamepadPrefabB()
    {

    }

    public void SetControlMappingGamepadPrefabC()
    {

    }

    #endregion Public Control Mapping "Prefab" Methods


    #region Plagiarized Public Methods - Player settings
    // These functions were borrowed and heavily altered from the original article:
    // https://www.red-gate.com/simple-talk/dotnet/c-programming/saving-game-data-with-unity/

    public void SavePlayerConfiguration()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(playerConfigFilePath);


        // Create an object so serialize for file creation
        PlayerConfigurationData playerConfigurationData = new PlayerConfigurationData()
        {
             SelectedPlayerInputType = this.selectedPlayerInputType
            ,InputMappingTable = this.inputMappingTable
            ,PlayerInputMappingPrefabName = this.playerInputMappingPrefabName
            ,PlayerConfigurationDictionary = this.playerConfigurationDictionary
        };


        // Create file containing serialized data
        bf.Serialize(file, playerConfigurationData);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public void LoadPlayerConfiguration()
    {
        if (File.Exists(playerConfigFilePath))
        {
            // Load serialized data from file, right into a deserialized data object
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(playerConfigFilePath, FileMode.Open);
            PlayerConfigurationData playerConfigurationData = (PlayerConfigurationData)bf.Deserialize(file);
            file.Close();


            // Create an object so serialize for file creation
            this.selectedPlayerInputType = playerConfigurationData.SelectedPlayerInputType;
            this.inputMappingTable = playerConfigurationData.InputMappingTable;
            this.playerInputMappingPrefabName = playerConfigurationData.PlayerInputMappingPrefabName;
            this.playerConfigurationDictionary = playerConfigurationData.PlayerConfigurationDictionary;

            Debug.Log("Game data loaded!");
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
        //    case PlayerInput.MoveBackward:
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
        //    case PlayerInput.SystemMenu:
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
