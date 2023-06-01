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
    private bool isHeuristic;
    private Transform nextCheckpoint;
    private float totalDistanceToNextCheckpoint;
    private Car car;
    private void Start()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed += OnCorrectCheckpointPassed;
        CheckpointManager.Instance.WrongCheckpointPassed += OnWrongCheckpointPassed;
        isHeuristic = GetComponent<BehaviorParameters>().BehaviorType == BehaviorType.HeuristicOnly;
        car = GetComponent<Car>();
    }
    private void OnDestroy()
    {
        CheckpointManager.Instance.CorrectCheckpointPassed -= OnCorrectCheckpointPassed;
        CheckpointManager.Instance.WrongCheckpointPassed -= OnWrongCheckpointPassed;
    }
    public override void OnEpisodeBegin()
    {
        GameManager.Instance.ResetCarPosition(transform);
        CheckpointManager.Instance.ResetCheckpoint(transform);
        GetNextCheckpoint();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        float direction = Vector3.Dot(transform.forward, nextCheckpoint.forward);
        sensor.AddObservation(direction);
    }

    private void Update()
    {
        if (isHeuristic)
        {
            carInput.Reverse = Input.GetKeyDown(KeyCode.R);
        }
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
        if (car.GetSpeedKph() > 5f)
        {
            float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
            float checkpointReward = (totalDistanceToNextCheckpoint - distanceToCheckpoint) / totalDistanceToNextCheckpoint * .01f;
            AddReward(checkpointReward);
        }
        else
        {
            AddReward(-.01f);
        }
    }
    private void GetNextCheckpoint()
    {
        nextCheckpoint = CheckpointManager.Instance.GetNextCheckpoint(transform).transform;
        totalDistanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Wall"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }
    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.TryGetComponent<Ground>(out Ground ground))
    //     {
    //         AddReward(-0.1f);
    //     }
    // }
    private void OnCorrectCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Checkpoint Correct");
            AddReward(1f);
            GetNextCheckpoint();
        }
    }
    private void OnWrongCheckpointPassed(Transform carTransform)
    {
        if (carTransform == transform)
        {
            Debug.Log("Checkpoint Wrong");
            AddReward(-1f);
        }
    }
}
