using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    private bool isSet;

    private void Update()
    {
        if (isSet)
        {
            return;
        }
        FollowMouse();
    }
    private void FollowMouse()
    {
        Vector3? mousePosition = TrackBuilder.GetMouseSnappedPosition();
        if (mousePosition == null)
        {
            return;
        }
        MoveTowards(mousePosition.Value);
    }
    private void MoveTowards(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 10);
    }
}