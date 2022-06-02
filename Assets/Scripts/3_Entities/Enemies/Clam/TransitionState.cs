using System;
using Tools.StateMachine;

public class TransitionState : StatesFunctions
{
    Action TransitionEnter;
    Action TransitionExit;

    public TransitionState(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, Action _TransitionEnter, Action _TransitionExit) : base(myState, _sm)
    {
        TransitionEnter = _TransitionEnter;
        TransitionExit = _TransitionExit;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
        TransitionEnter.Invoke();
    }

    protected override void Exit(StateMachineInputs input)
    {
        TransitionExit.Invoke();
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
