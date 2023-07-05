using UnityEngine;


[CreateAssetMenu(fileName = "CarSpecifications", menuName = "Custom/Car Specs")]
public class CarSpecs : ScriptableObject
{
    [field: SerializeField] public DriveType Drive { get; private set; }
    [field: SerializeField] public float WheelBase { get; private set; }
    [field: SerializeField] public float TurnRadius { get; private set; }
    [field: SerializeField] public float RearTrack { get; private set; }
    [field: SerializeField] public AnimationCurve EngineCurve { get; private set; }
    [SerializeField] private float[] gearRatios;
    [SerializeField] private float finalDrive;
    public float[] EffectiveGearRatios { get; private set; }

    private void OnEnable()
    {
        ComputeEffectiveGearRatios();
    }

    private void ComputeEffectiveGearRatios()
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
