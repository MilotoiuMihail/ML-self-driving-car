using UnityEngine;

public class Straight : TrackPiece
{
    public override void Place(Vector3 position)
    {
        base.Place(position);
        if (base.HasRotation(180) || base.HasRotation(270))
        {
            base.RotateBy(-180);
            transform.rotation = base.rotation;
        }
        base.LockCheckpointsDirection();
    }

    protected override TrackPieceType GetTrackPieceType()
    {
        return TrackPieceType.STRAIGHT;
    }
}