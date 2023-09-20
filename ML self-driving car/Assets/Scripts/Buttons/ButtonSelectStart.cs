using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSelectStart : MonoBehaviour
{
    [SerializeField] private Track track;
    private Button button;
    private Color defaultColor;
    private void OnEnable()
    {
        InputManager.Instance.LeftControlDown += UnsetButtonPressed;
        track.SelectStartChanged += ChangeButtonColor;
    }
    private void OnDisable()
    {
        InputManager.Instance.LeftControlDown -= UnsetButtonPressed;
        track.SelectStartChanged -= ChangeButtonColor;
    }
    private void Awake()
    {
        button = GetComponent<Button>();
        defaultColor = button.image.color;
    }
    public void ToggleButtonPressed()
    {
        SetSelectStart(!track.SelectStart);
    }
    private void UnsetButtonPressed()
    {
        SetSelectStart(false);
    }
    private void SetSelectStart(bool buttonPressed)
    {
        track.SelectStart = buttonPressed;
    }
    private void ChangeButtonColor(bool buttonPressed)
    {
        if (buttonPressed)
        {
            button.image.color = defaultColor * button.colors.pressedColor;
        }
        else
        {
            button.image.color = defaultColor;
        }
    }
}
