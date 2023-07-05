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
    [SerializeField] private Transform target;
    private bool followTarget;
    private bool previousFollowTarget;
    private float edgeSizeRatio;
    private Vector3 zoomAmount;
    private Vector3 desiredPosition;
    private Vector3 desiredZoom;
    private Quaternion desiredRotation;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    [SerializeField] private Renderer boundsRenderer;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    [SerializeField] private Vector2 zoomBounds;
    public Vector2 ZoomBounds => zoomBounds;
    private void OnEnable()
    {
        GameManager.Instance.EnterEditState += UnsetFollowTarget;
        GameManager.Instance.ExitEditState += SetPreviousFollowTarget;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= UnsetFollowTarget;
        GameManager.Instance.ExitEditState += SetPreviousFollowTarget;
    }
    private void Start()
    {
        minBounds = boundsRenderer.bounds.min;
        maxBounds = boundsRenderer.bounds.max;
        transform.position = boundsRenderer.bounds.center;
        followTarget = true;
        desiredPosition = transform.position;
        desiredRotation = transform.rotation;
        float defaultZoom = (ZoomBounds.x + ZoomBounds.y) * .5f;
        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            defaultZoom,
            -defaultZoom
        );
        desiredZoom = cameraTransform.localPosition;
        edgeSizeRatio = 1 / edgeSize;
        zoomAmount = new Vector3(0, -zoomSpeed, zoomSpeed) * 100;
    }
    void LateUpdate()
    {
        HandleZoom();
        Vector2 mousePosition = Input.mousePosition;
        HandleRotation(mousePosition);
        if (followTarget)
        {
            FollowTarget();
            desiredPosition = transform.position;
            return;
        }
        HandleMovement(mousePosition);
    }
    private void UnsetFollowTarget()
    {
        SetFollowTarget(false);
    }
    private void SetPreviousFollowTarget()
    {
        SetFollowTarget(previousFollowTarget);
    }
    public void SetFollowTarget(bool follow)
    {
        SaveFollowTarget();
        followTarget = follow;
    }
    private void SaveFollowTarget()
    {
        previousFollowTarget = followTarget;
    }
    private void FollowTarget()
    {
        if (!target)
        {
            return;
        }
        transform.position = target.position;
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
        ClampMovement();
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
        ClampZoom();
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
    private void ClampMovement()
    {
        desiredPosition = new Vector3(
                    Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.z),
                    desiredPosition.y,
                    Mathf.Clamp(desiredPosition.z, minBounds.z, maxBounds.z)
                );
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
    private void ClampZoom()
    {
        desiredZoom = new Vector3(
            desiredZoom.x,
            Mathf.Clamp(desiredZoom.y, ZoomBounds.x, ZoomBounds.y),
            Mathf.Clamp(desiredZoom.z, -ZoomBounds.y, -ZoomBounds.x)
        );
    }
}
