using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

[Serializable]
public struct Preference
{
    // Structure for a single preference
    public string description;
    public int value;
    public Preference (string description, int value)
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
    public const int MaximumNoOfFiles = 6;
    private const int diffBetweenFileUIs = 60; // How far apart the options to access a new file should be spread out on the main menu

    public delegate void CommitPreferences();
    public event CommitPreferences commitEvent;
    // this event runs when the 'Commit Preferences' button is pressed

    private string folderPath; // path for folder where all the 
    private GameObject preferencesSettings; // Parent for the UI for the preferences menu
    private GameObject mainMenu; // Parent for the UI for the main menu

    public List<Preference> preferences = new List<Preference>();
    public GameObject fileUI;
    public static SetOfPreferences preferencesToRun;

    private Transform loadFilesFromHere;
    private GameObject tooManyFilesWarning;
    private void Start()
    {
        commitEvent += SerializeSettings; // commit event runs when a set of preferences are commited (line 99)

        string folderName = Path.DirectorySeparatorChar + "Preferences_Data" + Path.DirectorySeparatorChar;
        folderPath = Application.persistentDataPath + folderName; 
        if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); } // If "Preferences_Data" doesn't exist already, create it

        preferencesSettings = GameObject.Find("Preferences_Settings");
        mainMenu = GameObject.Find("Main_Menu");
        loadFilesFromHere = GameObject.Find("LoadFileFromHere").transform;

        tooManyFilesWarning = GameObject.Find("too many files");
        tooManyFilesWarning.GetComponent<Text>().text += MaximumNoOfFiles + "!";
        tooManyFilesWarning.SetActive(false);

        GoToMainMenu();
        // /Users/alancgregg/Library/Application Support/DefaultCompany/EvolutionSimulatorPrototype/Preferences_Data/ is the persistent data path
    }
    private void SerializeSettings() // serializes user's preferences in JSON, run when Commit Prefences is pressed
    {
        if(Directory.GetFiles(folderPath).Length >= MaximumNoOfFiles) // using >= because INDEXING STARTS AT 0!!!!
        {
            tooManyFilesWarning.SetActive(true);
            Debug.Log("TOO MANY FILES");
        }
        else // add a new JSON file
        {
            SetOfPreferences json_preferences = new SetOfPreferences();
            json_preferences.preferences = preferences;
            string currentFilePath = folderPath + json_preferences.file_ID;
            string json = JsonUtility.ToJson(json_preferences, true);
            FileStream fs = File.Create(currentFilePath);
            using (StreamWriter sw = new StreamWriter(fs)) { sw.WriteLine(json); }
            fs.Close();
            GoToMainMenu();
        }        
    }

    public void GoToMainMenu() // called when 'cancel' is pressed
    // Loads the Main Menu scene and loads an option to select each file in the save data folder
    {
        preferencesSettings.SetActive(false);
        mainMenu.SetActive(true);

        // load UI options for each save file in the main menu
        string[] files = Directory.GetFiles(folderPath);
        for (int i = 0; i < files.Length; i++)
        {
            GameObject currentFileUI = Instantiate(fileUI, loadFilesFromHere.position + Vector3.down * (i + 1) * diffBetweenFileUIs,
                new Quaternion(0f, 0f, 0f, 0f), loadFilesFromHere);
            currentFileUI.GetComponent<FileUI>().filePath = files[i]; // assigns the filePath for each option in the main menu
            currentFileUI.GetComponent<Text>().text = files[i].Substring(folderPath.Length); // displays the name of the file in the main menu
        }
    }
    public void GoToNewPreferenceMenu()
    // Loads the menu 
    {
        tooManyFilesWarning.SetActive(false);
        mainMenu.SetActive(false);
        preferencesSettings.SetActive(true);
        for(int i = 0; i < loadFilesFromHere.childCount; i++) { Destroy(loadFilesFromHere.GetChild(i).gameObject); } // Destroy all file UI elements
    }
    public void button() { commitEvent(); }
}