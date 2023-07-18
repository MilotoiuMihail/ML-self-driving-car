using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private Track track;
    private void Start()
    {
        track.TrackComputed += MoveToLastCheckpoint;
        MoveToLastCheckpoint();
    }
    private void OnDestroy()
    {
        track.TrackComputed -= MoveToLastCheckpoint;
    }
    private void HandleVisibility()
    {
        if (track == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(!track.IsCircular && track.StartPiece != null);
    }
    private void MoveToLastCheckpoint()
    {
        HandleVisibility();
        if (track.StartPiece == null)
        {
            return;
        }
        Transform lastCheckpoint = track.GetLastCheckpoint().transform;
        transform.position = new Vector3(lastCheckpoint.position.x, transform.position.y, lastCheckpoint.position.z);
        transform.forward = lastCheckpoint.forward;
    }
}
