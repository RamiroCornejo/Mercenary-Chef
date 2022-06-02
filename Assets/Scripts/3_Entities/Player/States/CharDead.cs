using System.Collections;
using System.Collections.Generic;
using Tools.StateMachine;
using UnityEngine;

public class CharDead : CharacterStates
{
    public CharDead(EState<CharTransitions> myState, EventStateMachine<CharTransitions> _sm) : base(myState, _sm)
    {
    }

    protected override void Enter(EState<CharTransitions> lastState)
    {
    }

    protected override void Exit(CharTransitions input)
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
