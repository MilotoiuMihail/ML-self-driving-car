using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class Car : MonoBehaviour
{
    [field: SerializeField] public CarSpecs Specs { get; private set; }
    [field: SerializeField] public bool HasManualGearBox { get; set; }
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private float downforce;
    [SerializeField] private float brakeTorque;
    [SerializeField] private Transform centreOfMass;
    public Wheel[] Wheels { get; private set; }
    private Rigidbody rb;
    public CarEngine Engine { get; private set; }
    public CarInput Input { get; set; }
    private float throttle;
    private int currentGear;
    private Gear[] gears;
    private bool isStopped;
    public event Action<int> GearShift;
    private Vector3 velocityOnSleep;
    private Vector3 angularVelocityOnSleep;
    private Coroutine fullStopCoroutine;

    private void OnEnable()
    {
        CarManager.Instance.CarInputBlocked += FullStop;
        CarManager.Instance.CarInputUnblocked += WakeUp;
    }
    private void OnDisable()
    {
        CarManager.Instance.CarInputBlocked -= FullStop;
        CarManager.Instance.CarInputUnblocked -= WakeUp;
    }
    private void Awake()
    {
        Wheels = GetComponentsInChildren<Wheel>();
        ApplyDriveType();
        Engine = GetComponent<CarEngine>();
        Input = GetComponent<CarInput>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.centerOfMass = centreOfMass.localPosition;
        InitializeGears();
    }
    void Update()
    {
        TakeInput();
        Movement();
        Shifting();
        ApplyDownforce();
    }

    public void DebugInfo()
    {
        Debug.Log($"speed: {Mathf.Round(GetSpeed())}; RPM: {Engine.Rpm}; gear: {gears[currentGear].Name}; throttle: {throttle}; torque: {Engine.CurrentEngineTorque}; steering: {Input.SteerInput}; steer angle: {Wheels[0].SteeringAngle}");
    }
    private void WakeUp()
    {
        if (CheckpointManager.Instance.HasFinishedRace(transform))
        {
            return;
        }
        rb.velocity = velocityOnSleep;
        rb.angularVelocity = angularVelocityOnSleep;
        velocityOnSleep = Vector3.zero;
        angularVelocityOnSleep = Vector3.zero;
        Input.UnblockInput();
    }
    public void FullStop()
    {
        Stop(true);
    }
    public void IdleStop()
    {
        Stop(false);
    }
    private void Stop(bool doFullStop)
    {
        if (fullStopCoroutine != null)
        {
            StopCoroutine(fullStopCoroutine);
        }
        if (doFullStop)
        {
            Input.BlockInput();
        }
        velocityOnSleep = doFullStop ? rb.velocity : Vector3.zero;
        angularVelocityOnSleep = doFullStop ? rb.angularVelocity : Vector3.zero;
        fullStopCoroutine = StartCoroutine(StopCarCoroutine());
    }
    private IEnumerator StopCarCoroutine()
    {
        do
        {
            foreach (Wheel wheel in Wheels)
            {
                wheel.SteeringAngle = 0;
                wheel.Torque = 0;
                wheel.BrakeTorque = Mathf.Infinity;
            }
            Engine.StopEngine();
            currentGear = 1;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            yield return null;
        } while (GetWheelsRpm() < -1f || GetWheelsRpm() > 1f);
        OnGearShift();
    }
    private void InitializeGears()
    {
        gears = new Gear[Specs.EffectiveGearRatios.Length];
        gears[0] = new Gear(Specs.EffectiveGearRatios[0], 0, Engine.IdleRpm, Engine.RedlineRpm);
        for (int i = 1; i < Specs.EffectiveGearRatios.Length; i++)
        {
            gears[i] = new Gear(Specs.EffectiveGearRatios[i], i, Engine.GetLowerRpm(), Engine.GetUpperRpm());
        }
    }

    public float GetCurrentGearRatio()
    {
        return gears[currentGear].Ratio;
    }

    public int GetCurrentGearIndex()
    {
        return currentGear;
    }
    private void TakeInput()
    {
        throttle = Input.ThrottleInput;
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
            return IsInReverse() ? 0 : Engine.CurrentEngineTorque;
        }
        return IsInReverse() ? Engine.CurrentEngineTorque : 0;
    }

    private float GetBrakeTorque()
    {
        if (throttle > 0)
        {
            return IsInReverse() ? brakeTorque * throttle : 0;
        }
        return IsInReverse() ? 0 : brakeTorque * -throttle;
    }

    public void SetLeftSteering(float desiredSteerAngle)
    {
        Wheels[0].SteeringAngle = Mathf.Lerp(Wheels[0].SteeringAngle, desiredSteerAngle, steeringSmoothness * Time.deltaTime);

    }

    public void SetRightSteering(float desireedSteerAngle)
    {
        Wheels[1].SteeringAngle = Mathf.Lerp(Wheels[1].SteeringAngle, desireedSteerAngle, steeringSmoothness * Time.deltaTime);

    }

    private bool IsInReverse()
    {
        return currentGear == 0;
    }

    private void Shifting()
    {
        if (HasManualGearBox)
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
        if (Input.GearUp)
        {
            ShiftUp();
        }
        if (Input.GearDown)
        {
            ShiftDown();
        }
    }

    private void AutomaticShift()
    {
        if (Input.Reverse)
        {
            ToggleReverse();
        }
        if (IsInReverse())
        {
            return;
        }
        if (Engine.Rpm > gears[currentGear].MaxRpm && currentGear < gears.Length - 1)
        {
            ShiftUp();
        }
        if (Engine.Rpm < gears[currentGear].MinRpm && currentGear > 1)
        {
            ShiftDown();
        }
    }

    private void ToggleReverse()
    {
        if (IsInReverse())
        {
            ShiftUp();
        }
        else if (currentGear == 1)
        {
            ShiftDown();
        }
    }
    private void OnGearShift()
    {
        GearShift?.Invoke(currentGear);
    }
    private void ShiftUp()
    {
        currentGear = Mathf.Min(currentGear + 1, gears.Length - 1);
        OnGearShift();
    }

    private void ShiftDown()
    {
        currentGear = Mathf.Max(currentGear - 1, 0);
        OnGearShift();
    }

    public float GetSpeed()
    {
        return rb.velocity.magnitude;
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
        Wheels[0].HasPower = front;
        Wheels[1].HasPower = front;
        Wheels[2].HasPower = rear;
        Wheels[3].HasPower = rear;
    }

    public float GetWheelsRpm()
    {
        return Wheels.Where(wheel => wheel.HasPower).Average(wheel => wheel.Rpm);
    }
    public bool IsOnTrack()
    {
        return Wheels.Where(wheel => wheel.IsOnTrack()).Count() > 1;
    }
}
