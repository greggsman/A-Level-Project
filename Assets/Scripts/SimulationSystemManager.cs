using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[Serializable]
class Binarytree
{
    private List<ConsumerData> nodes;
    private List<int> leftIndexes;
    private List<int> rightIndexes;

    private int defaultIndex;

    public int FamilyPopulation { get { return nodes.Count; } }
    public Binarytree(int defaultValue, ConsumerData root)
    {
        nodes = new List<ConsumerData>();
        leftIndexes = new List<int>();
        rightIndexes = new List<int>();

        defaultIndex = defaultValue;
        nodes.Add(root);
        leftIndexes.Add(defaultIndex);
        rightIndexes.Add(defaultIndex);
    }
    public void AddNode(ConsumerData additem)
    {
        additem.generation = nodes.Count / 2;
        // doesn't work at the moment, save for testing section
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
    private string CreateCSVRecords(int rootIndex, ref string overallString)
    {
        if (rootIndex == defaultIndex) return "";
        overallString += nodes[rootIndex].ConvertToCSV() + "\n";
        CreateCSVRecords(leftIndexes[rootIndex], ref overallString);
        CreateCSVRecords(rightIndexes[rootIndex], ref overallString);
        return overallString;
    }
    public string TreeInCSV
    {
        get
        {
            string value = "";
            return value = CreateCSVRecords(0, ref value) + "\n";
        }
    }
    // we're not using traverse and convert at the moment
    public void PrintDebugAdjacencyList()
    {
        string message = "";
        for (int i = 0; i < nodes.Count; i++)
        {
            message += nodes[i].ConvertToCSV() + " " + leftIndexes[i] + " " + rightIndexes[i] + "\n";
        }
        Debug.Log(message);
    }
    // check using a random value
}
public class SimulationSystemManager : MonoBehaviour
{
    public SetOfPreferences jsonPrefences;

    private string folderPath;
    private List<Binarytree> familyTrees;
    private string fields;

    public int livingConsumerPopulation;
    public int livingProducerPopulation;

    public Dictionary<string, List<float>> attributeLists = new Dictionary<string, List<float>>();

    public GameObject terrainUnit;
    public GameObject consumer;
    public GameObject producer;

    public Dictionary<string, int> simulationSettings;
    public LayerMask lm;

    private TerrainUnitData[,] terrainUnits;

    public Vector3 terrainScale;
    public int terrainSize;
    public Slider MutationChanceSlider;
    public Text MutationChanceText;
    public Slider ReproductionThresholdSlider;
    public Text ReproductionThresholdText;
    public Slider timeScale;

