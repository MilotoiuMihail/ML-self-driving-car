using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// CAR IS ASSUMED TO HAVE 4 WHEELS
// NOTED WITH FL - 0, FR - 1, RL - 2, RR - 3
// STEERING IS DONE ONLY WITH FRONT WHEELS
public class Car : MonoBehaviour
{
    private enum DriveType
    {
        REAR,
        FRONT,
        FULL
    }
    private DriveType drive;
    [SerializeField] private Transform centreOfMass;
    [SerializeField] private Wheel[] wheels;
    private Rigidbody rb;
    private float motorTorque;
    private float maxSteerAngle;
    private float brakeForce;
    private float wheelBase;
    private float turnRadius;
    private float rearTrack;
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
        Steer();
        ApplyTorque();
    }

    private void Steer()
    {

        if (steer > 0)
        {
            wheels[0].SteeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack * .5f)) * steer;
            wheels[1].SteeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack * .5f)) * steer;
        }
        else if (steer < 0)
        {
            wheels[0].SteeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack * .5f)) * steer;
            wheels[1].SteeringAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack * .5f)) * steer;
        }
        else
        {
            wheels[0].SteeringAngle = 0;
            wheels[1].SteeringAngle = 0;
        }
    }

    private void ApplyTorque()
    {
        foreach (var wheel in wheels)
            wheel.Torque = motorTorque * throttle;
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

    private void InitializeCar()
    {
        if (wheels.Length != 4)
            return; //EXCEPTION
        ApplyDriveType();
    }

}
