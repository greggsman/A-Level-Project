using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public float value;
    public int min;
    public int max;

    private Scrollbar sb;
    private Text valueText;
    private DataManager datamanager;

    public float scale;

    private void Awake()
    {
        datamanager = GetComponentInParent<DataManager>();
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
        value = sb.value * scale;
        valueText.text = value.ToString();
    }
    public void FormatSettings()
    {
        Preference preference = new Preference(this.name, value);
        datamanager.preferences.Add(preference);
    }
}
