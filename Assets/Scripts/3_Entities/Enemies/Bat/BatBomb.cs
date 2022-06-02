using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatBomb : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion = null;
    [SerializeField] Rigidbody myRig = null;

    [SerializeField] LayerMask floorLayer = 1 << 0;
    [SerializeField] DamageDetector dmgDetector = null;

    public void SetOwner(LifeComponent lifeComp)
    {
        dmgDetector.owner = lifeComp;
    }

    private void Start()
    {
        ParticlesManager.Instance.GetParticlePool(explosion.name, explosion);
    }



    public void Shoot(Vector3 shoot_direction)
    {
        myRig.velocity = shoot_direction;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if ((floorLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Explode();
        }
    }


    public void Explode()
    {

        ParticlesManager.Instance.PlayParticle(explosion.name, this.transform.position);

        dmgDetector.DetecteEntitie(transform.position, transform.forward, QueryTriggerInteraction.Ignore);

        Destroy(this.gameObject);
    }
}
