using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class FileUI : MonoBehaviour
{
    private Button b;
    private DataManager dm;

    public string filePath;
    public GameObject simulationManager;
    void Start()
    {
        b = GetComponentInChildren<Button>();
        dm = GameObject.Find("SystemManager").GetComponent<DataManager>();
        b.onClick.AddListener(runPreferences);
    }
    private void runPreferences() // kick off the simulation before the menu data manager is deactivated
    {
        FileStream fs = new FileStream(filePath, FileMode.Open);
        string contents;
        using(StreamReader sr = new StreamReader(fs)) { contents = sr.ReadToEnd(); }
        SetOfPreferences sp = JsonUtility.FromJson<SetOfPreferences>(contents);

        SceneManager.LoadScene("Simulation", LoadSceneMode.Single);
        SimulationSystemManager ssm = Instantiate(simulationManager).GetComponent<SimulationSystemManager>();
        // loads the simulation manager
        ssm.simulationPreferences = sp;
    }
}
