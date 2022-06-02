using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectThrowable : MonoBehaviour
{
    public ParticleSystem explosion;
    public Rigidbody myRig;
    bool begin_cd;
    float timer;

    [SerializeField] float explosion_force = 20;
    [SerializeField] float explosion_radius = 5;

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
        var receiver = other.GetComponent<DamageReceiver>();
        if (receiver)
        {
            Explode();

            var objects = Physics.OverlapSphere(this.transform.position, explosion_radius);

            for (int i = 0; i < objects.Length; i++)
            {
                Damage damage = new Damage(10, () => this.transform.position, true, 10, DamageType.Normal);

                var damage_component = objects[i].GetComponent<DamageReceiver>();

                if (receiver)
                {
                    receiver.ReceiveDamage(damage);
                }
            }
            return;
        }

        begin_cd = true;
    }

    private void Update()
    {
        if (begin_cd)
        {
            if (timer < 3)
            {
                timer = timer + 1 * Time.deltaTime;
            }
            else
            {
                Explode();
                timer = 0;
            }
        }
    }

    public void Explode()
    {
       
        ParticlesManager.Instance.PlayParticle(explosion.name, this.transform.position);

        var cols = Physics.OverlapSphere(this.transform.position, explosion_radius);
        for (int i = 0; i < cols.Length; i++)
        {
            var rig = cols[i].GetComponent<Rigidbody>();
            if (rig)
            {
                Vector3 dir = rig.transform.position - this.transform.position;
                dir.Normalize();
                rig.AddForce(dir * explosion_force, ForceMode.VelocityChange);
            }

            var receiver = cols[i].GetComponent<DamageReceiver>();
            if (receiver)
            {
                Damage damage = new Damage(10, () => this.transform.position, true, 10, DamageType.Explosion);
                if (receiver)
                {
                    receiver.ReceiveDamage(damage);
                }
            }
        }

        Destroy(this.gameObject);
    }

    
}
