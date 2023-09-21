using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileUI : MonoBehaviour
{
    private Button b;
    private DataManager dm;

    public string filePath;
    void Start()
    {
        b = GetComponentInChildren<Button>();
        dm = GameObject.Find("SystemManager").GetComponent<DataManager>();
        Debug.Log(filePath);
        b.onClick.AddListener(runSettings);
    }
    private void runSettings()
    {
        dm.runSettings(filePath);
    }
}
