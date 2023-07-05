using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierMetal : Obstacle
{
    protected override PlaceableItemType GetItemType()
    {
        return PlaceableItemType.ITEM2;
    }
}
