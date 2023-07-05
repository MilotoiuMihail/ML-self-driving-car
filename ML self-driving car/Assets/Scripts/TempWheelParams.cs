using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TempWheelParams : MonoBehaviour
{
    private float forwardStiff = 1;
    private float sidewaysStiff = 1;
    [SerializeField] private TMP_Text forwardStiffText;
    [SerializeField] private TMP_Text sidewaysStiffText;
    [SerializeField] List<WheelCollider> Wheels = new List<WheelCollider>();
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            sidewaysStiff -= .1f;
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            sidewaysStiff += .1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            forwardStiff -= .1f;
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            forwardStiff += .1f;
        }
        foreach (WheelCollider wheelCollider in Wheels)
        {
            WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
            forwardFriction.stiffness = forwardStiff;
            wheelCollider.forwardFriction = forwardFriction;
            WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;
            sidewaysFriction.stiffness = sidewaysStiff;
            wheelCollider.sidewaysFriction = sidewaysFriction;
        }
        forwardStiffText.text = $"Forward stiffness: {forwardStiff.ToString()}";
        sidewaysStiffText.text = $"Sideways stiffness: {sidewaysStiff.ToString()}";
    }
}
