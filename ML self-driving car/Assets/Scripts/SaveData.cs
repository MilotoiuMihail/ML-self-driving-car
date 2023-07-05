using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public TrackData Track = new TrackData();
    public List<PlaceableData> TrackPieces = new List<PlaceableData>();
    public List<PlaceableData> Obstacles = new List<PlaceableData>();
}
