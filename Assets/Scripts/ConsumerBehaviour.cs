using System.Collections.Generic;
using UnityEngine;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born

    private SimulationSystemManager ssm;
    private Rigidbody _rigidbody;
    private void Start()
    {
        ssm = FindObjectOfType<SimulationSystemManager>();
        _rigidbody = GetComponent<Rigidbody>();
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            stats.attributes[attributeKey] = ssm.simulationSettings[attributeKey];
        }
        //_rigidbody.velocity = Vector3.forward * stats.Speed * Time.deltaTime;
    }
    private void Update()
    {
        Vector3 target;
        try
        {
            GameObject nearestObject = Physics2D.CircleCast(new Vector2(transform.position.x, transform.position.y),
                stats.Perceptiveness, Vector2.up).collider.gameObject; // finds the nearest object and returns it
            target = Vector3.Normalize(nearestObject.transform.position - transform.position); // by default move towards
            if (nearestObject.tag == "Consumer")
            {
                // at the moment testing whether it will run away from other organisms since 
                ConsumerBehaviour nearestConsumerData = nearestObject.GetComponent<ConsumerBehaviour>();
                if (nearestConsumerData.stats.Strength == stats.Strength) // run away
                {
                    target = Quaternion.AngleAxis(180f, Vector3.up) * target; // rotate 180;
                }
                // else continue moving towards
            }
        }
        catch
        {
            target = transform.forward;
        }
        _rigidbody.MovePosition(transform.position + target * stats.Speed * Time.deltaTime);
    }
}