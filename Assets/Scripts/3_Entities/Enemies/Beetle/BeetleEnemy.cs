using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public class BeetleEnemy : AgressiveEnemy
{
    [SerializeField] DistanceSensor distanceSensor = null;
    [SerializeField] MovementHandler movementHandler = null;
    [SerializeField] BeetleView view = null;
    [SerializeField] DamageDetector detector = null;
    [SerializeField] AnimEvent animEvent = null;

    [SerializeField] float distanceToAttack = 2;
    [SerializeField] float attackAnticipation = 2;
    [SerializeField] float recallTime = 2;

    protected override void Start()
    {
        view.Initialize();
        distanceSensor.SetTarget(Character.instance.transform);
        distanceSensor.EnterToCombat = EnterCombat;
        distanceSensor.ExitToCombat = ExitCombat;
        animEvent.ADD_ANIM_EVENT_LISTENER("DeadEvent", Dissappear);
        animEvent.ADD_ANIM_EVENT_LISTENER("AnticipationOver", RunAnticipationEvent);
        animEvent.ADD_ANIM_EVENT_LISTENER("AttackOver", AttackEnd);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        distanceSensor.Refresh();
        attackHandler.Refresh();
    }

    protected override bool CanDoNormalAttack() => sm.CanTransition(StateMachineInputs.NormalAttack) && distanceSensor.HasTargetInDistance(distanceToAttack);

    protected override bool CanDoSpecialAttack() => sm.CanTransition(StateMachineInputs.SpecialAttack) && distanceSensor.HasTargetInDistance(distanceToAttack);

    protected override void MakePossibleNormalAttack()
    {
    }

    protected override void MakePossibleSpecialAttack()
    {
    }

    protected override void SetStateMachine()
    {
        var idle = new EState<StateMachineInputs>("Idle");
        var persuitAnticipation = new EState<StateMachineInputs>("PersuitAnt");
        var persuit = new EState<StateMachineInputs>("Persuit");
        var attack = new EState<StateMachineInputs>("Attack");
        var recall = new EState<StateMachineInputs>("Recall");
        var death = new EState<StateMachineInputs>("Death");

        ConfigureState.Create(idle)
            .SetTransition(StateMachineInputs.Anticipation, persuitAnticipation)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(persuitAnticipation)
            .SetTransition(StateMachineInputs.Persuit, persuit)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(persuit)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.NormalAttack, attack)
            .SetTransition(StateMachineInputs.SpecialAttack, attack)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(attack)
            .SetTransition(StateMachineInputs.Recall, recall)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(recall)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.Anticipation, persuitAnticipation)
            .SetTransition(StateMachineInputs.NormalAttack, attack)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(death)
            .Done();

        new EnemyIdle(idle, sm);
        new TransitionState(persuitAnticipation, sm, () => { view.RunFX(); }, () => { });
        new EnemyPersuit(persuit, sm, movementHandler, () => distanceToAttack, distanceSensor.GetTarget, () => { });
        new BeetleAttackState(attack, sm, attackAnticipation, detector, view, movementHandler, distanceSensor.GetTarget);
        new RecallState(recall, sm, recallTime, OnEndRecall);
        new EnemyDeath(death, sm);

        sm = new EventStateMachine<StateMachineInputs>(idle);
    }
    
    void AttackEnd()
    {
        attackHandler.EndAttack();
        sm.SendInput(StateMachineInputs.Recall);
    }

    protected override void DeadFeedback(Damage dmg)
    {
        sm.SendInput(StateMachineInputs.Death);
        view.DeadFX();
        base.DeadFeedback(dmg);
    }

    void RunAnticipationEvent()
    {
        sm.SendInput(StateMachineInputs.Persuit);
    }

    void OnEndRecall()
    {
        if (distanceSensor.InCombat)
        {
            sm.SendInput(StateMachineInputs.Anticipation);
        }
        else
        {
            sm.SendInput(StateMachineInputs.Idle);
            view.PlayIdle();
        }
    }

    void EnterCombat()
    {
        sm.SendInput(StateMachineInputs.Anticipation);
    }

    void ExitCombat()
    {
        sm.SendInput(StateMachineInputs.Idle);
        view.PlayIdle();
    }

    protected override void Dissappear()
    {
        view.DespawnFX();
        base.Dissappear();
    }
}
