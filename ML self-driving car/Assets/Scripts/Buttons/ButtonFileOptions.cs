using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFileOptions : MonoBehaviour
{
    private Button button;
    private void Start()
    {

    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterEditState -= SetInteractable;
        GameManager.Instance.ExitEditState -= UnsetInteractable;
    }

    private async void Awake()
    {
        button = GetComponent<Button>();
        UnsetInteractable();
        await System.Threading.Tasks.Task.Yield();
        GameManager.Instance.EnterEditState += SetInteractable;
        GameManager.Instance.ExitEditState += UnsetInteractable;
    }

    private void SetInteractable()
    {
        button.interactable = true;
    }
    private void UnsetInteractable()
    {
        button.interactable = false;
    }
}
