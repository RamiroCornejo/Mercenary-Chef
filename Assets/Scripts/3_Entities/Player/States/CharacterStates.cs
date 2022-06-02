using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public abstract class CharacterStates
{
    protected EventStateMachine<CharTransitions> sm;
    protected EState<CharTransitions> state;

    public CharacterStates(EState<CharTransitions> myState, EventStateMachine<CharTransitions> _sm)
    {
        myState.OnEnter += Enter;

        myState.OnUpdate += Update;

        myState.OnLateUpdate += LateUpdate;

        myState.OnFixedUpdate += FixedUpdate;

        myState.OnExit += Exit;

        sm = _sm;
        state = myState;
    }

    protected abstract void Enter(EState<CharTransitions> lastState);

    protected abstract void Update();

    protected abstract void LateUpdate();

    protected abstract void FixedUpdate();

    protected abstract void Exit(CharTransitions input);
}

public enum CharTransitions { Idle, Move, IdleRoto, Jump, Attack, DialogueStart, CinemataicStart, Dead, Throwing }
