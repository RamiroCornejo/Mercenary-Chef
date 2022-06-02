using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopTools;

public class RockPool : SingleObjectPool<RockDamage>
{
    [SerializeField] private RockDamage particle;
    public bool soundPoolPlaying = false;

    public void Configure(RockDamage _particle)
    {
        extendible = true;
        particle = _particle;
    }

    protected override void AddObject(int prewarm = 3)
    {
        var newParticle = Instantiate(particle);
        newParticle.gameObject.SetActive(false);
        newParticle.transform.SetParent(transform);
        objects.Enqueue(newParticle);
    }

    public void ReturnParticle(RockDamage particle)
    {
        if (particle == null) return;

        particle.transform.SetParent(transform);
        ReturnToPool(particle);
    }
}
