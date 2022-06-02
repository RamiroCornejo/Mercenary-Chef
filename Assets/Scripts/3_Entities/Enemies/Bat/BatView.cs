using UnityEngine;

public class BatView : MonoBehaviour
{
    [SerializeField] ParticleSystem despawnParticle = null;
    [SerializeField] Animator anim = null;
    [SerializeField] ParticleSystem hit_particle = null;
    [SerializeField] ParticleSystem hit_particle_spark = null;
    [SerializeField] ParticleSystem anticipationParticle = null;

    public void Initialize()
    {
        ParticlesManager.Instance.GetParticlePool(despawnParticle.name, despawnParticle);
        ParticlesManager.Instance.GetParticlePool(hit_particle_spark.name, hit_particle_spark);
        ParticlesManager.Instance.GetParticlePool(hit_particle.name, hit_particle);
        ParticlesManager.Instance.GetParticlePool(anticipationParticle.name, anticipationParticle);
    }

    public void DeadFX()
    {
        anim.Play("Dead");
    }

    public void DespawnFX()
    {
        ParticlesManager.Instance.PlayParticle(despawnParticle.name, transform.position);
    }

    public void PlayFlyAttack(bool b)
    {
        anim.SetBool("FlyAttack", b);
    }

    public void PlayBombAttack(bool b)
    {
        anim.SetBool("BombAttack", b);
    }

    public void PlayHit()
    {
        ParticlesManager.Instance.PlayParticle(hit_particle.name, this.transform.position);
        ParticlesManager.Instance.PlayParticle(hit_particle_spark.name, this.transform.position);
    }
    public void PlayAnticipationParticle()
    {
        ParticlesManager.Instance.PlayParticle(anticipationParticle.name, transform.position);
    }
}
