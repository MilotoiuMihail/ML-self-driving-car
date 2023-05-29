using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// CAR IS ASSUMED TO HAVE 4 WHEELS
// NOTED WITH FL - 0, FR - 1, RL - 2, RR - 3
// STEERING IS DONE ONLY WITH FRONT WHEELS
public class Car : MonoBehaviour
{
    private enum DriverType
    {
        PLAYER,
        AI
    }
    [SerializeField] private CarSpecs specs;
    // private enum GearBox
    // {
    //     MANUAL,
    //     AUTOMATIC
    // }
    private DriverType driver;
    // [SerializeField] private GearBox gearBox;
    private CarInput input;

    private Wheel[] wheels;
    [SerializeField] private Transform steeringWheel;

    private float steer;
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private float maxSteeringWheelAngle;

    [SerializeField] private float downforce;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float deceleration;

    private Rigidbody rb;
    [SerializeField] private Transform centreOfMass;

    private float throttle;
    private float currentEngineTorque;
    private int currentGear;
    // [SerializeField] private float idleTreshold;

    private Gear[] gears;
    private Engine engine;
    // private float direction;
    void Start()
    {
        currentGear = 1;
        wheels = GetComponentsInChildren<Wheel>();
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.localPosition;
        engine = new Engine(specs.EngineCurve);
        InitializeCar();

        // gears = new Gear[specs.FinalGearRatios.Length + 1];
        gears = new Gear[specs.EffectiveGearRatios.Length];
        gears[0] = new Gear(specs.EffectiveGearRatios[0], Gear.ConvertGearIndexToName(0), 1000, 8500);
        // gears[1] = new Gear(0, Gear.ConvertGearIndexToName(1), 1000, 8500);
        for (int i = 1; i < specs.EffectiveGearRatios.Length; i++)
        {
            // gears[i + 1] = new Gear(specs.FinalGearRatios[i], Gear.ConvertGearIndexToName(i + 1), 4500, 7000);
            gears[i] = new Gear(specs.EffectiveGearRatios[i], Gear.ConvertGearIndexToName(i), 4500, 7000);
        }
    }

    void Update()
    {
        ManageDriver();
        Steering();
        AnimateSteeringWheel();
        ApplyDownforce();
        ComputeEngineTorque();
        Movement();
        AutomaticShift();
        // Shifting();
        // direction = Vector3.Dot(rb.velocity, transform.forward);
        // Debug.Log(rb.velocity.magnitude);
        if (input.HandBrake)
        {
            ToggleReverse();
        }
        Debug.Log($"speed: {Mathf.Round(GetSpeedKph())}; RPM: {engine.Rpm}; gear: {gears[currentGear].Name}; throttle: {throttle}; torque: {currentEngineTorque}");
        // Debug.Log(wheels[0].getRpm());

    }

    private void ToggleReverse()
    {
        if (currentGear == 0)
        {
            currentGear = 1;
        }
        else if (currentGear == 1)
        {
            currentGear = 0;
        }
    }

    // private void OnDrawGizmos()
    // {
    //     if (!rb)
    //         return;
    //     // Set the color of the gizmo
    //     Gizmos.color = Color.green;

    //     // Get the start position of the gizmo line
    //     Vector3 startPosition = transform.position;

    //     // Calculate the end position of the gizmo line based on the transform's forward direction
    //     Vector3 endPosition = startPosition + transform.forward * 5;

    //     // Draw the gizmo line
    //     Gizmos.DrawLine(startPosition, endPosition);
    // }
    // by computing the engine torque we also compute the rpm
    private void ComputeEngineTorque()
    {
        // if (currentGear == 1 && gearBox == GearBox.MANUAL)
        // {
        //     engine.ComputeNeutralRpm(throttle);
        //     currentEngineTorque = 0;
        //     return;
        // }
        // engineRpm = 1000 + Mathf.Abs(GetWheelsRpm()) * gears[currentGear].Ratio;
        engine.ComputeRpmInGear(GetWheelsRpm(), gears[currentGear].Ratio);
        if (throttle >= 0)
        {
        }
        // currentEngineTorque = throttle * engine.GetCurrentMaxTorque();
        // currentEngineTorque = throttle * (engineRpm < 8500 ? specs.EngineCurve.Evaluate(engineRpm / 1000f) : 0);
    }

    private float GetWheelsRpm()
    {
        switch (specs.Drive)
        {
            case DriveType.REAR:
                return (wheels[2].Rpm + wheels[3].Rpm) * .5f;
            case DriveType.FRONT:
                return (wheels[0].Rpm + wheels[1].Rpm) * .5f;
            case DriveType.FULL:
                return wheels.Average(wheel => wheel.Rpm);
            default:
                return 0;
        }
    }

