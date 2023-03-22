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
    [SerializeField] private Straight straight;
    [SerializeField] private Corner corner;
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
        // CreateItem(straight, grid.SnapPositionToGrid(TrackBuilder.GetMouseWorldPosition()));
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
    private void SetCurrentItem(TrackSegment item)
    {
        if (currentItem)
        {
            Destroy(currentItem);
        }
        currentItem = item;
        currentItem.transform.parent = track;
    }
    private void PlaceItemOnGrid(Vector3 position)
    {
        SetCurrentItem(DetermineItem(TrackBuilder.GetMouseWorldPosition()));
        if (!grid.TryPlaceItem(position, currentItem.gameObject))
        {
            Destroy(currentItem.gameObject);
            return;
        }
        currentItem.Place(grid.SnapPositionToGrid(position));
        currentItem = null;
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
    private TrackSegment DetermineItem(Vector3 position)
    {
        GameObject[] neighbors = grid.GetNeighbors(position);
        foreach (var neighbor in neighbors)
        {
            // Debug.Log(neighbor);
        }
        // straight - 0 default nu ii trebe reguli?
        // 0 - null || straight - 0 || corner - 0 || corner - 90
        // ||
        // 2 - null || straight - 0 || corner - 270 || corner - 180
        // straight - 90
        // 1 - null || straight - 90 || corner - 90 || corner - 180
        // ||
        // 3 - null || straight - 90 || corner - 0 || corner - 270
        if (IsStraight(neighbors[1]) && HasYRotation(neighbors[1], 90) && IsStraight(neighbors[2]) && HasYRotation(neighbors[2], 0))
        {
            return Instantiate(corner, position, Quaternion.identity);
        }
        if (IsStraight(neighbors[1]) || IsStraight(neighbors[3]))
        {
            Debug.Log("bruh");
            return Instantiate(straight, position, Quaternion.Euler(0, 90, 0));
        }
        // corner - 0
        // 1 - straight - 90 || corner - 90 || corner - 180
        // &&
        // 2 - straight - 0 || corner - 270 || corner - 180
        return Instantiate(straight, position, Quaternion.identity);
        // corner - 90
        // 2 - straight - 0 || corner - 180 || corner - 270
        // &&
        // 3 - straight - 90 || corner - 0 || corner - 270
        // corner - 180
        // 0 - straight - 0 || corner - 90 || corner - 0
        // &&
        // 3 - straight - 90 || corner - 270 || corner - 0
        // corner - 270
        // 0 - straight - 0 || corner - 0 || corner - 90
        // &&
        // 1 - straight - 90 || corner - 180 || corner - 90
    }
    private bool IsStraight(GameObject item)
    {
        return item?.GetComponent<Straight>();
    }
    private bool IsCorner(GameObject item)
    {
        return item?.GetComponent<Corner>();
    }
    private bool HasYRotation(GameObject item, int degrees)
    {
        return item?.transform.rotation.eulerAngles.y == degrees;
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
