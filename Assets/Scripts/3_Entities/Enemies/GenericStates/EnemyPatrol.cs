using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public class EnemyPatrol : StatesFunctions
{
    public EnemyPatrol(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm) : base(myState, _sm)
    {
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
    }

    protected override void Exit(StateMachineInputs input)
    {
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
    }
}