    private void Movement()
    {
        foreach (var wheel in wheels)
        {
            // wheel.Torque = currentEngineTorque;
            // wheel.BrakeTorque = input.HandBrake ? brakeTorque : (throttle != 0 ? 0 : deceleration);
            // wheel.BrakeTorque = throttle < 0 ? -throttle * brakeTorque : 0;

            if (currentGear != 0)
            {
                wheel.Torque = (throttle > 0) ? currentEngineTorque : 0;
                wheel.BrakeTorque = (throttle > 0) ? 0 : -throttle * brakeTorque;
            }
            else
            {
                wheel.Torque = (throttle > 0) ? 0 : currentEngineTorque;
                wheel.BrakeTorque = (throttle > 0) ? throttle * brakeTorque : 0;
            }
        }
    }
    // private void Shifting()
    // {
    //     switch (gearBox)
    //     {
    //         case GearBox.MANUAL:
    //             ManualShift();
    //             return;
    //         case GearBox.AUTOMATIC:
    //             AutomaticShift();
    //             return;
    //     }
    // }
    // private void ManualShift()
    // {
    //     if (input.GearUp)
    //     {
    //         ShiftUp();
    //     }
    //     if (input.GearDown)
    //     {
    //         ShiftDown();
    //     }
    // }
    private void ShiftUp()
    {
        currentGear = Mathf.Min(currentGear + 1, gears.Length - 1);
    }
    private void ShiftDown()
    {
        currentGear = Mathf.Max(currentGear - 1, 0);
    }
    private void AutomaticShift()
    {
        // if (IsIdle())
        // {
        //     currentGear = 1;
        //     return;
        // }
        // if (IsReversed())
        // {
        //     currentGear = 0;
        //     return;
        // }
        // if (currentGear == 1 && throttle > 0)
        // {
        //     ShiftUp();
        // }
        if (currentGear == 0 || GetSpeedKph() <= 5)
        {
            return;
        }
        if (engine.Rpm > gears[currentGear].MaxRpm && currentGear < gears.Length - 1)
        {
            // StartCoroutine(ShiftUpWithDelay(0.5f));
            ShiftUp();
        }
        // if (engine.Rpm < gears[currentGear].MinRpm && currentGear > 2)
        if (engine.Rpm < gears[currentGear].MinRpm && currentGear > 1)
        {
            ShiftDown();
        }
    }
    // private IEnumerator ShiftUpWithDelay(float shiftDelay)
    // {
    //     Debug.Log("Start");
    //     yield return new WaitForSeconds(shiftDelay);
    //     ShiftUp();
    //     Debug.Log("Finish");
    // }
    // used for automatic gearbox. Checks if it is needed to shift in reverse
    // private bool IsReversed()
    // {
    //     // Check if the throttle input is not positive and the car is moving in the opposite direction of its forward vector
    //     return Vector3.Dot(rb.velocity, transform.forward) < -0.1f;
    // }
    // // used for automatic gearbox. Check if it is needed to shift in neutral
    // private bool IsIdle()
    // {
    //     return rb.velocity.magnitude <= idleTreshold && throttle == 0;
    // }
    private void Steering()
    {
        float leftSteerAngle = 0;
        float rightSteerAngle = 0;
        if (steer != 0)
        {
            leftSteerAngle = steer > 0 ? GetLeftSteerAngle() : GetRightSteerAngle();
            rightSteerAngle = steer > 0 ? GetRightSteerAngle() : GetLeftSteerAngle();
        }
        wheels[0].SteeringAngle = Mathf.Lerp(wheels[0].SteeringAngle, leftSteerAngle, steeringSmoothness * Time.deltaTime);
        wheels[1].SteeringAngle = Mathf.Lerp(wheels[1].SteeringAngle, rightSteerAngle, steeringSmoothness * Time.deltaTime);
    }

    private float GetLeftSteerAngle()
    {
        return Mathf.Rad2Deg * Mathf.Atan(specs.WheelBase / (specs.TurnRadius + specs.RearTrack * .5f)) * steer;
    }

    private float GetRightSteerAngle()
    {
        return Mathf.Rad2Deg * Mathf.Atan(specs.WheelBase / (specs.TurnRadius - specs.RearTrack * .5f)) * steer;
    }

    private void AnimateSteeringWheel()
    {
        var desiredRotation = Quaternion.Euler(steeringWheel.localEulerAngles.x, steeringWheel.localEulerAngles.y, -steer * maxSteeringWheelAngle);
        steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, desiredRotation, steeringSmoothness * Time.deltaTime);
    }

    private float GetSpeedKph()
    {
        return rb.velocity.magnitude * 3.6f;
    }
    // private float GetSpeedMph()
    // {
    //     return rb.velocity.magnitude * 2.23694f;
    // }

    // private bool isGrounded()
    // {
    //     return wheels.All(x => x.isGrounded());
    // }
    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }

    // ?
    private void ApplyDriveType()
    {
        bool rear = false;
        bool front = false;
        switch (specs.Drive)
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
    }

    // ?
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
            case DriverType.PLAYER:
                steer = input.SteerInput;
                throttle = input.ThrottleInput;
                break;
            case DriverType.AI:
                break;
            default:
                break;
        }
    }

}