    private float mutationChance;
    public float MutationChance
    {
        get { return mutationChance; }
    }
    private float reproductionThreshold;
    public float ReproductionThreshold
    {
        get { return reproductionThreshold; }
    }
    public float timeSinceInitialization;
    private void Start()
    {
        string folderName = Path.DirectorySeparatorChar + "Snapshot_Data" + Path.DirectorySeparatorChar;
        folderPath = Application.persistentDataPath + folderName;
        if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

        simulationSettings = new Dictionary<string, int>();
        jsonPrefences = DataManager.preferencesToRun;
        familyTrees = new List<Binarytree>();

        ReproductionThresholdSlider.minValue = ConsumerData.DefaultEnergyValue * 2f;
        ReproductionThresholdSlider.maxValue = ConsumerData.DefaultEnergyValue * 4f;
        foreach (Preference preference in jsonPrefences.preferences)
        {
            simulationSettings.Add(preference.description, preference.value);
        }
        livingConsumerPopulation = simulationSettings["Initial Consumer Population"];
        livingProducerPopulation = simulationSettings["Initial Producer Population"];
        terrainScale = terrainUnit.transform.localScale;
        timeSinceInitialization = 0f;
        fields = "Spawn Time, Generation";
        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            fields += "," + attributeKey;
            attributeLists.Add(attributeKey, new List<float>());
        }
        SimulationGenerationInstructions();
    }
    private void Update()
    {
        mutationChance = MutationChanceSlider.value;
        MutationChanceText.text = "Mutation chance: " + Math.Round(mutationChance, 2);
        reproductionThreshold = ReproductionThresholdSlider.value;
        ReproductionThresholdText.text = "Reproduction threshold: " + Math.Round(reproductionThreshold, 2);
        timeSinceInitialization += Time.deltaTime;
        Time.timeScale = timeScale.value;
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
        for (int i = 0; i < simulationSettings["Initial Consumer Population"]; i++) // for CONSUMERS
        {
            ConsumerBehaviour cb = SpawnRandom(consumer).GetComponent<ConsumerBehaviour>();
            cb.gameObject.name += i;
            ConsumerData currentConsumer = cb.stats;
            currentConsumer.StarterOrganism = true;
            familyTrees.Add(new Binarytree(-1, currentConsumer)); // create a new binary tree
            currentConsumer.familyTreeIndex = i;
            currentConsumer.generation = 0;
        }
        for (int i = 0; i < simulationSettings["Initial Producer Population"]; i++)
        {
            ProducerBehaviour currentProducer = SpawnRandom(producer).GetComponent<ProducerBehaviour>();
            currentProducer.stats = new ProducerData(ProducerType.One);
        }
    }
    public GameObject SpawnRandom(GameObject entityPrefab)
    {
        bool placeNotFound = true;
        GameObject prefabSceneInstance = entityPrefab; // rather than new gameobject
        while (placeNotFound)
        {
            Vector3 organismLocation = new Vector3Int((int)UnityEngine.Random.Range(0f, terrainSize),
                1, (int)UnityEngine.Random.Range(0f, terrainSize));
            TerrainUnitData currentTerrainUnit = terrainUnits[(int)organismLocation.x, (int)organismLocation.z];
            if (currentTerrainUnit.alreadyHasEntity) continue;
            else
            {
                prefabSceneInstance = Instantiate(entityPrefab, Vector3.Scale(organismLocation, terrainScale), entityPrefab.transform.localRotation);
                //loads entity into scene
                // position of organism needs to be scaled by terrain scale to make sure it doesn't spawn in one corner of the terrain
                placeNotFound = false;
                currentTerrainUnit.alreadyHasEntity = true;
            }
        }
        return prefabSceneInstance;
    }
    public void Respawn(ProducerType producerType)
    {
        ProducerBehaviour producerBehaviour = SpawnRandom(producer).GetComponent<ProducerBehaviour>();
        producerBehaviour.stats = new ProducerData(producerType);
    }
    public void AddToFamilyTrees(int index, ConsumerData newData) // public method so that this can be called in consumerbehaviour.cs
    {
        familyTrees[index].AddNode(newData);
    }
    public void CallOnSnapshot()
    {
        string filename = folderPath + "Snapshot Taken" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
        FileStream fileStream = File.Create(filename);
        Debug.Log("Saved to " + filename);
        using(StreamWriter sw = new StreamWriter(fileStream))
        {
            sw.WriteLine("Living Consumer Population," + livingConsumerPopulation);
            sw.WriteLine("Living Producer Population," + livingProducerPopulation);
            sw.WriteLine("Running for " + timeSinceInitialization + " seconds");
            foreach(Binarytree family in familyTrees)
            {
                sw.WriteLine("Population of consumers in this family: " + family.FamilyPopulation);
                sw.WriteLine(fields);
                sw.WriteLine(family.TreeInCSV);
            }
            sw.WriteLine("Correlation between strength/speed and stealth/perceptiveness " +
                Pearson_Product_Moment_Correlation_Coefficient(attributeLists["Strength/Speed"], attributeLists["Stealth/Perceptiveness"]));
            sw.WriteLine("Correlation between strength/speed and maximum consumption rate " +
                Pearson_Product_Moment_Correlation_Coefficient(attributeLists["Strength/Speed"], attributeLists["Maximum Consumption Rate"]));
            sw.WriteLine("Correlation between stealth/perceptiveness and maximum consumption rate " +
                Pearson_Product_Moment_Correlation_Coefficient(attributeLists["Stealth/Perceptiveness"], attributeLists["Maximum Consumption Rate"]));
        };
        fileStream.Close();
    }
    private static double Pearson_Product_Moment_Correlation_Coefficient(List<float> xData, List<float> yData)
    {
        float sigmaX = 0;
        float sigmaY = 0;   
        float sigmaXY = 0;
        float sigmaXSquared = 0;
        float sigmaYSquared = 0;
        int dataSize = xData.Count;

        for(int i = 0; i < dataSize; i++)
        {
            sigmaX += xData[i];
            sigmaY += yData[i];
            sigmaXY += xData[i] * yData[i];
            sigmaXSquared += xData[i] * xData[i];
            sigmaYSquared += yData[i] * yData[i];
        }

        return (dataSize * sigmaXY) - (sigmaX * sigmaY) /
            Math.Sqrt((dataSize * sigmaXSquared - sigmaX * sigmaX) *
            (dataSize * sigmaYSquared - sigmaY * sigmaY));
    }
}