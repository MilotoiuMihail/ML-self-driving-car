using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CarInput))]
public class Car : MonoBehaviour
{
    private const float MeterpsToKph = 3.6f;
    private const float MeterpsToMph = 2.23694f;
    [field: SerializeField] public CarSpecs Specs { get; private set; }
    [SerializeField] private bool hasManualGearBox;
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private float downforce;
    [SerializeField] private float brakeTorque;
    [SerializeField] private Transform centreOfMass;
    public Wheel[] Wheels { get; private set; }
    private Rigidbody rb;
    private CarEngine engine;
    private CarInput input;
    private float throttle;
    private int currentGear;
    private Gear[] gears;

    private void Awake()
    {
        Wheels = GetComponentsInChildren<Wheel>();
        engine = GetComponent<CarEngine>();
        input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentGear = 1;
        rb.centerOfMass = centreOfMass.localPosition;
        ApplyDriveType();
        InitializeGears();
    }

    void Update()
    {
        TakeInput();
        Movement();
        Shifting();
        ApplyDownforce();
        Debug.Log($"speed: {Mathf.Round(GetSpeedKph())}; RPM: {engine.Rpm}; gear: {gears[currentGear].Name}; throttle: {throttle}; torque: {engine.CurrentEngineTorque}");

    }

    private void InitializeGears()
    {
        gears = new Gear[Specs.EffectiveGearRatios.Length];
        gears[0] = new Gear(Specs.EffectiveGearRatios[0], 0, engine.IdleRpm, engine.RedlineRpm);
        for (int i = 1; i < Specs.EffectiveGearRatios.Length; i++)
        {
            gears[i] = new Gear(Specs.EffectiveGearRatios[i], i, 4500, 7000);
        }
    }

    public float GetCurrentGearRatio()
    {
        return gears[currentGear].Ratio;
    }

    private void TakeInput()
    {
        throttle = input.ThrottleInput;
    }

    private void Movement()
    {
        foreach (var wheel in Wheels)
        {
            wheel.Torque = GetTorque();
            wheel.BrakeTorque = GetBrakeTorque();
        }
    }

    private float GetTorque()
    {
        if (throttle > 0)
        {
            return currentGear != 0 ? engine.CurrentEngineTorque : 0;
        }
        return currentGear != 0 ? 0 : engine.CurrentEngineTorque;
    }

    private float GetBrakeTorque()
    {
        if (throttle > 0)
        {
            return currentGear != 0 ? 0 : brakeTorque;
        }
        return currentGear != 0 ? brakeTorque : 0;
    }

    public void SetLeftSteering(float desiredSteerAngle)
    {
        Wheels[0].SteeringAngle = Mathf.Lerp(Wheels[0].SteeringAngle, desiredSteerAngle, steeringSmoothness * Time.deltaTime);

    }

    public void SetRightSteering(float desireedSteerAngle)
    {
        Wheels[1].SteeringAngle = Mathf.Lerp(Wheels[1].SteeringAngle, desireedSteerAngle, steeringSmoothness * Time.deltaTime);

    }

    private void Shifting()
    {
        if (hasManualGearBox)
        {
            ManualShift();
        }
        else
        {
            AutomaticShift();
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

    private void ShiftUp()
    {
        currentGear = Mathf.Min(currentGear + 1, gears.Length - 1);
    }

    private void ShiftDown()
    {
        currentGear = Mathf.Max(currentGear - 1, 0);
    }

    private float GetSpeedKph()
    {
        return rb.velocity.magnitude * MeterpsToKph;
    }

    private float GetSpeedMph()
    {
        return rb.velocity.magnitude * MeterpsToMph;
    }

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.velocity.magnitude);
    }

    private void ApplyDriveType()
    {
        bool rear = false;
        bool front = false;
        switch (Specs.Drive)
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
        Wheels[0].SetPower(front);
        Wheels[1].SetPower(front);
        Wheels[2].SetPower(rear);
        Wheels[3].SetPower(rear);
    }

    public float GetWheelsRpm()
    {
        switch (Specs.Drive)
        {
            case DriveType.REAR:
                return (Wheels[2].Rpm + Wheels[3].Rpm) * .5f;
            case DriveType.FRONT:
                return (Wheels[0].Rpm + Wheels[1].Rpm) * .5f;
            case DriveType.FULL:
                return Wheels.Average(wheel => wheel.Rpm);
            default:
                return 0;
        }
    }
}
