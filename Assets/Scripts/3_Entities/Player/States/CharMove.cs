using System.Collections;
using System.Collections.Generic;
using Tools.StateMachine;
using UnityEngine;

public class CharMove : CharacterStates
{
    PlayerView view;
    PlayerMovementComp move;
    public CharMove(EState<CharTransitions> myState, EventStateMachine<CharTransitions> _sm, PlayerView _view, PlayerMovementComp _move) : base(myState, _sm)
    {
        view = _view;
        move = _move;
    }

    protected override void Enter(EState<CharTransitions> lastState)
    {
        view.ANIM_IsMoving(true);
    }

    protected override void Exit(CharTransitions input)
    {
        view.ANIM_IsMoving(false);
        view.ANIM_SetDirValue(0, 0);
    }

    protected override void FixedUpdate()
    {
        move.Move();
        view.ANIM_SetDirValue(Mathf.Clamp(move.InputDirection.magnitude, 0, 1), 0);
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        if (move.InputDirection == Vector3.zero)
            sm.SendInput(CharTransitions.Idle);
    }
}
