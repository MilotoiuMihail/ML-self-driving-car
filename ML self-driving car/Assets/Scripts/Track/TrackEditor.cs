using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackEditor : EnvironmentEditor<TrackPiece>
{
    [SerializeField] private Grid grid;
    public event Action PiecePlaced;
    public event Action<TrackPiece> PieceRemoved;

    private void PlacePieceOnGrid()
    {
        Vector3 position = InputManager.Instance.MousePosition;
        if (!base.currentItem || !grid.CanPlacePiece(position))
        {
            return;
        }
        grid.PlacePiece(position, base.currentItem);
        base.currentItem.Place(grid.SnapPositionToGrid(position));
        base.currentItem = null;
    }

    protected override void PlaceItemLogic()
    {
        PlacePieceOnGrid();
        PiecePlaced?.Invoke();
    }

    protected override void RemoveItemLogic()
    {
        TrackPiece piece = grid.RemovePiece(InputManager.Instance.MousePosition);
        if (piece == null)
        {
            return;
        }
        Destroy(piece.gameObject);
        PieceRemoved?.Invoke(piece);
    }
}
