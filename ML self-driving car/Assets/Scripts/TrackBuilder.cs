using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBuilder : MonoBehaviour
{
    private Grid grid;
    private TrackSegment currentItem;
    [SerializeField] private Transform track;
    [SerializeField] private Straight straight;
    [SerializeField] private Corner corner;
    private TrackSegment lastPiece;
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
        CreateItem(straight);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceItemOnGrid(TrackBuilder.GetMouseWorldPosition());
            CreateItem(lastPiece);
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            RemoveItemFromGrid(TrackBuilder.GetMouseWorldPosition());
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItemBy(90);
            return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateItemBy(-90);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CreateItem(straight);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CreateItem(corner);
            return;
        }
    }

    private void CreateItem(TrackSegment item)
    {
        if (currentItem)
        {
            if (currentItem.GetType() == item.GetType())
            {
                return;
            }
            Destroy(currentItem.gameObject);
        }
        lastPiece = item;
        currentItem = Instantiate(item, transform.position, Quaternion.identity);
        currentItem.transform.parent = track;
    }
    private void PlaceItemOnGrid(Vector3 position)
    {
        if (!currentItem || !grid.TryPlaceItem(position, currentItem.gameObject))
        {
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
    private void RotateItemBy(float degrees)
    {
        currentItem.RotateBy(degrees);
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
        return TrackBuilder.Instance.grid.SnapPositionToGrid(TrackBuilder.GetMouseWorldPosition());
    }
    private void OnDrawGizmos()
    {
        grid?.DrawGizmos();
    }
}
