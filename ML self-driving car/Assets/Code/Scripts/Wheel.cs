using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private WheelCollider wheelCollider;
    [SerializeField] private Transform wheelTransform;
    private bool hasPower;

    public float SteeringAngle { get; set; }
    public float Torque { get; set; }
    public float BrakeTorque { get; set; }

    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelTransform = GetComponentInChildren<MeshRenderer>().transform;

        hasPower = false;
        SteeringAngle = 0;
    }

    void Update()
    {
        Animate();
    }

    void FixedUpdate()
    {
        Steer();
        if (hasPower) Accelerate();
        Brake();
    }

    private void Animate()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
    private void Steer()
    {
        wheelCollider.steerAngle = SteeringAngle;
    }
    private void Accelerate()
    {
        wheelCollider.motorTorque = Torque;
    }
    private void Brake()
    {
        wheelCollider.brakeTorque = BrakeTorque;
    }
    public void SetPower(bool _hasPower)
    {
        hasPower = _hasPower;
    }

    public float getRpm()
    {
        return wheelCollider.rpm;
    }

    public bool isGrounded()
    {
        return wheelCollider.isGrounded;
    }

    public float Radius => wheelCollider.radius;
}
