using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private TopDownCameraRig cameraRig;
    [SerializeField] private Transform startGate;
    [SerializeField] private SaveDataManager saveDataManager;
    private async void OnEnable()
    {
        await System.Threading.Tasks.Task.Yield();
        saveDataManager.MapLoaded += MoveToStartGate;
        GameManager.Instance.EnterEditState += MoveToStartGate;
    }
    private void OnDisable()
    {
        saveDataManager.MapLoaded -= MoveToStartGate;
        GameManager.Instance.EnterEditState -= MoveToStartGate;
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
