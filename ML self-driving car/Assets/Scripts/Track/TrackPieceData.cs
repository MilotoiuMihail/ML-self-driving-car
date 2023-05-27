using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackPieceData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public TrackPieceType Type;
}
[System.Serializable]
public enum TrackPieceType
{
    STRAIGHT,
    CORNER
}
