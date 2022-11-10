using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCarFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 eulerRotation;
    private float smoothness;
    private float speed;

    void Start()
    {
        transform.eulerAngles = eulerRotation;
    }

    void Update()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothness * Time.deltaTime);
        transform.LookAt(target.position);
    }

}
