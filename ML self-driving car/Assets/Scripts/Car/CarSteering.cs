using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Car))]
public class CarSteering : MonoBehaviour
{
    private Car car;
    private CarInput carInput;
    private float steer;

    private void Awake()
    {
        car = GetComponent<Car>();
        carInput = GetComponent<CarInput>();
    }

    private void Update()
    {
        steer = carInput.SteerInput;
        ManageSteering();
    }

    private void ManageSteering()
    {
        if (CarManager.Instance.BlockInput)
        {
            return;
        }
        car.SetLeftSteering(ComputeLeftSteerAngle());
        car.SetRightSteering(ComputeRightSteerAngle());
    }

    private float ComputeLeftSteerAngle()
    {
        float desiredLeftSteerAngle = 0;
        if (steer != 0)
        {
            desiredLeftSteerAngle = steer > 0 ? GetLeftSteerAngle() : GetRightSteerAngle();
        }
        return desiredLeftSteerAngle;
    }

    private float ComputeRightSteerAngle()
    {
        float desiredRightSteerAngle = 0;
        if (steer != 0)
        {
            desiredRightSteerAngle = steer > 0 ? GetRightSteerAngle() : GetLeftSteerAngle();
        }
        return desiredRightSteerAngle;
    }

    private float GetLeftSteerAngle()
    {
        return Mathf.Rad2Deg * Mathf.Atan(car.Specs.WheelBase / (car.Specs.TurnRadius + car.Specs.RearTrack * .5f)) * steer;
    }

    private float GetRightSteerAngle()
    {
        return Mathf.Rad2Deg * Mathf.Atan(car.Specs.WheelBase / (car.Specs.TurnRadius - car.Specs.RearTrack * .5f)) * steer;
    }
}
