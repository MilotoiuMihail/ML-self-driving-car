using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraRig : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float smoothing;
    [SerializeField] private float edgeSize;
    [SerializeField] private float minMovementSpeed;
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float rotationSpeed;
    private float edgeSizeRatio;
    private Vector3 zoomAmount;
    private Vector3 desiredPosition;
    private Vector3 desiredZoom;
    private Quaternion desiredRotation;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    // [SerializeField] private Transform bounds;
    // private Vector3 minBounds;
    // private Vector3 maxBounds;

    void Start()
    {
        desiredPosition = transform.position;
        desiredRotation = transform.rotation;
        desiredZoom = cameraTransform.localPosition;
        edgeSizeRatio = 1 / edgeSize;
        zoomAmount = new Vector3(0, -zoomSpeed, zoomSpeed) * 100;

        // Get the bounds of the map from the mapBounds transform
        // Renderer renderer = bounds.GetComponent<Renderer>();
        // minBounds = renderer.bounds.min;
        // maxBounds = renderer.bounds.max;
    }
    void LateUpdate()
    {
        // Clamp camera position to map bounds
        // transform.position = new Vector3(
        //     Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.z),
        //     transform.position.y,
        //     Mathf.Clamp(transform.position.z, minBounds.z, maxBounds.z)
        // );

        Vector2 mousePosition = Input.mousePosition;
        HandleMovement(mousePosition);
        HandleRotation(mousePosition);
        HandleZoom();
    }

    private void HandleMovement(Vector2 mousePosition)
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            return;
        }
        if (mousePosition.y >= Screen.height - edgeSize)
        {
            float distanceFromEdge = mousePosition.y - (Screen.height - edgeSize);
            desiredPosition += CalculateMovementAmount(transform.forward, distanceFromEdge);
        }
        else if (mousePosition.y <= edgeSize)
        {
            float distanceFromEdge = edgeSize - mousePosition.y;
            desiredPosition += CalculateMovementAmount(-transform.forward, distanceFromEdge);
        }
        if (mousePosition.x >= Screen.width - edgeSize)
        {
            float distanceFromEdge = mousePosition.x - (Screen.width - edgeSize);
            desiredPosition += CalculateMovementAmount(transform.right, distanceFromEdge);
        }
        else if (mousePosition.x <= edgeSize)
        {
            float distanceFromEdge = edgeSize - mousePosition.x;
            desiredPosition += CalculateMovementAmount(-transform.right, distanceFromEdge);
        }
        UpdatePosition();
    }
    private void HandleRotation(Vector2 mousePosition)
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                rotateStartPosition = mousePosition;
            }
            rotateCurrentPosition = mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            desiredRotation *= CalculateRotationAmount(difference.x);
        }
        UpdateRotation();
    }
    private void HandleZoom()
    {
        float mouseScroll = Input.mouseScrollDelta.y;
        if (mouseScroll != 0)
        {
            desiredZoom += CalculateZoomAmount(mouseScroll);
        }
        UpdateZoom();
    }
    float CalculateSpeedMultiplier(float distanceFromEdge)
    {
        return Mathf.Clamp01(distanceFromEdge * edgeSizeRatio);
    }
    private float CalculateMovementSpeed(float distanceFromEdge)
    {
        return Mathf.Lerp(minMovementSpeed, maxMovementSpeed, CalculateSpeedMultiplier(distanceFromEdge));
    }
    private Vector3 CalculateMovementAmount(Vector3 direction, float distanceFromEdge)
    {
        return direction * CalculateMovementSpeed(distanceFromEdge) * Time.deltaTime;
    }
    private void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothing);
    }
    private Quaternion CalculateRotationAmount(float positionDelta)
    {
        return Quaternion.Euler(Vector3.up * (-positionDelta * rotationSpeed * Time.deltaTime));
    }
    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothing);
    }
    private Vector3 CalculateZoomAmount(float mouseScroll)
    {
        return mouseScroll * zoomAmount * Time.deltaTime;
    }
    private void UpdateZoom()
    {
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, desiredZoom, Time.deltaTime * smoothing);
    }
}
