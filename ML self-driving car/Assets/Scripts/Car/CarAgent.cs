using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent, CarInput
{
    private Transform nextCheckpointTransform;
    public Car Car { get; private set; }
    private Rigidbody rb;
    private Vector3 lastLocalVelocity;
    private Vector3 lastLocalAngularVelocity;
    private int baseCheckpointIndex;
    private bool hasManualGearBox;
    private float maxStepRatio;
    [SerializeField] private bool hasRandomReset;
    [SerializeField] private bool hasRandomDirection;
    public float SteerInput { get; private set; }

    public float ThrottleInput { get; private set; }

    public bool GearUp => false;

    public bool GearDown => false;

    public bool Reverse { get; private set; }
    public bool IsBlocked { get; private set; }

    private void Awake()
    {
        Car = GetComponent<Car>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        GameManager.Instance.EnterViewState += Show;
        GameManager.Instance.ExitViewState += Hide;
        hasManualGearBox = Car.HasManualGearBox;
        maxStepRatio = MaxStep > 0 ? 1f / MaxStep : 1;
        CarManager.Instance.LoadNpcLevel();
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterViewState -= Show;
        GameManager.Instance.ExitViewState -= Hide;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        CheckpointManager.Instance.CorrectCheckpointPassed += OnCorrectCheckpointPassed;
        CheckpointManager.Instance.CompletedLap += OnCompletedLap;
        CheckpointManager.Instance.FinishedRace += OnFinishedRace;
        GameManager.Instance.ExitPlayState += EndEpisode;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        CheckpointManager.Instance.CorrectCheckpointPassed -= OnCorrectCheckpointPassed;
        CheckpointManager.Instance.CompletedLap -= OnCompletedLap;
        CheckpointManager.Instance.FinishedRace -= OnFinishedRace;
        GameManager.Instance.ExitPlayState -= EndEpisode;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        EndEpisode();
        gameObject.SetActive(false);
    }
    public override void OnEpisodeBegin()
    {
        SetReward(-0.25f);
        Reset(hasRandomReset);
        GetNextCheckpointTransform();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (nextCheckpointTransform != null)
        {

            float forwardAlignment = Vector3.Dot(transform.forward, nextCheckpointTransform.forward);
            sensor.AddObservation(forwardAlignment);
            float rightAlignment = Vector3.Dot(transform.right, nextCheckpointTransform.right);
            sensor.AddObservation(rightAlignment);
            float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpointTransform.position);
            sensor.AddObservation(distanceToCheckpoint);
        }

        Vector3 localVelocity = GetLocalVelocity();
        sensor.AddObservation(localVelocity);
        Vector3 localAcceleration = (localVelocity - lastLocalVelocity) / Time.fixedDeltaTime;
        sensor.AddObservation(localAcceleration);
        lastLocalVelocity = localVelocity;
        Vector3 localAngularVelocity = GetLocalAngularVelocity();
        sensor.AddObservation(localAngularVelocity);
        Vector3 localAngularAcceleration = (localAngularVelocity - lastLocalAngularVelocity) / Time.fixedDeltaTime;
        sensor.AddObservation(localAngularAcceleration);
        lastLocalAngularVelocity = localAngularVelocity;
        sensor.AddObservation(Car.GetCurrentGearIndex());
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = InputManager.Instance.InputX;
        continuousActions[1] = InputManager.Instance.InputY;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = InputManager.Instance.IsRKeyDown ? 1 : 0;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        SteerInput = IsBlocked ? 0 : actions.ContinuousActions[0];
        ThrottleInput = IsBlocked ? 0 : actions.ContinuousActions[1];
        Reverse = IsBlocked ? false : (actions.DiscreteActions[0] == 1);
    }
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        actionMask.SetActionEnabled(0, 1, false);
        if (hasManualGearBox || Car.GetCurrentGearIndex() > 1)
        {
        }
    }
    private void OnCorrectCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            AddReward((1f - (float)StepCount * maxStepRatio) * (1f - NormalizeCurrentCheckpoint()));
            GetNextCheckpointTransform();
        }
    }
    private Vector3 GetLocalVelocity()
    {
        return transform.InverseTransformDirection(rb.velocity);
    }
    private Vector3 GetLocalAngularVelocity()
    {
        return transform.InverseTransformDirection(rb.angularVelocity);
    }
    private float NormalizeCurrentCheckpoint()
    {
        float nextCheckpointIndex = CheckpointManager.Instance.Tracker[transform].NextCheckpointIndex;
        float checkpointsCount = CheckpointManager.Instance.GetCheckpointsCount();
        return (nextCheckpointIndex - baseCheckpointIndex) / (checkpointsCount - baseCheckpointIndex);
    }
    private void OnCompletedLap(Transform carTransform)
    {
        if (carTransform == transform)
        {
            SetReward(1f);
        }
    }
    private void OnFinishedRace(Transform carTransform)
    {
        if (carTransform == transform)
        {
            SetReward(1f);
            if (GameManager.Instance.IsGameState(GameState.PLAY) || GameManager.Instance.IsGameState(GameState.PAUSED) && GameManager.Instance.IsPreviousGameState(GameState.PLAY))
            {
                Car.FullStop();
                CheckpointManager.Instance.Tracker[transform].LapTimer.ResetTimer();
            }
            else
            {
                EndEpisode();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled)
        {
            return;
        }
        if (other.tag.Equals("Wall"))
        {
            AddReward(-.025f);
            EndEpisode();
        }

    }
    private void OnCollisionEnter(Collision other)
    {
        if (!this.enabled)
        {
            return;
        }
        if (other.collider.tag.Equals("Obstacle"))
        {
            AddReward(-.01f);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (!this.enabled)
        {
            return;
        }
        if (other.collider.tag.Equals("Obstacle"))
        {
            AddReward(.005f);
        }
    }
    private void GetNextCheckpointTransform()
    {
        Checkpoint nextCheckpoint = CheckpointManager.Instance.GetNextCheckpoint(transform);
        nextCheckpointTransform = nextCheckpoint ? nextCheckpoint.transform : this.transform;
    }
    private void Reset(bool randomReset)
    {
        ResetVariables();
        CarManager.Instance.ResetCar(Car, randomReset, hasRandomDirection);
        if (randomReset && CheckpointManager.Instance.Tracker[transform].NextCheckpointIndex > 0)
        {
            CheckpointManager.Instance.Tracker[transform].IncreaseLap();
        }
        //-----------------------------------------
        // transform.position += transform.right * Random.Range(-1f, 1f) * 6f;
        baseCheckpointIndex = CheckpointManager.Instance.Tracker[transform].NextCheckpointIndex;
    }
    private void ResetVariables()
    {
        lastLocalVelocity = GetLocalVelocity();
        lastLocalAngularVelocity = GetLocalAngularVelocity();
    }
    public void BlockInput()
    {
        IsBlocked = true;
    }
    public void UnblockInput()
    {
        IsBlocked = false;
    }
}
