using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSystemManager : MonoBehaviour
{
    public SetOfPreferences simulationPreferences;

    private int terrainSize;
    private int mutationChance;
    private int initialPopulation;
    private int foodDensity;
    private void Start()
    {
        InitializeSimulation();
    }
    private void InitializeSimulation()
    {
        for(int i  = 0; i < terrainSize; i++)
        {
            for(int j = 0; j < terrainSize; j++)
            {
                
            }
        }
    }
}
