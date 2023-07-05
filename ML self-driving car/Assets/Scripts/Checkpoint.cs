using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3 InitialForward { get; private set; }
    public void LockForward()
    {
        InitialForward = transform.forward;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<Car>(out Car car))
        {
            CheckpointManager.Instance.PassedCheckpoint(car.transform, this);
        }
    }
}
