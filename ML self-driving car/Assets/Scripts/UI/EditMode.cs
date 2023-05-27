using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMode : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.EnterEditState += Show;
        GameManager.Instance.ExitEditState += Hide;
        Hide();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterEditState -= Show;
        GameManager.Instance.ExitEditState -= Hide;
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
