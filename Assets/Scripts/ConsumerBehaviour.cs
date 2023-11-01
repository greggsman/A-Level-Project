using System.Collections.Generic;
using UnityEngine;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born
    private SimulationSystemManager ssm;
    // object created
    private void Start()
    {
        ssm = FindObjectOfType<SimulationSystemManager>();
        Debug.Log("object created");
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            stats.attributes[attributeKey] = ssm.simulationSettings[attributeKey];
        }
        foreach(KeyValuePair<string, int> kvp in stats.attributes)
        {
            Debug.Log(kvp.Key + ":" + kvp.Value);
        }
    }
    private void Update()
    {

    }
}
