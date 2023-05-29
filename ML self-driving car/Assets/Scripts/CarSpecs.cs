using UnityEngine;


[CreateAssetMenu(fileName = "CarSpecifications", menuName = "ScriptableObjects/CarSpecs")]
public class CarSpecs : ScriptableObject
{
    [field: SerializeField] public DriveType Drive { get; private set; }
    [field: SerializeField] public float WheelBase { get; private set; }
    [field: SerializeField] public float TurnRadius { get; private set; }
    [field: SerializeField] public float RearTrack { get; private set; }

    // first value should correspond to idle RPM
    // last value should correspond to redline RPM
    [field: SerializeField] public AnimationCurve EngineCurve { get; private set; }
    // reverse at index 0
    [field: SerializeField] private float[] gearRatios;
    [field: SerializeField] private float finalDrive;

    public float[] EffectiveGearRatios { get; private set; }

    private void OnEnable()
    {
        ComputeFinalGearRatios();
    }
    private void ComputeFinalGearRatios()
    {
        if (gearRatios == null)
        {
            return;
        }
        EffectiveGearRatios = new float[gearRatios.Length];
        for (int i = 0; i < gearRatios.Length; i++)
        {
            EffectiveGearRatios[i] = gearRatios[i] * finalDrive;
        }
    }
}
