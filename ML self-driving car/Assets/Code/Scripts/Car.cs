using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Transform centreOfMass;
    private Wheel[] wheels;
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
    }

    void Update()
    {
        foreach (var wheel in wheels)
        {
            wheel.SteeringAngle = maxSteerAngle * steer;
            wheel.Torque = motorTorque / wheels.Length * throttle;
        }
    }

}
