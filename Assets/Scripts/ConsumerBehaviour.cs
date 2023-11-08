using System.Collections.Generic;
using System;
using UnityEngine;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born

    private SimulationSystemManager ssm;
    private Rigidbody _rigidbody;
    int terrainSize;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        ssm = FindObjectOfType<SimulationSystemManager>();
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            stats.attributes[attributeKey] = ssm.simulationSettings[attributeKey];
        }
        terrainSize = ssm.terrainSize;
    }
    private void FixedUpdate()
    {
        Vector3 target = FindTarget(stats.Perceptiveness);
        transform.position += target * stats.Speed * Time.fixedDeltaTime;
        transform.LookAt(transform.position + target);

        if(transform.position.x < -1 || transform.position.x > (terrainSize) * ssm.terrainScale.x ||
            transform.position.z < -1 || transform.position.z > (terrainSize) * ssm.terrainScale.z)
        {
            Debug.Log("out of bounds");
            transform.Rotate(Vector3.up, 180f);
        }
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
                if (surroundingObject.tag == "Consumer")
                {
                    if (name == surroundingObject.name) continue; // ignore if the organism detected is itself
                    // at the moment testing whether it will run away from other organisms since there is no differentiation between organisms  
                    ConsumerBehaviour nearestConsumerData = surroundingObject.GetComponent<ConsumerBehaviour>();
                    if (nearestConsumerData.stats.Strength == stats.Strength) // run away
                    {
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position) ; // rotate 180;
                    }
                    /*
                    if (nearestConsumerData.stats.Strength > stats.Strength) // run away
                    {
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position); // rotate 180;
                    }
                    if (nearestConsumerData.stats.Strength == stats.Strength) continue;
                    if (nearestConsumerData.stats.Strength < stats.Strength) return Vector3.Normalize(surroundingObject.transform.position - transform.position);
                    */
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
            return transform.forward;
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