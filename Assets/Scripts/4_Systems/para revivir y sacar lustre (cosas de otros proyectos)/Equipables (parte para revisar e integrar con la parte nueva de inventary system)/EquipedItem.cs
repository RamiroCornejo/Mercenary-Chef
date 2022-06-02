using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tools.EventClasses;
using System;

public class EquipedItem : Usable
{

    public Equipable_Unity_Events equipable_events;
    public Usable_Unity_Events usable_events;

    public Func<bool> CanUseCallback = () => true;

    ///////////////////////////////////////////////////////////
    /// E Q U I P A B L E
    ///////////////////////////////////////////////////////////
    public override void Equip()
    {
        base.Equip();
        equipable_events.EV_Equipable_Equip.Invoke();
    }
    public override void UnEquip()
    {
        base.UnEquip();
        equipable_events.EV_Equipable_Unequip.Invoke();
    }
    protected override void OnUpdateEquipation()
    {
        equipable_events.EV_Equipable_Update.Invoke();
    }

    ///////////////////////////////////////////////////////////
    /// U S A B L E
    ///////////////////////////////////////////////////////////
    protected override void OnPress() 
    {
        usable_events.EV_Usable_Press.Invoke();
    }
    protected override void OnRelease() 
    {
        usable_events.EV_Usable_Release.Invoke(); 
    }

    protected override void OnExecute(int charges = 0) 
    {
        //recibo cargas en el caso de que use el ChargeModule
        //esto esra para mandioca, cuando mantenias apretado el boton
        //de attack, mientras mas lo holdeabas mas poderoso era el Release

        usable_events.EV_Execute.Invoke(charges);
    }
    protected override void OnTick() { usable_events.EV_UpdateUse.Invoke(); }
    protected override bool OnCanUse() { return CanUseCallback(); }
}
