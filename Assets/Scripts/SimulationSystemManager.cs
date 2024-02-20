using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

[Serializable]
class Binarytree
{
    private List<ConsumerData> nodes;

    // for each node in the tree,
    // there is a value for the location left and right children respectively in
    // leftIndexes and rightIndexes
    private List<int> leftIndexes;
    private List<int> rightIndexes;

    private int defaultIndex;
    // the index for when a node has empty locations for children, normally set as -1

    public int FamilyPopulation { get { return nodes.Count; } } // abstraction of private list consumer data
    public Binarytree(int defaultValue, ConsumerData root)
    {
        //setting default values for the binary tree
        nodes = new List<ConsumerData>();
        leftIndexes = new List<int>();
        rightIndexes = new List<int>();

        defaultIndex = defaultValue;
        // adding the root node to the tree so other nodes can be added
        nodes.Add(root);
        leftIndexes.Add(defaultIndex);
        rightIndexes.Add(defaultIndex);
    }
    public void AddNode(ConsumerData additem)
    {
        additem.generation = (nodes.Count + 1) / 2;
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
                    // look for another space by moving to the next elemetn in nodes list
                }
                else // if right index is free put addItem there
                {
                    rightIndexes[i] = addItemPosition;
                    break;
                }
            }
            else // if left index is free put addItem there
            {
                leftIndexes[i] = addItemPosition;
                break;
            }
        }
    }
    private string CreateCSVRecords(int rootIndex, ref string overallString)
    {
        // overallString is a reference parameter since we want to use the same string on each recursion
        if (rootIndex == defaultIndex) return "";
        CreateCSVRecords(leftIndexes[rootIndex], ref overallString); // Going down left branch
        overallString += nodes[rootIndex].ConvertToCSV() + "\n"; // visiting this node and converting to CSV
        CreateCSVRecords(rightIndexes[rootIndex], ref overallString); // Goint down right branch
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
}
public class SimulationSystemManager : MonoBehaviour
{
    public SetOfPreferences jsonPrefences;

    private string folderPath;
    private List<Binarytree> familyTrees;
    private string fields;

    public List<List<float>> attributeLists = new List<List<float>>();

    public GameObject terrainUnit;
    public Transform terrain;
    public GameObject consumer;
    public GameObject producer;

    public Dictionary<string, float> simulationSettings;
    public LayerMask potentialTargetLayer;

    private TerrainUnitData[,] terrainUnits;

    private Vector3 terrainScale;
    private int terrainSize;

    public Slider MutationChanceSlider;
    public Text MutationChanceText;
    public Slider ReproductionThresholdSlider;
    public Text ReproductionThresholdText;
    public Slider timeScale;
    public Slider MinimumConsumptionSlider;
    public Text MinimumConsumptionText;

    public Slider ProducerOne;
    public Slider ProducerTwo;
    public Slider ProducerThree;
    public Text timePassed;

    public float MutationChance;
    public float ReproductionThreshold;
    public float timeSinceInitialization;
    public int MinimumConsumptionLimit;

    private void Start()
    {
        string folderName = Path.DirectorySeparatorChar + "Snapshot_Data" + Path.DirectorySeparatorChar;
        folderPath = Application.persistentDataPath + folderName;
        if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

        simulationSettings = new Dictionary<string, float>();
        jsonPrefences = DataManager.preferencesToRun;
        familyTrees = new List<Binarytree>();

        foreach (Preference preference in jsonPrefences.preferences) // storing the preferences to a dictionary for easy access
        {
            simulationSettings.Add(preference.description, preference.value);
        }
        ReproductionThresholdSlider.value = ReproductionThresholdSlider.maxValue;
        terrainScale = terrainUnit.transform.localScale;
        timeSinceInitialization = 0f;
        fields = "Spawn Time, Generation";

        foreach (string attributeKey in ConsumerData.attributeKeys)
        {
            fields += "," + attributeKey;
            attributeLists.Add(new List<float>());
        }
        ProducerOne.value = ProducerOne.maxValue;
        SimulationGenerationInstructions();
    }

    private bool spawnAvailable = true; // when this is 0, producers can not spawn
    private bool timeCheck = false;

