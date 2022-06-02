using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitView : MonoBehaviour
{
    [SerializeField] ParticleSystem hit_particle = null;
    [SerializeField] ParticleSystem hit_particle_spark = null;
    [SerializeField] ParticleSystem despawn_particle = null;
    //[SerializeField] ParticleSystem hit_circle = null;
    [SerializeField] Animator myAnim = null;

    public void Initialize()
    {
        ParticlesManager.Instance.GetParticlePool(hit_particle.name, hit_particle);
        ParticlesManager.Instance.GetParticlePool(hit_particle_spark.name, hit_particle_spark);
        ParticlesManager.Instance.GetParticlePool(despawn_particle.name, despawn_particle);
    }

    public void ANIM_Death(bool val) => myAnim.SetBool("death", val);
    public void ANIM_Walk(bool val) => myAnim.SetBool("walk", val);
    public void Play_HitParticle() { ParticlesManager.Instance.PlayParticle(hit_particle.name, this.transform.position); ParticlesManager.Instance.PlayParticle(hit_particle_spark.name, this.transform.position); }
    public void PlayDespawnParticle() => ParticlesManager.Instance.PlayParticle(despawn_particle.name, this.transform.position);
}
