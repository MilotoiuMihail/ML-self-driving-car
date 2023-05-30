using UnityEngine;
using System.Linq;

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
    private enum GearBox
    {
        MANUAL,
        AUTOMATIC
    }
    private DriverType driver;
    [SerializeField] private GearBox gearBox;
    private CarInput input;

    private Wheel[] wheels;
    [SerializeField] private Transform steeringWheel;

    private float steer;
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private float maxSteeringWheelAngle;

    [SerializeField] private float downforce;
    [SerializeField] private float brakeTorque;

    private Rigidbody rb;
    [SerializeField] private Transform centreOfMass;

    private float throttle;
    private float currentEngineTorque;
    private int currentGear;

    private Gear[] gears;
    private Engine engine;
    void Start()
    {
        currentGear = 1;
        wheels = GetComponentsInChildren<Wheel>();
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.localPosition;
        engine = new Engine(specs.EngineCurve);
        ApplyDriveType();
        InitializeGears();
    }

    void Update()
    {
        TakeInputFromDriver();
        Steering();
        engine.ComputeRpmInGear(GetWheelsRpm(), gears[currentGear].Ratio);
        currentEngineTorque = throttle * engine.GetCurrentMaxTorque();
        Movement();
        Shifting();
        AnimateSteeringWheel();
        ApplyDownforce();
        Debug.Log($"speed: {Mathf.Round(GetSpeedKph())}; RPM: {engine.Rpm}; gear: {gears[currentGear].Name}; throttle: {throttle}; torque: {currentEngineTorque}");

    }
    private void InitializeGears()
    {
        gears = new Gear[specs.EffectiveGearRatios.Length];
        gears[0] = new Gear(specs.EffectiveGearRatios[0], 0, 1000, 8500);
        for (int i = 1; i < specs.EffectiveGearRatios.Length; i++)
        {
            gears[i] = new Gear(specs.EffectiveGearRatios[i], i, 4500, 7000);
        }
    }
    private void ToggleReverse()
    {
        if (currentGear == 0)
        {
            ShiftUp();
        }
        else if (currentGear == 1)
        {
            ShiftDown();
        }
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
            wheel.Torque = GetTorque();
            wheel.BrakeTorque = GetBrakeTorque();
        }
    }
    private float GetTorque()
    {
        if (throttle > 0)
        {
            return currentGear != 0 ? currentEngineTorque : 0;
        }
        return currentGear != 0 ? 0 : currentEngineTorque;
    }
    private float GetBrakeTorque()
    {
        if (throttle > 0)
        {
            return currentGear != 0 ? 0 : brakeTorque;
        }
        return currentGear != 0 ? brakeTorque : 0;
    }
    private void Shifting()
    {
        switch (gearBox)
        {
            case GearBox.MANUAL:
                ManualShift();
                break;
            case GearBox.AUTOMATIC:
                AutomaticShift();
                break;
        }
    }
    private void ManualShift()
    {
        if (input.GearUp)
        {
            ShiftUp();
        }
        if (input.GearDown)
        {
            ShiftDown();
        }
    }
    private void AutomaticShift()
    {
        if (input.Reverse)
        {
            ToggleReverse();
        }
        if (currentGear == 0 || GetSpeedKph() <= 5)
        {
            return;
        }
        if (engine.Rpm > gears[currentGear].MaxRpm && currentGear < gears.Length - 1)
        {
            ShiftUp();
        }
        if (engine.Rpm < gears[currentGear].MinRpm && currentGear > 1)
        {
            ShiftDown();
        }
    }
    private void ShiftUp()
    {
        currentGear = Mathf.Min(currentGear + 1, gears.Length - 1);
    }
    private void ShiftDown()
    {
        currentGear = Mathf.Max(currentGear - 1, 0);
    }
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
    private float GetSpeedMph()
    {
        return rb.velocity.magnitude * 2.23694f;
    }
    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }
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

    private void TakeInputFromDriver()
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
