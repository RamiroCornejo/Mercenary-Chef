using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropHitteable : InteractHitteable
{
    [SerializeField] Dropper spawner = null;
    [SerializeField] bool usePositions = true;
    [SerializeField] List<Transform> positionsToSpawn = new List<Transform>();
    [SerializeField] UnityEvent InvulnerablilityFeedback = new UnityEvent();
    [SerializeField] UnityEvent HitFeedback = new UnityEvent();
    [SerializeField] UnityEvent DeadFeedback = new UnityEvent();

    protected override void InvulnerableFeedback(DamageType dmgType)
    {
        InvulnerablilityFeedback.Invoke();
    }

    protected override void OnDeath(Damage dmg)
    {
        if (usePositions)
        {
            var temp = new List<Transform>();

            for (int i = 0; i < positionsToSpawn.Count; i++)
            {
                temp.Add(positionsToSpawn[i]);
            }

            spawner.SpawnWithPositions(temp);
        }
        else
            spawner.Spawn();

        DeadFeedback.Invoke();
    }

    protected override void OnHit()
    {
        HitFeedback.Invoke();
    }
}
