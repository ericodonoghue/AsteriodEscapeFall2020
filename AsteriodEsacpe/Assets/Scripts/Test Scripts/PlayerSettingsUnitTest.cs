using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class PlayerSettingsUnitTest : MonoBehaviour
{
    // Local references to central objects (held by main camera)
    private PlayerInputManager playerInputManager;


    // Testing "new" input for settings menu
    private void Start()
    {
        this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();

        // Assign handlers for keyboard input
        this.playerInputManager.OnKeyDown += this.OnKeyDown;
        this.playerInputManager.OnKeyUp += this.OnKeyUp;

        // Assign handlers for mouse button input
        this.playerInputManager.OnMouseButtonDown += this.OnMouseButtonDown;
        this.playerInputManager.OnMouseButtonUp += this.OnMouseButtonUp;
    }

    private void OnKeyDown(KeyCode key)
    {
        print(string.Format("'{0}' key was pressed", key.ToString()));
    }

    private void OnKeyUp(KeyCode key)
    {
        print(string.Format("'{0}' key was released", key.ToString()));
    }

    private void OnMouseButtonDown(MouseButtons mouseButton)
    {
        print(string.Format("Mouse Button '{0}' was pressed", mouseButton.ToString()));
    }

    private void OnMouseButtonUp(MouseButtons mouseButton)
    {
        print(string.Format("Mouse Button '{0}' was released", mouseButton.ToString()));
    }



    //
    // Old Testing Code
    //

    //int intToSave;
    //float floatToSave;
    //string stringToSave = "";
    //bool boolToSave;
    //string stateOfMoveUp = "";


    //[Serializable]
    //internal class SaveData
    //{
    //    public int savedInt;
    //    public float savedFloat;
    //    public bool savedBool;

    //    public Dictionary<string, object> data = new Dictionary<string, object>();
    //}


    //private void Start()
    //{
    //    this.playerInputManager = Camera.main.GetComponent<PlayerInputManager>();
    //}

    //private void Update()
    //{
    //    this.playerInputManager.AssignPlayerInputEventHandler(PlayerInput.MoveUp, OnMoveUp_Pressed, OnMoveUp_Released);

    //}

    //public void OnMoveUp_Pressed()
    //{
    //    stateOfMoveUp = "PRESSED";
    //}

    //public void OnMoveUp_Released()
    //{
    //    stateOfMoveUp = "RELEASED";
    //}

    //void OnGUI()
    //{
    //    //if (GUI.Button(new Rect(0, 0, 125, 50), "Raise Integer"))
    //    //    intToSave++;
    //    //if (GUI.Button(new Rect(0, 100, 125, 50), "Raise Float"))
    //    //    floatToSave += 0.1f;
    //    //if (GUI.Button(new Rect(0, 200, 125, 50), "Change Bool"))
    //    //    boolToSave = boolToSave ? boolToSave
    //    //                   = false : boolToSave = true;
    //    //GUI.Label(new Rect(375, 0, 125, 50), "Integer value is "
    //    //            + intToSave);
    //    //GUI.Label(new Rect(375, 100, 125, 50), "Float value is "
    //    //            + floatToSave.ToString("F1"));
    //    //GUI.Label(new Rect(375, 200, 125, 50), "Bool value is "
    //    //            + boolToSave);


    //    // GUI.Label(new Rect(375, 300, 125, 50), "MoveUp is: " + stateOfMoveUp);


    //    //if (GUI.Button(new Rect(750, 0, 125, 50), "Save Your Game"))
    //    //    SaveGame();
    //    //if (GUI.Button(new Rect(750, 100, 125, 50),
    //    //            "Load Your Game"))
    //    //    LoadGame();
    //    //if (GUI.Button(new Rect(750, 200, 125, 50),
    //    //            "Reset Save Data"))
    //    //    ResetData();
    //}

    //void SaveGame()
    //{
    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Create(Application.persistentDataPath
    //                 + "/MySaveData.dat");
    //    SaveData data = new SaveData();
    //    data.savedInt = intToSave;
    //    data.savedFloat = floatToSave;
    //    data.savedBool = boolToSave;


    //    data.data.Add("A", "a");
    //    data.data.Add("B", 3.4f);
    //    data.data.Add("C", true);


    //    bf.Serialize(file, data);
    //    file.Close();
    //    Debug.Log("Game data saved!");


    //    //inputMappingTable

    //    //selectedPlayerInputType



    //}

    //void LoadGame()
    //{
    //    if (File.Exists(Application.persistentDataPath
    //                   + "/MySaveData.dat"))
    //    {
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file =
    //                   File.Open(Application.persistentDataPath
    //                   + "/MySaveData.dat", FileMode.Open);
    //        SaveData data = (SaveData)bf.Deserialize(file);
    //        file.Close();
    //        intToSave = data.savedInt;
    //        floatToSave = data.savedFloat;
    //        boolToSave = data.savedBool;
    //        Debug.Log("Game data loaded!");
    //    }
    //    else
    //        Debug.LogError("There is no save data!");
    //}

    //void ResetData()
    //{
    //    if (File.Exists(Application.persistentDataPath
    //                  + "/MySaveData.dat"))
    //    {
    //        File.Delete(Application.persistentDataPath
    //                          + "/MySaveData.dat");
    //        intToSave = 0;
    //        floatToSave = 0.0f;
    //        boolToSave = false;
    //        Debug.Log("Data reset complete!");
    //    }
    //    else
    //        Debug.LogError("No save data to delete.");
    //}
}
