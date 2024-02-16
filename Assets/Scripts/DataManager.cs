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
    public int attributeMaxSliderValue = 100;

    private const int diffBetweenFileUIs = 60; // How far apart the options to access a new file should be spread out on the main menu
    private const int diffBetweenPreferences = 30;

    public delegate void CommitPreferences();
    public event CommitPreferences commitEvent;
    // this event runs when the 'Commit Preferences' button is pressed

    private string folderPath; // path for folder where all the preferences will be stored
    private GameObject preferencesSettings; // Parent for the UI for the preferences menu
    private GameObject mainMenu; // Parent for the UI for the main menu

    public List<Preference> preferences = new List<Preference>();
    public GameObject fileUI;
    public GameObject initialAttributePreference;
    public static SetOfPreferences preferencesToRun;

    private Transform loadFilesFromHere;
    private Transform loadPreferencesFromHere;
    private GameObject tooManyFilesWarning;

    private const int sliderMax = 100;

    private void Start()
    {
        string folderName = Path.DirectorySeparatorChar + "Preferences_Data" + Path.DirectorySeparatorChar;
        folderPath = Application.persistentDataPath + folderName; 
        if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); } // If "Preferences_Data" doesn't exist already, create it

        preferencesSettings = GameObject.Find("Preferences_Settings");
        mainMenu = GameObject.Find("Main_Menu");
        loadFilesFromHere = GameObject.Find("LoadFileFromHere").transform;
        loadPreferencesFromHere = GameObject.Find("Load Preferences From Here").transform;

        for (int i = 0; i < ConsumerData.attributeKeys.Length; i++) // NEW 
        {
            GameObject currentPreference = Instantiate(initialAttributePreference, loadPreferencesFromHere.position + Vector3.down * (i + 1) * diffBetweenPreferences,
                initialAttributePreference.transform.rotation, loadPreferencesFromHere);

            string attributeName = ConsumerData.attributeKeys[i];
            currentPreference.GetComponent<Text>().text = attributeName;
            currentPreference.name = attributeName;

            Slider slider = currentPreference.GetComponentInChildren<Slider>();
            if (attributeName == "Max Energy/Hunger")
            {
                slider.minValue = 150;
                slider.maxValue = 400;
            }
            else
            {
                slider.minValue = ConsumerData.DefaultEnergyValue;
                slider.maxValue = ConsumerData.DefaultEnergyValue + ConsumerData.DefaultEnergyValue * 2;
            }
        }

        tooManyFilesWarning = GameObject.Find("too many files");
        tooManyFilesWarning.GetComponent<Text>().text += MaximumNoOfFiles + "!";
        tooManyFilesWarning.SetActive(false);

        GoToMainMenu();
        // /Users/alancgregg/Library/Application Support/DefaultCompany/EvolutionSimulatorPrototype/Preferences_Data/ is the persistent data path
    }

    public void SerializeSettings() // serializes user's preferences in JSON, run when Commit Prefences is pressed
    {
        if(Directory.GetFiles(folderPath).Length >= MaximumNoOfFiles)
        {
            tooManyFilesWarning.SetActive(true);
            // UI element informing the user they have created to many files to display on the home page
        }
        else // add a new JSON file
        {
            SetOfPreferences json_preferences = new SetOfPreferences();
            json_preferences.preferences = preferences;
            string currentFilePath = folderPath + json_preferences.file_ID;
            string json = JsonUtility.ToJson(json_preferences, true);
            // converts the object into Json

            FileStream fs = File.Create(currentFilePath);
            using (StreamWriter sw = new StreamWriter(fs)) { sw.WriteLine(json); }
            // stream writer writes the data into a file
            fs.Close();
            Debug.Log("File saved to " + folderPath); // Testing objective 2.4
            GoToMainMenu();
        }
    }

    public void GoToMainMenu() // called when 'cancel' is pressed
    // Loads the Main Menu scene and loads an option to select each file in the save data folder
    {
        for (int i = 0; i < loadFilesFromHere.childCount; i++) // Destroy all file UI elementsn (in case files have been deleted)
        {
            Destroy(loadFilesFromHere.GetChild(i).gameObject);
        }
        preferencesSettings.SetActive(false);
        mainMenu.SetActive(true);
        // load UI options for each save file in the main menu
        string[] files = Directory.GetFiles(folderPath);
        for (int i = 0; i < files.Length; i++) // load files back again
        {
            GameObject currentFileUI = Instantiate(fileUI, loadFilesFromHere.position + Vector3.down * (i + 1) * diffBetweenFileUIs,
                fileUI.transform.rotation, loadFilesFromHere);
            currentFileUI.GetComponent<FileUI>().filePath = files[i]; // assigns the filePath for each option in the main menu
            currentFileUI.GetComponent<Text>().text = files[i].Substring(folderPath.Length); // displays the name of the file in the main menu
        }
    }

    public void GoToNewPreferenceMenu()
    // Loads the menu to create a new preference
    {
        tooManyFilesWarning.SetActive(false);
        mainMenu.SetActive(false);
        preferencesSettings.SetActive(true);

        for (int i = 0; i < loadFilesFromHere.childCount; i++) // Destroy all file UI elements
        {
            Destroy(loadFilesFromHere.GetChild(i).gameObject);
        }
    }

    public void button()
    {
        commitEvent();
        SerializeSettings();
    }
}