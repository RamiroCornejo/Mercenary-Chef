using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.StateMachine;
using System.Linq;

public class Rabbit : PassiveEnemies
{
    [SerializeField] float holeRadious = 10;
    [SerializeField] float carrotRadious = 10;
    [SerializeField] float distanceToCarrot = 3;
    [SerializeField] float distanceToHole = 1;

    [SerializeField] DistanceSensor sensor = null;
    [SerializeField] RabbitView view = null;
    [SerializeField] MovementHandler movementHandler = null;
    [SerializeField] AnimEvent animEvent = null;

    bool inHole;

    protected override void Start()
    {
        base.Start();
        view.Initialize();
        sensor.SetTarget(Character.instance.transform);
        sensor.EnterToCombat = OnPlayerDistance;
        sensor.ExitToCombat = DontPlayerDistance;
        animEvent.ADD_ANIM_EVENT_LISTENER("death_finish", Dissappear);
    }

    protected override void Update()
    {
        base.Update();
        sensor.Refresh();
    }

    protected override void SetStateMachine()
    {
        var idle = new EState<StateMachineInputs>("Idle");
        var goToHole = new EState<StateMachineInputs>("GoToHole");
        var goToCarrot = new EState<StateMachineInputs>("GoToCarrot");
        var death = new EState<StateMachineInputs>("Death");

        ConfigureState.Create(idle)
            .SetTransition(StateMachineInputs.GoToHole, goToHole)
            .SetTransition(StateMachineInputs.GoToCarrot, goToCarrot)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(goToHole)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.GoToCarrot, goToCarrot)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(goToCarrot)
            .SetTransition(StateMachineInputs.Idle, idle)
            .SetTransition(StateMachineInputs.GoToHole, goToHole)
            .SetTransition(StateMachineInputs.Death, death)
            .Done();

        ConfigureState.Create(death)
            .Done();

        new EnemyIdle(idle, sm);
        new GoToPosState(goToHole, sm, movementHandler, GetHole, InTheHole, distanceToHole);
        new GoToPosState(goToCarrot, sm, movementHandler, GetCarrot, InTheCarrot, distanceToCarrot);
        new EnemyDeath(death, sm);

        sm = new EventStateMachine<StateMachineInputs>(goToCarrot);
    }

    void OnPlayerDistance()
    {
        view.ANIM_Walk(true);
        sm.SendInput(StateMachineInputs.GoToHole);
    }

    void DontPlayerDistance()
    {
        view.ANIM_Walk(true);
        inHole = false;
        sm.SendInput(StateMachineInputs.GoToCarrot);
    }

    protected override void TakeDamageFeedback()
    {
        base.TakeDamageFeedback();
        view.Play_HitParticle();
    }

    protected override void DeadFeedback(Damage dmg)
    {
        sm.SendInput(StateMachineInputs.Death);
        view.ANIM_Death(true);
        base.DeadFeedback(dmg);
    }
    protected override void Dissappear()
    {
        view.PlayDespawnParticle();
        base.Dissappear();
    }

    Transform GetHole()
    {
        Transform hole = null;

        var holes = Physics.OverlapSphere(transform.position, holeRadious).Where(x => x.GetComponent<BunnyCave>()).Select(x => x.transform).ToArray();

        if (holes.Length != 0) hole = holes[Random.Range(0, holes.Length)];
        else Debug.LogWarning("No tenés hoyos cerca banana");

        return hole;
    }

    Transform GetCarrot()
    {
        Transform carrot = null;

        var carrots = Physics.OverlapSphere(transform.position, carrotRadious).Where(x => x.GetComponent<BunnyCarrot>()).Select(x => x.transform).ToArray();

        if (carrots.Length != 0) carrot = carrots[Random.Range(0, carrots.Length)];
        else Debug.LogWarning("No tenés zanahorias cerca banana");

        return carrot;
    }

    void InTheHole()
    {
        inHole = true;
        view.ANIM_Walk(false);
        sm.SendInput(StateMachineInputs.Idle);
    }

    void InTheCarrot()
    {
        view.ANIM_Walk(false);
        sm.SendInput(StateMachineInputs.Idle);
    }

    protected override bool Invulnerability(Damage dmg)
    {
        return inHole;
    }
}
