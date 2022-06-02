using UnityEngine;
using Tools.StateMachine;
using System;

public class WingAttackState : StatesFunctions
{
    float anticipationTime;
    float attackTime;
    float backTime;
    float distanceToTarget;
    Rigidbody myRig;
    Func<Transform> GetTarget;
    float heightToElevate;
    DamageDetector damageDetector;
    Action EndAttack;
    BatView view;
    Action<Vector3> RotateTransform;

    float timer;
    Vector3 initPos;
    Vector3 posElevate;
    Vector3 attackPos;
    Vector3 finalPos;

    public WingAttackState(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, float _anticipationTime, float _attackTime, float _backTime,
                           float _heightToElevate, float _distanceToTarget, Rigidbody _myRig, Func<Transform> _GetTarget, DamageDetector _damageDetector, Action _EndAttack,
                           BatView _view, Action<Vector3> _RotateTransform) : base(myState, _sm)
    {
        anticipationTime = _anticipationTime;
        attackTime = _attackTime + anticipationTime;
        backTime = _backTime + attackTime;
        distanceToTarget = _distanceToTarget;
        myRig = _myRig;
        GetTarget = _GetTarget;
        heightToElevate = _heightToElevate;
        damageDetector = _damageDetector;
        EndAttack = _EndAttack;
        view = _view;
        RotateTransform = _RotateTransform;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
        myRig.useGravity = false;
        initPos = myRig.transform.position;
        posElevate = myRig.transform.position + Vector3.up * heightToElevate;
        view.PlayFlyAttack(true);
    }

    protected override void Exit(StateMachineInputs input)
    {
        myRig.useGravity = true;
        timer = 0;
        view.PlayFlyAttack(false);
    }

    protected override void FixedUpdate()
    {
    }

    protected override void LateUpdate()
    {
    }

    protected override void Update()
    {

        if (timer < anticipationTime)
        {
            Vector3 dir = (GetTarget().position - myRig.transform.position).normalized;

            RotateTransform(dir);

            timer += Time.deltaTime;

            myRig.transform.position = Vector3.Lerp(initPos, posElevate, timer / anticipationTime);

            if (timer >= anticipationTime)
            {
                attackPos = new Vector3(GetTarget().position.x, initPos.y, GetTarget().position.z);
                timer = anticipationTime;
            }
        }
        else if (timer < attackTime)
        {
            timer += Time.deltaTime;

            myRig.transform.position = Vector3.Lerp(posElevate, attackPos, (timer - anticipationTime) / (attackTime - anticipationTime));

            if (timer >= attackTime)
            {
                timer = attackTime;
                damageDetector.DetecteEntitie(myRig.transform.position, myRig.transform.forward, QueryTriggerInteraction.Ignore);
                Vector3 dir = (attackPos - initPos).normalized;

                if (dir == Vector3.zero) dir = myRig.transform.forward;

                finalPos = myRig.transform.position + dir * distanceToTarget;
            }
        }
        else if (timer < backTime)
        {
            timer += Time.deltaTime;

            myRig.transform.position = Vector3.Lerp(attackPos, finalPos, (timer - attackTime) / (backTime - attackTime));

            if (timer >= backTime)
            {
                EndAttack();
            }
        }
    }
}
