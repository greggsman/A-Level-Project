using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public struct Preference
{
    // Structure for a single preference
    public string description;
    public float value;
    public Preference (string description, float value)
    {
        this.description = description;
        this.value = value;
    }
}
public class SetOfPreferences 
{
    // A set of preferences to be exported as a save file
    // JsonUtility.ToJson(); doesn't take a struct as an input so using a class makes it more clean
    // It also allows a whole set of preferences to be kept under one file name
    public string file_ID;
    public List<Preference> preferences = new List<Preference>();
    public SetOfPreferences()
    {
        DateTime now = DateTime.Now;
        file_ID = "File created " + now.ToLongDateString() + " " + now.ToLongTimeString();
    }
}
public class DataManager : MonoBehaviour
{
    public delegate void CommitPreferences();
    public event CommitPreferences commitEvent;
    // this event runs when the 'Commit Preferences' button is pressed

    private string folderPath; // path for folder where all the 
    private GameObject preferencesSettings; // Parent for the UI for the preferences menu
    private GameObject mainMenu; // Parent for the UI for the main menu

    public List<Preference> preferences = new List<Preference>();
    public GameObject fileUI;

    private Transform lffh;
    private void Start()
    {
        commitEvent += SerializeSettings; // commit event runs when a set of preferences are commited (line 84)
        commitEvent += GoToMainMenu;

        string folderName = Path.DirectorySeparatorChar + "Preferences_Data" + Path.DirectorySeparatorChar;
        folderPath = Application.persistentDataPath + folderName; 
        if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); } // If "Preferences_Data" doesn't exist already, create it

        preferencesSettings = GameObject.Find("Preferences_Settings");
        mainMenu = GameObject.Find("Main_Menu");
        lffh = GameObject.Find("LoadFileFromHere").transform;

        GoToMainMenu();
    }
    private void SerializeSettings() // serializes user's preferences in JSON
    {
        SetOfPreferences json_preferences = new SetOfPreferences();
        json_preferences.preferences = preferences;
        string currentFilePath = folderPath + json_preferences.file_ID;
        string json = JsonUtility.ToJson(json_preferences, true);
        FileStream fs = File.Create(currentFilePath);
        using (StreamWriter sw = new StreamWriter(fs)) { sw.WriteLine(json); } 
    }

    private const int diffBetweenFileUIs = 60; // How far apart the options to access a new file should be spread out on the main menu

    public void GoToMainMenu()
    // Loads the Main Menu scene and loads an option to select each file in the save data folder
    {
        preferencesSettings.SetActive(false);
        mainMenu.SetActive(true);

        // load UI options for each save file in the main menu
        string[] files = Directory.GetFiles(folderPath);
        for (int i = 0; i < files.Length; i++)
        {
            GameObject currentFileUI = Instantiate(fileUI, lffh.position + Vector3.down * (i + 1) * diffBetweenFileUIs,
                new Quaternion(0f, 0f, 0f, 0f), lffh);
            currentFileUI.GetComponent<FileUI>().filePath = files[i]; // assigns the filePath for each option in the main menu
            currentFileUI.GetComponent<Text>().text = files[i].Substring(folderPath.Length); // displays the name of the file in the main menu
        }
    }
    public void GoToNewPreferenceMenu()
    // Loads the menu 
    {
        mainMenu.SetActive(false);
        preferencesSettings.SetActive(true);
        for(int i = 0; i < lffh.childCount; i++) { Destroy(lffh.GetChild(i).gameObject); } // Destroy all file UI elements
    }
    public void button() { commitEvent(); }
}