using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    // Behaviour for one individual setting UI element
    private float value; // default value is 0

    private Slider slider;
    public int maxValue;
    private Text valueText;
    private DataManager datamanager;

    private void Awake()
    {
        // Add this instances 'format settings' method to the commit event
        datamanager = FindObjectOfType<DataManager>();
        datamanager.commitEvent += FormatSettings;
    }
    private void Start()
    {
        valueText = transform.GetChild(0).GetComponent<Text>();
        slider = transform.GetChild(1).GetComponent<Slider>();
    }
    private void Update()
    {
        value = slider.value;
        valueText.text = value.ToString();
    }
    public void FormatSettings()
    {
        // Activated when preferences are commited
        Preference preference = new Preference(this.name, (int)value);
        datamanager.preferences.Add(preference);
    }
}