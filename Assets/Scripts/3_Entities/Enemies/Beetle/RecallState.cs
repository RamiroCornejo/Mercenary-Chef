using UnityEngine;
using System;
using Tools.StateMachine;

public class RecallState : StatesFunctions
{
    float recallTime;
    Action OnTimerComplete;

    float timer;


    public RecallState(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, float _recallTime, Action _OnTimerComplete) : base(myState, _sm)
    {
        recallTime = _recallTime;
        OnTimerComplete = _OnTimerComplete;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
    }

    protected override void Exit(StateMachineInputs input)
    {
        timer = 0;
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        timer += Time.deltaTime;

        if(timer >= recallTime)
        {
            OnTimerComplete.Invoke();
            timer = 0;
        }
    }
}
