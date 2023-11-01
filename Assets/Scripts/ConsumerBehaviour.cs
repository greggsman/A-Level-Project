using System.Collections.Generic;
using UnityEngine;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born
    // object created
    private void Start()
    {
        Debug.Log("object created");
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            Debug.Log(stats.attributes[attributeKey]);
        }
        
    }
    private void Update()
    {

    }
}
