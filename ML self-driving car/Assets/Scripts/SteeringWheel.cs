using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private CarInput carInput;
    [SerializeField] private float maxSteeringWheelAngle;
    [SerializeField] private float steeringSmoothness;

    private void Update()
    {
        Animate();
    }

    private void Animate()
    {
        var desiredRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, -carInput.SteerInput * maxSteeringWheelAngle);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, steeringSmoothness * Time.deltaTime);
    }
}
