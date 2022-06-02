using UnityEngine;
using Tools.StateMachine;
using System;

public class EnemyPersuit : StatesFunctions
{
    MovementHandler movement;
    Func<float> distanceToNear;
    Func<Transform> target;

    Action ToDoWhenGetTarget;

    public EnemyPersuit(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, MovementHandler _movement, Func<float> getMinDistance,
        Func<Transform> getTarget, Action _ToDoWhenGetTarget) : base(myState, _sm)
    {
        movement = _movement;
        distanceToNear = getMinDistance;
        target = getTarget;
        ToDoWhenGetTarget = _ToDoWhenGetTarget;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
    }

    protected override void Exit(StateMachineInputs input)
    {
        movement.Stop();
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        Vector3 dir = target.Invoke().position - movement.transform.position;
        float distance = dir.sqrMagnitude;

        movement.Rotate(dir.normalized);

        if (distance <= distanceToNear.Invoke() * distanceToNear.Invoke())
        {
            movement.Stop();
            ToDoWhenGetTarget.Invoke();
        }
        else
        {
            movement.Movement(target.Invoke().position);
        }
    }
}
