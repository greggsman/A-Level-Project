using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class HomeMenu : MonoBehaviour
{
    public static string preferenceFolderPath;
    public static string snapshotFolderPath;
    void Start()
    {
        preferenceFolderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Preferences_Data" + Path.DirectorySeparatorChar;
        snapshotFolderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Snapshot_Data" + Path.DirectorySeparatorChar;
    }
    public void GoMainMenu()
    {
        SceneManager.LoadScene("Preferences");
    }
    public void GoToInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }
}
