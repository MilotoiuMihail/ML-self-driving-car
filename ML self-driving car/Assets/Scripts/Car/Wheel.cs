using UnityEngine;

public class Wheel : MonoBehaviour
{
    private WheelCollider wheelCollider;
    private Transform wheelTransform;
    public bool HasPower { get; set; }
    public float SteeringAngle { get; set; }
    public float Torque { get; set; }
    public float BrakeTorque { get; set; }
    public float Rpm => wheelCollider.rpm;
    [SerializeField] private LayerMask trackLayer;

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
        if (HasPower)
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
    public bool IsOnTrack()
    {
        WheelHit hit;
        if (!wheelCollider.GetGroundHit(out hit))
        {
            return false;
        }
        return (trackLayer.value & (1 << hit.collider.gameObject.layer)) != 0;
    }
}
