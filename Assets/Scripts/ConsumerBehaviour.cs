using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Reproduction issue:
 * Every so often, the consumer's will reproduce way too much, causing loads of consumers to spawn out of nowhere
 * which causes the program to crash.
 * The issue is, when an organism's strength mutates it eats other organisms.
 * Other organisms spawn as a result, which get eaten by other organisms, which causes them to reproduce.
 * Suddenly too many organisms are reproducing all at once.
 * Basically some values need to be tweaked so that the organisms don't respond so rapidly. <3
 */
public class ConsumerBehaviour : MonoBehaviour
{
    public ConsumerData stats; // stats will be provided when organism is born
    public int familyIndex;
    public const float energyRegulator = 0.01f;
    public SphereCollider stealthRange;
    private SimulationSystemManager simulationSystemManager;
    private static int separationConstant = 10;
    private static float mutationMaximum = 10f;
    private void Awake()
    {
        simulationSystemManager = FindObjectOfType<SimulationSystemManager>(); // finds the simulation system manager gameobject in the scene
        stats = new ConsumerData(simulationSystemManager.timeSinceInitialization);
    }
    private void Start()
    {
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            if (stats.StarterOrganism) stats.attributes[attributeKey] = simulationSystemManager.simulationSettings[attributeKey];
            simulationSystemManager.attributeLists[attributeKey].Add(stats.attributes[attributeKey]);
        }
        stealthRange.radius = stats.Perceptiveness;
    }
    private void Update()
    {
        stats.Energy -= energyRegulator; // loses energy slowly when at a standstill
        if (stats.Energy < Mathf.Abs(stats.HungerLimit)) // Death
        {
            simulationSystemManager.livingConsumerPopulation--;
            Destroy(this.gameObject);
        }

        if(stats.Energy > simulationSystemManager.ReproductionThreshold) //Reproduction
        {
            stats.Energy = ConsumerData.DefaultEnergyValue;
            // Debug.Log("reproducing by binary fission");
            Reproduce(simulationSystemManager.MutationChance, Random.Range(-mutationMaximum, mutationMaximum), transform.position + Vector3.left * separationConstant);
            Reproduce(simulationSystemManager.MutationChance, Random.Range(-mutationMaximum, mutationMaximum), transform.position + Vector3.right * separationConstant);
        }
    }
    private void Reproduce(float mutationChance, float mutationAmount, Vector3 newPosition) // Reproduce One organism
    {
        float mutationRNG = Random.Range(0f, 1f);
        ConsumerData newConsumerData = stats;
        if (mutationRNG < mutationChance)
        {
            int indexToMutate = Random.Range(0, ConsumerData.attributeKeys.Length);
            newConsumerData.attributes[ConsumerData.attributeKeys[indexToMutate]] += mutationAmount;
            // Debug.Log("Mutation RNG is " + mutationRNG + ", so this organism is mutating " + ConsumerData.attributeKeys[indexToMutate]);
        }
        ConsumerData offspring = Instantiate(simulationSystemManager.consumer, newPosition, transform.rotation).GetComponent<ConsumerBehaviour>().stats;
        offspring.attributes = newConsumerData.attributes;
        simulationSystemManager.AddToFamilyTrees(stats.familyTreeIndex, offspring); // adds it to the family tree
        Destroy(gameObject);
        simulationSystemManager.livingConsumerPopulation++;
    }
    private void FixedUpdate()
    {
        Vector3 movementVector = FindTarget(stats.Perceptiveness);
        transform.position += movementVector * stats.Speed * Time.fixedDeltaTime;
        transform.LookAt(transform.position + movementVector); 
        stats.Energy -= (movementVector * stats.Speed).magnitude * energyRegulator; // loses energy faster when moving
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
                    Debug.Log("My name is " + name + "Found another consumer called " + surroundingObject.name);
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
                else // if it finds a producer
                {
                    // Debug.Log("My name is " + name + "Found a producer");
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
            simulationSystemManager.Resspawn(producerBehaviour.type);
            stats.Energy += producerBehaviour.Energy;
        }
        if(collisionObject.tag == "Consumer")
        {
            ConsumerData consumerData = collisionObject.GetComponent<ConsumerBehaviour>().stats;
            if (consumerData.Strength < stats.Strength) Destroy(collisionObject);
            stats.Energy += consumerData.Energy;
            simulationSystemManager.livingConsumerPopulation--;
        }
    }

    public static void DebugLog(object value)
    {
        Debug.Log(value);
    }
}