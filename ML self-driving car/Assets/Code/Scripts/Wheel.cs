using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    private WheelCollider wheelCollider;
    private Transform wheelTransform;
    private bool isSteering;
    private bool hasTorque;

    private float steeringAngle;
    private float torque;

    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelTransform = GetComponentInChildren<Transform>();
    }

    void Update()
    {
        Animate();
    }

    void FixedUpdate()
    {
        if (isSteering) Steer();
        if (hasTorque) ApplyTorque();
    }

    private void Animate()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
    private void Steer()
    {
        wheelCollider.steerAngle = steeringAngle;
    }
    private void ApplyTorque()
    {
        wheelCollider.motorTorque = torque;
    }

}
