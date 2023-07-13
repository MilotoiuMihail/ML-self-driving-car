using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGate : MonoBehaviour
{
    [SerializeField] private Track track;
    private void Start()
    {
        track.StartPieceChanged += MoveToFirstCheckpoint;
        track.HasTrackDirectionChanged += MoveToFirstCheckpoint;
        MoveToFirstCheckpoint();
    }
    private void OnDestroy()
    {
        track.StartPieceChanged -= MoveToFirstCheckpoint;
        track.HasTrackDirectionChanged -= MoveToFirstCheckpoint;
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
        transform.position = firstCheckpoint.position + Vector3.forward * (track.StartPiece.IsFacingForward ? 1 : (-1));
        transform.forward = firstCheckpoint.forward;
    }
}
