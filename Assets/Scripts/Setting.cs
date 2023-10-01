using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    // Behaviour for one individual setting UI element
    public float value;
    public int min;
    public int max;

    private Scrollbar sb;
    private Text valueText;
    private DataManager datamanager;

    public float scale;

    private void Awake()
    {
        // Add this instances 'format settings' method to the commit event
        datamanager = FindObjectOfType<DataManager>();
        datamanager.commitEvent += FormatSettings;
    }
    private void Start()
    {
        value = 0;
        valueText = transform.GetChild(1).GetComponent<Text>();
        sb = GetComponentInChildren<Scrollbar>();
    }
    private void Update()
    {
        // Allows scrollbar to be interacted with
        value = sb.value * scale;
        valueText.text = value.ToString();
    }
    public void FormatSettings()
    {
        // Activated when preferences are commited
        Preference preference = new Preference(this.name, value);
        datamanager.preferences.Add(preference);
    }
}
