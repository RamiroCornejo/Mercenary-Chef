using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumible_RGBPotion : Consumible
{
    public bool red, green, blue;
    protected override void OnExecute(int charges = 0)
    {
        SoundFX.Play_Potion_Consume();
        if (red) { Debug.Log("RED POTION");  return; }
        if (green) { Debug.Log("GREEN POTION"); return; }
        if (blue) { Debug.Log("BLUE POTION"); return; }
        
        base.OnExecute(charges);
    }
}
