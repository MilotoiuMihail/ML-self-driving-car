using UnityEngine;

public abstract class EnvironmentEditor<T> : MonoBehaviour, IEnvironmentEditor where T : Placeable
{
    [field: SerializeField] public T Item1 { get; private set; }
    [field: SerializeField] public T Item2 { get; private set; }
    [SerializeField] private float rotationAmount;
    [SerializeField] protected Transform itemParent;
    protected T currentItem;
    private T lastPlacedItem;
    public virtual void CreateItem(T item)
    {
        if (currentItem)
        {
            if (currentItem.GetType() == item.GetType())
            {
                return;
            }
            RemoveCurrentItem();
        }
        lastPlacedItem = item;
        currentItem = Instantiate(item, item.transform.position, Quaternion.identity);
        currentItem.transform.parent = itemParent.transform;
    }
    public T InstantiatePiece(PlaceableData pieceData)
    {
        T piecePrefab = Item1;
        switch (pieceData.Type)
        {
            case PlaceableItemType.ITEM2:
                piecePrefab = Item2;
                break;
        }
        T piece = Instantiate(piecePrefab, pieceData.Position, pieceData.Rotation);
        piece.transform.parent = itemParent;
        piece.SetRotation(pieceData.Rotation);
        piece.Place(pieceData.Position);
        return piece;
    }
    public void PlaceItem()
    {
        if (InputManager.Instance.IsMouseOverUI)
        {
            return;
        }
        PlaceItemLogic();
        CreateLastItem();
    }
    protected abstract void PlaceItemLogic();

    public void RemoveItem()
    {
        if (InputManager.Instance.IsMouseOverUI)
        {
            return;
        }
        RemoveItemLogic();
    }
    protected abstract void RemoveItemLogic();

    public void RemoveCurrentItem()
    {
        RemoveCurrentItemLogic();
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
        }
    }
    protected virtual void RemoveCurrentItemLogic()
    {
        return;
    }

    public void RotateItemClockwise()
    {
        currentItem.RotateBy(rotationAmount);
    }

    public void RotateItemCounterClockwise()
    {
        currentItem.RotateBy(-rotationAmount);
    }

    public void CreateType1Item()
    {
        CreateItem(Item1);
    }

    public void CreateType2Item()
    {
        CreateItem(Item2);
    }

    public void CreateLastItem()
    {
        CreateItem(lastPlacedItem);
    }
}