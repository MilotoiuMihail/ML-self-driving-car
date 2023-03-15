using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
    private enum BuildState
    {
        IDLE,
        ADD,
        REMOVE
    }
    private Grid grid;
    private GameObject currentItem;
    [SerializeField] private Transform track;
    [SerializeField] private GameObject straight;
    [SerializeField] private GameObject corner;
    [SerializeField] private BuildState state;
    public static TrackBuilder Instance { get; private set; }
    private void Awake()
    {
        IntializeSingleton();
    }

    private void Start()
    {
        grid = new Grid(20, 10, 40, Vector3.zero);
    }
    private void IntializeSingleton()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void CreateItem(GameObject item, Vector3 position)
    {
        if (currentItem)
        {
            Destroy(currentItem);
        }
        currentItem = Instantiate(item, position, Quaternion.identity);
        currentItem.transform.parent = track;
    }
    private void PlaceItemOnGrid(Vector3 position)
    {
        grid.PlaceItem(position, currentItem);
        currentItem = null;
    }
    private void RemoveItemFromGrid(Vector3 position)
    {
        GameObject item = grid.RemoveItem(position);
        Destroy(item);
    }
    private void RotateItem(Quaternion rotation)
    {
        currentItem.transform.rotation = Quaternion.Lerp(currentItem.transform.rotation, rotation, Time.deltaTime);
    }
    private void MoveItem(Vector3 position)
    {
        currentItem.transform.position = Vector3.Lerp(currentItem.transform.position, position, Time.deltaTime);
    }

    private static Vector3? GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3? mouseWorldPosition = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            mouseWorldPosition = new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
        }
        return mouseWorldPosition;
    }

    public static Vector3? GetMouseSnappedPosition()
    {
        Vector3? mousePosition = GetMouseWorldPosition();
        if (mousePosition != null)
        {
            return TrackBuilder.Instance.grid.SnappedToGridPosition(mousePosition.Value);
        }
        return null;
    }
}
