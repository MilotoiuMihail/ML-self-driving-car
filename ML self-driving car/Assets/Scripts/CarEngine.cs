using System.Collections;
using System.Collections.Generic;
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
    private CarInput carInput;
    private AnimationCurve curve;
    public float IdleRpm { get; private set; }
    public float RedlineRpm { get; private set; }
    public float Rpm { get; private set; }
    public float CurrentEngineTorque { get; private set; }

    private void Awake()
    {
        car = GetComponent<Car>();
        carInput = GetComponent<CarInput>();
        curve = car.Specs.EngineCurve;
        IdleRpm = DetermineIdleRpm();
        RedlineRpm = DetermineRedlineRpm();
        Rpm = IdleRpm;
    }

    private void Start()
    {
    }

    private void Update()
    {
        ComputeRpmInGear(car.GetWheelsRpm(), car.GetCurrentGearRatio());
        ComputeCurrentEngineTorque();
    }

    private void ComputeCurrentEngineTorque()
    {
        CurrentEngineTorque = carInput.ThrottleInput * GetCurrentMaxTorque();
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
        // float desiredEngineRpm = Mathf.Max(IdleRpm, Mathf.Abs(wheelRpm) * gearRatio);
        float desiredEngineRpm = Mathf.Abs(wheelRpm) * gearRatio;
        float v = 0;
        Rpm = Mathf.SmoothDamp(Rpm, desiredEngineRpm, ref v, Time.fixedDeltaTime);
    }

    private float GetCurrentMaxTorque()
    {
        return Rpm < RedlineRpm ? curve.Evaluate(Rpm * RpmRatio) * TransmissionEfficiency : 0;
    }
}
