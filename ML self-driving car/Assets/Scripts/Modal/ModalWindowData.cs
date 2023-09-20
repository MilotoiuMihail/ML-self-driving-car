using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ModalWindowData
{
    public string Title;
    [TextArea]
    public string Body;
    [SerializeField] private ModalButtonData[] buttons;

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
    public void SetButton(int index, string text, UnityAction buttonAction)
    {
        if (IsInRange(index))
        {
            buttons[index].Text = text;
            buttons[index].ActionEvent.RemoveAllListeners();
            buttons[index].ActionEvent.AddListener(buttonAction);
        }
        else
        {
            Debug.LogWarning("Button index out of range");
        }
    }
    public void SetButton(int index, string text, UnityAction buttonAction, Color32 color)
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
