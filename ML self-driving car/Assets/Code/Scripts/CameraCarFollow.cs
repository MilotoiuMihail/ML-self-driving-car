using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCarFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Transform lookAt;
    [SerializeField] private float speed;

    void FixedUpdate()
    {
        if (!followTarget) return;

        transform.position = Vector3.Lerp(transform.position, followTarget.position, speed * Time.deltaTime);
        transform.LookAt(lookAt.position);
    }

}
