using System.Collections;
using System.Collections.Generic;
using Tools.StateMachine;
using UnityEngine;

public class CharCinematic : CharacterStates
{
    public CharCinematic(EState<CharTransitions> myState, EventStateMachine<CharTransitions> _sm) : base(myState, _sm)
    {
    }

    protected override void Enter(EState<CharTransitions> lastState)
    {
    }

    protected override void Exit(CharTransitions input)
    {
        Debug.Log("de donde salgo");
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        Debug.Log("en cinematicaaa");
    }
}
