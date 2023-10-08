using UnityEngine;

public abstract class Organism
{
    protected float energy { get; set; }
}
public class ConsumerData : Organism
{
    // attributes
    private int _strength_speed_scale;
    private int _stealth_perceptivenesss_scale;

    public float Energy
    {
        get { return energy; }
        set { energy = value; }
    }
    public int StrengthSpeedScale
    {
        get { return _strength_speed_scale; }
        set { _strength_speed_scale = value; }
    }
    public int StealthPerceptivenessScale
    {
        get { return StealthPerceptivenessScale; }
        set { StealthPerceptivenessScale = value; }
    }

    public string ID;
    public ConsumerData(int sss, int sps)
    {
        _strength_speed_scale = sss;
        _stealth_perceptivenesss_scale = sps;
        string ID = "Con."+_strength_speed_scale+"|"+_stealth_perceptivenesss_scale;
    }
}
public class Producer : Organism
{
    // prod1, prod2 or prod3
}