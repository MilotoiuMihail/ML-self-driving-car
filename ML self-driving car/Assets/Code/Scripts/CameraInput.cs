using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    public bool SwitchCamera { get; private set; }

    void Update()
    {
        SwitchCamera = Input.GetKeyDown(KeyCode.Tab);
    }
}
