using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LifeComponent))]
public class Live_Entity : Physical_Entity
{
    protected LifeComponent life;
    Damage_Feedback_PopUp ui_pop_up;

    protected override void Awake()
    {
        life = GetComponent<LifeComponent>();
        ui_pop_up = GetComponentInChildren<Damage_Feedback_PopUp>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void PhysicalDamage(float damage)
    {
        life.ReceiveDamage((int)damage);
        if(ui_pop_up) ui_pop_up.PopUp(damage);
    }
}
