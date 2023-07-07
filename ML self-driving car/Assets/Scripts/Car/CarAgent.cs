using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class CarAgent : Agent
{
    [SerializeField] private CarInput carInput;
    // [SerializeField] private LayerMask layerMask;
    private bool isHeuristic;
    private Transform nextCheckpointTransform;
    private List<Checkpoint> nextCheckpoints = new List<Checkpoint>();
    private Car car;
    private Rigidbody rb;
    private Vector3 lastLocalVelocity;
    private Vector3 lastLocalAngularVelocity;
    private int baseCheckpointIndex;
    private bool hasManualGearBox;
    private float maxStepRatio;
    // private int wrongCheckpointsPassed;
    // private int lastWrongCheckpointIndex;
    [SerializeField] private bool hasRandomReset;
    private void Start()
    {
        GameManager.Instance.EnterViewState += GetNextCheckpointTransform;
        GameManager.Instance.EnterViewState += EnableCar;
        GameManager.Instance.ExitViewState += DisableCar;
        isHeuristic = GetComponent<BehaviorParameters>().BehaviorType == BehaviorType.HeuristicOnly;
        car = GetComponent<Car>();
        hasManualGearBox = car.HasManualGearBox;
        rb = GetComponent<Rigidbody>();
        maxStepRatio = 1f / MaxStep;
    }
    private void OnDestroy()
    {
        GameManager.Instance.EnterViewState -= GetNextCheckpointTransform;
        GameManager.Instance.EnterViewState -= EnableCar;
        GameManager.Instance.ExitViewState -= DisableCar;
    }
    protected async override void OnEnable()
    {
        base.OnEnable();
        await System.Threading.Tasks.Task.Yield();
        CheckpointManager.Instance.CorrectCheckpointPassed += OnCorrectCheckpointPassed;
        CheckpointManager.Instance.WrongCheckpointPassed += OnWrongCheckpointPassed;
        CheckpointManager.Instance.CompletedLap += OnCompletedLap;
        CheckpointManager.Instance.FinishedRace += OnFinishedRace;
        GetNextCheckpointTransform();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        CheckpointManager.Instance.CorrectCheckpointPassed -= OnCorrectCheckpointPassed;
        CheckpointManager.Instance.WrongCheckpointPassed -= OnWrongCheckpointPassed;
        CheckpointManager.Instance.CompletedLap -= OnCompletedLap;
        CheckpointManager.Instance.FinishedRace -= OnFinishedRace;
    }
    public override void OnEpisodeBegin()
    {
        SetReward(-0.25f);
        Reset(hasRandomReset);
        GetNextCheckpointTransform();
    }
    private void Update()
    {
        if (isHeuristic)
        {
            carInput.Reverse = Input.GetKeyDown(KeyCode.R);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // foreach (Checkpoint checkpoint in nextCheckpoints)
        // {
        //     float forwardAlignment = Vector3.Dot(transform.forward, checkpoint.transform.forward);
        //     sensor.AddObservation(forwardAlignment);
        //     float rightAlignment = Vector3.Dot(transform.right, checkpoint.transform.right);
        //     sensor.AddObservation(rightAlignment);
        // }
        float forwardAlignment = Vector3.Dot(transform.forward, nextCheckpointTransform.forward);
        sensor.AddObservation(forwardAlignment);
        float rightAlignment = Vector3.Dot(transform.right, nextCheckpointTransform.right);
        sensor.AddObservation(rightAlignment);
        float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpointTransform.position);
        sensor.AddObservation(distanceToCheckpoint);

        // sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(transform.localRotation);
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
        sensor.AddObservation(car.GetCurrentGearIndex());
        // sensor.AddObservation(hasManualGearBox);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = carInput.Reverse ? 1 : 0;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        carInput.SteerInput = actions.ContinuousActions[0];
        carInput.ThrottleInput = actions.ContinuousActions[1];
        carInput.Reverse = (actions.DiscreteActions[0] == 1);
        // if (car.GetSpeedKph() > 0f)
        // {
        //     DistanceToNextCheckpointReward();
        // }
        // else
        // {
        //     // AddReward(-.01f);
        // }
        // StayOnGroundReward();
    }
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (hasManualGearBox || car.GetCurrentGearIndex() > 1)
        {
            actionMask.SetActionEnabled(0, 1, false);
        }
    }
    private void OnCorrectCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            // Debug.Log("Checkpoint Correct");
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
        float nextCheckpointIndex = CheckpointManager.Instance.CheckpointTracker[transform];
        float checkpointsCount = CheckpointManager.Instance.GetCheckpointsCount();
        return (nextCheckpointIndex - baseCheckpointIndex) / (checkpointsCount - baseCheckpointIndex);
    }
    private void OnWrongCheckpointPassed(Transform carTransform, int passedCheckpointIndex)
    {
        if (carTransform == transform)
        {
            // wrongCheckpointsPassed += 1;
            // float passedCheckpointsRatio = (float)wrongCheckpointsPassed / (wrongCheckpointsPassed + CheckpointManager.Instance.CheckpointTracker[transform]);
            // Debug.Log("Checkpoint Wrong");
            // AddReward(0f - (1f - (float)StepCount / MaxStep) * passedCheckpointsRatio);
        }
    }
    private void OnCompletedLap(Transform carTransform)
    {
        if (carTransform == transform)
        {
            // Debug.Log("Completed Lap");
            SetReward(1f);
        }
    }
    private void OnFinishedRace(Transform carTransform)
    {
        if (carTransform == transform)
        {
            // Debug.Log("Finished Race");
            SetReward(1f);
            EndEpisode();
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
            // SetReward(-1f);
            // Debug.Log("Reward: " + GetCumulativeReward());
            // Debug.Log("Wall");
            AddReward(-.025f);
            EndEpisode();
        }

    }
    private void OnCollisionStay(Collision other)
    {
        if (!this.enabled)
        {
            return;
        }
        if (other.collider.tag.Equals("Obstacle"))
        {
            // Debug.Log("Obstacle");
            AddReward(-.001f);
        }
    }
    // private void StayOnGroundReward()
    // {
    //     RaycastHit hit;
    //     if (Physics.Raycast(transform.position, -transform.up, out hit, .5f, layerMask.value))
    //     {
    //         if (hit.transform.tag.Equals("Ground"))
    //         {
    //             AddReward(-0.1f);
    //         }
    //     }
    // }
    // private void DistanceToNextCheckpointReward()
    // {
    //     float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
    //     float checkpointReward = (totalDistanceToNextCheckpoint - distanceToCheckpoint) / totalDistanceToNextCheckpoint * .005f;
    //     AddReward(checkpointReward);
    // }
    private void GetNextCheckpointTransform()
    {
        // totalDistanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
        Checkpoint nextCheckpoint = CheckpointManager.Instance.GetNextCheckpoint(transform);
        nextCheckpointTransform = nextCheckpoint ? nextCheckpoint.transform : this.transform;
        // int length = 5;
        // nextCheckpoints = CheckpointManager.Instance.GetNextCheckpoints(transform, length);
        // Checkpoint lastCheckpoint = nextCheckpoints[nextCheckpoints.Count - 1];
        // for (int i = nextCheckpoints.Count; i < length; i++)
        // {
        //     nextCheckpoints.Add(lastCheckpoint);
        // }
    }
    private void Reset(bool randomReset)
    {
        Stop();
        ResetVariables();
        if (randomReset)
        {
            CarManager.Instance.RandomResetCar(transform);
            if (CheckpointManager.Instance.CheckpointTracker[transform] > 0)
            {
                CheckpointManager.Instance.LapTracker[transform] = 1;
            }
        }
        else
        {
            CarManager.Instance.ResetCar(transform);
        }
        baseCheckpointIndex = CheckpointManager.Instance.CheckpointTracker[transform];
    }
    private void ResetVariables()
    {
        lastLocalVelocity = GetLocalVelocity();
        lastLocalAngularVelocity = GetLocalAngularVelocity();
        // wrongCheckpointsPassed = 0;
        // lastWrongCheckpointIndex = 0;
        CheckpointManager.Instance.ResetProgress(transform);
    }
    private void Stop()
    {
        car.StopCompletely();
        carInput.ThrottleInput = 0;
        carInput.SteerInput = 0;
    }
    private void EnableCar()
    {
        this.enabled = true;
        Reset(false);
    }
    private void DisableCar()
    {
        Reset(false);
        this.enabled = false;
    }
}
