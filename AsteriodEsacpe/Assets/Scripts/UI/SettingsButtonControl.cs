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
        keyInputs = new Dictionary<string, KeyCode>();
        keyInputs.Add("up", KeyCode.Space);

        keyInputButtonPressed = false;
        keyDetect = new bool[6];

        settingsControl = Camera.main.GetComponent<SettingsControl>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                if (keyDetect[UpKeyDetect])
                {
                    keyInputs["up"] = vKey;
                    Button upButton = GameObject.FindGameObjectWithTag("UpButton").GetComponent<Button>();
                    upButton.GetComponent<TextMeshPro>().text = "Up: " + vKey.ToString();
                    
                    keyDetect[UpKeyDetect] = false;
                }

            }
        }
    }


    public void UpInputPressed()
    {
        keyInputButtonPressed = true;

        if (keyInputButtonPressed == true)
        {
            keyDetect[UpKeyDetect] = true;
        }
    }

    public void DownInputPressed()
    {
        
    }

    public void LeftInputPressed()
    {
        
    }

    public void RightInputPressed()
    {
        
    }

    public void ForwardInputPressed()
    {
       
    }

    public void ReverseInputPressed()
    {

    }

    public void BackPressed()
    {
        settingsControl.SetSettingMenuDeactive();
    }
}
