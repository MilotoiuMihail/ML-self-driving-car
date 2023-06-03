using UnityEngine;

public class Straight : TrackPiece
{
    public override void Place(Vector3 position)
    {
        base.Place(position);
        if (HasRotation(180) || HasRotation(270))
        {
            RotateBy(-180);
            transform.rotation = base.rotation;
        }
    }

    protected override TrackPieceType GetTrackPieceType()
    {
        return TrackPieceType.STRAIGHT;
    }
    public override int GetFirstCheckpointIndex()
    {
        int checkpoint1 = CheckpointManager.Instance.GetCheckpointIndex(base.checkpoints[0]);
        int checkpoint2 = CheckpointManager.Instance.GetCheckpointIndex(base.checkpoints[1]);
        return Mathf.Max(checkpoint1, checkpoint2);
    }
}