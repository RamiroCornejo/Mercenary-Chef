using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;

public class Clam : PassiveEnemies
{
    [SerializeField] DistanceSensor distanceSensor = null;
    [SerializeField] BombReceiver bombReceiver = null;
    [SerializeField] ClamView view = null;
    [SerializeField] AnimEvent animEvent = null;
    [SerializeField] float timeToExplode = 2;
    [SerializeField] Collider myCollider = null;

    [SerializeField] float vulnerableDistance = 0.5f;

    bool hiding;

    protected override void Start()
    {
        distanceSensor.EnterToCombat = OnTargetNear;
        distanceSensor.ExitToCombat = DontTargetNear;
        distanceSensor.SetTarget(Character.instance.transform);
        view.Initialize();
        animEvent.ADD_ANIM_EVENT_LISTENER("DeadFinish", Dissappear);
        bombReceiver.OnGetBomb = GetBomb;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        distanceSensor.Refresh();
    }

    protected override void SetStateMachine()
    {
        var idle = new EState<StateMachineInputs>("Idle");
        var hide = new EState<StateMachineInputs>("Hide");
        var swallowBomb = new EState<StateMachineInputs>("SwallowBomb");
        var death = new EState<StateMachineInputs>("Death");

        ConfigureState.Create(idle)
            .SetTransition(StateMachineInputs.GoToHole, hide)
            .SetTransition(StateMachineInputs.Recall, swallowBomb)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(hide)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(swallowBomb)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(death)
            .Done();

        new TransitionState(idle, sm, GoToIdle, ()=> { });
        new TransitionState(hide, sm, GoToHide, ()=> { });
        new RecallState(swallowBomb, sm, timeToExplode, Explode);
        new EnemyDeath(death, sm);

        sm = new EventStateMachine<StateMachineInputs>(idle);
    }

    void OnTargetNear()
    {
        sm.SendInput(StateMachineInputs.GoToHole);
    }

    void DontTargetNear()
    {
        sm.SendInput(StateMachineInputs.Idle);
    }

    void GoToIdle()
    {
        hiding = false;
        bombReceiver.gameObject.SetActive(true);
        view.UnhideFX();
        myCollider.enabled = false;
    }

    void GoToHide()
    {
        hiding = true;
        bombReceiver.gameObject.SetActive(false);
        view.HideFX();
        myCollider.enabled = false;
    }

    void Explode()
    {
        dmgReciever.ReceiveDamage(new Damage(10000, () => bombReceiver.transform.position, false, 2, DamageType.Explosion));
        view.ExplodeFX();
    }

    void GetBomb()
    {
        sm.SendInput(StateMachineInputs.Recall);
        bombReceiver.gameObject.SetActive(false);
        view.HideFX();
    }

    protected override bool Invulnerability(Damage dmg)
    {
        return hiding;
    }

    protected override void TakeDamageFeedback()
    {
        base.TakeDamageFeedback();
    }

    protected override void DeadFeedback(Damage dmg)
    {
        sm.SendInput(StateMachineInputs.Death);
        view.DeadFX();
        base.DeadFeedback(dmg);
    }
    protected override void Dissappear()
    {
        base.Dissappear();
    }
}
