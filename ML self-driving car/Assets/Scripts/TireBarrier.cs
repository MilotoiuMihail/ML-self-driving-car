using UnityEngine;

public class TireBarrier : MonoBehaviour
{
    [SerializeField] private Track track;
    [SerializeField] private Transform tireBarrierStart;
    [SerializeField] private Transform tireBarrierEnd;
    private void OnEnable()
    {
        GameManager.Instance.EnterEditState += DisableColliders;
        GameManager.Instance.ExitEditState += EnableColliders;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= DisableColliders;
        GameManager.Instance.ExitEditState -= EnableColliders;
    }
    private void Start()
    {
        track.TrackComputed += MoveAtTrackEnds;
        MoveAtTrackEnds();
    }
    private void OnDestroy()
    {
        track.TrackComputed -= MoveAtTrackEnds;
    }
    private void DisableColliders()
    {
        tireBarrierEnd.GetComponent<Collider>().enabled = false;
        tireBarrierStart.GetComponent<Collider>().enabled = false;
    }
    private void EnableColliders()
    {
        tireBarrierEnd.GetComponent<Collider>().enabled = true;
        tireBarrierStart.GetComponent<Collider>().enabled = true;
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
    private void MoveAtTrackEnds()
    {
        HandleVisibility();
        if (track.StartPiece == null)
        {
            return;
        }
        MoveTireBarrierToStart();
        MoveTireBarrierToEnd();
    }
    private void MoveTireBarrierToStart()
    {
        TrackPiece startPiece = track.StartPiece;
        MoveToPiece(tireBarrierStart, startPiece);
        if (startPiece.IsFacingForward)
        {
            MoveInDirection(tireBarrierStart, -startPiece.transform.forward);
        }
        else
        {
            MoveBasedOnShape(tireBarrierStart, startPiece);
        }
    }
    private void MoveTireBarrierToEnd()
    {
        TrackPiece lastPiece = track.GetLastPiece();
        MoveToPiece(tireBarrierEnd, lastPiece);
        if (!lastPiece.IsFacingForward)
        {
            MoveInDirection(tireBarrierEnd, -lastPiece.transform.forward);
        }
        else
        {
            MoveBasedOnShape(tireBarrierEnd, lastPiece);
        }
    }
    private void MoveInDirection(Transform barrier, Vector3 direction)
    {
        barrier.position += direction * 20f;
    }
    private void MoveBasedOnShape(Transform barrier, TrackPiece piece)
    {
        if (piece.IsStraight())
        {
            MoveInDirection(barrier, piece.transform.forward);
        }
        else
        {
            MoveInDirection(barrier, piece.transform.right);
            barrier.forward = piece.transform.right;
        }
    }
    private void MoveToPiece(Transform barrier, TrackPiece piece)
    {
        barrier.position = piece.transform.position;
        barrier.forward = piece.transform.forward;
    }
}
