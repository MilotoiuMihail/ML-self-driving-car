using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCarFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Transform lookAt;
    [SerializeField] private float speed;
    private Rigidbody rb;

    void Start()
    {
        rb = followTarget.GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position, speed * Time.deltaTime);
        transform.LookAt(lookAt.position);
    }

}
