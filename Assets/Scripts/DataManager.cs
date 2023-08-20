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
        file_ID = "File created " + now.ToLongDateString() + " "+ now.ToLongTimeString();
    }
}
public class DataManager : MonoBehaviour
{
    public delegate void CommitPreferences();
    public event CommitPreferences commitEvent;
    // this event runs when the 'Commit Preferences' button is pressed

    private string filePath;

    public List<Preference> preferences = new List<Preference>();
    private void Start()
    {
        commitEvent += SerializeSettings;
        commitEvent += LoadMainMenu;
        string folderName = Path.DirectorySeparatorChar + "Preferences_Data" + Path.DirectorySeparatorChar;
        filePath = Application.persistentDataPath + folderName;
        if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
    }
    private void SerializeSettings() // serializes user's preferences in JSON
    {
        GetPreferences JSONhandler = new GetPreferences();
        JSONhandler.preferences = preferences;
        filePath += JSONhandler.file_ID;
        string json = JsonUtility.ToJson(JSONhandler, true);
        FileStream fs = File.Create(filePath);
        using(StreamWriter sw = new StreamWriter(fs)) { sw.WriteLine(json); }
    }
    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Additive);
    }
    public void button() { commitEvent(); }
}