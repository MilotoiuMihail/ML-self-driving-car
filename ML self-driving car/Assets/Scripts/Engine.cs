using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine
{
    private AnimationCurve curve;

    public float IdleRpm { get; private set; }
    public float RedlineRpm { get; private set; }
    public float Rpm { get; private set; }
    private const float RpmNeutralIncreaseRate = 1500;
    private const float RpmNeutralDecreaseRate = 2000;
    private const float SmoothTime = 0.1f;
    private const float RpmMultiplier = 1000f;
    private const float RpmRatio = 1f / RpmMultiplier;

    public Engine(AnimationCurve engineCurve)
    {
        curve = engineCurve;
        IdleRpm = DetermineIdleRpm();
        RedlineRpm = DetermineRedlineRpm();
        Rpm = IdleRpm;
    }

    private float DetermineIdleRpm()
    {
        return curve.keys[0].time * RpmMultiplier;
    }

    private float DetermineRedlineRpm()
    {
        return curve.keys[curve.keys.Length - 1].time * RpmMultiplier;
    }
    public void ComputeRpmInGear(float wheelRpm, float gearRatio)
    {
        float desiredEngineRpm = IdleRpm + Mathf.Abs(wheelRpm) * gearRatio;
        float v = 0;
        // Rpm = Mathf.SmoothDamp(Rpm, desiredEngineRpm, ref v, SmoothTime);
        Rpm = Mathf.SmoothDamp(Rpm, desiredEngineRpm, ref v, Time.deltaTime);
    }
    public void ComputeNeutralRpm(float throttle)
    {
        Rpm = Mathf.Clamp(Rpm + ComputeRpmChangeRate(throttle), IdleRpm, RedlineRpm);
    }
    private float ComputeRpmChangeRate(float throttle)
    {
        if (throttle > 0)
        {
            return throttle * Time.deltaTime * RpmNeutralIncreaseRate;
        }
        return -Time.deltaTime * RpmNeutralDecreaseRate;
    }

    public float GetCurrentMaxTorque()
    {
        // divided to keep curve x values small
        // limits rpm and speed indirectly
        return Rpm < RedlineRpm ? curve.Evaluate(Rpm * RpmRatio) : 0;
    }
}
