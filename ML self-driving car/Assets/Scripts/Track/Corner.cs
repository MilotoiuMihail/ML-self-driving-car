using UnityEngine;
public class Corner : TrackPiece
{
    public override int GetFirstCheckpointIndex()
    {
        int checkpoint1 = CheckpointManager.Instance.GetCheckpointIndex(base.checkpoints[1]);
        int checkpoint2 = CheckpointManager.Instance.GetCheckpointIndex(base.checkpoints[3]);
        return Mathf.Min(checkpoint1, checkpoint2);
    }

    protected override TrackPieceType GetTrackPieceType()
    {
        return TrackPieceType.CORNER;
    }
}