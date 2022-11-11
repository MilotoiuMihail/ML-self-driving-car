using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// CAR IS ASSUMED TO HAVE 4 WHEELS
// NOTED WITH FL - 0, FR - 1, RL - 2, RR - 3
// STEERING IS DONE ONLY WITH FRONT WHEELS
public class Car : MonoBehaviour
{
    private enum Driver
    {
        PLAYER,
        AI
    }
    private enum DriveType
    {
        REAR,
        FRONT,
        FULL
    }
    private DriveType drive;
    private Driver driver;
    private CarInput input;
    [SerializeField] private Transform centreOfMass;
    [SerializeField] private Transform steeringWheel;
    private Wheel[] wheels;
    private Rigidbody rb;
    [SerializeField] private float motorTorque;
    [SerializeField] private float wheelBase;
    [SerializeField] private float turnRadius;
    [SerializeField] private float rearTrack;
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private float maxSteeringWheelAngle;
    [SerializeField] private float downforce;
    private float KPH;
    private float steer;
    private float throttle;
    private float leftSteerAngle;
    private float rightSteerAngle;

    void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.localPosition;
        InitializeCar();
    }

    void Update()
    {
        ManageDriver();
        Steering();
        ApplyDownforce();
        AnimateSteeringWheel();
        Movement();
    }

    private void Steering()
    {
        if (steer > 0)
        {
            leftSteerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack * .5f)) * steer;
            rightSteerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack * .5f)) * steer;
        }
        else if (steer < 0)
        {
            leftSteerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack * .5f)) * steer;
            rightSteerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack * .5f)) * steer;
        }
        else
        {
            leftSteerAngle = 0;
            rightSteerAngle = 0;
        }
        wheels[0].SteeringAngle = Mathf.Lerp(wheels[0].SteeringAngle, leftSteerAngle, steeringSmoothness * Time.deltaTime);
        wheels[1].SteeringAngle = Mathf.Lerp(wheels[0].SteeringAngle, rightSteerAngle, steeringSmoothness * Time.deltaTime);
    }

    private void AnimateSteeringWheel()
    {
        var desiredRotation = Quaternion.Euler(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, -steer * maxSteeringWheelAngle);
        steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, desiredRotation, steeringSmoothness * Time.deltaTime);
    }

    private void Movement()
    {
        foreach (var wheel in wheels)
            wheel.Torque = motorTorque * throttle;
        KPH = rb.velocity.magnitude * 3.6f;
    }

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
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
        wheels[0].SetPower(front);
        wheels[1].SetPower(front);
        wheels[2].SetPower(rear);
        wheels[3].SetPower(rear);

        motorTorque = rear && front ? motorTorque / 4 : motorTorque / 2;
    }

    private void InitializeCar()
    {
        if (wheels.Length != 4)
            return; //EXCEPTION
        ApplyDriveType();
    }

    private void ManageDriver()
    {
        if (!input) return; //momentan
        switch (driver)
        {
            case Driver.PLAYER:
                steer = input.SteerInput;
                throttle = input.ThrottleInput;
                break;
            case Driver.AI:
                break;
            default:
                break;
        }
    }

}
