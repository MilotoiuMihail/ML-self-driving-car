using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private TopDownCameraRig cameraRig;
    [SerializeField] private Transform startGate;
    [SerializeField] private SaveDataManager saveDataManager;
    private void OnEnable()
    {
        saveDataManager.MapLoaded += MoveToStartGate;
        GameManager.Instance.EnterEditState += MoveToStartGate;
        GameManager.Instance.EnterPlayState += SwitchTarget;
        GameManager.Instance.EnterViewState += SwitchTarget;
    }
    private void Start()
    {
        SwitchTarget();
    }
    private void OnDisable()
    {
        saveDataManager.MapLoaded -= MoveToStartGate;
        GameManager.Instance.EnterEditState -= MoveToStartGate;
        GameManager.Instance.EnterPlayState -= SwitchTarget;
        GameManager.Instance.EnterViewState -= SwitchTarget;
    }

    private void SwitchTarget()
    {
        cameraRig.SetTarget(CarManager.Instance.Car.transform);
    }
    private void MoveToStartGate()
    {
        if (startGate && startGate.gameObject.activeInHierarchy)
        {
            cameraRig.SetPosition(startGate.position);
        }
        else
        {
            cameraRig.MoveToMapCenter();
        }
    }
}
