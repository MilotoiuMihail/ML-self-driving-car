using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text level;
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
        gameObject.SetActive(false);
    }
    public void Close()
    {
        CarManager.Instance.ChangeAgentTrainingLevel(Mathf.FloorToInt(slider.value));
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        isManualToggle.isOn = CarManager.Instance.GetPlayerPrefsGearbox();
        slider.value = CarManager.Instance.GetPlayerPrefsAgentLevel();
        OnValueChanged();
    }
    public bool IsVisible()
    {
        return gameObject.activeInHierarchy;
    }
    public void OnValueChanged()
    {
        switch (slider.value)
        {
            case 1:
                level.text = "Low";
                body.text = "Trained only on straight tracks.";
                break;
            case 2:
                level.text = "Medium";
                body.text = $"Trained on complex track layouts.{System.Environment.NewLine}The car has a slow speed.";
                break;
            case 3:
                level.text = "High";
                body.text = "Trained to avoid obstacles.";
                break;
            default:
                level.text = "None";
                body.text = "Dummy car. Random actions.";
                break;
        }
    }
    private void SetTitle(string text)
    {
        level.text = text;
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
