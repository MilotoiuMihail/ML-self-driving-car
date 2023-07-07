using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCircle : MonoBehaviour
{
    private Renderer circleRenderer;
    private void Awake()
    {
        circleRenderer = GetComponent<Renderer>();
    }
    private void Start()
    {
        SetCheckCircleVisibility(false);
    }
    public void Show(Obstacle obstacle)
    {
        transform.localScale = new Vector3(1, 1, 1) * obstacle.CheckRadius * .22f;
        transform.parent = obstacle.transform;
        transform.localPosition = new Vector3(0, 0.1f, 0);
        SetCheckCircleVisibility(true);
    }
    private void SetCheckCircleVisibility(bool isVisibile)
    {
        circleRenderer.enabled = isVisibile;
    }
    public void Hide()
    {
        transform.parent = null;
        SetCheckCircleVisibility(false);
    }
}
