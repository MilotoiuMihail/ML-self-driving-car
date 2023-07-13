using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Placeable : MonoBehaviour
{
    public bool IsPlaced { get; private set; }
    protected Quaternion rotation = Quaternion.identity;

    protected virtual void Awake()
    {
        transform.position = InputManager.Instance.MousePosition;
    }
    protected virtual void Update()
    {
        if (IsPlaced)
        {
            return;
        }
        FollowMouse();
        RotateTowards(rotation);
    }
    private void FollowMouse()
    {
        SetPosition(InputManager.Instance.MousePosition + Vector3.up * .1f);
    }
    private void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void RotateTowards(Quaternion rotation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }
    public void RotateBy(float degrees)
    {
        rotation = Quaternion.Euler(0, rotation.eulerAngles.y + degrees, 0);
    }
    public void SetRotation(Quaternion rotation)
    {
        this.rotation = rotation;
    }
    public virtual void Place(Vector3 position)
    {
        IsPlaced = true;
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, Mathf.Round(rotation.eulerAngles.y), 0);
    }
    public PlaceableData ToData()
    {
        PlaceableData data = new PlaceableData();
        data.Position = transform.position;
        data.Rotation = transform.rotation;
        data.Type = GetItemType();
        return data;
    }
    protected abstract PlaceableItemType GetItemType();
}
