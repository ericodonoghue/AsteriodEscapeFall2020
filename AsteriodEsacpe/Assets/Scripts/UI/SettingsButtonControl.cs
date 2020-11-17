using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class SettingsButtonControl : MonoBehaviour
{

    private Dictionary<string, KeyCode> keyInputs;

    private bool keyInputButtonPressed;

    private bool[] keyDetect;

    private readonly int UpKeyDetect = 0;
    private readonly int DownKeyDetect = 1;
    private readonly int LeftKeyDetect = 2;
    private readonly int RightKeyDetect = 3;
    private readonly int ForwardKeyDetect = 4;
    private readonly int ReverseKeyDetect = 5;

    private SettingsControl settingsControl;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void Awake()
    {
        // todo: add in code that on load that loads in the players previous keybinds
        keyInputs = new Dictionary<string, KeyCode>
        {
            { "up", KeyCode.Space },
            { "down", KeyCode.LeftShift },
            { "left", KeyCode.A },
            { "right", KeyCode.D },
            { "forward", KeyCode.W },
            { "reverse", KeyCode.S }
        };

        keyInputButtonPressed = false;
        keyDetect = new bool[6];

        settingsControl = Camera.main.GetComponent<SettingsControl>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (settingsControl.isActive)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    foreach (string keyInput in keyInputs.Keys)
                    {
                        if (keyInputs[keyInput] == vKey)
                        {
                            return;
                        }
                    }
                    if (keyDetect[UpKeyDetect])
                    {
                        keyInputs["up"] = vKey;

                        // Set the text of the button
                        Button button = GameObject.FindGameObjectWithTag("UpButton").GetComponent<Button>();
                        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
                        t.text = "Up: " + vKey.ToString();

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.clear;
                        colors.highlightedColor = new Color32(233, 0, 0, 109);
                        button.colors = colors;

                        keyDetect[UpKeyDetect] = false;

                        keyInputButtonPressed = false;
                    }
                    else if (keyDetect[DownKeyDetect])
                    {
                        keyInputs["down"] = vKey;

                        // Set the text of the button
                        Button button = GameObject.FindGameObjectWithTag("DownButton").GetComponent<Button>();
                        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
                        t.text = "Down: " + vKey.ToString();

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.clear;
                        colors.highlightedColor = new Color32(233, 0, 0, 109);
                        button.colors = colors;

                        keyDetect[DownKeyDetect] = false;

                        keyInputButtonPressed = false;
                    }
                    else if (keyDetect[LeftKeyDetect])
                    {
                        keyInputs["left"] = vKey;

                        // Set the text of the button
                        Button button = GameObject.FindGameObjectWithTag("LeftButton").GetComponent<Button>();
                        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
                        t.text = "Left: " + vKey.ToString();

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.clear;
                        colors.highlightedColor = new Color32(233, 0, 0, 109);
                        button.colors = colors;

                        keyDetect[LeftKeyDetect] = false;

                        keyInputButtonPressed = false;
                    }
                    else if (keyDetect[RightKeyDetect])
                    {
                        keyInputs["right"] = vKey;

                        // Set the text of the button
                        Button button = GameObject.FindGameObjectWithTag("RightButton").GetComponent<Button>();
                        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
                        t.text = "Right: " + vKey.ToString();

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.clear;
                        colors.highlightedColor = new Color32(233, 0, 0, 109);
                        button.colors = colors;

                        keyDetect[RightKeyDetect] = false;

                        keyInputButtonPressed = false;
                    }
                    else if (keyDetect[ForwardKeyDetect])
                    {
                        keyInputs["forward"] = vKey;

                        // Set the text of the button
                        Button button = GameObject.FindGameObjectWithTag("ForwardButton").GetComponent<Button>();
                        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
                        t.text = "Forward: " + vKey.ToString();

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.clear;
                        colors.highlightedColor = new Color32(233, 0, 0, 109);
                        button.colors = colors;

                        keyDetect[ForwardKeyDetect] = false;

                        keyInputButtonPressed = false;
                    }
                    else if (keyDetect[ReverseKeyDetect])
                    {
                        keyInputs["reverse"] = vKey;

                        // Set the text of the button
                        Button button = GameObject.FindGameObjectWithTag("ReverseButton").GetComponent<Button>();
                        TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
                        t.text = "Reverse: " + vKey.ToString();

                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.clear;
                        colors.highlightedColor = new Color32(233, 0, 0, 109);
                        button.colors = colors;

                        keyDetect[ReverseKeyDetect] = false;

                        keyInputButtonPressed = false;
                    }
                }
            }
        }
        
    }


    public void UpInputPressed()
    {
        if (!keyInputButtonPressed)
        {
            keyInputButtonPressed = true;
        }
        else
        {
            return;
        }

        if (keyInputButtonPressed == true)
        {
            keyDetect[UpKeyDetect] = true;

            Button button = GameObject.FindGameObjectWithTag("UpButton").GetComponent<Button>();
            TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Up: ";

            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            button.colors = colors;
        }
    }

    public void DownInputPressed()
    {
        if (!keyInputButtonPressed)
        {
            keyInputButtonPressed = true;
        }
        else
        {
            return;
        }

        if (keyInputButtonPressed == true)
        {
            keyDetect[DownKeyDetect] = true;

            Button button = GameObject.FindGameObjectWithTag("DownButton").GetComponent<Button>();
            TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Down: ";

            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            button.colors = colors;
        }
    }

    public void LeftInputPressed()
    {
        if (!keyInputButtonPressed)
        {
            keyInputButtonPressed = true;
        }
        else
        {
            return;
        }

        if (keyInputButtonPressed == true)
        {
            keyDetect[LeftKeyDetect] = true;

            Button button = GameObject.FindGameObjectWithTag("LeftButton").GetComponent<Button>();
            TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Left: ";

            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            button.colors = colors;
        }
    }

    public void RightInputPressed()
    {
        if (!keyInputButtonPressed)
        {
            keyInputButtonPressed = true;
        }
        else
        {
            return;
        }

        if (keyInputButtonPressed == true)
        {
            keyDetect[RightKeyDetect] = true;

            Button button = GameObject.FindGameObjectWithTag("RightButton").GetComponent<Button>();
            TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Right: ";

            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            button.colors = colors;
        }
    }

    public void ForwardInputPressed()
    {
        if (!keyInputButtonPressed)
        {
            keyInputButtonPressed = true;
        }
        else
        {
            return;
        }

        if (keyInputButtonPressed == true)
        {
            keyDetect[ForwardKeyDetect] = true;

            Button button = GameObject.FindGameObjectWithTag("ForwardButton").GetComponent<Button>();
            TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Forward: ";

            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            button.colors = colors;
        }
    }

    public void ReverseInputPressed()
    {
        if (!keyInputButtonPressed)
        {
            keyInputButtonPressed = true;
        }
        else
        {
            return;
        }

        if (keyInputButtonPressed == true)
        {
            keyDetect[ReverseKeyDetect] = true;

            Button button = GameObject.FindGameObjectWithTag("ReverseButton").GetComponent<Button>();
            TextMeshProUGUI t = button.GetComponentInChildren<TextMeshProUGUI>();
            t.text = "Reverse: ";

            ColorBlock colors = button.colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.red;
            button.colors = colors;
        }
    }

    public void BackPressed()
    {
        settingsControl.SetSettingMenuDeactive();
    }

    public KeyCode GetKeyCodeMappedToDirection (string direction)
    {
        KeyCode result = KeyCode.None;

        switch (direction)
        {
            case "up":
                result = keyInputs["up"];
                break;
            case "down":
                result = keyInputs["down"];
                break;
            case "left":
                result = keyInputs["left"];
                break;
            case "right":
                result = keyInputs["right"];
                break;
            case "forward":
                result = keyInputs["forward"];
                break;
            case "reverse":
                result = keyInputs["reverse"];
                break;
        }

        return result;
    }
}
