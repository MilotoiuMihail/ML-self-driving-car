using UnityEngine;

public class PlayerCarController : MonoBehaviour, CarInput
{
    public float SteerInput => IsBlocked ? 0 : InputManager.Instance.InputX;
    public float ThrottleInput => IsBlocked ? 0 : InputManager.Instance.InputY;
    public bool GearUp => IsBlocked ? false : InputManager.Instance.IsVKeyDown;
    public bool GearDown => IsBlocked ? false : InputManager.Instance.IsCKeyDown;
    public bool Reverse => IsBlocked ? false : InputManager.Instance.IsRKeyDown;
    public bool IsBlocked { get; private set; }
    private Car car;
    private void OnEnable()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed += HandleCorrectCheckpoint;
        CheckpointManager.Instance.WrongCheckpointPassed += HandleWrongCheckpoint;
    }
    private void OnDisable()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed -= HandleCorrectCheckpoint;
        CheckpointManager.Instance.WrongCheckpointPassed -= HandleWrongCheckpoint;
    }
    private void Awake()
    {
        car = GetComponent<Car>();
    }
    private void Update()
    {
        if (!IsBlocked && !car.IsOnTrack())
        {
            CheckpointManager.Instance.GetCurrentCheckpoint(transform).Show();
        }
    }
    public void BlockInput()
    {
        IsBlocked = true;
    }
    public void UnblockInput()
    {
        IsBlocked = false;
    }
    private void HandleCorrectCheckpoint(Transform carTransform)
    {
        if (carTransform != transform)
        {
            return;
        }
        CheckpointManager.Instance.GetCurrentCheckpoint(transform).Hide();
    }
    private void HandleWrongCheckpoint(Transform carTransform)
    {
        if (carTransform != transform)
        {
            return;
        }
        CheckpointManager.Instance.GetCurrentCheckpoint(transform).Show();
    }

}
