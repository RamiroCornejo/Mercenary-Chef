using UnityEngine;
using Tools.StateMachine;
using System;

public class BombAttackState : StatesFunctions
{
    float anticipationTime;
    float attackTime;
    float backTime;
    Rigidbody myRig;
    Func<Transform> GetTarget;
    float heightToElevate;
    float bombTargetDistance;
    Action EndAttack;
    Action DropBomb;
    BatView view;
    Action<Vector3> RotateTransform;

    float timer;
    Vector3 backPos;
    Vector3 posElevate;
    Vector3 attackPos;
    Vector3 initPos;
    bool dropBomb;

    public BombAttackState(EState<StateMachineInputs> myState, EventStateMachine<StateMachineInputs> _sm, float _anticipationTime, float _attackTime, float _backTime,
                           float _bombTargetDis, float _heightToElevate, Rigidbody _myRig, Func<Transform> _GetTarget, Action _DropBomb, Action _EndAttack, BatView _view,
                           Action<Vector3> _RotateTransform) : base(myState, _sm)
    {
        anticipationTime = _anticipationTime;
        attackTime = _attackTime + anticipationTime;
        backTime = _backTime + attackTime;
        bombTargetDistance = _bombTargetDis;
        myRig = _myRig;
        GetTarget = _GetTarget;
        heightToElevate = _heightToElevate;
        DropBomb = _DropBomb;

        EndAttack = _EndAttack;
        view = _view;
        RotateTransform = _RotateTransform;
    }

    protected override void Enter(EState<StateMachineInputs> lastState)
    {
        myRig.useGravity = false;
        initPos = myRig.transform.position;
        posElevate = myRig.transform.position + Vector3.up * heightToElevate;
        view.PlayBombAttack(true);
    }

    protected override void Exit(StateMachineInputs input)
    {
        myRig.useGravity = true;
        timer = 0;
        dropBomb = false;
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
                Vector3 targetDir = (GetTarget().position - myRig.transform.position).normalized;
                targetDir.y = 0;


                attackPos = myRig.transform.position + targetDir * bombTargetDistance * 4;
                timer = anticipationTime;
            }
        }
        else if (timer < attackTime)
        {
            timer += Time.deltaTime;

            float time = (timer - anticipationTime) / (attackTime - anticipationTime);

            myRig.transform.position = Vector3.Lerp(posElevate, attackPos, time);

            if(time >= 0.5f && !dropBomb)
            {
                DropBomb.Invoke();
                dropBomb = true;
            }

            if (timer >= attackTime)
            {
                timer = attackTime;
                backPos = myRig.transform.position - Vector3.up * heightToElevate;
                view.PlayBombAttack(false);
            }
        }
        else if (timer < backTime)
        {
            timer += Time.deltaTime;

            myRig.transform.position = Vector3.Lerp(attackPos, backPos, (timer - attackTime) / (backTime - attackTime));

            if (timer >= backTime)
            {
                EndAttack();
            }
        }
    }
}
