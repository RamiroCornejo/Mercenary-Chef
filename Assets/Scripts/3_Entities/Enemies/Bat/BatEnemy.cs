using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public class BatEnemy : AgressiveEnemy
{
    [SerializeField] DistanceSensor distanceSensor = null;
    [SerializeField] MovementHandler movementHandler = null;
    [SerializeField] float distanceToNormalAttack = 3;
    [SerializeField] AnimEvent animEvent = null;

    [Header("Wing Attack Params")]
    [SerializeField] float wingAnticipation = 1;
    [SerializeField] float wingAttackTime = 1;
    [SerializeField] float wingBackTime = 1;
    [SerializeField] float heightElevate = 3;
    [SerializeField] DamageDetector wingDmg = null;

    [Header("Bomb Attack Params")]
    [SerializeField] float bombAnticipation = 1;
    [SerializeField] float bombAttackTime = 1;
    [SerializeField] float bombBackTime = 1;
    [SerializeField] float bombHeightElevate = 3;
    [SerializeField] float bombDropDistance = 3;
    [SerializeField] BatBomb bombPrefab = null;
    [SerializeField] Transform posToDropBomb = null;

    [SerializeField] BatView view = null;

    protected override void Start()
    {
        view.Initialize();
        base.Start();
        distanceSensor.SetTarget(Character.instance.transform);
        distanceSensor.EnterToCombat = OnEnterCombat;
        distanceSensor.ExitToCombat = OnExitCombat;
        animEvent.ADD_ANIM_EVENT_LISTENER("DeadEvent", Dissappear);
        animEvent.ADD_ANIM_EVENT_LISTENER("AntParticle", view.PlayAnticipationParticle);
    }

    protected override void Update()
    {
        base.Update();

        distanceSensor.Refresh();
        if (distanceSensor.InCombat) attackHandler.Refresh();
    }

    protected override bool CanDoNormalAttack() => distanceSensor.HasTargetInDistance(distanceToNormalAttack);

    protected override bool CanDoSpecialAttack() => distanceSensor.InCombat;

    protected override void MakePossibleNormalAttack()
    {
    }

    protected override void MakePossibleSpecialAttack()
    {
    }

    protected override void SetStateMachine()
    {
        var idle = new EState<StateMachineInputs>("Idle");
        var persuit = new EState<StateMachineInputs>("Persuit");
        var wingAttack = new EState<StateMachineInputs>("WingAttack");
        var bombAttack = new EState<StateMachineInputs>("BombAttack");
        var death = new EState<StateMachineInputs>("Death");

        ConfigureState.Create(idle)
            .SetTransition(StateMachineInputs.Persuit, persuit)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(persuit)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.NormalAttack, wingAttack)
            .SetTransition(StateMachineInputs.SpecialAttack, bombAttack)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(wingAttack)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.Persuit, persuit)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(bombAttack)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.Persuit, persuit)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(death)
            .Done();

        new EnemyIdle(idle, sm);
        new EnemyPersuit(persuit, sm, movementHandler, () => distanceToNormalAttack, distanceSensor.GetTarget, () => { });
        new WingAttackState(wingAttack, sm, wingAnticipation, wingAttackTime, wingBackTime, heightElevate, distanceToNormalAttack,
                            MyRig, distanceSensor.GetTarget, wingDmg, EndAttack, view, movementHandler.Rotate);
        new BombAttackState(bombAttack, sm, bombAnticipation, bombAttackTime, bombBackTime, bombDropDistance, bombHeightElevate,
                            MyRig, distanceSensor.GetTarget, DropBomb, EndAttack, view, movementHandler.Rotate);
        new EnemyDeath(death, sm);

        sm = new EventStateMachine<StateMachineInputs>(idle);
    }

    void DropBomb()
    {
        var bomb = Instantiate(bombPrefab, posToDropBomb.position, posToDropBomb.rotation);
        bomb.SetOwner(life);
    }

    protected override void OnEnterCombat()
    {
        base.OnEnterCombat();

        sm.SendInput(StateMachineInputs.Persuit);
    }

    protected override void OnExitCombat()
    {
        base.OnEnterCombat();
        attackHandler.ResetAttack();
        sm.SendInput(StateMachineInputs.Idle);
    }

    void EndAttack()
    {
        attackHandler.EndAttack();

        if (distanceSensor.InCombat) sm.SendInput(StateMachineInputs.Persuit);
        else sm.SendInput(StateMachineInputs.Idle);
    }

    protected override void TakeDamageFeedback()
    {
        base.TakeDamageFeedback();
        view.PlayHit();
    }

    protected override void DeadFeedback(Damage dmg)
    {
        sm.SendInput(StateMachineInputs.Death);
        view.DeadFX();
        base.DeadFeedback(dmg);
    }

    protected override void Dissappear()
    {
        view.DespawnFX();
        base.Dissappear();
    }
}
