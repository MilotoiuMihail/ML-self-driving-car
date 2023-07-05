using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesEditor : EnvironmentEditor<Obstacle>
{
    [SerializeField] Transform checkCircle;
    [SerializeField] private float removeRadius;
    [SerializeField] private LayerMask removeMask;
    private void Awake()
    {
        SetCheckCircleVisibility(false);
    }

    public override void CreateItem(Obstacle item)
    {
        base.CreateItem(item);
        MoveCheckCircle();
    }
    private void MoveCheckCircle()
    {
        checkCircle.localScale = new Vector3(1, 1, 1) * currentItem.CheckRadius * .22f;
        checkCircle.parent = base.currentItem.transform;
        checkCircle.localPosition = new Vector3(0, 0.1f, 0);
        SetCheckCircleVisibility(true);
    }
    protected override void RemoveCurrentItemLogic()
    {
        checkCircle.parent = null;
        SetCheckCircleVisibility(false);
    }
    protected override void PlaceItemLogic()
    {
        if (!base.currentItem || !base.currentItem.CanPlace())
        {
            return;
        }
        Vector3 position = InputManager.Instance.MousePosition;
        base.currentItem.Place(position);
        base.currentItem = null;
    }

    protected override void RemoveItemLogic()
    {
        Collider[] colliders = Physics.OverlapSphere(InputManager.Instance.MousePosition, removeRadius, removeMask.value);
        if (colliders.Length > 0)
        {
            GameObject obstacle = colliders[0].gameObject;
            Destroy(obstacle);
        }
    }
    private void SetCheckCircleVisibility(bool isVisibile)
    {
        checkCircle.GetComponent<Renderer>().enabled = isVisibile;
    }
}
