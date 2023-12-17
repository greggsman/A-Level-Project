// raw c#
using System;
using System.Collections.Generic;

public abstract class Organism
{
    protected float energy { get; set; }
    protected bool starterOrganism { get; set; }

    public float Energy
    {
        get { return energy; }
        set { energy = value; }
    }
    public bool StarterOrganism
    {
        get { return starterOrganism; }
        set { starterOrganism = value; }
    }
    public Organism()
    {
        starterOrganism = false;
    }
}
public class ConsumerData : Organism
    // fix this
    // proportionality constant rather than default value
{
    private static int defaultEnergyValue = 100;
    public static int DefaultEnergyValue
    {
        get { return defaultEnergyValue; }
    }
    private static int proportionalityConstant = 1000;
    public static string[] attributeKeys = new string[] { "Strength/Speed", "Stealth/Perceptiveness", "Maximum Consumption Rate" };
    public Dictionary<string, float> attributes = new Dictionary<string, float>();

    // this data will be serialized
    public int familyTreeIndex;
    public float timeInitialized;
    public int generation;
    // default value + scale value
    public float Strength
    {
        get { return attributes["Strength/Speed"]; }
        set { attributes["Strength/Speed"] = value; }
    }
    public float Speed
    {
        get { return proportionalityConstant / attributes["Strength/Speed"]; }
        set { attributes["Strength/Speed"] = value; }
    }
    public float Stealth
    {
        get { return defaultEnergyValue + attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public float Perceptiveness
    {
        get { return proportionalityConstant / attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public float Maximum_Consumption_Rate
    {
        get { return attributes["Maximum Consumption Rate"]; }
        set { attributes["Maximum Consumption Rate"] = value; }
    }
    public ConsumerData(float timeInitialized) : base()
    {
        energy = defaultEnergyValue;
        this.timeInitialized = timeInitialized;
        for(int i = 0; i < attributeKeys.Length; i++)
        {
            attributes.Add(attributeKeys[i], 0);
        }
    }


    public string ConvertToJSON()
    {
        string json = "{\n";
        json +=
            "\"timeInitialized\":" + timeInitialized.ToString() + ",\n" +
            "\"generation\":" + generation.ToString() + ",\n";
        json += "\"attributes\": {";
        int counter = 1;
        foreach(KeyValuePair<string, float> kvp in attributes)
        {
            json += "\n\"" + kvp.Key + "\":" + kvp.Value;
            if (counter != attributes.Count) json += ",";
            counter++;
        }
        json += "}}";
        return json;
    }
}
public enum ProducerType { One, Two, Three }
public class ProducerData : Organism
{
    private const int defaultEnergy = 50;
    public ProducerType type;
    public ProducerData(ProducerType type) : base()
    {
        this.type = type;
        if(type == ProducerType.One) { energy = defaultEnergy; }
        if(type == ProducerType.Two) { energy = defaultEnergy * 1.5f; }
        if(type == ProducerType.Three) { energy = defaultEnergy * 2f; }
    }
}
