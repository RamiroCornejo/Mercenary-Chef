using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDamage : MonoBehaviour
{
    [SerializeField] Damage dmg = new Damage();
    bool onCollision;

    [SerializeField] float duration = 5;
    [SerializeField] Rigidbody rb = null;
    [SerializeField] float torqueForce = 8;
    [SerializeField] RockTrap parent = null;

    float timer;


    public void Initialize(RockTrap _parent)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * torqueForce, ForceMode.Force);

        rb.mass = Random.Range(1, 10);

        parent = _parent;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= duration)
        {
            timer = 0;
            parent.ReturnRock(this);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (onCollision) return;
        if (collision.gameObject.GetComponent<RockDamage>()) return;

        var life = collision.gameObject.GetComponent<LifeComponent>();
        var receiver = collision.gameObject.GetComponent<EffectReceiver>();
        if (life != null)
            life.ReceiveDamage(dmg);
        if (receiver != null)
            receiver.TakeEffect(EffectName.OnStun);

        onCollision = true;
    }
}
