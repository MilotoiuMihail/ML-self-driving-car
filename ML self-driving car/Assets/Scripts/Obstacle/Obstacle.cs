using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : Placeable
{
    [field: SerializeField] public float CheckRadius { get; private set; }
    [SerializeField] private LayerMask cantPlaceLayerMask;

    public bool CanPlace()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CheckRadius, cantPlaceLayerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.transform != transform)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CheckRadius);
    }
    protected abstract override PlaceableItemType GetItemType();
}
