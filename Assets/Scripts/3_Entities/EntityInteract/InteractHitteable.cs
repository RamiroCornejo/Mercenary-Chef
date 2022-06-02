using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractHitteable : MonoBehaviour
{
    [SerializeField] DamageReceiver dmgReceiver = null;
    [SerializeField] LifeComponent lifeComponent = null;

    bool isDeath;

    protected virtual void Start()
    {
        dmgReceiver.SetEvents(Hit, (x, y) => { }, Death, (x) => false);
        dmgReceiver.InvulnerabilityFeedback += InvulnerableFeedback;
    }

    public void Hit(float damagePhysic)
    {
        lifeComponent.ReceiveDamage((int)damagePhysic);
        Debug.Log(lifeComponent.HP);
        OnHit();
    }

    public void Death(Damage dmg)
    {
        if (isDeath) return;

        OnDeath(dmg);
        isDeath = true;
    }

    protected abstract void InvulnerableFeedback(DamageType dmgType);

    protected abstract void OnHit();

    protected abstract void OnDeath(Damage dmg);
}
