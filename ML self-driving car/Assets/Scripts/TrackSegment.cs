using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    private bool isPlaced;
    private Quaternion rotation = Quaternion.identity;
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
    public void SetYRotation(float degrees)
    {
        rotation = Quaternion.Euler(0, degrees, 0);
    }
    public void Place(Vector3 position)
    {
        isPlaced = true;
        transform.position = position;
        transform.rotation = rotation;
    }
}