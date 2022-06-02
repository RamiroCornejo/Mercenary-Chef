using System.Collections;
using System.Collections.Generic;
using Tools.StateMachine;
using UnityEngine;

public class CharJump : CharacterStates
{
    PlayerMovementComp dashComp;

    public CharJump(EState<CharTransitions> myState, EventStateMachine<CharTransitions> _sm, PlayerMovementComp _dashComp) : base(myState, _sm)
    {
        dashComp = _dashComp;
    }

    protected override void Enter(EState<CharTransitions> lastState)
    {
        dashComp.StartDash();
    }

    protected override void Exit(CharTransitions input)
    {
        dashComp.EndDash();
    }

    protected override void FixedUpdate()
    {
        dashComp.DashFixedUpdate();
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        if (!dashComp.DashUpdate())
            sm.SendInput(CharTransitions.Idle);
    }
}
