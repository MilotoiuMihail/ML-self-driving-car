using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    [SerializeField] private CarInput carInput;
    public override void OnEpisodeBegin()
    {
        GameManager.Instance.ResetCarPosition();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        carInput.SteerInput = actions.ContinuousActions[0];
        carInput.ThrottleInput = actions.ContinuousActions[1];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ground>(out Ground ground))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}
