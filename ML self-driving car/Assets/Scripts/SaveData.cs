using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public TrackData Track;
    public List<TrackPieceData> Grid = new List<TrackPieceData>();
}
