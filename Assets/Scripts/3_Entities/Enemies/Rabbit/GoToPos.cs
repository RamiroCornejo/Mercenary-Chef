using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tools.StateMachine;

public class GoToPosState : StatesFunctions
{
    Func<Transform> GetTargetPos;
    Action ToDoWhenGetTarget;
    MovementHandler move;
    float distanceToPos;

    Transform currentTargetPos;

    public GoToPosState(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, MovementHandler _move, Func<Transform> _GetTargetPos,
                   Action _ToDoWhenGetTarget, float _distanceToPos) : base(myState, _sm)
    {
        move = _move;
        GetTargetPos = _GetTargetPos;
        ToDoWhenGetTarget = _ToDoWhenGetTarget;
        distanceToPos = _distanceToPos;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
        currentTargetPos = GetTargetPos();
    }

    protected override void Exit(StateMachineInputs input)
    {
        move.Stop();
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        if(currentTargetPos == null) { currentTargetPos = GetTargetPos(); return; }

        Vector3 dir = currentTargetPos.position - move.transform.position;
        float distance = dir.sqrMagnitude;

        move.Rotate(dir.normalized);


        if (distance <= distanceToPos * distanceToPos)
        {
            move.Stop();
            ToDoWhenGetTarget.Invoke();
        }
        else
        {
            move.Movement(currentTargetPos.position);
        }

    }
}
