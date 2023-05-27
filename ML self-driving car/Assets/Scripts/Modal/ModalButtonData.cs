using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[System.Serializable]
public class ModalButtonData
{
    public string Text;
    public Color32 Color;
    public UnityEvent ActionEvent = new UnityEvent();
}
