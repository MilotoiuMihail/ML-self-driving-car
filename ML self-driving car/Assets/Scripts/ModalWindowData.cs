using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ModalWindowData
{
    public string Title { get; set; }
    public string Body { get; set; }
    private ModalButtonData[] buttons;

    public ModalWindowData(int noButtons)
    {
        buttons = new ModalButtonData[noButtons];
        for (int i = 0; i < noButtons; i++)
        {
            buttons[i] = new ModalButtonData();
        }
    }
    public ModalButtonData[] GetButtons()
    {
        return buttons;
    }
    public void SetButton(int index, string text, Action buttonAction)
    {
        if (IsInRange(index))
        {
            buttons[index].Text = text;
            buttons[index].Action = buttonAction;
        }
        else
        {
            Debug.LogWarning("Button index out of range");
        }
    }
    public void SetButton(int index, string text, Action buttonAction, Color32 color)
    {
        if (IsInRange(index))
        {
            buttons[index].Color = color;
        }
        SetButton(index, text, buttonAction);
    }
    private bool IsInRange(int index)
    {
        return index >= 0 && index < buttons?.Length;
    }
}
