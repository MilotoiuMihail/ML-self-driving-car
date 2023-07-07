using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaveDataManager : Singleton<SaveDataManager>
{
    [SerializeField] private Map map;
    [SerializeField] private MapEditor mapEditor;
    public event Action MapLoaded;

    private void Start()
    {
        GameManager.Instance.EnterEditState += LoadObstaclesData;
        LoadMap();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterEditState -= LoadObstaclesData;
    }
    public void SaveMap()
    {
        SaveMapData(SaveManager.CurrentSaveData);
        SaveManager.Save();
    }
    public void SaveMapData(SaveData saveData)
    {
        SaveTrack(saveData);
        SaveTrackPieces(saveData);
        SaveObstacles(saveData);
    }
    private void SaveTrack(SaveData saveData)
    {
        saveData.Track = map.Track.ToData();
    }
    private void SaveTrackPieces(SaveData saveData)
    {
        List<PlaceableData> trackPiecesData = new List<PlaceableData>();
        foreach (TrackPiece piece in map.TrackPieces)
        {
            trackPiecesData.Add(piece.ToData());
        }
        saveData.TrackPieces = trackPiecesData;
    }
    private void SaveObstacles(SaveData saveData)
    {
        List<PlaceableData> obstaclesData = new List<PlaceableData>();
        foreach (Obstacle piece in map.Obstacles)
        {
            obstaclesData.Add(piece.ToData());
        }
        saveData.Obstacles = obstaclesData;
    }
    public void LoadMap()
    {
        SaveManager.Load();
        LoadMapData(SaveManager.CurrentSaveData);
    }
    public void LoadMapData(SaveData saveData)
    {
        TrackData trackData = saveData.Track;
        List<PlaceableData> trackPiecesData = saveData.TrackPieces;
        LoadTrackPieces(trackPiecesData);
        map.Track.Load(trackData);
        LoadObstacles(saveData);
        MapLoaded?.Invoke();
    }

    private void LoadTrackPieces(List<PlaceableData> trackPiecesData)
    {
        map.Grid.Reset();
        map.TrackPieces.Clear();
        if (trackPiecesData == null)
        {
            return;
        }
        foreach (PlaceableData pieceData in trackPiecesData)
        {
            if (pieceData != null)
            {
                TrackPiece piece = mapEditor.TrackEditor.InstantiatePiece(pieceData);
                map.Grid.PlacePiece(pieceData.Position, piece);
                map.TrackPieces.Add(piece);
            }
        }
    }
    private void LoadObstaclesData()
    {
        LoadObstacles(SaveManager.CurrentSaveData);
    }
    private void LoadObstacles(SaveData saveData)
    {
        map.ResetObstacles();
        List<PlaceableData> obstaclesData = saveData.Obstacles;
        if (obstaclesData == null)
        {
            return;
        }
        foreach (PlaceableData obstacleData in obstaclesData)
        {
            if (obstacleData != null)
            {
                map.Obstacles.Add(mapEditor.ObstaclesEditor.InstantiatePiece(obstacleData));
            }
        }
    }
    public void UpdateMap()
    {
        map.UpdateEnvironment();
    }
    private bool AreSameTrackPieces()
    {
        if (map.TrackPieces.Count != SaveManager.CurrentSaveData.TrackPieces.Count)
        {
            return false;
        }
        foreach (TrackPiece piece in map.TrackPieces)
        {
            if (!SaveManager.CurrentSaveData.TrackPieces.Contains(piece.ToData()))
            {
                return false;
            }
        }
        return true;
    }
    private bool AreSameObstacles()
    {
        if (map.Obstacles.Count != SaveManager.CurrentSaveData.Obstacles.Count)
        {
            return false;
        }
        foreach (Obstacle obstacle in map.Obstacles)
        {
            if (!SaveManager.CurrentSaveData.Obstacles.Contains(obstacle.ToData()))
            {
                return false;
            }
        }
        return true;
    }
    private bool IsSameStartPiece()
    {
        return map.Track.StartPiece && map.Track.StartPiece.ToData().Equals(SaveManager.CurrentSaveData.Track.StartPiece);
    }
    private bool IsSameTrackDirection()
    {
        return map.Track.IsTrackDirectionClockwise == SaveManager.CurrentSaveData.Track.IsTrackDirectionClockwise;
    }
    private bool HasSameLaps()
    {
        return map.Track.Laps == SaveManager.CurrentSaveData.Track.Laps;
    }
    private bool IsSameTrack()
    {
        return IsSameStartPiece() && IsSameTrackDirection() && HasSameLaps();
    }
    public bool IsSameData()
    {
        return IsSameTrack() && AreSameTrackPieces() && AreSameObstacles();
    }
}