    private void Update()
    {
        MutationChance = MutationChanceSlider.value;
        MutationChanceText.text = "Mutation chance: " + Math.Round(MutationChance, 2);
        ReproductionThreshold = ReproductionThresholdSlider.value;
        ReproductionThresholdText.text = "Reproduction threshold: " + Math.Round(ReproductionThreshold, 2);
        MinimumConsumptionLimit = (int) MinimumConsumptionSlider.value;
        MinimumConsumptionText.text = "Minimum Consumption Limit: " + MinimumConsumptionLimit;

        Time.timeScale = timeScale.value;
        timeSinceInitialization += Time.deltaTime;
        timePassed.text = timeSinceInitialization.ToString() + " seconds";

        timeCheck = (Math.Truncate(timeSinceInitialization) % simulationSettings["Time Interval to Spawn Producers in frames"]) == 0;
        if (spawnAvailable && timeCheck) SpawnProducerGeneration(ProducerOne.value, ProducerTwo.value, ProducerThree.value);
        spawnAvailable = !timeCheck;
    }
    private void SimulationGenerationInstructions()
    {
        terrainSize = (int)simulationSettings["Terrain Size"]; // terrainSize stores the length of one side of the terrain
        terrainUnits = new TerrainUnitData[terrainSize, terrainSize];

        for (int i = 0; i < terrainSize; i++) // generates the a square grid for the terrain and stores each unit in a 2D array
        {
            for (int j = 0; j < terrainSize; j++)
            {
                Vector3 newPosition = new Vector3(i, 0f, j); // position of a new terrain unit to be generated
                terrainUnits[i, j] = Instantiate(terrainUnit, Vector3.Scale(newPosition, terrainScale),
                    terrainUnit.transform.localRotation, parent: terrain).GetComponent<TerrainUnitData>();
                // new position multiplied by the scale of a terrain unit (set during development)
            }
        }
        for (int i = 0; i < simulationSettings["Initial Consumer Population"]; i++) // spawns the first generation of consumers
        {
            ConsumerBehaviour cb = SpawnRandom(consumer).GetComponent<ConsumerBehaviour>(); // spawns a new consumer and gets its consumer behaviour
            ConsumerData currentConsumer = cb.stats;
            currentConsumer.StarterOrganism = true;
            // Indicates that this organism should have the initial attribute values
            familyTrees.Add(new Binarytree(-1, currentConsumer));
            // create a new binary tree, with the conumser that has just spawned as the root node
            currentConsumer.familyTreeIndex = i;
            // familyTreeIndex indicates which family tree a consumer should be part of when it is spawned into the simulation
            currentConsumer.generation = 0;
        }
        SpawnProducerGeneration(1, 0, 0); // will spawn a generation of producers 100% type one
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
    public void SpawnProducerGeneration(float oneProporition, float twoProportion, float threeProportion)
    {
        int livingProducerPopulation = FindObjectsOfType<ProducerBehaviour>().Length;
        int overallProducerPopulation = (int)simulationSettings["Producer Generation Population"];
        int terrainSizeSquared = terrainSize * terrainSize;
        float total = oneProporition + twoProportion + threeProportion;                                                                                            ;
        float valuePerProportion = overallProducerPopulation / total;
        // splitting the values of the sliders into their relative proportions
        for (int i = 0; i < Math.Truncate(oneProporition * valuePerProportion); i++)
        {
            if(livingProducerPopulation < terrainSizeSquared) // checking if there are too many producers in the scene already3
            {
                ProducerBehaviour currentProducer = SpawnRandom(producer).GetComponent<ProducerBehaviour>();
                currentProducer.stats = new ProducerData(ProducerType.One);
            }
            livingProducerPopulation++;
        }   
        for(int i = 0; i < Math.Truncate(twoProportion * valuePerProportion); i++)
        {
            if(livingProducerPopulation < terrainSizeSquared)
            {
                ProducerBehaviour currentProducer = SpawnRandom(producer).GetComponent<ProducerBehaviour>();
                currentProducer.stats = new ProducerData(ProducerType.Two);
            }
            livingProducerPopulation++;
        }
        for (int i = 0; i < Math.Truncate(threeProportion * valuePerProportion); i++)
        {
            if(livingProducerPopulation < terrainSizeSquared)
            {
                ProducerBehaviour currentProducer = SpawnRandom(producer).GetComponent<ProducerBehaviour>();
                currentProducer.stats = new ProducerData(ProducerType.Three);
            }
            livingProducerPopulation++;
        }
    }
    public void AddToFamilyTrees(int index, ConsumerData newData) // public method so that this can be called in consumerbehaviour.cs
    {
        familyTrees[index].AddNode(newData);
    }
    public void CallOnSnapshot()
    {
        string filename = folderPath + "Snapshot Taken" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ".csv";
        Debug.Log("Snapshot saved to " + folderPath);
        FileStream fileStream = File.Create(filename);
        using(StreamWriter sw = new StreamWriter(fileStream))
        {
            sw.WriteLine("Living Consumer Population," + FindObjectsOfType<ConsumerBehaviour>().Length);
            sw.WriteLine("Living Producer Population," + FindObjectsOfType<ProducerBehaviour>().Length);
            sw.WriteLine("Running for " + timeSinceInitialization + " seconds");
            foreach(Binarytree family in familyTrees)
            {
                sw.WriteLine("Population of consumers in this family: " + family.FamilyPopulation);
                sw.WriteLine(fields);
                sw.WriteLine(family.TreeInCSV);
            }

            for(int i = 0; i < attributeLists.Count; i++)
            {
                for (int j = i+1; j < attributeLists.Count; j++)
                // By setting j as i+1, it is comparing elements of the same list together without making the same comparison twice.
                {
                    Debug.Log(ConsumerData.attributeKeys[i] + " against " + ConsumerData.attributeKeys[j]);
                    Debug.Log(PMCC(attributeLists[i], attributeLists[j]).ToString());
                }
            }
        };
        fileStream.Close();
    }
    private static double PMCC(List<float> xData, List<float> yData)
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
        return ((dataSize * sigmaXY) - (sigmaX * sigmaY))/(Math.Sqrt((dataSize * sigmaXSquared - sigmaX * sigmaX) * (dataSize * sigmaYSquared - sigmaY * sigmaY)));
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Preferences");
    }
}