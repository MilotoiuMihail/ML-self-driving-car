using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// CAR IS ASSUMED TO HAVE 4 WHEELS
// NOTED WITH FL - 0, FR - 1, RL - 2, RR - 3
public class Car : MonoBehaviour
{
    private enum DriveType
    {
        REAR,
        FRONT,
        FULL
    }
    private DriveType drive;
    [SerializeField]
    private Transform centreOfMass;
    [SerializeField] private Wheel[] wheels;
    private Rigidbody rb;
    private float motorTorque;
    private float maxSteerAngle;
    private float brakeForce;

    private float steer;
    private float throttle;

    void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.localPosition;
        InitializeCar();
    }

    void Update()
    {
        foreach (var wheel in wheels)
        {
            wheel.SteeringAngle = maxSteerAngle * steer;
            wheel.Torque = motorTorque * throttle;
        }
    }

    private void ApplyDriveType()
    {
        bool rear = false;
        bool front = false;
        switch (drive)
        {
            case DriveType.REAR:
                rear = true;
                break;
            case DriveType.FRONT:
                front = true;
                break;
            case DriveType.FULL:
                rear = true;
                front = true;
                break;
            default:
                break;
        }
        wheels[0].SetTorque(front);
        wheels[1].SetTorque(front);
        wheels[2].SetTorque(rear);
        wheels[3].SetTorque(rear);

        motorTorque = rear && front ? motorTorque / 4 : motorTorque / 2;
    }

    private void InitializeSteering()
    {
        wheels[0].setSteering(true);
        wheels[1].setSteering(true);
        wheels[2].setSteering(false);
        wheels[3].setSteering(false);
    }

    private void InitializeCar()
    {
        if (wheels.Length != 4)
            return; //EXCEPTION
        ApplyDriveType();
        InitializeSteering();
    }

}
