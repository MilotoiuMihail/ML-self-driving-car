using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackEditor), typeof(ObstaclesEditor))]
public class MapEditor : MonoBehaviour, IEnvironmentEditor
{
    public TrackEditor TrackEditor { get; private set; }
    public ObstaclesEditor ObstaclesEditor { get; private set; }
    private IEnvironmentEditor currentEditor;
    private void OnEnable()
    {
        InputManager.Instance.LeftMouseButton += PlaceItem;
        InputManager.Instance.LeftCtrlLeftMouseButton += RemoveItem;
        InputManager.Instance.LeftControlDown += RemoveCurrentItem;
        InputManager.Instance.LeftControlUp += CreateLastItem;
        InputManager.Instance.Key1Down += CreateType1Item;
        InputManager.Instance.Key2Down += CreateType2Item;
        InputManager.Instance.RKeyDown += RotateItemClockwise;
        InputManager.Instance.EKeyDown += RotateItemCounterClockwise;
    }
    private void OnDisable()
    {
        InputManager.Instance.LeftMouseButton -= PlaceItem;
        InputManager.Instance.LeftCtrlLeftMouseButton -= RemoveItem;
        InputManager.Instance.LeftControlDown -= RemoveCurrentItem;
        InputManager.Instance.LeftControlUp -= CreateLastItem;
        InputManager.Instance.Key1Down -= CreateType1Item;
        InputManager.Instance.Key2Down -= CreateType2Item;
        InputManager.Instance.RKeyDown -= RotateItemClockwise;
        InputManager.Instance.EKeyDown -= RotateItemCounterClockwise;
    }
    private void Awake()
    {
        TrackEditor = GetComponent<TrackEditor>();
        ObstaclesEditor = GetComponent<ObstaclesEditor>();
    }
    private void Start()
    {
        GameManager.Instance.EnterEditState += HandleEnterEdit;
        GameManager.Instance.ExitEditState += HandleExitEdit;
        EditTrack();
        Hide();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterEditState -= HandleEnterEdit;
        GameManager.Instance.ExitEditState -= HandleExitEdit;
    }
    private void HandleEnterEdit()
    {
        Show();
        CreateType1Item();
    }
    private void HandleExitEdit()
    {
        RemoveCurrentItem();
        Hide();
    }
    private void EditTrack()
    {
        currentEditor = TrackEditor;
    }
    private void EditObstacles()
    {
        currentEditor = ObstaclesEditor;
    }
    public void SwitchEditor(bool isEditingObstacles)
    {
        currentEditor.RemoveCurrentItem();
        if (isEditingObstacles)
        {
            EditObstacles();
        }
        else
        {
            EditTrack();
        }
        currentEditor.CreateType1Item();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void CreateLastItem()
    {
        currentEditor.CreateLastItem();
    }

    public void CreateType1Item()
    {
        currentEditor.CreateType1Item();
    }

    public void CreateType2Item()
    {
        currentEditor.CreateType2Item();
    }

    public void PlaceItem()
    {
        currentEditor.PlaceItem();
    }

    public void RemoveCurrentItem()
    {
        currentEditor.RemoveCurrentItem();
    }

    public void RemoveItem()
    {
        currentEditor.RemoveItem();
    }

    public void RotateItemClockwise()
    {
        currentEditor.RotateItemClockwise();
    }

    public void RotateItemCounterClockwise()
    {
        currentEditor.RotateItemCounterClockwise();
    }
}
