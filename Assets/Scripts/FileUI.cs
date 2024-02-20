using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class FileUI : MonoBehaviour
{
    public string filePath;
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
        SetOfPreferences preferencesToRun = JsonUtility.FromJson<SetOfPreferences>(contents);
        DataManager.preferencesToRun = preferencesToRun;
        SceneManager.LoadScene("Simulation");
    }
    private void DeleteFile()
    {
        File.Delete(filePath);
        DataManager dm = FindObjectOfType<DataManager>();
        // reload main menu without deleted file
        dm.GoToMainMenu();
    }
}
