using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironmentEditor
{
    public void PlaceItem();
    public void RemoveItem();
    public void CreateType1Item();
    public void CreateType2Item();
    public void CreateLastItem();
    public void RotateItemClockwise();
    public void RotateItemCounterClockwise();
    public void RemoveCurrentItem();
}
