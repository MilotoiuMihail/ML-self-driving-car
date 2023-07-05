using UnityEngine;
public class Corner : TrackPiece
{
    public override void Place(Vector3 position)
    {
        base.Place(position);
        base.LockCheckpointsDirection();
    }
    protected override PlaceableItemType GetItemType()
    {
        return PlaceableItemType.ITEM2;
    }
}