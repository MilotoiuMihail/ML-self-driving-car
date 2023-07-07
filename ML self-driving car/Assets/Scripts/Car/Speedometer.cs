using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private const float minAngle = 60f;
    private const float maxAngle = 300f;
    private const float MeterpsToKph = 3.6f;
    private const float MeterpsToMph = 2.23694f;
    private const string kphMeasure = "KPH";
    private const string mphMeasure = "MPH";
    [SerializeField] private Car car;
    [SerializeField] private Transform needle;
    private float minRpm;
    private float maxRpm;
    [SerializeField] private TMP_Text gearDisplay;
    [SerializeField] private TMP_Text speedDisplay;
    [SerializeField] private TMP_Text measureDisplay;
    private bool isMph;
    private float speed
    {
        get
        {
            if (car == null)
                return 0;
            return isMph ? car.GetSpeed() * MeterpsToMph : car.GetSpeed() * MeterpsToKph;
        }
    }
    private float currentSpeed;
    private void OnEnable()
    {
        car.GearShift += DisplayGear;
    }
    private void OnDisable()
    {
        car.GearShift -= DisplayGear;
    }
    private void Start()
    {
        DisplayGear(1);
        speedDisplay.text = "0";
        measureDisplay.text = kphMeasure;
        minRpm = car.Engine.IdleRpm;
        maxRpm = Mathf.CeilToInt(car.Engine.RedlineRpm * .001f) * 1000;
    }

    private void Update()
    {
        RotateNeedle();
        DisplaySpeed();
    }
    private void RotateNeedle()
    {
        needle.localEulerAngles = new Vector3(0, ComputeAngle(), 0);
    }
    private float NormalizeRpm()
    {
        return (car.Engine.Rpm - minRpm) / (maxRpm - minRpm);
    }

    private float ComputeAngle()
    {
        return Mathf.Lerp(minAngle, maxAngle, NormalizeRpm());
    }
    private void DisplayGear(int gear)
    {
        gearDisplay.text = gear > 0 ? gear.ToString() : "R";
    }
    private float SmoothenSpeed()
    {
        float velocity = 0f;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref velocity, Time.deltaTime * 5f);
        return currentSpeed;
    }
    private void DisplaySpeed()
    {
        speedDisplay.text = Mathf.RoundToInt(SmoothenSpeed()).ToString();
    }
    private void ChangeMeasure()
    {
        isMph = !isMph;
        measureDisplay.text = isMph ? mphMeasure : kphMeasure;
    }
    private void OnMouseUp()
    {
        ChangeMeasure();
    }
}
