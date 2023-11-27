using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats = new ConsumerData(); // stats will be provided when organism is born
    public int familyIndex;
    public const float energyRegulator = 0.01f;
    private SimulationSystemManager simulationSystemManager;
    private Rigidbody _rigidbody;
    private static int separationConstant = 2;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>(); // set this variable as the rigidbody attached to this consumer
        simulationSystemManager = FindObjectOfType<SimulationSystemManager>(); // finds the simulation system manager gameobject in the scene
        if (stats.StarterOrganism)
        {
            foreach (string attributeKey in ConsumerData.attributeKeys)
            {
                stats.attributes[attributeKey] = simulationSystemManager.simulationSettings[attributeKey];
            }
        }
    }
    private void Update()
    {
        stats.Energy -= energyRegulator; // loses energy slowly when at a standstill
        if (stats.Energy < Mathf.Abs(stats.Maximum_Consumption_Rate)) // Death
        {
            Destroy(this.gameObject);
        }

        if(stats.Energy > simulationSystemManager.ReproductionThreshold) //Reproduction
        {
            stats.Energy = ConsumerData.DefaultEnergyValue;
            Reproduce(simulationSystemManager.MutationChance, Random.Range(1f, 50f), transform.position + Vector3.left * separationConstant);
            Reproduce(simulationSystemManager.MutationChance, Random.Range(1f, 50f), transform.position + Vector3.right * separationConstant);
        }
    }
    private void FixedUpdate()
    {
        Vector3 movementVector = FindTarget(stats.Perceptiveness);
        transform.position += movementVector * stats.Speed * Time.fixedDeltaTime;
        transform.LookAt(transform.position + movementVector); // loses energy faster when moving
        stats.Energy -= (movementVector * stats.Speed).magnitude * energyRegulator;
        // rather than subtract stats.Speed, since that will always be the magnitude as movementVector is normalized,
        // we multiply by movementVector in case movementVector has no magnitude i.e. its at a standstill
    }
    private Vector3 FindTarget(float perceptiveness)
    {
        List<Collider> surroundingObjects = Physics.OverlapSphere(transform.position, perceptiveness, simulationSystemManager.lm).ToList();
        bool allSurroundingsAreEqual = true;
        if(surroundingObjects.Count != 1) // 1 because surroundingObjects arrary still includes this consumer object
        {
            Vector3 shortestPath = new Vector3(perceptiveness, 100, perceptiveness);
            for(int i = 0; i < surroundingObjects.Count; i++)
            {
                GameObject surroundingObject = surroundingObjects[i].gameObject;
                if (surroundingObject.tag == "Consumer")
                { 
                    ConsumerBehaviour nearestConsumerData = surroundingObject.GetComponent<ConsumerBehaviour>();
                    if (nearestConsumerData.stats.Strength > stats.Strength) // run away
                    {
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position); // rotate 180;
                    }
                    if (nearestConsumerData.stats.Strength < stats.Strength)
                    {
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
            return Vector3.Normalize(shortestPath);
        }
        return Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if(collisionObject.tag == "Producer")
        {
            Destroy(collision.gameObject);
            ProducerData producerBehaviour = collisionObject.GetComponent<ProducerBehaviour>().stats;
            simulationSystemManager.Respawn(producerBehaviour.type);
            stats.Energy += producerBehaviour.Energy;
        }
        if(collisionObject.tag == "Consumer")
        {
            ConsumerData consumerData = collisionObject.GetComponent<ConsumerBehaviour>().stats;
            if (consumerData.Strength < stats.Strength) Destroy(collisionObject);
        }
    }
    private void Reproduce(float mutationChance, float mutationAmount, Vector3 newPosition) // Reproduce One organism
    {
        float mutationRNG = Random.Range(0f, 1f);
        ConsumerData newConsumerData = stats;
        if(mutationRNG > mutationChance)
        {
            int indexToMutate = Random.Range(0, ConsumerData.attributeKeys.Length);
            newConsumerData.attributes[ConsumerData.attributeKeys[indexToMutate]] += mutationAmount;
        }
        ConsumerBehaviour offspring = Instantiate(simulationSystemManager.consumer, newPosition, transform.rotation).GetComponent<ConsumerBehaviour>();
        offspring.stats = newConsumerData;
        simulationSystemManager.AddToFamilyTrees(stats.familyTreeIndex, offspring.stats); // adds it to the family tree
        Destroy(gameObject);
    }
}