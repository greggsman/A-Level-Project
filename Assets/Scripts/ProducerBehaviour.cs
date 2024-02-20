using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProducerBehaviour : MonoBehaviour
{
    public ProducerData stats;
    static int alphaComponent = 100;
    private void Start()
    {
        if(stats.type == ProducerType.One)
        {
            GetComponent<MeshRenderer>().material.color = new Color(43, 255f, 0f, alphaComponent);
        }
        else if(stats.type == ProducerType.Two)
        {
            GetComponent<MeshRenderer>().material.color = new Color(50, 0f, 50f, alphaComponent);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = new Color(100f, 0f, 0f, alphaComponent);
        }
    }
}
