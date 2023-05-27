using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackPieceData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public TrackPieceType Type;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        TrackPieceData other = (TrackPieceData)obj;
        return Position.Equals(other.Position)
            && Rotation.Equals(other.Rotation)
            && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode() ^ Rotation.GetHashCode() ^ Type.GetHashCode();
    }
}

[System.Serializable]
public enum TrackPieceType
{
    STRAIGHT,
    CORNER
}
