using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;
using System;

namespace Tools.StateMachine
{
    public abstract class StatesFunctions
    {
        protected EventStateMachine<StateMachineInputs> sm;
        protected EState<StateMachineInputs> state;

        public StatesFunctions(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm)
        {
            myState.OnEnter += Enter;

            myState.OnUpdate += Update;

            myState.OnLateUpdate += LateUpdate;

            myState.OnFixedUpdate += FixedUpdate;

            myState.OnExit += Exit;

            sm = _sm;
            state = myState;
        }

        protected abstract void Enter(EState<StateMachineInputs> lastState);

        protected abstract void Update();

        protected abstract void LateUpdate();

        protected abstract void FixedUpdate();

        protected abstract void Exit(StateMachineInputs input);

    }
}
