using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class ModalButton : MonoBehaviour
{
    private TMP_Text textComponent;
    private Image image;
    private UnityEvent interactEvent = new UnityEvent();

    private void Awake()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
        image = GetComponent<Image>();
    }
    public void OnInteract()
    {
        interactEvent?.Invoke();
    }
    public void SetInteractAction(UnityEvent action)
    {
        interactEvent.RemoveAllListeners();
        interactEvent.AddListener(action.Invoke);
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
