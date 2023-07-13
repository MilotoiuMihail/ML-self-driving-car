using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager>
{
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";
    private Vector3 defaultScreenMousePosition { get { return new Vector3(Screen.width / 2f, Screen.height / 2f, 0); } }
    public bool IsMouseOverUI => EventSystem.current.IsPointerOverGameObject();
    public Vector3 MousePosition { get; private set; } = Vector3.zero;
    public Vector3 ScreenMousePosition { get; private set; }
    public float MouseScrollDelta { get; private set; } = 0;
    public event Action LeftMouseButton;
    private bool isLeftMouseButtonDown => Input.GetMouseButtonDown(0);
    public event Action LeftControlDown;
    private bool isLeftCtrlDown => Input.GetKeyDown(KeyCode.LeftControl);
    public event Action LeftCtrlLeftMouseButton;
    private bool isLeftCtrlPressed => Input.GetKey(KeyCode.LeftControl);
    public event Action LeftControlUp;
    private bool isLeftCtrlUp => Input.GetKeyUp(KeyCode.LeftControl);
    public event Action RKeyDown;
    public bool IsRKeyDown => Input.GetKeyDown(KeyCode.R);
    public event Action EKeyDown;
    private bool isEKeyDown => Input.GetKeyDown(KeyCode.E);
    public event Action Key1Down;
    private bool isKey1Down => Input.GetKeyDown(KeyCode.Alpha1);
    public event Action Key2Down;
    private bool isKey2Down => Input.GetKeyDown(KeyCode.Alpha2);
    public event Action EscDown;
    private bool isEscDown => Input.GetKeyDown(KeyCode.Escape);
    private bool isLeftAltPressed => Input.GetKey(KeyCode.LeftAlt);
    public bool IsLeftAltPressed
    {
        get
        {
            return GameManager.Instance.IsGameState(GameState.PAUSED) ? false : isLeftAltPressed;
        }
    }
    public event Action LeftAltDown;
    private bool isLeftAltDown => Input.GetKeyDown(KeyCode.LeftAlt);
    public bool IsCKeyDown => Input.GetKeyDown(KeyCode.C);
    public bool IsVKeyDown => Input.GetKeyDown(KeyCode.V);
    public float InputX => Input.GetAxis(HorizontalInput);
    public float InputY => Input.GetAxis(VerticalInput);
    [SerializeField] private LayerMask groundLayer;
    private void Update()
    {
        if (isEscDown)
        {
            OnEscDown();
        }
        if (GameManager.Instance.IsGameState(GameState.PAUSED))
        {
            ScreenMousePosition = defaultScreenMousePosition;
            MouseScrollDelta = 0;
            return;
        }
        GetMouseWorldPosition();
        ScreenMousePosition = Input.mousePosition;
        MouseScrollDelta = Input.mouseScrollDelta.y;
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
        if (IsRKeyDown)
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
        if (isLeftAltDown)
        {
            OnLeftAltDown();
        }
    }
    private void OnLeftAltDown()
    {
        LeftAltDown?.Invoke();
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
