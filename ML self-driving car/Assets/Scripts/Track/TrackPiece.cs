using UnityEngine;

public abstract class TrackPiece : MonoBehaviour
{
    private bool isPlaced;
    [SerializeField] private Transform spawn;
    [SerializeField] private Transform alternateSpawn;
    protected Quaternion rotation = Quaternion.identity;
    protected Checkpoint[] checkpoints;
    public bool IsFacingForward { get; set; } = true;
    private void Awake()
    {
        checkpoints = GetComponentsInChildren<Checkpoint>();
        transform.position = InputManager.Instance.MousePosition;
    }
    private void Update()
    {
        if (isPlaced)
        {
            return;
        }
        FollowMouse();
        RotateTowards(rotation);
    }
    public Transform GetSpawnPoint()
    {
        return IsFacingForward ? spawn : alternateSpawn;
    }
    public TrackPieceData ToData()
    {
        TrackPieceData data = new TrackPieceData();
        data.Position = transform.position;
        data.Rotation = transform.rotation;
        data.Type = GetTrackPieceType();
        return data;
    }
    protected abstract TrackPieceType GetTrackPieceType();
    private void FollowMouse()
    {
        MoveTowards(InputManager.Instance.MousePosition);
    }
    private void MoveTowards(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
    }
    private void RotateTowards(Quaternion rotation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }
    public void RotateBy(float degrees)
    {
        rotation = Quaternion.Euler(0, rotation.eulerAngles.y + degrees, 0);
    }
    public void SetRotation(Quaternion rotation)
    {
        this.rotation = rotation;
    }
    public virtual void Place(Vector3 position)
    {
        isPlaced = true;
        transform.position = position;
        transform.rotation = rotation;
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
    public bool HasRotation(float degrees)
    {
        return Mathf.Approximately(transform.rotation.eulerAngles.y, degrees);
    }
    public Checkpoint[] GetCheckpoints()
    {
        return checkpoints;
    }
    public int GetFirstCheckpointIndex()
    {
        int checkpoint1 = CheckpointManager.Instance.GetCheckpointIndex(checkpoints[1]);
        int checkpoint2 = CheckpointManager.Instance.GetCheckpointIndex(checkpoints[checkpoints.Length - 2]);
        return Mathf.Min(checkpoint1, checkpoint2);
    }
}