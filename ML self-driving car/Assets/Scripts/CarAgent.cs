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
    [SerializeField] private LayerMask layerMask;
    private bool isHeuristic;
    private Transform nextCheckpoint;
    private float totalDistanceToNextCheckpoint;
    private Car car;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;
    private int baseCheckpointIndex;
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
        rb = GetComponent<Rigidbody>();
        // baseCheckpointIndex = CheckpointManager.Instance.CheckpointTracker[transform];
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
        lastVelocity = rb.velocity;
        lastAngularVelocity = rb.angularVelocity;
        Reset();
        // RandomReset();
        CheckpointManager.Instance.ResetProgress(transform);
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
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        Vector3 velocity = rb.velocity;
        sensor.AddObservation(velocity);
        Vector3 acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
        sensor.AddObservation(acceleration);
        lastVelocity = velocity;
        Vector3 angularVelocity = rb.angularVelocity;
        sensor.AddObservation(angularVelocity);
        Vector3 angularAcceleration = (angularVelocity - lastAngularVelocity) / Time.fixedDeltaTime;
        sensor.AddObservation(angularAcceleration);
        lastAngularVelocity = angularVelocity;
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
    private void OnCorrectCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Checkpoint Correct");

            AddReward((1f - StepCount / MaxStep) * NormalizeCurrentCheckpoint());
            GetNextCheckpoint();
        }
    }
    private float NormalizeCurrentCheckpoint()
    {
        float currentCheckpointIndex = CheckpointManager.Instance.CheckpointTracker[transform];
        float checkpointsCount = CheckpointManager.Instance.GetCheckpointsCount();
        return (currentCheckpointIndex - baseCheckpointIndex) / (checkpointsCount - baseCheckpointIndex);
    }
    private void OnWrongCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Checkpoint Wrong");
            SetReward(-1f);
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
    private void StayOnGroundReward()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, .5f, layerMask.value))
        {
            if (hit.transform.tag.Equals("Ground"))
            {
                AddReward(-0.1f);
            }
        }
    }
    private void DistanceToNextCheckpointReward()
    {
        float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
        float checkpointReward = (totalDistanceToNextCheckpoint - distanceToCheckpoint) / totalDistanceToNextCheckpoint * .005f;
        AddReward(checkpointReward);
    }
    private void GetNextCheckpoint()
    {
        nextCheckpoint = CheckpointManager.Instance.GetNextCheckpoint(transform).transform;
        totalDistanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
    }
    private void Reset()
    {
        Stop();
        baseCheckpointIndex = 0;
        CarManager.Instance.ResetCar(transform);
    }
    private void RandomReset()
    {
        Stop();
        CarManager.Instance.RandomResetCar(transform);
        baseCheckpointIndex = CheckpointManager.Instance.CheckpointTracker[transform];
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
        Reset();
    }
    private void DisableCar()
    {
        Reset();
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
            EndEpisode();
        }
    }
}
