public class Gear
{
    public float Ratio { get; private set; }
    public string Name { get; private set; }
    public float MinRpm { get; private set; }
    public float MaxRpm { get; private set; }

    public Gear(float ratio, int index, float minRpm, float maxRpm)
    {
        Ratio = ratio;
        Name = ConvertGearIndexToName(index);
        MinRpm = minRpm;
        MaxRpm = maxRpm;
    }

    private string ConvertGearIndexToName(int gearIndex)
    {
        switch (gearIndex)
        {
            case 0:
                return "R";
            case 1:
                return "1st";
            case 2:
                return "2nd";
            case 3:
                return "3rd";
            default:
                return $"{gearIndex}th";
        }
    }
}
