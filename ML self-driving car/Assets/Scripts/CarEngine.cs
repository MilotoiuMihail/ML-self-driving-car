using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Car))]
public class CarEngine : MonoBehaviour
{
    private const float RpmMultiplier = 1000f;
    private const float RpmRatio = 1f / RpmMultiplier;
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
    }

    private void Start()
    {
        curve = car.Specs.EngineCurve;
        IdleRpm = DetermineIdleRpm();
        RedlineRpm = DetermineRedlineRpm();
        Rpm = IdleRpm;
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

    private void ComputeRpmInGear(float wheelRpm, float gearRatio)
    {
        float desiredEngineRpm = IdleRpm + Mathf.Abs(wheelRpm) * gearRatio;
        float v = 0;
        Rpm = Mathf.SmoothDamp(Rpm, desiredEngineRpm, ref v, Time.deltaTime);
    }

    public float GetCurrentMaxTorque()
    {
        return Rpm < RedlineRpm ? curve.Evaluate(Rpm * RpmRatio) : 0;
    }
}
