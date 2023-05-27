using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class ModalWindowSystem : Singleton<ModalWindowSystem>
{
    [SerializeField] private ModalWindow modalWindow;
    private ModalWindowData modalWindowData;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        modalWindowData = new ModalWindowData(modalWindow.GetNoButtons());
    }
    public ModalWindowSystem SetTitle(string text)
    {
        modalWindowData.Title = text;
        return this;
    }
    public ModalWindowSystem SetBody(string text)
    {
        modalWindowData.Body = text;
        return this;
    }
    public ModalWindowSystem SetButton(int index, string text, UnityAction buttonAction)
    {
        modalWindowData.SetButton(index, text, buttonAction);
        return this;
    }
    public ModalWindowSystem SetButton(int index, string text, UnityAction buttonAction, Color32 color)
    {
        modalWindowData.SetButton(index, text, buttonAction, color);
        return this;
    }
    public ModalWindowData BuildData()
    {
        ModalWindowData copy = modalWindowData;
        modalWindowData = new ModalWindowData(modalWindow.GetNoButtons());
        return copy;
    }
    public void Show(ModalWindowData data)
    {
        modalWindow.Show(data);
    }
}
