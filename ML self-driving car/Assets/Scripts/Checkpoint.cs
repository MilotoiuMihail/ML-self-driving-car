using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<CarAgent>(out CarAgent car))
        {
            CheckpointManager.Instance.PassedCheckpoint(car.transform, this);
        }
    }
}
