using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInput : MonoBehaviour
{
    [SerializeField] private bool isHumanDriver;
    private string inputX = "Horizontal";
    private string inputY = "Vertical";
    public float SteerInput { get; set; }
    public float ThrottleInput { get; set; }
    public bool GearUp { get; private set; }
    public bool GearDown { get; private set; }
    public bool HandBrake { get; private set; }
    public bool Reverse { get; private set; }

    void Update()
    {
        // SteerInput = isHumanDriver ? Input.GetAxis(inputX) : 0;
        // ThrottleInput = isHumanDriver ? Input.GetAxis(inputY) : 0;
        GearUp = Input.GetKeyDown(KeyCode.E);
        GearDown = Input.GetKeyDown(KeyCode.Q);
        HandBrake = Input.GetKeyDown(KeyCode.Space);
        Reverse = Input.GetKeyDown(KeyCode.R);
    }
}
