using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClamView : MonoBehaviour
{
    [SerializeField] Animator anim = null;
    [SerializeField] ParticleSystem explodeParticle = null;

    public void Initialize()
    {
        ParticlesManager.Instance.GetParticlePool(explodeParticle.name, explodeParticle);
    }

    public void HideFX()
    {
        anim.Play("Close");
    }

    public void UnhideFX()
    {
        anim.Play("Open");
    }

    public void DeadFX()
    {
        anim.Play("Death");
    }

    public void ExplodeFX()
    {
        ParticlesManager.Instance.PlayParticle(explodeParticle.name, transform.position);
    }
}
