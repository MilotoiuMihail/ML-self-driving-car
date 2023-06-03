using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private Transform grid;
    [SerializeField] private TopDownCameraRig cameraRig;
    private const float scaleFactor = .064f;
    private void Awake()
    {
        float value = grid.transform.localScale.x + (cameraRig.ZoomBounds.y - transform.position.y) * scaleFactor;
        transform.localScale = new Vector3(value, 1, value);
        transform.position = grid.GetComponentInChildren<Renderer>().bounds.center;
    }
}
