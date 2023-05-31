using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject[] cameras;
    private CameraInput input;
    private int currentIndex;

    void Awake()
    {
        cameras[0].SetActive(true);
        for (int i = 1; i < cameras.Length; i++)
            cameras[i].SetActive(false);
    }
    void Start()
    {
        input = GetComponent<CameraInput>();
    }

    void Update()
    {
        if (input.SwitchCamera) SwitchCameras();
    }

    private void SwitchCameras()
    {
        cameras[currentIndex].SetActive(false);
        currentIndex = ++currentIndex % cameras.Length;
        cameras[currentIndex].SetActive(true);
    }
}
