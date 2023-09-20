using UnityEngine;

public class ModalWindowTrigger : MonoBehaviour
{
    [SerializeField] private ModalWindowData modalWindowData;

    public void Trigger()
    {
        ModalWindowSystem.Instance.Show(modalWindowData);
    }
}
