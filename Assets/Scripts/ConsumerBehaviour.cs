using System.Collections.Generic;
using System;
using UnityEngine;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born

    private SimulationSystemManager ssm;
    private void Start()
    {
        ssm = FindObjectOfType<SimulationSystemManager>();
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            stats.attributes[attributeKey] = ssm.simulationSettings[attributeKey];
        }
    }
    private void FixedUpdate()
    {
        transform.position += FindTarget(stats.Perceptiveness) * stats.Speed * Time.fixedDeltaTime;
    }
    private Vector3 FindTarget(int perceptiveness)
    {
        Collider[] surroundingObjects = Physics.OverlapSphere(transform.position, perceptiveness, ssm.lm);
        if(surroundingObjects.Length != 1) // 1 because surroundingObjects arrary still includes this consumer object
        {
            Vector3 shortestPath = new Vector3(perceptiveness, 100, perceptiveness);
            foreach (Collider c in surroundingObjects)
            {
                GameObject surroundingObject = c.gameObject;
                if (name == surroundingObject.name) continue; // ignore if the organism detected is itself
                if (surroundingObject.tag == "Consumer")
                {
                    // at the moment testing whether it will run away from other organisms since 
                    ConsumerBehaviour nearestConsumerData = surroundingObject.GetComponent<ConsumerBehaviour>();
                    if (nearestConsumerData.stats.Strength == stats.Strength) // run away
                    {
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position) ; // rotate 180;
                    }
                }
                else // if its a producer
                {
                    Vector3 path = surroundingObject.transform.position - transform.position;
                    if (path.sqrMagnitude < shortestPath.sqrMagnitude) // using sqrMagnitude to avoid sqrRoot every frame update
                    {
                        shortestPath = path;
                    }
                }
            }
            shortestPath.y = 0f;
            return Vector3.Normalize(shortestPath);
        }
        else
        {
            return Vector3.forward;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Producer")
        {
            Destroy(collision.gameObject);
        }
    }
}