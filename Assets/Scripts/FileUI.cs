using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class FileUI : MonoBehaviour
{
    private Button b;

    public string filePath;
    public GameObject simulationManager;
    void Start()
    {
        b = GetComponentInChildren<Button>();
        b.onClick.AddListener(runPreferences);
    }
    private void runPreferences() // kick off the simulation before the menu data manager is deactivated
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);
        string contents; // contents of the json file
        using(StreamReader sr = new StreamReader(fs)) { contents = sr.ReadToEnd(); } 
        SetOfPreferences sp = JsonUtility.FromJson<SetOfPreferences>(contents);
        DataManager.preferencesToRun = sp;
        SceneManager.LoadScene("Simulation");
    }
}
