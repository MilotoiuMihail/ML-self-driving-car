using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    public bool IsMouseOverUI => EventSystem.current.IsPointerOverGameObject();
    public Vector3 MousePosition { get; private set; } = Vector3.zero;
    public event Action LeftMouseButton;
    private bool isLeftMouseButtonDown => Input.GetMouseButtonDown(0);
    public event Action LeftControlDown;
    private bool isLeftCtrlDown => Input.GetKeyDown(KeyCode.LeftControl);
    public event Action LeftCtrlLeftMouseButton;
    private bool isLeftCtrlPressed => Input.GetKey(KeyCode.LeftControl);
    public event Action LeftControlUp;
    private bool isLeftCtrlUp => Input.GetKeyUp(KeyCode.LeftControl);
    public event Action RKeyDown;
    private bool isRKeyDown => Input.GetKeyDown(KeyCode.R);
    public event Action EKeyDown;
    private bool isEKeyDown => Input.GetKeyDown(KeyCode.E);
    public event Action Key1Down;
    private bool isKey1Down => Input.GetKeyDown(KeyCode.Alpha1);
    public event Action Key2Down;
    private bool isKey2Down => Input.GetKeyDown(KeyCode.Alpha2);
    public event Action EscDown;
    private bool isEscDown => Input.GetKeyDown(KeyCode.Escape);
    [SerializeField]
    private LayerMask groundLayer;
    private void Update()
    {
        if (isEscDown)
        {
            OnEscDown();
        }
        if (GameManager.Instance.IsGameState(GameState.PAUSED))
        {
            return;
        }
        GetMouseWorldPosition();
        if (isLeftCtrlDown)
        {
            OnLeftControlDown();
        }
        else if (isLeftCtrlUp)
        {
            OnLeftControlUp();
        }
        if (isLeftCtrlPressed && isLeftMouseButtonDown)
        {
            OnLeftCtrlLeftMouseButton();
        }
        else if (isLeftMouseButtonDown)
        {
            OnLeftMouseButton();
        }
        if (isRKeyDown)
        {
            OnRKeyDown();
        }
        else if (isEKeyDown)
        {
            OnEKeyDown();
        }
        if (isKey1Down)
        {
            OnKey1Down();
        }
        else if (isKey2Down)
        {
            OnKey2Down();
        }
    }

    private void OnEscDown()
    {
        EscDown?.Invoke();
    }

    private void GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, groundLayer.value))
        {
            MousePosition = new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
        }
    }
    private void OnLeftMouseButton()
    {
        LeftMouseButton?.Invoke();
    }
    private void OnLeftControlDown()
    {
        LeftControlDown?.Invoke();
    }
    private void OnLeftControlUp()
    {
        LeftControlUp?.Invoke();
    }
    private void OnLeftCtrlLeftMouseButton()
    {
        LeftCtrlLeftMouseButton?.Invoke();
    }
    private void OnRKeyDown()
    {
        RKeyDown?.Invoke();
    }
    private void OnEKeyDown()
    {
        EKeyDown?.Invoke();
    }
    private void OnKey1Down()
    {
        Key1Down?.Invoke();
    }
    private void OnKey2Down()
    {
        Key2Down?.Invoke();
    }
}
