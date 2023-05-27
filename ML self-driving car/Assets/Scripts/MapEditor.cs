using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour, IEnvironmentEditor
{
    [SerializeField] private TrackEditor trackEditor;
    // [SerializeField] private IEnvironmentEditor obstaclesEditor;
    private IEnvironmentEditor currentEditor;
    private async void OnEnable()
    {
        await System.Threading.Tasks.Task.Yield();
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
    private async void Start()
    {
        GameManager.Instance.EnterEditState += Show;
        GameManager.Instance.ExitEditState += Hide;
        GameManager.Instance.EnterEditState += EditTrack;
        GameManager.Instance.EnterEditState += CreateType1Item;
        GameManager.Instance.ExitEditState += RemoveCurrentItem;
        await System.Threading.Tasks.Task.Yield();
        Hide();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterEditState -= Show;
        GameManager.Instance.EnterEditState -= EditTrack;
        GameManager.Instance.ExitEditState -= Hide;
        GameManager.Instance.EnterEditState -= CreateType1Item;
        GameManager.Instance.ExitEditState -= RemoveCurrentItem;
    }
    private void EditTrack()
    {
        currentEditor = trackEditor;
    }
    private void Show()
    {
        this.enabled = true;
    }
    private void Hide()
    {
        this.enabled = false;
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
