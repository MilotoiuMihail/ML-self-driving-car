using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear
{
    public float Ratio { get; private set; }
    public string Name { get; private set; }
    public float MinRpm { get; private set; }
    public float MaxRpm { get; private set; }

    public Gear(float ratio, string name, float minRpm, float maxRpm)
    {
        Ratio = ratio;
        Name = name;
        MinRpm = minRpm;
        MaxRpm = maxRpm;
    }
    public static string ConvertGearIndexToName(int gearIndex)
    {
        switch (gearIndex)
        {
            case 0:
                return "R";
            case 1:
                return "N";
            case 2:
                return "1st";
            case 3:
                return "2nd";
            case 4:
                return "3rd";
            default:
                return $"{gearIndex - 1}th";
        }
    }
}
