using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born

    private SimulationSystemManager ssm;
    private Rigidbody _rigidbody;
    private List<int> indexesToIgnore;
    int terrainSize;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        ssm = FindObjectOfType<SimulationSystemManager>();
        indexesToIgnore = new List<int>();
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            stats.attributes[attributeKey] = ssm.simulationSettings[attributeKey];
        }
        terrainSize = ssm.terrainSize;
    }
    private void FixedUpdate()
    {
        if (transform.position.x < 0 || transform.position.x > (terrainSize) * ssm.terrainScale.x ||
            transform.position.z < 0 || transform.position.z > (terrainSize) * ssm.terrainScale.z)
        {
            transform.Rotate(Vector3.up, 180f);
        }
        Vector3 target = FindTarget(stats.Perceptiveness);
        transform.position += target * stats.Speed * Time.fixedDeltaTime;
        transform.LookAt(transform.position + target);
    }
    private Vector3 FindTarget(int perceptiveness)
    {
        List<Collider> surroundingObjects = Physics.OverlapSphere(transform.position, perceptiveness, ssm.lm).ToList();
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
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position); // rotate 180;
                    }
                    if (nearestConsumerData.stats.Strength < stats.Strength)
                    {
                        return Vector3.Normalize(surroundingObject.transform.position - transform.position);
                    }
                    if (nearestConsumerData.stats.Strength == stats.Strength) break;
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
        return transform.forward;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Producer")
        {
            Destroy(collision.gameObject);
        }
    }
}