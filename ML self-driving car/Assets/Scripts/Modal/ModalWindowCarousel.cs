using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalWindowCarousel : MonoBehaviour
{
    [SerializeField] private List<ModalWindowCarouselData> carousel = new List<ModalWindowCarouselData>();
    [SerializeField] private ModalButtonData button1;
    [SerializeField] private ModalButtonData button2;
    private int currentIndex = 0;
    public void Trigger()
    {
        currentIndex = 0;
        ShowWindowAtIndex(currentIndex);
    }
    private void ShowWindowAtIndex(int index)
    {
        if (index < 0 || index >= carousel.Count)
        {
            return;
        }
        ModalWindowData data = ModalWindowSystem.Instance
        .SetTitle(carousel[index].Title)
        .SetBody(carousel[index].Body)
        .SetButton(0, "Back", BackButtonAction, ColorSystem.ColorPalette.Warning)
        .SetButton(1, "Next", NextButtonAction, ColorSystem.ColorPalette.Success)
        .BuildData();
        if (index == carousel.Count - 1)
        {
            data.SetButton(1, button1.Text, () => button1.ActionEvent?.Invoke(), button1.Color);
            data.SetButton(2, button2.Text, () => button2.ActionEvent?.Invoke(), button2.Color);
        }
        else if (index == 0)
        {
            data.SetButton(0, "", () => { });
        }
        ModalWindowSystem.Instance.Show(data);

    }
    private void BackButtonAction()
    {
        currentIndex--;
        ShowWindowAtIndex(currentIndex);
    }

    private void NextButtonAction()
    {
        currentIndex++;
        ShowWindowAtIndex(currentIndex);
    }
}
