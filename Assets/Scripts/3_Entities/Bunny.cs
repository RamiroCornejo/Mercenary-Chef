using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bunny : Live_Entity
{
    Bunny_View view;
    AnimEvent anim_event;
    Dropper spawner;

    SimpleFlee basic_ai;

    protected override void Start()
    {
        base.Start();
        view = GetComponent<Bunny_View>();

        anim_event = GetComponentInChildren<AnimEvent>();

        anim_event.ADD_ANIM_EVENT_LISTENER("death_finish", ANIM_EVENT_OnDeathAnim_Finish);
        anim_event.ADD_ANIM_EVENT_LISTENER("despawn_finish", ANIM_EVENT_OnDespawnAnim_Finish);

        spawner = GetComponent<Dropper>();

        basic_ai = GetComponent<SimpleFlee>();
    }

    private void Update()
    {
        view.ANIM_Walk(basic_ai.IsMoving);
    }

    #region UNITY EVENTS & ANIMATION EVENTSa

    public void UEVENT_OnHit()
    {
        view.Play_HitParticle();
    }
    
    public void UEVENT_OnHeal()
    {

    }

    #region Death Sequence
    // SEC - 1
    public void UEVENT_OnDeath()
    {
        view.ANIM_Death(true);
        basic_ai.Stop();
    }
    // SEC - 2
    private void ANIM_EVENT_OnDeathAnim_Finish()
    {
        view.PlayDespawnParticle();
        view.ANIM_Death(false);
    }
    // SEC - 3
    private void ANIM_EVENT_OnDespawnAnim_Finish()
    {
        spawner.Spawn();
        Destroy(this.gameObject);
        //destroy o return to pool
    }
    #endregion

    #endregion
}
