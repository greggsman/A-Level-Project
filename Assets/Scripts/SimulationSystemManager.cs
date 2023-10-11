using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSystemManager : MonoBehaviour
{
    public SetOfPreferences jsonPrefences;
    public GameObject terrainUnit;
    public GameObject consumer;
    public GameObject producer;

    private Dictionary<string, int> simulationSettings;

    //keys
    private string consumerPop = "Initial Consumer Population";
    private string prodPop = "Initial Producer Population";

    private TerrainUnitData[,] terrainUnits;
    private Vector3 terrainScale;

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
        int terrainSize = simulationSettings["Terrain Size"];
        terrainUnits = new TerrainUnitData[terrainSize, terrainSize];

        for (int i = 0; i < terrainSize; i++) // generates the a square grid for the terrain and stores it in a 2D array
        {
            for (int j = 0; j < terrainSize; j++)
            {
                Vector3 newPosition = new Vector3(i, 0f, j); // position of a new terrain unit to be generated
                terrainUnits[i,j] = Instantiate(terrainUnit, Vector3.Scale(newPosition, terrainScale),
                    terrainUnit.transform.localRotation).GetComponent<TerrainUnitData>();
                // new position multiplied by the scale of a terrain unit (set by me)
            }
        }
        Spawn(terrainSize, consumerPop, consumer); // avoids code repetition
        Spawn(terrainSize, prodPop, producer);
    }
    private void Spawn(int terrainSize, string entity_count, GameObject entity)
    {
        for (int i = 0; i < simulationSettings[entity_count]; i++)
        {
            bool placeNotFound = true;
            while (placeNotFound)
            {
                Vector3 organismLocation = new Vector3Int((int)Random.Range(0f, terrainSize),
                    1, (int)Random.Range(0f, terrainSize));
                TerrainUnitData currentTerrainUnit = terrainUnits[(int)organismLocation.x, (int)organismLocation.z];
                if (currentTerrainUnit.consumerSpawn || currentTerrainUnit.producerSpawn) { continue; }
                else
                {
                    Instantiate(entity, Vector3.Scale(organismLocation, terrainScale), entity.transform.localRotation);
                    // position of organism needs to be scaled by terrain scale to make sure it doesn't spawn in one corner of the terrain
                    placeNotFound = false;
                }
            }
        }
    }
}