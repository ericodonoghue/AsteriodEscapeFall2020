using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.LowLevel;


public class SettingsButtonControl : MonoBehaviour
{
    #region Private Fields

    // Local script references (held by main camera)
    private SettingsControl settingsControl;
    private PlayerInputManager playerInputManager;

    // Keep track of which button the user selected to re-map
    private PlayerInput playerInputSelected = PlayerInput.None;
    private PlayerInput mostRecentButtonClicked = PlayerInput.None;

    Dictionary<PlayerInput, string> playerInputLables = new Dictionary<PlayerInput, string>()
        {
             { PlayerInput.Interact, "Interact" }
            ,{ PlayerInput.MoveReverse, "Reverse" }
            ,{ PlayerInput.MoveDown, "Down" }
            ,{ PlayerInput.MoveForward, "Forward" }
            ,{ PlayerInput.MoveLeft, "Left" }
            ,{ PlayerInput.MoveRight, "Right" }
            ,{ PlayerInput.MoveUp, "Up" }
            ,{ PlayerInput.PauseGame, "PauseMenu" }
            ,{ PlayerInput.StabilizeAvatar, "Stabilize" }
            ,{ PlayerInput.GameMenu, "GameMenu" }
        };

    #endregion Private Fields


    #region Private Properties (value mappings for simpler coding)

    // Current player input type (keyboard, mouse, gamepad)
    private PlayerInputType SelectedPlayerInputType
    {
        get { return this.playerInputManager.PlayerConfig.SelectedPlayerInputType; }
        set { this.playerInputManager.PlayerConfig.SelectedPlayerInputType = value; }
    }

    // Current player input mappings (which button moves the avatar up, etc.)
    private Dictionary<PlayerInput, MappedControl> InputMappingTable
    {
        get { return this.playerInputManager.PlayerConfig.InputMappingTable; }
        set { this.playerInputManager.PlayerConfig.InputMappingTable = value; }
    }

    // Do player control mappings come from a Prefab or Custom map?
    private string PlayerInputMappingName
    {
        get { return this.playerInputManager.PlayerConfig.PlayerInputMappingName; }
        set { this.playerInputManager.PlayerConfig.PlayerInputMappingName = value; }
    }

    private Dictionary<string, object> PlayerConfigurationDictionary
    {
        get { return this.playerInputManager.PlayerConfig.PlayerConfigurationDictionary; }
        set { this.playerInputManager.PlayerConfig.PlayerConfigurationDictionary = value; }
    }

    #endregion Private Properties (value mappings for simpler coding)


    #region Unity Events

    private void Awake()
    {
        // Get references to script components from Main Camera
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
        this.settingsControl = Camera.main.GetComponent<SettingsControl>();

        // Set up button events

        // No input has been selected yet, set default
        this.playerInputSelected = PlayerInput.None;
    }

    #endregion Unity Events


    #region Event Handlers for UI Button OnClick
    // NOTE: These handlers capture left-button clicks used to select a button representing
    // an input the player wishes to re-map.  Actual mapping input is handled elsewhere.

    // Helper method for processing UI button clicks
    private void ActivateButtonForMapping(PlayerInput playerInputToActivate)
    {
        // This silly hack handles a race condition where this event is called AFTER the
        // event handler that assigns mouse clicks to the button (if applicable)
        if (this.mostRecentButtonClicked == playerInputToActivate)
            this.mostRecentButtonClicked = PlayerInput.None;

        // "Actviate" the UI button to indicate which input type the user has selected to be mapped
        else if ((this.playerInputSelected == PlayerInput.None)
        && (this.mostRecentButtonClicked != playerInputToActivate))
        {
            this.playerInputSelected = playerInputToActivate;
            this.mostRecentButtonClicked = playerInputToActivate;

            // "Activate" the corresponding UI button so they can see which input they are mapping
            this.SetButtonMappingMode(true, this.playerInputLables[playerInputToActivate]);
        }
    }

    public void UpInput()
    {
        this.ActivateButtonForMapping(PlayerInput.MoveUp);
    }

    public void DownInput()
    {
        this.ActivateButtonForMapping(PlayerInput.MoveDown);
    }

    public void LeftInput()
    {
        this.ActivateButtonForMapping(PlayerInput.MoveLeft);
    }

    public void RightInput()
    {
        this.ActivateButtonForMapping(PlayerInput.MoveRight);
    }

