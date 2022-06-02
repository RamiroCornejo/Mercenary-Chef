using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float rotateSpeed = 10;
    [SerializeField] NavMeshAgent agent = null;
    Action MovementFeedback;
    Action StopFeedback;

    float currentSpeed;

    private void Start()
    {
        agent.speed = speed;
        currentSpeed = speed;
    }

    public float GetCurrentSpeed() => currentSpeed;

    public void ChangeSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
        agent.speed = currentSpeed;
    }

    public void ResetSpeedToInit() => ChangeSpeed(speed);


    public void Movement(Vector3 pos)
    {
        if(agent.isStopped) agent.isStopped = false;
        agent.SetDestination(pos);
        MovementFeedback?.Invoke();
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        StopFeedback?.Invoke();
    }

    public void Rotate(Vector3 dirToRot)
    {
        agent.transform.forward = Vector3.Slerp(agent.transform.forward, dirToRot, rotateSpeed * Time.deltaTime);
    }
}
