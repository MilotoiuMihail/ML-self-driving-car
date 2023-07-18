using UnityEngine;

public class ViewPanel : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.EnterViewState += Show;
        GameManager.Instance.ExitViewState += Hide;
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterViewState -= Show;
        GameManager.Instance.ExitViewState -= Hide;
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
