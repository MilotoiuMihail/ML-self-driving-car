using UnityEngine;

public class StartGate : MonoBehaviour
{
    [SerializeField] private Track track;
    private void Start()
    {
        track.TrackComputed += MoveToFirstCheckpoint;
        MoveToFirstCheckpoint();
    }
    private void OnDestroy()
    {
        track.TrackComputed -= MoveToFirstCheckpoint;
    }
    private void HandleVisibility()
    {
        if (track == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(track.StartPiece != null);
    }
    private void MoveToFirstCheckpoint()
    {
        HandleVisibility();
        if (track.StartPiece == null)
        {
            return;
        }
        Transform firstCheckpoint = track.GetFirstCheckpoint().transform;
        transform.position = new Vector3(firstCheckpoint.position.x, transform.position.y, firstCheckpoint.position.z);
        transform.forward = firstCheckpoint.forward;
    }
}
