using UnityEngine;

public class ObstaclesEditor : EnvironmentEditor<Obstacle>
{
    [SerializeField] CheckCircle checkCircle;
    [SerializeField] private float removeRadius;
    [SerializeField] private LayerMask removeMask;

    public override void CreateItem(Obstacle item)
    {
        base.CreateItem(item);
        MoveCheckCircle();
    }
    private void MoveCheckCircle()
    {
        checkCircle.Show(base.currentItem);
    }
    protected override void RemoveCurrentItemLogic()
    {
        checkCircle.Hide();
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
}
