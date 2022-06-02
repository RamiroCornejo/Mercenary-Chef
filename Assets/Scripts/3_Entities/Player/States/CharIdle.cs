using System.Collections;
using System.Collections.Generic;
using Tools.StateMachine;
using UnityEngine;

public class CharIdle : CharacterStates
{
    PlayerView view;
    PlayerMovementComp move;

    public CharIdle(EState<CharTransitions> myState, EventStateMachine<CharTransitions> _sm, PlayerView _view, PlayerMovementComp _move) : base(myState, _sm)
    {
        view = _view;
        move = _move;
    }

    protected override void Enter(EState<CharTransitions> lastState)
    {
        view.ANIM_IsMoving(false);
        view.ANIM_SetDirValue(0, 0);
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
        if (move.InputDirection != Vector3.zero)
            sm.SendInput(CharTransitions.Move);
    }
}
