using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFileOptions : MonoBehaviour
{
    private Button button;
    private void OnEnable()
    {
        GameManager.Instance.EnterEditState += SetInteractable;
        GameManager.Instance.ExitEditState += UnsetInteractable;

    }
    private void OnDisable()
    {

        GameManager.Instance.EnterEditState -= SetInteractable;
        GameManager.Instance.ExitEditState -= UnsetInteractable;
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        UnsetInteractable();
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
