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
    private enum GearBox
    {
        MANUAL,
        AUTOMATIC
    }
    private DriveType drive;
    private Driver driver;
    private GearBox gearBox;
    private CarInput input;

    private Wheel[] wheels;
    [SerializeField] private Transform steeringWheel;

    private float steer;
    private float leftSteerAngle;
    private float rightSteerAngle;
    [SerializeField] private float wheelBase;
    [SerializeField] private float turnRadius;
    [SerializeField] private float rearTrack;
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private float maxSteeringWheelAngle;

    [SerializeField] private float downforce;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float deceleration;

    private Rigidbody rb;
    [SerializeField] private Transform centreOfMass;

    private float throttle;
    private float v; //necessary for smoothDamp, but never used
    [SerializeField] private AnimationCurve engineCurve;
    private float wheelTorque => engineTorque * gearRatio[gear] * finalDrive;
    private float engineTorque => throttle * currentEngineTorque;
    private float currentEngineTorque => engineCurve.Evaluate(engineRpm);
    private float engineRpm;
    private float wheelRpm => wheelRotationRate * conversionFactor;
    private float wheelRotationRate => speed / wheels[0].Radius;
    private const float conversionFactor = 60 / 2 / Mathf.PI;
    private float speed => rb.velocity.magnitude;
    [SerializeField] private float[] gearRatio;
    private int gear;
    [SerializeField] private float finalDrive;
    private float minRpm = 1000;
    private float smoothTime = .01f;
    private float maxRpm = 3000;

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
        Shift();
    }

    private void Shift()
    {
        engineRpm = Mathf.Max(minRpm, Mathf.SmoothDamp(engineRpm, Mathf.Abs(wheelRpm) * gearRatio[gear] * finalDrive, ref v, smoothTime));
        if (engineRpm > maxRpm && gear < gearRatio.Length - 1)
        {
            gear += 1;
        }
        if (engineRpm < minRpm && gear > 0)
        {
            gear -= 1;
        }
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
        {

            if (throttle != 0)
            {
                wheel.Torque = wheelTorque;
                wheel.BrakeTorque = 0;
            }
            // else if (throttle < 0)
            // {
            //     wheel.Torque = 0;
            //     wheel.BrakeTorque = brakeTorque * Time.deltaTime;
            // }
            else
            {

                wheel.Torque = 0;
                wheel.BrakeTorque = deceleration * Time.deltaTime;
            }
        }
    }

    // private void CalculateSpeed()
    // {
    //     kph = rb.velocity.magnitude * 3.6f;
    // }

    // private void CalculateWheelsRpm()
    // {
    //     float sum = 0;
    //     for (int i = 0; i < wheels.Length; i++)
    //         sum += wheels[i].getRpm();
    //     wheelsRpm = sum / wheels.Length;

    //     reverse = wheelsRpm < 0;
    // }

    // private void calculateEnginePower()
    // {
    //     CalculateWheelsRpm();

    //     totalPower = enginePowerCurve.Evaluate(engineRpm) * gears[gear] * throttle;
    //     float velocity = 0f;
    //     engineRpm = Mathf.SmoothDamp(engineRpm, 1000 + Mathf.Abs(wheelsRpm) * gears[gear], ref velocity, smoothTime);
    // }

    // private void shiftGear()
    // {
    //     if (!isGrounded())
    //         return;
    //     if (gearBox == GearBox.MANUAL)
    //         if (engineRpm > maxRpm && gear < gears.Length - 1 && !reverse)
    //             gear += 1;
    //         else
    //         {
    //             if (Input.GetKeyDown(KeyCode.E))
    //                 gear += 1;
    //         }
    //     if (engineRpm < minRpm && gear > 0)
    //         gear -= 1;
    // }

    private bool isGrounded()
    {
        return wheels.All(x => x.isGrounded());
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

        //motorTorque = rear && front ? motorTorque / 4 : motorTorque / 2;
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
