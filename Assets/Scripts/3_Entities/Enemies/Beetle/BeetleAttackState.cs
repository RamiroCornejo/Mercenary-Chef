using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;
using System;

public class BeetleAttackState : StatesFunctions
{
    float anticipationTime;

    DamageDetector dmgDetector;
    Transform ownerTransform;
    BeetleView view;
    MovementHandler movementHandler;
    Func<Transform> target;

    float timer;
    bool attack;

    public BeetleAttackState(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, float _anticipationTime, DamageDetector _dmgDetector,
                             BeetleView _view, MovementHandler _movementHandler, Func<Transform> _target) : base(myState, _sm)
    {
        anticipationTime = _anticipationTime;
        dmgDetector = _dmgDetector;
        view = _view;
        movementHandler = _movementHandler;
        target = _target;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
        view.AttackAnticipation();
    }

    protected override void Exit(StateMachineInputs input)
    {
        timer = 0;
        attack = false;
        view.Attack(false);
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {
        Vector3 dir = target.Invoke().position - movementHandler.transform.position;
        movementHandler.Rotate(dir.normalized);
        if(timer < anticipationTime)
        {
            timer += Time.deltaTime;

            if (timer >= anticipationTime)
            {
                view.Attack(true);
                dmgDetector.DetecteEntitie(movementHandler.transform.position, movementHandler.transform.forward, QueryTriggerInteraction.Ignore);
                attack = true;
            }
        }
    }
}
