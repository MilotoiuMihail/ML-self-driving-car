using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceableData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public PlaceableItemType Type;

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        PlaceableData other = (PlaceableData)obj;
        return IsAlmostEqual(Position, other.Position, .0001f)
            && IsAlmostEqual(Rotation, other.Rotation, .0001f)
            && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode() ^ Rotation.GetHashCode() ^ Type.GetHashCode();
    }
    private bool IsAlmostEqual(Quaternion a, Quaternion b, float epsilon)
    {
        float angle = Quaternion.Angle(a, b);
        return angle < epsilon;
    }
    private bool IsAlmostEqual(Vector3 a, Vector3 b, float epsilon)
    {
        float distance = Vector3.SqrMagnitude(a - b);
        return distance < epsilon;
    }
}

[System.Serializable]
public enum PlaceableItemType
{
    ITEM1,
    ITEM2
}
