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

    void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelTransform = GetComponentInChildren<MeshRenderer>().transform;
    }

    void LateUpdate()
    {
        Animate();
    }

    void FixedUpdate()
    {
        Steer();
        if (hasPower)
        {
            Accelerate();
        }
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
    public void SetPower(bool hasPower)
    {
        this.hasPower = hasPower;
    }

    public float Rpm => wheelCollider.rpm;
    public bool isGrounded()
    {
        return wheelCollider.isGrounded;
    }
}
