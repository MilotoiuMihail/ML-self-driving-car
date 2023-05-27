using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackEditor : MonoBehaviour, IEnvironmentEditor
{
    [SerializeField] private Straight straightPrefab;
    [SerializeField] private Corner cornerPrefab;
    private TrackPiece currentPiece;
    private TrackPiece lastPlacedPrefab;
    [SerializeField] private Grid grid;
    public Grid Grid => grid;
    [SerializeField] private Transform track;
    public Action PiecePlaced;

    // grid = new Grid(width, height, cellSize, origin);
    // ground.position = origin + Vector3.down * .1f;
    // ground.localScale = new Vector3(-width * cellSize * .1f, 1, -height * cellSize * .1f);

    private void CreatePiece(TrackPiece piece)
    {
        if (currentPiece)
        {
            if (currentPiece.GetType() == piece.GetType())
            {
                return;
            }
            RemoveCurrentPiece();
        }
        lastPlacedPrefab = piece;
        currentPiece = Instantiate(piece, transform.position, Quaternion.identity);
        currentPiece.transform.parent = track.transform;
    }
    public TrackPiece InstantiatePiece(TrackPieceData pieceData)
    {
        TrackPiece piecePrefab = straightPrefab;
        switch (pieceData.Type)
        {
            case TrackPieceType.CORNER:
                piecePrefab = cornerPrefab;
                break;
        }
        TrackPiece piece = Instantiate(piecePrefab, pieceData.Position, pieceData.Rotation);
        piece.transform.parent = track;
        piece.SetRotation(pieceData.Rotation);
        piece.Place(pieceData.Position);
        return piece;
    }
    private void PlacePieceOnGrid()
    {
        Vector3 position = InputManager.Instance.MousePosition;
        if (!Grid.CanPlacePiece(position) || !currentPiece)
        {
            return;
        }
        Grid.PlacePiece(position, currentPiece);
        currentPiece.Place(Grid.SnapPositionToGrid(position));
        currentPiece = null;
    }
    private void RemoveCurrentPiece()
    {
        Destroy(currentPiece?.gameObject);
    }

    private void CreateLastPiece()
    {
        CreatePiece(lastPlacedPrefab);
    }

    public void PlaceItem()
    {
        if (InputManager.Instance.IsMouseOverUI)
        {
            return;
        }
        PlacePieceOnGrid();
        PiecePlaced?.Invoke();
        CreateLastPiece();
    }

    public void RemoveItem()
    {
        if (InputManager.Instance.IsMouseOverUI)
        {
            return;
        }
        Destroy(Grid.RemovePiece(InputManager.Instance.MousePosition)?.gameObject);
    }

    public void CreateType1Item()
    {
        CreatePiece(straightPrefab);
    }

    public void CreateType2Item()
    {
        CreatePiece(cornerPrefab);
    }

    public void CreateLastItem()
    {
        CreateLastPiece();
    }

    public void RotateItemClockwise()
    {
        currentPiece.RotateBy(90);
    }

    public void RotateItemCounterClockwise()
    {
        currentPiece.RotateBy(-90);
    }

    public void RemoveCurrentItem()
    {
        RemoveCurrentPiece();
    }
}
