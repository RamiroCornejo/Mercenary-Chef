using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    bool openwindow = false;
    public void ANIM_EVENT_OpenAttackWindow()
    {
        openwindow = true;
        anim_attack_window = true;
        timer_attack_window = 0;
    }
    [SerializeField] float attack_window;
    [SerializeField] AnimEvent animevent = null;
    [SerializeField] PlayerView view = null;
    [SerializeField] PlayerDamageComponentByTrigger playerDamageComponent = null;
    [SerializeField] float cd_attack = 1.0f;
    CountDown CD_attack = new CountDown();
    float timer_attack_window;
    bool anim_attack_window;

    public bool IsAttacking { get; private set; }

    public void Initialize()
    {
        animevent.ADD_ANIM_EVENT_LISTENER("openwindow", ANIM_EVENT_OpenAttackWindow);
        animevent.ADD_ANIM_EVENT_LISTENER("begin_physical_hit", ANIM_EVENT_Begin_Attack);
        animevent.ADD_ANIM_EVENT_LISTENER("end_physical_hit", ANIM_EVENT_End_Attack);
        animevent.ADD_ANIM_EVENT_LISTENER("LastAttack", ANIM_EVENT_LastAttack);

        playerDamageComponent.AddOwnerTransform(this.transform);
        playerDamageComponent.Initialize(true);
        playerDamageComponent.Set_Callback_Victim_Position(Callback_Victim_Positions);
        playerDamageComponent.Set_CustomOnHit(UsableManager.instance.UseFirstHand);

        SMB_AttackBehaviour anim_sm_attack = view.MyAnimator.GetBehaviour<SMB_AttackBehaviour>();
        anim_sm_attack.SubscribeEnterAndExitState(ANIM_STATEMACHINE_OnEnter, ANIM_STATEMACHINE_OnExit);
        CD_attack = new CountDown(() => { }, cd_attack);
    }
    public void Update_WindowAttack()
    {
        if (anim_attack_window)
        {
            if (timer_attack_window < attack_window)
            {
                timer_attack_window = timer_attack_window + 1 * Time.deltaTime;
            }
            else
            {
                openwindow = false;
                view.ANIM_AttackInOpenWindow(false);
                IsAttacking = false;

                timer_attack_window = 0;
                anim_attack_window = false;
            }
        }
        playerDamageComponent.Tick(Time.deltaTime);
        CD_attack.Tick(Time.deltaTime);
    }

    public void DoAttack()
    {
        if (CD_attack.InCD) return;
        IsAttacking = true;

        if (!openwindow)
        {
            view.ANIM_DoAttackAttack();
        }
        else
        {
            view.ANIM_AttackInOpenWindow(true);
        }
    }

    public void ANIM_EVENT_Begin_Attack()
    {
        //el evento de anim attack empezó
        view.Play_Slash();
        playerDamageComponent.DoDamage();
    }
    public void ANIM_EVENT_End_Attack()
    {
        //el evento de anim attack termino
        IsAttacking = false;
        view.ANIM_AttackInOpenWindow(false);
        openwindow = false;
    }

    void Callback_Victim_Positions(Transform v)
    {
        view.Play_Hit(v.position + Vector3.up * 0.5f);
    }

    void ANIM_STATEMACHINE_OnEnter()
    {

    }
    void ANIM_STATEMACHINE_OnExit()
    {
        IsAttacking = false;
    }

    void ANIM_EVENT_LastAttack()
    {
        CD_attack.Start();
    }
}
