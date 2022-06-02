using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public abstract class EnemyBase : Live_Entity
{
    [SerializeField] protected Dropper dropper = null;
    protected EventStateMachine<StateMachineInputs> sm;
    [SerializeField] protected DamageReceiver dmgReciever = null;

    protected override void Start()
    {
        base.Start();
        dmgReciever.SetEvents(TakeDamageEvent, KnockBack, DeadEvent, Invulnerability);
        SetStateMachine();
    }

    protected virtual void Update()
    {
        sm.Update();
    }

    void TakeDamageEvent(float physicalDamage)
    {
        PhysicalDamage(physicalDamage);
        TakeDamageFeedback();
    }
  
    void HealEvent()
    {
        HealFeedback();
    }

    void DeadEvent(Damage dmg)
    {
        DeadFeedback(dmg);
    }

    protected virtual bool Invulnerability(Damage dmg)
    {
        return false;
    }

    protected virtual void TakeDamageFeedback() { }

    protected virtual void HealFeedback() { }

    protected virtual void DeadFeedback(Damage dmg) { }

    protected virtual void Dissappear()
    {
        dropper.Spawn();
        Destroy(this.gameObject);
    }

    protected abstract void SetStateMachine();
}

public enum StateMachineInputs { Idle, Patrol, Persuit, NormalAttack, SpecialAttack, GoToHole, GoToCarrot, Death, Recall, Anticipation }
