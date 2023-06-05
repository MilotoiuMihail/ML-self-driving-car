using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrackDataManager
{
    public static void SaveTrack(Track track)
    {
        TrackData data = new TrackData();
        data.IsTrackDirectionClockwise = track.IsTrackDirectionClockwise;
        data.StartPiece = track.StartPiece != null ? track.StartPiece.ToData() : null;
        int width = track.TrackEditor.Grid.Width;
        int height = track.TrackEditor.Grid.Height;
        List<TrackPieceData> gridData = new List<TrackPieceData>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TrackPiece piece = track.TrackEditor.Grid.GetPiece(x, y);
                if (piece != null)
                {
                    gridData.Add(piece.ToData());
                }
            }
        }
        SaveManager.CurrentSaveData.Track = data;
        SaveManager.CurrentSaveData.Grid = gridData;
        SaveManager.Save();
    }
    public static void LoadTrack(Track track)
    {
        SaveManager.Load();
        TrackData trackData = SaveManager.CurrentSaveData.Track;
        List<TrackPieceData> gridData = SaveManager.CurrentSaveData.Grid;
        track.TrackEditor.Grid.Reset();
        if (trackData != null)
        {
            track.IsTrackDirectionClockwise = trackData.IsTrackDirectionClockwise;
        }
        if (gridData != null)
        {
            foreach (TrackPieceData pieceData in gridData)
            {
                if (pieceData != null)
                {
                    TrackPiece piece = track.TrackEditor.InstantiatePiece(pieceData);
                    track.TrackEditor.Grid.PlacePiece(pieceData.Position, piece);
                }
            }
            if (trackData != null)
            {
                // if (trackData.StartPiece != null)
                // {
                //     track.StartPiece = track.TrackEditor.Grid.GetPiece(trackData.StartPiece.Position);
                //     track.StartPiece.IsFacingForward = track.IsTrackDirectionClockwise;
                // }
                // else
                // {
                //     track.StartPiece = null;
                // }
                track.StartPiece = trackData.StartPiece != null ? track.TrackEditor.Grid.GetPiece(trackData.StartPiece.Position) : null;
            }
            // track.ComputeTrack();
        }
    }
}
