using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ButtonExitEdit : MonoBehaviour
{
    [SerializeField] private Track track;

    private ModalWindowTrigger modalWindowTrigger;
    private void Awake()
    {
        modalWindowTrigger = GetComponent<ModalWindowTrigger>();
    }

    public void Interact()
    {
        if (IsSameData())
        {
            GameManager.Instance.ChangeToViewState();
            return;
        }
        track.ComputeTrack();
        if (track.IsValidStartPiece() && track.HasValidTrackLength())
        {
            modalWindowTrigger.Trigger();
        }
        else
        {
            ModalWindowData modalWindowData = ModalWindowSystem.Instance.SetTitle("Track Validation")
                        .SetBody($"Invalid Track. {(!track.IsValidStartPiece() ? "Please Select a Start Piece" : "Track is too short")}.")
                        .SetButton(0, "Ok", () => { }, ColorSystem.ColorPalette.Success)
                        .SetButton(1, "Discard Changes", DiscardChanges, ColorSystem.ColorPalette.Danger)
                        .BuildData();
            ModalWindowSystem.Instance.Show(modalWindowData);
        }
    }
    public void DiscardChanges()
    {
        TrackDataManager.LoadTrack(track);
        GameManager.Instance.ChangeToViewState();
    }
    public void SaveChanges()
    {
        TrackDataManager.SaveTrack(track);
        GameManager.Instance.ChangeToViewState();
    }
    private bool IsSameGrid()
    {
        int countPieces = 0;
        for (int x = 0; x < track.TrackEditor.Grid.Width; x++)
        {
            for (int y = 0; y < track.TrackEditor.Grid.Width; y++)
            {
                TrackPiece piece = track.TrackEditor.Grid.GetPiece(x, y);
                if (piece != null)
                {
                    countPieces += 1;
                    if (!SaveManager.CurrentSaveData.Grid.Contains(piece.ToData()))
                    {
                        return false;
                    }
                }
            }
        }
        return countPieces == SaveManager.CurrentSaveData.Grid.Count;
    }
    private bool IsSameStartPiece()
    {
        return track.StartPiece && track.StartPiece.ToData().Equals(SaveManager.CurrentSaveData.Track.StartPiece);
    }
    private bool IsSameTrackDirection()
    {
        return track.IsTrackDirectionClockwise == SaveManager.CurrentSaveData.Track.IsTrackDirectionClockwise;
    }
    private bool IsSameData()
    {
        return IsSameStartPiece() && IsSameTrackDirection() && IsSameGrid();
    }
}
