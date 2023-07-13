using System.Collections;
using System.Collections.Generic;
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
        // tireBarrierStart.position = (track.StartPiece.IsStraight() ? Vector3.forward : Vector3.right) * 20f;
        // Transform firstCheckpoint = track.GetFirstCheckpoint().transform;
        // transform.position = firstCheckpoint.position + Vector3.forward * (track.StartPiece.IsFacingForward ? 1 : (-1));
        // transform.forward = firstCheckpoint.forward;
    }
    private void MoveTireBarrierToStart()
    {
        TrackPiece startPiece = track.StartPiece;
        tireBarrierStart.position = startPiece.transform.position;
        tireBarrierStart.forward = startPiece.transform.forward;
        if (startPiece.IsFacingForward)
        {
            tireBarrierStart.position += startPiece.transform.forward * -20f;
        }
        else if (track.StartPiece.IsStraight())
        {
            tireBarrierStart.position += startPiece.transform.forward * 20f;
        }
        else
        {
            tireBarrierStart.position += startPiece.transform.right * 20f;
            tireBarrierStart.forward = startPiece.transform.right;
        }
    }
    private void MoveTireBarrierToEnd()
    {
        TrackPiece lastPiece = track.GetLastPiece();
        tireBarrierEnd.position = lastPiece.transform.position;
        tireBarrierEnd.forward = lastPiece.transform.forward;
        if (!lastPiece.IsFacingForward)
        {
            tireBarrierEnd.position += lastPiece.transform.forward * -20f;
        }
        else if (lastPiece.IsStraight())
        {
            tireBarrierEnd.position += lastPiece.transform.forward * 20f;
        }
        else
        {
            tireBarrierEnd.position += lastPiece.transform.right * 20f;
            tireBarrierEnd.forward = lastPiece.transform.right;
        }
    }
}