    public void ForwardInput()
    {
        this.ActivateButtonForMapping(PlayerInput.MoveForward);
    }

    public void ReverseInput()
    {
        this.ActivateButtonForMapping(PlayerInput.MoveReverse);
    }

    public void InteractInput()
    {
        this.ActivateButtonForMapping(PlayerInput.Interact);
    }

    public void StabilizeInput()
    {
        this.ActivateButtonForMapping(PlayerInput.StabilizeAvatar);
    }

    public void PauseMenuInput()
    {
        this.ActivateButtonForMapping(PlayerInput.PauseGame);
    }

    public void GameMenuInput()
    {
        this.ActivateButtonForMapping(PlayerInput.GameMenu);
    }

    public void BackPressed()
    {
        settingsControl.SetSettingMenuInactive();
    }

    #endregion Event Handlers for UI Button OnClick


    #region Event Handlers for Player Input

    private void KeyUp(KeyCode keyCode)
    {
        // Assign key to mapping
        this.AssignMapping(this.playerInputSelected, (new KeyboardInputMapping(keyCode)));


        // Set UI button appearance to "deactivated" now that assignment is complete
        this.SetButtonMappingMode(false, playerInputLables[this.playerInputSelected], keyCode.ToString());


        // Reset "current" input selection now that the input has been assigned and UI button has been deactivated
        this.playerInputSelected = PlayerInput.None;
    }

    private void MouseButtonUp(MouseButtons mouseButton)
    {
        if (this.playerInputSelected != PlayerInput.None)
        {
            // Assign button to mapping
            this.AssignMapping(this.playerInputSelected, (new MouseInputMapping(mouseButton)));

            // Set UI button appearance to "deactivated" now that assignment is complete
            this.SetButtonMappingMode(false, playerInputLables[this.playerInputSelected], mouseButton.ToString());

            // Reset "current" input selection now that the input has been assigned and
            // UI button has been deactivated
            this.playerInputSelected = PlayerInput.None;
        }
    }

    private void GamepadButtonUp(GamepadButton gamepadButton)
    {
        // Assign button to mapping
        this.AssignMapping(this.playerInputSelected, (new GamepadInputMapping(gamepadButton)));


        // Set UI button appearance to "deactivated" now that assignment is complete
        this.SetButtonMappingMode(false, playerInputLables[this.playerInputSelected], gamepadButton.ToString());


        // Reset "current" input selection now that the input has been assigned and UI button has been deactivated
        this.playerInputSelected = PlayerInput.None;
    }

    #endregion Event Handlers for Player Input


    #region Private Methods

    private void SetButtonMappingMode(bool active, string labelText, string labelTextAssignedValue = "")
    {
        // Set the text of the button in UI
        Button button = GameObject.FindGameObjectWithTag(labelText + "Button").GetComponent<Button>();
        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
        t.text = labelText + ": " + labelTextAssignedValue;
        ColorBlock colors = button.colors;


        if (active)
        {
            // Set the colors of the button in UI (activate after clicked on, until assignment made)
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
        }
        else
        {
            // Set the colors of the button in UI (deactivate after assignment made)
            colors.normalColor = Color.clear;
            colors.highlightedColor = new Color32(233, 0, 0, 109);
        }


        button.colors = colors;
    }

    private void AssignMapping(PlayerInput selectedPlayerInput, MappedControl selectedMappedControl)
    {
        PlayerInput currentPlayerInput = PlayerInput.None;
        MappedControl currentMappedControl = null;
        PlayerInput swapPlayerInput = PlayerInput.None;
        MappedControl swapMappedControl = null;


        // Only need to process if player is NOT just re-assigning the same mapping
        if (this.InputMappingTable[selectedPlayerInput].CompareTo(selectedMappedControl) != 0)
        {
            foreach (KeyValuePair<PlayerInput, MappedControl> entry in this.InputMappingTable)
            {
                currentPlayerInput = entry.Key;
                currentMappedControl = entry.Value;

                // Find the mapping that matches the PlayerInput type
                if (currentPlayerInput == selectedPlayerInput)
                {
                    // Hold onto this value to assign to the PlayerInput that is
                    // currently assigned the MappedControl the player wants to assign elsewhere
                    swapMappedControl = currentMappedControl;
                }

                // Find the mapping that matches the MappedControl
                else
                {
                    // if the current MappedControl (from the iterator) matches the
                    // value being asigned elsewhere, then this is the control that gets the old value
                    if (currentMappedControl.CompareTo(selectedMappedControl) == 0)
                    {
                        swapPlayerInput = currentPlayerInput;
                    }
                }
            }


            // If the control being assigned was already in use, assign it the
            // old value from the PlayerInput type being assigned
            if ((swapPlayerInput != PlayerInput.None) && (swapMappedControl != null))
                this.InputMappingTable[swapPlayerInput] = swapMappedControl;
        }


        // Assign the selected mapping
        this.InputMappingTable[selectedPlayerInput] = selectedMappedControl;


        // Refresh mappings as represented on UI buttons
        this.RefreshSettingsButtonLabels();
    }

