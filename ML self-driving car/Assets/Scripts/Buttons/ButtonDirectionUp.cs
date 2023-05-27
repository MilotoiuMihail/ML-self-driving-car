using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ButtonDirectionUp : MonoBehaviour
{
    [SerializeField] private Track track;
    private Toggle toggle;
    private void Start()
    {
        track.IsTrackDirectionClockwiseChanged += SetToggle;
    }
    private void OnDestroy()
    {
        track.IsTrackDirectionClockwiseChanged -= SetToggle;
    }
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }
    private void SetToggle(bool isOn)
    {
        toggle.isOn = isOn;
    }
}
