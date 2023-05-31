using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Car>(out Car car))
        {
            CheckpointManager.Instance.PassedCheckpoint(car, this);
        }
    }
}
