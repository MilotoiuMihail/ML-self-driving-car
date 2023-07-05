using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ButtonExitEdit : MonoBehaviour
{
    [SerializeField] private Map map;

    private ModalWindowTrigger modalWindowTrigger;
    private void Awake()
    {
        modalWindowTrigger = GetComponent<ModalWindowTrigger>();
    }

    public void Interact()
    {
        map.UpdateEnvironment();
        if (SaveDataManager.Instance.IsSameData())
        {
            GameManager.Instance.ChangeToViewState();
            return;
        }
        map.Track.ComputeTrack();
        if (map.Track.IsValidStartPiece() && map.Track.HasValidTrackLength())
        {
            modalWindowTrigger.Trigger();
        }
        else
        {
            ModalWindowData modalWindowData = ModalWindowSystem.Instance.SetTitle("Track Validation")
                        .SetBody($"Invalid Track. {(!map.Track.IsValidStartPiece() ? "Please Select a Start Piece" : "Track is too short")}.")
                        .SetButton(0, "Ok", () => { }, ColorSystem.ColorPalette.Success)
                        .SetButton(1, "Discard Changes", DiscardChanges, ColorSystem.ColorPalette.Danger)
                        .BuildData();
            ModalWindowSystem.Instance.Show(modalWindowData);
        }
    }
    public void DiscardChanges()
    {
        SaveDataManager.Instance.LoadMap();
        GameManager.Instance.ChangeToViewState();
    }
    public void SaveChanges()
    {
        GameManager.Instance.ChangeToViewState();
        SaveDataManager.Instance.SaveMap(map.Track);
    }
}
