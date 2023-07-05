using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGate : MonoBehaviour
{
    [SerializeField] private Track track;
    private void OnEnable()
    {
        track.StartPieceChanged += MoveToFirstCheckpoint;
        track.HasTrackDirectionChanged += MoveToFirstCheckpoint;
    }
    private void OnDisable()
    {
        track.StartPieceChanged -= MoveToFirstCheckpoint;
        track.HasTrackDirectionChanged -= MoveToFirstCheckpoint;
    }
    private void MoveToFirstCheckpoint()
    {
        if (track.StartPiece == null)
        {
            return;
        }
        Transform firstCheckpoint = track.StartPiece.GetFirstCheckpoint().transform;
        transform.position = firstCheckpoint.position + Vector3.forward * (track.StartPiece.IsFacingForward ? 1 : (-1));
        transform.forward = firstCheckpoint.forward;
    }
}
