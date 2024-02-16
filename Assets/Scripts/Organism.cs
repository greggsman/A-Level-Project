// raw c#
using System;
using System.Collections.Generic;

public abstract class Organism // base class for consumers and producers
{
    protected float energy { get; set; }
    protected bool starterOrganism { get; set; }

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
    private static int strengthSpeedProportion = 1000;
    public static string[] attributeKeys = new string[] { "Strength/Speed", "Stealth/Perceptiveness", "Max Energy/Hunger" };
    public Dictionary<string, float> attributes = new Dictionary<string, float>(); // descriptions of attributes with their respective values

    // this data will be serialized
    public int familyTreeIndex;
    private float timeInitialized;
    public int generation;
    // default value + scale value
    public float Energy
    {
        get { return energy; }
        set
        {
            if (value > attributes["Max Energy/Hunger"])
            {
                energy = attributes["Max Energy/Hunger"];
            }
            else energy = value;
        }
    }
    public float Strength // absolute strength value (read only)
    {
        get { return attributes["Strength/Speed"]; }
    }
    public float Speed// absolute speed value (read only)
    {
        get { return strengthSpeedProportion / attributes["Strength/Speed"]; }
    }
    public float Stealth // absolute stealth value (read only)
    {
        get { return attributes["Stealth/Perceptiveness"]; }
    }
    public float Perceptiveness // absolute perceptiveness (read only)
    {
        get { return strengthSpeedProportion / attributes["Stealth/Perceptiveness"]; }
    }
    private static float MaxEnergyHungerProportion = 0.05f;
    public float EnergyLostOverTime // greater hunger value = less energy lost over time,
                                    // therefore energy lost over time should be proportional to Maximum Energy
    {
        get { return attributes["Max Energy/Hunger"] * MaxEnergyHungerProportion; }
    }
    public float MaxEnergy
    {
        get { return attributes["Max Energy/Hunger"]; }
    }
    public ConsumerData(float timeInitialized) : base()
    {
        energy = defaultEnergyValue;
        this.timeInitialized = timeInitialized; // records the time that this consumer was spawned into the scene
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
    public float Energy
    {
        get { return energy; }
        set { energy = value; }
    }
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