using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ModalButton : MonoBehaviour
{
    private TMP_Text textComponent;
    private Image image;
    private Action interact;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
        image = GetComponent<Image>();
    }
    public void OnInteract()
    {
        interact?.Invoke();
    }
    public void SetInteractAction(Action action)
    {
        interact = action;
    }
    public void SetColor(Color32 color)
    {
        image.color = ColorSystem.IsClear(color) ? ColorSystem.ColorPalette.Default : color;
    }
    public void SetText(string text)
    {
        textComponent.text = !string.IsNullOrEmpty(text) ? text : string.Empty;
    }
}
