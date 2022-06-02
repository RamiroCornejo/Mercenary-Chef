using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgressiveEnemy : EnemyBase
{
    [SerializeField] protected AttackHandler attackHandler = null;

    protected override void Start()
    {
        base.Start();

        attackHandler.Initialize(MakePossibleNormalAttack, DoNormalAttack, CanDoNormalAttack, MakePossibleSpecialAttack, DoSpecialAttack, CanDoSpecialAttack);
    }

    protected abstract void MakePossibleNormalAttack();

    protected virtual void DoNormalAttack() { Debug.Log("?"); sm.SendInput(StateMachineInputs.NormalAttack); }

    protected abstract bool CanDoNormalAttack();

    protected abstract void MakePossibleSpecialAttack();

    protected virtual void DoSpecialAttack() { sm.SendInput(StateMachineInputs.SpecialAttack); }

    protected abstract bool CanDoSpecialAttack();

    protected virtual void OnEnterCombat()
    {

    }

    protected virtual void OnExitCombat()
    {

    }
}
