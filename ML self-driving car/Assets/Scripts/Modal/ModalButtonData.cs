using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ModalButtonData
{
    public string Text;
    public Color32 Color;
    public UnityEvent ActionEvent = new UnityEvent();
}
