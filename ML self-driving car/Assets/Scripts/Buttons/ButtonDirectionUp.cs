using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ButtonDirectionUp : MonoBehaviour
{
    [SerializeField] private Track track;
    // private Toggle toggle;
    private void Awake()
    {
        // toggle = GetComponent<Toggle>();
    }
    private void Start()
    {
        track.HasTrackDirectionChanged += SetToggle;
    }
    private void OnDestroy()
    {
        track.HasTrackDirectionChanged -= SetToggle;
    }
    private void SetToggle(bool isOn)
    {
        // toggle.isOn = isOn;
    }
}
