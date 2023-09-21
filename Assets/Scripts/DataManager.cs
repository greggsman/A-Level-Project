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
    public string description;
    public float value;
    public Preference (string description, float value)
    {
        this.description = description;
        this.value = value;
    }
}
class GetPreferences // JsonUtility.ToJson(); doesn't take a struct as an input so using a class makes it more clean
{
    public string file_ID;
    public List<Preference> preferences = new List<Preference>();
    public GetPreferences()
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

    private string filePath;
    private GameObject preferencesSettings;
    private GameObject mainMenu;

    public List<Preference> preferences = new List<Preference>();
    public GameObject fileUI;
    private void Start()
    {
        commitEvent += SerializeSettings;
        commitEvent += GoToMainMenu;

        string folderName = Path.DirectorySeparatorChar + "Preferences_Data" + Path.DirectorySeparatorChar;
        filePath = Application.persistentDataPath + folderName;
        if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }

        preferencesSettings = GameObject.Find("Preferences_Settings");
        mainMenu = GameObject.Find("Main_Menu");
        GoToNewPreference();
    }
    private void SerializeSettings() // serializes user's preferences in JSON
    {
        GetPreferences JSONhandler = new GetPreferences();
        JSONhandler.preferences = preferences;
        string currentFilePath = filePath + JSONhandler.file_ID;
        string json = JsonUtility.ToJson(JSONhandler, true);
        FileStream fs = File.Create(currentFilePath);
        using (StreamWriter sw = new StreamWriter(fs)) { sw.WriteLine(json); } 
    }
    private const int diffBetweenFileUIs = 60;
    public void GoToMainMenu()
    {
        preferencesSettings.SetActive(false);
        mainMenu.SetActive(true);
        Transform lffh = GameObject.Find("LoadFileFromHere").transform;
        string[] files = Directory.GetFiles(filePath);
        for(int i = 0; i < files.Length; i++)
        {
            GameObject currentFileUI = Instantiate(fileUI, lffh.position + Vector3.down * (i + 1) * diffBetweenFileUIs,
                new Quaternion(0f, 0f, 0f, 0f), mainMenu.transform);
            currentFileUI.AddComponent<FileUI>().filePath = files[i];
            currentFileUI.GetComponent<Text>().text = files[i].Substring(filePath.Length);
        }
    }
    public void GoToNewPreference()
    {
        mainMenu.SetActive(false);
        preferencesSettings.SetActive(true);
    }
    public void button() { commitEvent(); }
    public void runSettings(string filePath)
    {
        SceneManager.LoadScene("Simulation", LoadSceneMode.Additive);
        SimulationDataManager sdm = FindObjectOfType<SimulationDataManager>();
        sdm.file = filePath;
        SceneManager.UnloadSceneAsync("Preferences");
    }
}