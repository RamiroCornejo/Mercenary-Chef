using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumible_HeadachePotion : Consumible
{
    protected override void OnExecute(int charges = 0)
    {
        Character.instance.effectReceiver.RemoveToActive(EffectName.OnStun);
        SoundFX.Play_Potion_Consume();
        base.OnExecute(charges);
    }
}
