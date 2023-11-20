using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born

    private SimulationSystemManager ssm;
    private Rigidbody _rigidbody;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        ssm = FindObjectOfType<SimulationSystemManager>();
        if (stats.StarterOrganism)
        {
            foreach (string attributeKey in ConsumerData.attributeKeys)
            {
                stats.attributes[attributeKey] = ssm.simulationSettings[attributeKey];
            }
        }
    }
    private void FixedUpdate()
    {
        Vector3 movementVector = FindTarget(stats.Perceptiveness);
        transform.position += movementVector * stats.Speed * Time.fixedDeltaTime;
    }
    private Vector3 FindTarget(int perceptiveness)
    {
        List<Collider> surroundingObjects = Physics.OverlapSphere(transform.position, perceptiveness, ssm.lm).ToList();
        bool allSurroundingsAreEqual = true;
        if(surroundingObjects.Count != 1) // 1 because surroundingObjects arrary still includes this consumer object
        {
            Vector3 shortestPath = new Vector3(perceptiveness, 100, perceptiveness);
            for(int i = 0; i < surroundingObjects.Count; i++)
            {
                GameObject surroundingObject = surroundingObjects[i].gameObject;
                if (surroundingObject.tag == "Consumer")
                {
                    if (name == surroundingObject.name) continue; // ignore if the organism detected is itself
                    // at the moment testing whether it will run away from other organisms since there is no differentiation between organisms  
                    ConsumerBehaviour nearestConsumerData = surroundingObject.GetComponent<ConsumerBehaviour>();
                    if (nearestConsumerData.stats.Strength > stats.Strength) // run away
                    {
                        Debug.Log("DEBUGGING Found a weaker organism");
                        Debug.Log("This strength " + stats.Strength + " their strength: " + stats.Strength);
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position); // rotate 180;
                    }
                    if (nearestConsumerData.stats.Strength < stats.Strength)
                    {
                        Debug.Log("DEBUGGING Found a stronger organism");
                        Debug.Log("This strength " + stats.Strength + " their strength: " + stats.Strength);
                        return Vector3.Normalize(surroundingObject.transform.position - transform.position);
                    }
                    if (nearestConsumerData.stats.Strength == stats.Strength) continue;
                }
                else // if it finds a consumer
                {
                    allSurroundingsAreEqual = false;
                    Vector3 path = surroundingObject.transform.position - transform.position;
                    if (path.sqrMagnitude < shortestPath.sqrMagnitude) // using sqrMagnitude to avoid sqrRoot every frame update
                    {
                        shortestPath = path;
                    }
                }
            }
            if (allSurroundingsAreEqual) return Vector3.zero;
            shortestPath.y = 0f;
            Debug.Log("DEBUGGING Shortest path found");
            return Vector3.Normalize(shortestPath);
        }
        Debug.Log("DEBUGGING No surroundingobjects left");
        return Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Producer")
        {
            Destroy(collision.gameObject);
        }
    }
}