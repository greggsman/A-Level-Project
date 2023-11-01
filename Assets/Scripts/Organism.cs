// raw c#
using System.Collections.Generic;

public abstract class Organism
{
    protected float energy { get; set; }
    protected bool starterOrganism { get; set; }
}
public class ConsumerData : Organism
{
    // attributes
    public static string[] attributeKeys = new string[] { "Strength/Speed", "Stealth/Perceptiveness", "Maximum Consumption Rate" };
    // this dict is public because I need to access it in Simulation System Manager in Start(), but it kind of defeats the point of abstraction??
    public Dictionary<string, int> attributes = new Dictionary<string, int>();

    public string ID;
    public float Energy
    {
        get { return energy; }
        set { energy = value; }
    }
    // should i use negatives or reciprocals for the sclae
    public int StrengthSpeedScale
    {
        get { return attributes["Strength/Speed"]; }
        set { attributes["Strength/Speed"] = value; }
    }
    public int StealthPerceptivenessScale
    {
        get { return attributes["Stealth/Perceptiveness"]; }
        set { attributes["Stealth/Perceptiveness"] = value; }
    }
    public int Maximum_Consumption_Rate
    {
        get { return attributes["Maximum Consumption Rate"]; }
        set { attributes["Maximum Consumption Rate"] = value; }
    } 
    public ConsumerData()
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
}