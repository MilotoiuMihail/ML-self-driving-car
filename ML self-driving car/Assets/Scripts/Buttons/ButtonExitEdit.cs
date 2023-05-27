using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
