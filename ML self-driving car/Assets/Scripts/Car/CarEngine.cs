using UnityEngine;

[RequireComponent(typeof(Car))]
public class CarEngine : MonoBehaviour
{
    private const float RpmMultiplier = 1000f;
    private const float RpmRatio = 1f / RpmMultiplier;
    private const float LowerRpmRatio = .6f;
    private const float UpperRpmRatio = .9f;
    private const float TransmissionEfficiency = .9f;
    private Car car;
    private AnimationCurve curve;
    public float IdleRpm { get; private set; }
    public float RedlineRpm { get; private set; }
    public float Rpm { get; private set; }
    public float CurrentEngineTorque { get; private set; }

    private void Awake()
    {
        car = GetComponent<Car>();
        car.Input = GetComponent<CarInput>();
        curve = car.Specs.EngineCurve;
        IdleRpm = DetermineIdleRpm();
        RedlineRpm = DetermineRedlineRpm();
        Rpm = IdleRpm;
    }

    private void Update()
    {
        if (car.Input.IsBlocked)
        {
            return;
        }
        ComputeRpmInGear(car.GetWheelsRpm(), car.GetCurrentGearRatio());
        ComputeCurrentEngineTorque();
    }

    private void ComputeCurrentEngineTorque()
    {
        CurrentEngineTorque = car.Input.ThrottleInput * GetCurrentMaxTorque();
    }

    private float DetermineIdleRpm()
    {
        return curve.keys[0].time * RpmMultiplier;
    }

    private float DetermineRedlineRpm()
    {
        return curve.keys[curve.keys.Length - 1].time * RpmMultiplier;
    }
    public float GetLowerRpm()
    {
        return RedlineRpm * LowerRpmRatio;
    }
    public float GetUpperRpm()
    {
        return RedlineRpm * UpperRpmRatio;
    }
    private void ComputeRpmInGear(float wheelRpm, float gearRatio)
    {
        float desiredEngineRpm = Mathf.Abs(wheelRpm) * gearRatio;
        float v = 0;
        Rpm = Mathf.SmoothDamp(Rpm, desiredEngineRpm, ref v, Time.fixedDeltaTime);
    }

    private float GetCurrentMaxTorque()
    {
        return Rpm < RedlineRpm ? curve.Evaluate(Rpm * RpmRatio) * TransmissionEfficiency : 0;
    }
    public void StopEngine()
    {
        Rpm = IdleRpm;
        CurrentEngineTorque = 0;
    }
}
