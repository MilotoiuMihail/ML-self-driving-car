using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private Track track;
    private void Start()
    {
        track.StartPieceChanged += MoveToLastCheckpoint;
        track.HasTrackDirectionChanged += MoveToLastCheckpoint;
        MoveToLastCheckpoint();
    }
    private void OnDestroy()
    {
        track.StartPieceChanged -= MoveToLastCheckpoint;
        track.HasTrackDirectionChanged -= MoveToLastCheckpoint;
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
        transform.position = lastCheckpoint.position;
        transform.forward = lastCheckpoint.forward;
    }
}
