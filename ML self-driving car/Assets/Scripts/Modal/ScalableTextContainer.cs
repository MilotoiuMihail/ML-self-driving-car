using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScalableTextContainer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterLimit;

    private void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        layoutElement = GetComponent<LayoutElement>();
    }
    private void OnValidate()
    {
        if (!textComponent || !layoutElement)
        {
            return;
        }
        SetScalability();
    }
    public void SetText(string text)
    {
        textComponent.text = !string.IsNullOrEmpty(text) ? text : string.Empty;
        SetScalability();
    }
    private bool hasLongText()
    {
        return textComponent.text.Length > characterLimit;
    }
    private void SetScalability()
    {
        layoutElement.enabled = hasLongText();
    }
}
