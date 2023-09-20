using DG.Tweening;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private const float MaxAlpha = .25f;
    private const float MinAlpha = .03f;
    private const float FadeDuration = .5f;
    public Vector3 InitialForward { get; private set; }
    private MeshRenderer meshRenderer;
    private Tween fadeTween;
    private void OnEnable()
    {
        GameManager.Instance.RaceStart += Hide;
        GameManager.Instance.ExitPlayState += Hide;
    }
    private void OnDisable()
    {
        GameManager.Instance.RaceStart -= Hide;
        GameManager.Instance.ExitPlayState -= Hide;
    }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public void LockForward()
    {
        InitialForward = transform.forward;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<Car>(out Car car))
        {
            CheckpointManager.Instance.PassedCheckpoint(car, this);
        }
    }
    public void Show()
    {
        if (fadeTween.IsActive())
        {
            return;
        }
        meshRenderer.enabled = true;
        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, MaxAlpha);
        fadeTween = meshRenderer.material.DOFade(MinAlpha, FadeDuration)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutQuad);
        CheckpointManager.Instance.ShowWrongWay();
    }
    public void Hide()
    {
        if (fadeTween.IsActive())
        {
            fadeTween.Kill();
        }
        meshRenderer.enabled = false;
        CheckpointManager.Instance.HideWrongWay();
    }
}
