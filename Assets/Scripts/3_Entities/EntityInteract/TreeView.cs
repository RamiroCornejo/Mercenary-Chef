using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeView : MonoBehaviour
{
    [SerializeField] AudioClip hitSound = null;
    [SerializeField] ParticleSystem hitParticles = null;
    [SerializeField] Animator anim = null;
    [SerializeField] GameObject apples = null;

    void Start()
    {
        AudioManager.instance.GetSoundPool(hitSound.name, AudioManager.SoundDimesion.TwoD, hitSound);
        ParticlesManager.Instance.GetParticlePool(hitParticles.name, hitParticles);
    }

    public void Hit()
    {
        ParticlesManager.Instance.PlayParticle(hitParticles.name, transform.position);
        AudioManager.instance.PlaySound(hitSound.name);
        anim.Play("OnHit");
    }

    public void Dead()
    {
        apples.SetActive(false);
    }
}
