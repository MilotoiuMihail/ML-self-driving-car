using UnityEngine;

public class BarrierPlastic : Obstacle
{
    private Rigidbody rb;

    private void OnEnable()
    {
        GameManager.Instance.EnterEditState += DisableRigidbody;
        GameManager.Instance.ExitEditState += EnableRigidbody;
    }
    private void OnDisable()
    {
        GameManager.Instance.EnterEditState -= DisableRigidbody;
        GameManager.Instance.ExitEditState -= EnableRigidbody;
    }
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    private void EnableRigidbody()
    {
        rb.isKinematic = false;
    }
    private void DisableRigidbody()
    {
        rb.isKinematic = true;
    }
    public override void Place(Vector3 position)
    {
        base.Place(position);
        if (!GameManager.Instance.IsGameState(GameState.EDIT))
        {
            rb.isKinematic = false;
        }
    }
    protected override PlaceableItemType GetItemType()
    {
        return PlaceableItemType.ITEM1;
    }
}
