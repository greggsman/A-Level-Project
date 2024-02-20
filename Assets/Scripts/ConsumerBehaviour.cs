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
    private const float energyRegulator = 0.01f;
    public SphereCollider stealthRange;
    private SimulationSystemManager simulationSystemManager;
    private static int separationConstant = 8;
    private static float mutationMaximum = 10f;
    private const int positionForOrganismDeath = -3;
    private void Awake()
    {
        simulationSystemManager = FindObjectOfType<SimulationSystemManager>(); // finds the simulation system manager gameobject in the scene
        stats = new ConsumerData(simulationSystemManager.timeSinceInitialization);
    }
    private void Start()
    {
        for(int i = 0; i < ConsumerData.attributeKeys.Length; i++)
        {
            if (stats.StarterOrganism) stats.attributes[ConsumerData.attributeKeys[i]] = simulationSystemManager.simulationSettings[ConsumerData.attributeKeys[i]];
            simulationSystemManager.attributeLists[i].Add(stats.attributes[ConsumerData.attributeKeys[i]]);
        }
        stealthRange.radius = stats.Perceptiveness;
        if (stats.StarterOrganism)
        {
            Debug.Log("Attribute values of this first generation consumer:\n" +
            "Strength: " + stats.Strength + ", Speed: " + stats.Speed + "\n" +
            "Stealth: " + stats.Stealth + ", Perceptiveness: " + stats.Perceptiveness + "\n" +
            "Energy Lost Over Time: " + stats.EnergyLostOverTime + ", Max Energy: " + stats.MaxEnergy); // Testing objective 3.4, 2.6
        }
    }
    private void Update()
    {
        stats.Energy -= stats.EnergyLostOverTime * Time.deltaTime; // loses energy slowly when at a standstill
        //Debug.Log("Energy " + stats.Energy);
        if (stats.Energy < simulationSystemManager.MinimumConsumptionLimit) // Death
        {
            Debug.Log("This organism has run out of energy"); // Testing objective 2.7.2
            Destroy(this.gameObject);
        }

        if(stats.Energy > simulationSystemManager.ReproductionThreshold) //Reproduction
        {
            stats.Energy = ConsumerData.DefaultEnergyValue;
            Reproduce(simulationSystemManager.MutationChance, Random.Range(-mutationMaximum, mutationMaximum), transform.position + Vector3.left * separationConstant);
            Reproduce(simulationSystemManager.MutationChance, Random.Range(-mutationMaximum, mutationMaximum), transform.position + Vector3.right * separationConstant);
        }

        if(transform.position.y < positionForOrganismDeath)
        {
            Destroy(gameObject);
        }
    }
    private void Reproduce(float mutationChance, float mutationAmount, Vector3 newPosition) // Reproduce One organism
    {
        float mutationRNG = Random.Range(0f, 1f);
        ConsumerData newConsumerData = stats;
        if (mutationRNG < mutationChance) // If random condition to mutate is met
        {
            int indexToMutate = Random.Range(0, ConsumerData.attributeKeys.Length); // chooses a random attribute to mutate
            newConsumerData.attributes[ConsumerData.attributeKeys[indexToMutate]] += mutationAmount;
            //mutation amount dictates how much the value should change
            Debug.Log("Reproducing, with mutation on attribute " + ConsumerData.attributeKeys[indexToMutate]); // testing mutation
        }
        else
        {
            Debug.Log("Reproducing, no mutation");
        }
        // setting new attribute values
        ConsumerData offspring = Instantiate(simulationSystemManager.consumer, newPosition, transform.rotation).GetComponent<ConsumerBehaviour>().stats;
        offspring.attributes = newConsumerData.attributes; 
        // adding this new organism to the correct family trees
        simulationSystemManager.AddToFamilyTrees(stats.familyTreeIndex, offspring); // adds it to the family tree
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        Vector3 movementVector = FindTarget(stats.Perceptiveness);
        transform.position += movementVector * stats.Speed * Time.fixedDeltaTime;
        transform.LookAt(transform.position + movementVector);
        stats.Energy -= (movementVector * stats.Speed).magnitude * energyRegulator * Time.fixedDeltaTime; // loses energy faster when moving
        // rather than subtract stats.Speed, since that will always be the magnitude as movementVector is normalized,
        // we multiply by movementVector in case movementVector has no magnitude i.e. its at a standstill
    }
    private Vector3 FindTarget(float perceptiveness)
    {
        List<Collider> surroundingObjects = Physics.OverlapSphere(transform.position, perceptiveness, simulationSystemManager.potentialTargetLayer).ToList();
        // This is the list of targets in the organism's surroundings
        bool allSurroundingsAreEqual = true; // This will remain true if all surround targets are Consumers and all have the same strength value
        if(surroundingObjects.Count != 1) // checks if there are anything at all in its surroundings. If not, it will return an empty vector
        {
            Vector3 shortestPath = new Vector3(perceptiveness, 100, perceptiveness); // this will store the vector of the nearest producer
            for(int i = 0; i < surroundingObjects.Count; i++) // examines each detected target separately
            {
                GameObject surroundingObject = surroundingObjects[i].gameObject;
                if (surroundingObject.tag == "Consumer")
                {
                    ConsumerBehaviour nearestConsumerData = surroundingObject.GetComponent<ConsumerBehaviour>();
                    if (nearestConsumerData.stats.Strength > stats.Strength)
                    {
                        // Predator detected, run away
                        return Vector3.Normalize(Quaternion.AngleAxis(180f, Vector3.up) * surroundingObject.transform.position);
                        // returns a normalized vector with direction pointing away from the predator 
                    }
                    if (nearestConsumerData.stats.Strength < stats.Strength)
                    {
                        // Prey detected, move towards it
                        return Vector3.Normalize(surroundingObject.transform.position - transform.position);
                    }
                    if (nearestConsumerData.stats.Strength == stats.Strength) continue; // ignore if its a consumer with the same strength
                }
                else
                {
                    // producer detected, move towards it
                    allSurroundingsAreEqual = false;
                    Vector3 path = surroundingObject.transform.position - transform.position;
                    if (path.sqrMagnitude < shortestPath.sqrMagnitude) // using sqrMagnitude to avoid sqrRoot every frame update
                    {
                        // if a closer producer is detected, shortes path is reassigned
                        shortestPath = path;
                    }
                }
            }
            if (allSurroundingsAreEqual) return Vector3.zero; // If true, do nothing
            shortestPath.y = 0f; // takes care of issues where the shortest path instructs the organism to move up or down
            return Vector3.Normalize(shortestPath);
        }
        return Vector3.zero; // if nothing, i.e. no producers or consumers, are detected
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        if(collisionObject.tag == "Producer")
        {
            Destroy(collision.gameObject);
            ProducerData producerBehaviour = collisionObject.GetComponent<ProducerBehaviour>().stats;
            stats.Energy += producerBehaviour.Energy;
        }
        if(collisionObject.tag == "Consumer")
        {
            ConsumerData consumerData = collisionObject.GetComponent<ConsumerBehaviour>().stats;
            if (consumerData.Strength < stats.Strength)
            {
                Destroy(collisionObject);
                Debug.Log("One consumer has eaten another. Predator strength :" + stats.Strength + " + prey strength: " + consumerData.Strength);
            }
            stats.Energy += consumerData.Energy;
        }
    }
}