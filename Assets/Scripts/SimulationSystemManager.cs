using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class Binarytree<T>
{
    public List<T> nodes;
    public List<int> leftIndexes;
    public List<int> rightIndexes;

    int defaultIndex;
    public Binarytree(int defaultValue, T root)
    {
        // this is a bit shit so the alternative method is using a Dictionary<T, List<T>> where T is the type you're using and the list represents the children for each element in the tree
        nodes = new List<T>();
        leftIndexes = new List<int>();
        rightIndexes = new List<int>();

        this.defaultIndex = defaultValue;
        nodes.Add(root);
        leftIndexes.Add(defaultIndex);
        rightIndexes.Add(defaultIndex);
    }
    public void AddNode(T additem)
    {
        nodes.Add(additem);
        leftIndexes.Add(defaultIndex);
        rightIndexes.Add(defaultIndex);
        int addItemPosition = nodes.Count - 1;
        for (int i = 0; i < addItemPosition; i++)
        {
            if (leftIndexes[i] != defaultIndex)
            {
                if (rightIndexes[i] != defaultIndex) // if neither index is free
                {
                    continue;
                }
                else // if right index is free
                {
                    rightIndexes[i] = addItemPosition;
                    break;
                }
            }
            else // if left index is free
            {
                leftIndexes[i] = addItemPosition;
                break;
            }
        }
    }
    public void PreOrderTraversal(int rootIndex)
    {
        if (rootIndex == defaultIndex) { return; }
        // Console.WriteLine(nodes[rootIndex]);
        PreOrderTraversal(leftIndexes[rootIndex]);
        PreOrderTraversal(rightIndexes[rootIndex]);
    }
    public void PrintAdjacencyList()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            // Console.Write(nodes[i] + " " + leftIndexes[i] + " " + rightIndexes[i] + "\n");
        }
    }
}
public class SimulationSystemManager : MonoBehaviour
{
    public SetOfPreferences jsonPrefences;
    public GameObject terrainUnit;
    public GameObject consumer;
    public GameObject producer;
    public Dictionary<string, int> simulationSettings;
    public LayerMask lm;

    private TerrainUnitData[,] terrainUnits;
    public Vector3 terrainScale;
    public int terrainSize;

    private void Start()
    {
        simulationSettings = new Dictionary<string, int>();
        jsonPrefences = DataManager.preferencesToRun;
        foreach (Preference preference in jsonPrefences.preferences)
        {
            simulationSettings.Add(preference.description, preference.value);
        }
        terrainScale = terrainUnit.transform.localScale;
        SimulationGenerationInstructions();
    }

    private void SimulationGenerationInstructions()
    {
        // possible opportunity for recursion here
        terrainSize = simulationSettings["Terrain Size"];
        terrainUnits = new TerrainUnitData[terrainSize, terrainSize];

        for (int i = 0; i < terrainSize; i++) // generates the a square grid for the terrain and stores it in a 2D array
        {
            for (int j = 0; j < terrainSize; j++)
            {
                Vector3 newPosition = new Vector3(i, 0f, j); // position of a new terrain unit to be generated
                terrainUnits[i, j] = Instantiate(terrainUnit, Vector3.Scale(newPosition, terrainScale),
                    terrainUnit.transform.localRotation).GetComponent<TerrainUnitData>();
                // new position multiplied by the scale of a terrain unit (set by me)
            }
        }
        for (int i = 0; i < simulationSettings["Initial Consumer Population"]; i++)
        {
            ConsumerBehaviour currentConsumer = SpawnRandom(consumer).GetComponent<ConsumerBehaviour>();
            currentConsumer.stats.StarterOrganism = true;
        }
        for (int i = 0; i < simulationSettings["Initial Producer Population"]; i++)
        {
            ProducerBehaviour currentProducer = SpawnRandom(producer).GetComponent<ProducerBehaviour>();
            currentProducer.stats.StarterOrganism = true;
        }
    }
    private GameObject SpawnRandom(GameObject entityPrefab)
    {
        bool placeNotFound = true;
        GameObject prefabSceneInstance = new GameObject();
        while (placeNotFound)
        {
            Vector3 organismLocation = new Vector3Int((int)Random.Range(0f, terrainSize),
                1, (int)Random.Range(0f, terrainSize));
            TerrainUnitData currentTerrainUnit = terrainUnits[(int)organismLocation.x, (int)organismLocation.z];
            if (currentTerrainUnit.consumerSpawn || currentTerrainUnit.producerSpawn) continue;
            else
            {
                prefabSceneInstance = Instantiate(entityPrefab, Vector3.Scale(organismLocation, terrainScale), entityPrefab.transform.localRotation);
                //loads entity into scene
                // position of organism needs to be scaled by terrain scale to make sure it doesn't spawn in one corner of the terrain
                placeNotFound = false;
            }
        }
        return prefabSceneInstance;
    }

    /*
    public static Vector3[] MergeSort(Vector3[] objects)
    {
        Vector3[] left;
        Vector3[] right;
        Vector3[] newList = new Vector3[objects.Length];

        if (objects.Length <= 1) return objects;
        int midPointIndex = objects.Length / 2;
        left = new Vector3[midPointIndex];
        if (objects.Length % 2 == 0)
            right = new Vector3[midPointIndex];
        else right = new Vector3[midPointIndex + 1];
        for (int i = 0; i < midPointIndex; i++)
        {
            left[i] = objects[i];
        }
        int counter = 0;
        for (int i = midPointIndex; i < objects.Length; i++)
        {
            right[counter] = objects[i];
            counter++;
        }
        left = MergeSort(left);
        right = MergeSort(right); // recursion
        int newListLength = left.Length + right.Length;
        int leftIndex = 0;
        int rightIndex = 0;
        int newListIndex = 0;
        while (leftIndex < left.Length || rightIndex < right.Length)
        {
            if (leftIndex < left.Length && rightIndex < right.Length)
            {
                if (left[leftIndex].sqrMagnitude <= right[rightIndex].sqrMagnitude)
                {
                    newList[newListIndex] = left[leftIndex];
                    leftIndex++;
                    newListIndex++;
                }
                else
                {
                    newList[newListIndex] = right[rightIndex];
                    rightIndex++;
                    newListIndex++;
                }
            }
            else if (leftIndex < left.Length)
            {
                newList[newListIndex] = left[leftIndex];
                leftIndex++;
                newListIndex++;
            }
            else if (rightIndex < right.Length)
            {
                newList[newListIndex] = right[rightIndex];
                rightIndex++;
                newListIndex++;
            }
        }
        return newList;
    }
    */
}