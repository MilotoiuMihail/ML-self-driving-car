using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text body;
    [SerializeField] private Slider slider;
    [SerializeField] private Toggle isManualToggle;
    private void OnEnable()
    {
        InputManager.Instance.EscDown += Close;
    }
    private void OnDisable()
    {
        InputManager.Instance.EscDown -= Close;
    }
    private void Start()
    {
        Close();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        isManualToggle.isOn = CarManager.Instance.GetPlayerPrefsGearbox();
    }
    public bool IsVisible()
    {
        return gameObject.activeInHierarchy;
    }
    public void OnValueChanged()
    {
        switch (slider.value)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                Debug.LogWarning($"No Implementation for this value: {slider.value}");
                break;
        }
    }
    private void SetTitle(string text)
    {
        title.text = text;
    }
    private void SetBody(string text)
    {
        body.text = text;
    }
    public void ChangeGearbox(bool isManual)
    {
        CarManager.Instance.ChangePlayerCarGearbox(isManual);
    }
}