    private void RefreshSettingsButtonLabels()
    {
        PlayerInput inputType = PlayerInput.None;
        string assignedInputText = "";


        // Loop through all assignable inputs and find their saved mapping to update UI
        foreach (KeyValuePair<PlayerInput, string> entry in this.playerInputLables)
        {
            inputType = entry.Key;
            assignedInputText = string.Empty;

            if (this.InputMappingTable[inputType] is KeyboardInputMapping)
                assignedInputText =
                    (((KeyboardInputMapping)this.InputMappingTable[inputType]).MappingKeyCode).ToString();

            else if (this.InputMappingTable[inputType] is MouseInputMapping)
                assignedInputText =
                    (((MouseInputMapping)this.InputMappingTable[inputType]).MappingMouseButton).ToString();

            else if (this.InputMappingTable[inputType] is GamepadInputMapping)
                assignedInputText =
                    (((GamepadInputMapping)this.InputMappingTable[inputType]).MappingGamepadButton).ToString();


            // If found, set up the matching UI button
            if (assignedInputText != string.Empty)
                // Set UI button appearance to re-load mapped values
                this.SetButtonMappingMode(false, playerInputLables[inputType], assignedInputText);
        }
    }

    #endregion Private Methods


    #region Public Methods

    public void ActivateInputMonitoring()
    {
        this.LoadSettings();


        // Activate General key input monitoring in PlayerInputManager
        this.playerInputManager.ActivateOpenInputMonitoring = true;


        // Allow key\mouse button\gamepad button input for input mapping UI
        this.playerInputManager.OnKeyUp += KeyUp;
        this.playerInputManager.OnMouseButtonUp += MouseButtonUp;
        this.playerInputManager.OnGamepadButtonUp += GamepadButtonUp;
    }

    public void DeactivateInputMonitoring(bool cancel = false)
    {
        // Shut down event handlers for player input
        this.playerInputManager.OnKeyUp -= KeyUp;
        this.playerInputManager.OnMouseButtonUp -= MouseButtonUp;
        this.playerInputManager.OnGamepadButtonUp -= GamepadButtonUp;


        // Deactivate General key input monitoring in PlayerInputManager
        this.playerInputManager.ActivateOpenInputMonitoring = false;


        if (!cancel) this.SaveSettings();
    }

    #endregion Public Methods


    #region Configuration

    private void LoadSettings()
    {
        // Reset button tracking
        this.playerInputSelected = PlayerInput.None;
        this.mostRecentButtonClicked = PlayerInput.None;


        // Refresh mappings as represented on UI buttons
        this.RefreshSettingsButtonLabels();
    }

    private void SaveSettings()
    {
        // Data being stored must be [SERIALIZABLE], all native types (string, int, float, etc.) are.
        // Check PlayerInputManager for example of making your type (class, struct, enum) serializable.


        // Store control mapping details from UI (Only store non-PlayerInputManager values this way)
        //this.playerInputManager.SetPlayerConfigurationValue("name", "value");


        // Values are auto-loaded, but the actual file should be saved after making changes
        this.playerInputManager.SavePlayerConfiguration();
    }

    #endregion Configuration
}



//Deprecated code

//private Dictionary<string, KeyCode> keyInputs;
//private bool[] keyDetect;
//private readonly int UpKeyDetect = 0;
//private readonly int DownKeyDetect = 1;
//private readonly int LeftKeyDetect = 2;
//private readonly int RightKeyDetect = 3;
//private readonly int ForwardKeyDetect = 4;
//private readonly int ReverseKeyDetect = 5;
//private bool keyInputButtonPressed;

