using UnityEngine;

public class Straight : TrackPiece
{
    public override void Place(Vector3 position)
    {
        base.Place(position);
        if (HasRotation(180) || HasRotation(270))
        {
            base.RotateBy(-180);
            transform.rotation = base.rotation;
        }
        base.LockCheckpointsDirection();
    }
    private bool HasRotation(float degrees)
    {
        return Mathf.Approximately(transform.rotation.eulerAngles.y, degrees);
    }
    protected override PlaceableItemType GetItemType()
    {
        return PlaceableItemType.ITEM1;
    }
}