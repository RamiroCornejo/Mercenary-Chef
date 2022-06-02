using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetector : SampleEntityDetector<LifeComponent>
{
    [SerializeField] int maxEnemiesToDamage = 3;

    [SerializeField] Damage dmg = new Damage();

    protected override void DoSomethingWithEntities(LifeComponent[] entities)
    {
        Debug.Log(entities.Length);
        int length = entities.Length < maxEnemiesToDamage ? entities.Length : maxEnemiesToDamage;


        for (int i = 0; i < length; i++)
        {
            entities[i].ReceiveDamage(dmg);
        }
    }

    protected override void NoEntitiesOnRange()
    {
    }
}
