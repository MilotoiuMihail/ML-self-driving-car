using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private float maxSteeringWheelAngle;
    [SerializeField] private float steeringSmoothness;
    [SerializeField] private CarInput carInput;
    private void AnimateSteeringWheel()
    {
        var desiredRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, -carInput.SteerInput * maxSteeringWheelAngle);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, steeringSmoothness * Time.deltaTime);
    }
}