// Update is called once per frame
//void Update()
//{
//    foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
//    {
//        if (Input.GetKey(vKey))
//        {
//            foreach (string keyInput in keyInputs.Keys)
//            {
//                if (keyInputs[keyInput] == vKey)
//                {
//                    return;
//                }
//            }
//            if (keyDetect[UpKeyDetect])
//            {
//                keyInputs["up"] = vKey;

//                // Set the text of the button
//                Button button = GameObject.FindGameObjectWithTag("UpButton").GetComponent<Button>();
//                TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
//                t.text = "Up: " + vKey.ToString();

//                ColorBlock colors = button.colors;
//                colors.normalColor = Color.clear;
//                colors.highlightedColor = new Color32(233, 0, 0, 109);
//                button.colors = colors;

//                keyDetect[UpKeyDetect] = false;

//                keyInputButtonPressed = false;
//            }
//            else if (keyDetect[DownKeyDetect])
//            {
//                keyInputs["down"] = vKey;

//                // Set the text of the button
//                Button button = GameObject.FindGameObjectWithTag("DownButton").GetComponent<Button>();
//                TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
//                t.text = "Down: " + vKey.ToString();

//                ColorBlock colors = button.colors;
//                colors.normalColor = Color.clear;
//                colors.highlightedColor = new Color32(233, 0, 0, 109);
//                button.colors = colors;

//                keyDetect[DownKeyDetect] = false;

//                keyInputButtonPressed = false;
//            }
//            else if (keyDetect[LeftKeyDetect])
//            {
//                keyInputs["left"] = vKey;

//                // Set the text of the button
//                Button button = GameObject.FindGameObjectWithTag("LeftButton").GetComponent<Button>();
//                TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
//                t.text = "Left: " + vKey.ToString();

//                ColorBlock colors = button.colors;
//                colors.normalColor = Color.clear;
//                colors.highlightedColor = new Color32(233, 0, 0, 109);
//                button.colors = colors;

//                keyDetect[LeftKeyDetect] = false;

//                keyInputButtonPressed = false;
//            }
//            else if (keyDetect[RightKeyDetect])
//            {
//                keyInputs["right"] = vKey;

//                // Set the text of the button
//                Button button = GameObject.FindGameObjectWithTag("RightButton").GetComponent<Button>();
//                TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
//                t.text = "Right: " + vKey.ToString();

//                ColorBlock colors = button.colors;
//                colors.normalColor = Color.clear;
//                colors.highlightedColor = new Color32(233, 0, 0, 109);
//                button.colors = colors;

//                keyDetect[RightKeyDetect] = false;

//                keyInputButtonPressed = false;
//            }
//            else if (keyDetect[ForwardKeyDetect])
//            {
//                keyInputs["forward"] = vKey;

//                // Set the text of the button
//                Button button = GameObject.FindGameObjectWithTag("ForwardButton").GetComponent<Button>();
//                TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
//                t.text = "Forward: " + vKey.ToString();

//                ColorBlock colors = button.colors;
//                colors.normalColor = Color.clear;
//                colors.highlightedColor = new Color32(233, 0, 0, 109);
//                button.colors = colors;

//                keyDetect[ForwardKeyDetect] = false;

//                keyInputButtonPressed = false;
//            }
//            else if (keyDetect[ReverseKeyDetect])
//            {
//                keyInputs["reverse"] = vKey;

//                // Set the text of the button
//                Button button = GameObject.FindGameObjectWithTag("ReverseButton").GetComponent<Button>();
//                TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
//                t.text = "Reverse: " + vKey.ToString();

//                ColorBlock colors = button.colors;
//                colors.normalColor = Color.clear;
//                colors.highlightedColor = new Color32(233, 0, 0, 109);
//                button.colors = colors;

//                keyDetect[ReverseKeyDetect] = false;

//                keyInputButtonPressed = false;
//            }
//        }
//    }
//}

//public KeyCode GetKeyCodeMappedToDirection(string direction)
//{
//    KeyCode result = KeyCode.None;

//    switch (direction)
//    {
//        case "up":
//            result = keyInputs["up"];
//            break;
//        case "down":
//            result = keyInputs["down"];
//            break;
//        case "left":
//            result = keyInputs["left"];
//            break;
//        case "right":
//            result = keyInputs["right"];
//            break;
//        case "forward":
//            result = keyInputs["forward"];
//            break;
//        case "reverse":
//            result = keyInputs["reverse"];
//            break;
//    }

//    return result;
//}
