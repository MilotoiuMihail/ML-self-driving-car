using UnityEngine;

public abstract class TrackPiece : Placeable
{
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform alternateSpawn;
    protected Checkpoint[] checkpoints;
    public bool IsFacingForward { get; set; } = true;
    protected override void Awake()
    {
        base.Awake();
        checkpoints = GetComponentsInChildren<Checkpoint>();
    }
    public Transform GetSpawnPoint()
    {
        return IsFacingForward ? spawn : alternateSpawn;
    }
    protected void LockCheckpointsDirection()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.LockForward();
        }
    }
    public bool IsStraight()
    {
        return GetComponent<Straight>();
    }
    public Checkpoint[] GetCheckpoints()
    {
        return checkpoints;
    }
    public Checkpoint GetFirstCheckpoint()
    {
        return checkpoints[IsFacingForward ? 1 : checkpoints.Length - 2];
    }
}