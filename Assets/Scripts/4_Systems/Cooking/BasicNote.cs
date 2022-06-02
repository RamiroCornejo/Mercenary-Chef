using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNote : Note
{
    [SerializeField] Renderer render = null;
    [SerializeField] ParticleSystem particle;

    private void Start()
    {
        ParticlesManager.Instance.GetParticlePool(particle.name, particle);
    }

    public override void SetColor(Mesh mesh, Material mat)
    {
        model.mesh = mesh;
        render.material = mat;
    }

    public override void GoodNote()
    {
        base.GoodNote();
        ParticlesManager.Instance.PlayParticle(particle.name, transform.position);
    }

    public override void PerfectNote()
    {
        base.PerfectNote();
        ParticlesManager.Instance.PlayParticle(particle.name, transform.position);
    }
}
