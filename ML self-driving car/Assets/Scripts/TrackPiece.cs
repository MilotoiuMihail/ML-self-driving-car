using UnityEngine;

public class TrackPiece : MonoBehaviour
{
    private bool isPlaced;
    private Quaternion rotation = Quaternion.identity;
    private void Start()
    {
        transform.position = TrackBuilder.GetMouseSnappedPosition();
    }
    private void Update()
    {
        if (isPlaced)
        {
            return;
        }
        FollowMouse();
        RotateTowards(rotation);
    }
    private void FollowMouse()
    {
        MoveTowards(TrackBuilder.GetMouseSnappedPosition());
    }
    private void MoveTowards(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
    }
    private void RotateTowards(Quaternion rotation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }
    public void RotateBy(float degrees)
    {
        rotation = Quaternion.Euler(0, rotation.eulerAngles.y + degrees, 0);
    }
    public void Place(Vector3 position)
    {
        isPlaced = true;
        transform.position = position;
        transform.rotation = rotation;
        if (IsStraight())
        {
            if (HasRotation(180) || HasRotation(270))
            {
                RotateBy(180);
                transform.rotation = rotation;
            }
        }
    }
    public bool IsStraight()
    {
        return GetComponent<Straight>();
    }
    public bool IsCorner()
    {
        return GetComponent<Corner>();
    }
    public bool HasRotation(float degrees)
    {
        return Mathf.Approximately(transform.rotation.eulerAngles.y, degrees);
    }
}