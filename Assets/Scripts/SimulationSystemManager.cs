using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSystemManager : MonoBehaviour
{
    public SetOfPreferences jsonPrefences;
    public GameObject terrainUnit;
    public GameObject consumer;

    private Dictionary<string, int> simulationSettings = new Dictionary<string, int>();

    //keys
    private string consumerPop = "Initial Consumer Population";
    private string prodPop = "Initial Producer Population";

    private void Start()
    {
        jsonPrefences = DataManager.preferencesToRun;
        foreach (Preference preference in jsonPrefences.preferences)
        {
            simulationSettings.Add(preference.description, preference.value);
            Debug.Log(preference.description + ":" + preference.value);
        }
        GenerateSimulation();
    }

    private void GenerateSimulation()
    {
        // possible opportunity for recursion here
        int terrainSize = simulationSettings["Terrain Size"];
        GameObject[,] terrainUnits = new GameObject[terrainSize, terrainSize];
        for (int i = 0; i < terrainSize; i++)
        {
            for (int j = 0; j < terrainSize; j++)
            {
                Vector3 newPosition = new Vector3(i, 0f, j);
                Debug.LogFormat("{0} {1} {2}", newPosition.x, newPosition.y, newPosition.z);
                Instantiate(terrainUnit, newPosition, new Quaternion(0f, 0f, 0f, 0f));
            }
        }
        for(int i = 0; i < simulationSettings[consumerPop]; i++)
        {
            bool placeNotFound = true;
            while (placeNotFound)
            {
                Vector3 organismLocation = new Vector3Int((int)Random.Range(0f, terrainSize),
                    1, (int) Random.Range(0f, terrainSize));
                GameObject currentTerrainUnit = terrainUnits[(int) organismLocation.x, (int) organismLocation.z];
                if (currentTerrainUnit.GetComponent<TerrainUnitData>().consumerSpawn)
                {
                    continue;
                }
                else
                {
                    Instantiate(consumer, organismLocation, new Quaternion(0f, 0f, 0f, 0f));
                }
            }
        }
    }
}