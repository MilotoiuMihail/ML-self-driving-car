using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInput : MonoBehaviour
{
    private string inputX = "Horizontal";
    private string inputY = "Vertical";
    public float SteerInput { get; private set; }
    public float ThrottleInput { get; private set; }
    public bool GearUp { get; private set; }
    public bool GearDown { get; private set; }
    public bool HandBrake { get; private set; }

    void Update()
    {
        SteerInput = Input.GetAxis(inputX);
        ThrottleInput = Input.GetAxis(inputY);
        GearUp = Input.GetKeyDown(KeyCode.E);
        GearDown = Input.GetKeyDown(KeyCode.Q);
        HandBrake = Input.GetKeyDown(KeyCode.Space);
    }
}
