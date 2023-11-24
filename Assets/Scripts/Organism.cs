// raw c#
using System.Collections.Generic;

public abstract class Organism
{
    protected const int defaultValue = 100;
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
        energy = defaultValue;
        starterOrganism = false;
    }
}
public class ConsumerData : Organism
{
    public static string[] attributeKeys = new string[] { "Strength/Speed", "Stealth/Perceptiveness", "Maximum Consumption Rate" };
    public Dictionary<string, int> attributes = new Dictionary<string, int>();

    public string ID;
    // default value + scale value
    public int Strength
    {
        get { return defaultValue + attributes["Strength/Speed"]; }
        set { attributes["Strength/Speed"] = value; }
    }
    public int Speed
    {
        get { return defaultValue - attributes["Strength/Speed"]; }
        set { attributes["Strength/Speed"] = value; }
    }
    public int Stealth
    {
        get { return defaultValue + attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public int Perceptiveness
    {
        get { return defaultValue - attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public int Maximum_Consumption_Rate
    {
        get { return attributes["Maximum Consumption Rate"]; }
        set { attributes["Maximum Consumption Rate"] = value; }
    }
    public ConsumerData() : base()
    {
        for(int i = 0; i < attributeKeys.Length; i++)
        {
            attributes.Add(attributeKeys[i], 0);
        }
    }
}
public enum ProducerType { One, Two, Three }
public class ProducerData : Organism
{
    public ProducerType type;
    public ProducerData(ProducerType type) : base()
    {
        this.type = type;
    }
}