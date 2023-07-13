using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{
    Rigidbody body;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if(transform.localPosition.y < 0)
        {
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        target.localPosition = new Vector3(Random.value * 8 -4, 0.5f, Random.value * 8 -4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(body.velocity.x);
        sensor.AddObservation(body.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        body.AddForce(controlSignal * forceMultiplier);

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        if(distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if(transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continousActionOut = actionsOut.ContinuousActions;
        continousActionOut[0] = Input.GetAxis("Horizontal");
        continousActionOut[1] = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
