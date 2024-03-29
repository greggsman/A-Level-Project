using UnityEngine;
using UnityEngine.UI;

public class PreferenceBehaviour : MonoBehaviour
{
    // Behaviour for one individual setting UI element
    private int value; // default value is 0

    public Slider slider;
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
        // initializes default values for the sliders
        valueText = transform.GetChild(0).GetComponent<Text>();
        slider.value = 0;
    }
    private void Update()
    {
        value = (int)slider.value;
        valueText.text = value.ToString();
    }
    public void FormatSettings()
    {
        // Activated when preferences are commited - allowing for
        // more preferences to be added during debugging

        Preference preference = new Preference(this.name, value);
        datamanager.preferences.Add(preference);
    }
}