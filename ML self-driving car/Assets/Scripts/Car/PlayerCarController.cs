using UnityEngine;

public class PlayerCarController : MonoBehaviour, CarInput
{
    public float SteerInput => CarManager.Instance.BlockInput ? 0 : InputManager.Instance.InputX;
    public float ThrottleInput => CarManager.Instance.BlockInput ? 0 : InputManager.Instance.InputY;
    public bool GearUp => CarManager.Instance.BlockInput ? false : InputManager.Instance.IsVKeyDown;
    public bool GearDown => CarManager.Instance.BlockInput ? false : InputManager.Instance.IsCKeyDown;
    public bool Reverse => CarManager.Instance.BlockInput ? false : InputManager.Instance.IsRKeyDown;
}
