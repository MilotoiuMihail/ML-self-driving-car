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
    private Transform nextCheckpoint;
    // private float totalDistanceToNextCheckpoint;
    private Car car;
    private Rigidbody rb;
    private Vector3 lastLocalVelocity;
    private Vector3 lastLocalAngularVelocity;
    private int baseCheckpointIndex;
    private bool hasManualGearBox;
    // private int wrongCheckpointsPassed;
    // private int lastWrongCheckpointIndex;
    [SerializeField] private bool hasRandomReset;
    private void Start()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed += OnCorrectCheckpointPassed;
        CheckpointManager.Instance.WrongCheckpointPassed += OnWrongCheckpointPassed;
        CheckpointManager.Instance.CompletedLap += OnCompletedLap;
        CheckpointManager.Instance.FinishedRace += OnFinishedRace;
        GameManager.Instance.EnterViewState += GetNextCheckpoint;
        GameManager.Instance.EnterViewState += EnableCar;
        GameManager.Instance.ExitViewState += DisableCar;
        isHeuristic = GetComponent<BehaviorParameters>().BehaviorType == BehaviorType.HeuristicOnly;
        car = GetComponent<Car>();
        hasManualGearBox = car.HasManualGearBox;
        rb = GetComponent<Rigidbody>();
    }
    private void OnDestroy()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed -= OnCorrectCheckpointPassed;
        CheckpointManager.Instance.WrongCheckpointPassed -= OnWrongCheckpointPassed;
        CheckpointManager.Instance.CompletedLap -= OnCompletedLap;
        CheckpointManager.Instance.FinishedRace -= OnFinishedRace;
        GameManager.Instance.EnterViewState -= GetNextCheckpoint;
        GameManager.Instance.EnterViewState -= EnableCar;
        GameManager.Instance.ExitViewState -= DisableCar;
    }
    public override void OnEpisodeBegin()
    {
        SetReward(-0.05f);
        Reset(hasRandomReset);
        GetNextCheckpoint();
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
        float forwardAlignment = Vector3.Dot(transform.forward, nextCheckpoint.forward);
        sensor.AddObservation(forwardAlignment);
        float rightAlignment = Vector3.Dot(transform.right, nextCheckpoint.right);
        sensor.AddObservation(rightAlignment);
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
        sensor.AddObservation(hasManualGearBox);
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
        actionMask.SetActionEnabled(0, 1, false);
        if (hasManualGearBox || car.GetCurrentGearIndex() > 1)
        {
        }
    }
    private void OnCorrectCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Checkpoint Correct");
            AddReward((1f - (float)StepCount / MaxStep) * NormalizeCurrentCheckpoint());
            GetNextCheckpoint();
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
            Debug.Log("Checkpoint Wrong");
            // AddReward(0f - (1f - (float)StepCount / MaxStep) * passedCheckpointsRatio);
        }
    }
    private void OnCompletedLap(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Completed Lap");
            SetReward(1f);
        }
    }
    private void OnFinishedRace(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Finished Race");
            SetReward(1f);
            EndEpisode();
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
    private void GetNextCheckpoint()
    {
        nextCheckpoint = CheckpointManager.Instance.GetNextCheckpoint(transform).transform;
        // totalDistanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
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
    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled)
        {
            return;
        }
        if (other.tag.Equals("Wall"))
        {
            SetReward(-1f);
            Debug.Log("Reward: " + GetCumulativeReward());
            EndEpisode();
        }
    }
}
