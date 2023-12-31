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
{
    private static int defaultEnergyValue = 50;
    public static int DefaultEnergyValue
    {
        get { return defaultEnergyValue; }
    }
    private static int proportionalityConstant = 1000;
    public static string[] attributeKeys = new string[] { "Strength/Speed", "Stealth/Perceptiveness", "Hunger Limit" };
    public Dictionary<string, float> attributes = new Dictionary<string, float>();

    // this data will be serialized
    public int familyTreeIndex;
    private float timeInitialized;
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
        get { return attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public float Perceptiveness
    {
        get { return proportionalityConstant / attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public float HungerLimit
    {
        get { return attributes["Hunger Limit"]; }
        set { attributes["Hunger Limit"] = value; }
    }
    public ConsumerData(float timeInitialized) : base()
    {
        energy = defaultEnergyValue;
        this.timeInitialized = timeInitialized;
        for (int i = 0; i < attributeKeys.Length; i++)
        {
            attributes.Add(attributeKeys[i], 0);
        }
    }
    public string ConvertToCSV() 
    {
        string value = timeInitialized.ToString() + "," + generation.ToString();
        foreach(KeyValuePair<string, float> attribute in attributes)
        {
            value += "," + attribute.Value.ToString();
        }
        return value;
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