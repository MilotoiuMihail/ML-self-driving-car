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
    private TrackSegment currentItem;
    [SerializeField] private Transform track;
    [SerializeField] private TrackSegment straight;
    [SerializeField] private TrackSegment corner;
    [SerializeField] private BuildState state;
    public static TrackBuilder Instance { get; private set; }
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
    private void Awake()
    {
        IntializeSingleton();
    }

    private void Start()
    {
        grid = new Grid(20, 10, 40, Vector3.zero);
        CreateItem(straight, grid.SnapPositionToGrid(TrackBuilder.GetMouseWorldPosition()));
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceItemOnGrid(TrackBuilder.GetMouseWorldPosition());
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            RemoveItemFromGrid(TrackBuilder.GetMouseWorldPosition());
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem(90);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateItem(0);
            return;
        }
    }

    private void CreateItem(TrackSegment item, Vector3 position)
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
        if (!grid.TryPlaceItem(position, currentItem.gameObject))
        {
            return;
        }
        currentItem.Place(grid.SnapPositionToGrid(position));
        currentItem = null;
        CreateItem(straight, grid.SnapPositionToGrid(TrackBuilder.GetMouseWorldPosition()));
    }
    private void RemoveItemFromGrid(Vector3 position)
    {
        GameObject item = grid.RemoveItem(position);
        Destroy(item);
    }
    private void RotateItem(float degrees)
    {
        currentItem.SetYRotation(degrees);
    }

    private static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
        }
        return Vector3.zero;
    }
    public static Vector3 GetMouseSnappedPosition()
    {
        return TrackBuilder.Instance.grid.SnapPositionToGrid(GetMouseWorldPosition());
    }
}
