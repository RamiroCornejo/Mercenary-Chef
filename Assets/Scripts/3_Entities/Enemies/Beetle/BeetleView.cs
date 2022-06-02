using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleView : MonoBehaviour
{
    [SerializeField] Animator anim = null;
    [SerializeField] ParticleSystem despawnParticle = null;

    public void Initialize()
    {
        ParticlesManager.Instance.GetParticlePool(despawnParticle.name, despawnParticle);
    }

    public void DeadFX()
    {
        anim.Play("Death");
    }

    public void AttackAnticipation()
    {
        anim.Play("HornsUp");
    }

    public void Attack(bool b)
    {
        anim.SetBool("Attack", b);
    }

    public void RunFX()
    {
        anim.Play("AnticipationRush");
    }

    public void PlayIdle()
    {
        anim.Play("IdleNormal");
    }

    public void DespawnFX()
    {
        ParticlesManager.Instance.PlayParticle(despawnParticle.name, transform.position);
    }
}