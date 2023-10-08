using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class FileUI : MonoBehaviour
{
    public string filePath;
    public GameObject simulationManager;
    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(runPreferences);
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(DeleteFile);
    }
    private void runPreferences() // kick off the simulation before the menu data manager is deactivated
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);
        string contents; // contents of the json file
        using(StreamReader sr = new StreamReader(fs)) { contents = sr.ReadToEnd(); }
        fs.Close();
        SetOfPreferences sp = JsonUtility.FromJson<SetOfPreferences>(contents);
        DataManager.preferencesToRun = sp;
        SceneManager.LoadScene("Simulation");
    }
    private void DeleteFile()
    {
        File.Delete(filePath);
        DataManager dm = FindObjectOfType<DataManager>();
        // reload main menu without deleted file
        dm.GoToNewPreferenceMenu();
        dm.GoToMainMenu();
        // IDK why it has to work this way but if you don't go to the new preference menu first it just doesn't work fml
    }
}
