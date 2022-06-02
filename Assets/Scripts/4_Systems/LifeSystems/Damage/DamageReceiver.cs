using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Structs;
using Tools.EventClasses;
using System;

public class DamageReceiver : MonoBehaviour
{
    public EventFloat physical_damage;
    public EventKnockBack KnockBack;

    [SerializeField] List<DamageType> invulnerabilities = new List<DamageType>();
    Action<float> physicalDamageEvent = delegate { };
    Action<float, Vector3> knockBackEvent = delegate { };
    public Action<Damage> deathEvent = delegate { };
    Func<Damage, bool> IsInvulnerable;
    public Action<DamageType> InvulnerabilityFeedback = delegate { };
    Action<DamageReceiver> callback_remove_receiver = delegate { };


    [SerializeField] bool onlyVulnerableActive = false;
    [SerializeField] DamageType onlyVulnerableTo = DamageType.Normal;

    [SerializeField] LifeComponent lifecomponent = null;
    private void Start()
    {
        lifecomponent = GetComponent<LifeComponent>();
    }

    public void SetEvents(Action<float> _physicalDamageEvent, Action<float, Vector3> _knockBackEvent, Action<Damage> _deathEvent, Func<Damage,bool> _IsInvulnerable)
    {
        physicalDamageEvent = _physicalDamageEvent;
        knockBackEvent = _knockBackEvent;
        deathEvent += _deathEvent;
        IsInvulnerable = _IsInvulnerable;
    }

    public void ConfigureCallback_to_remove_me(Action<DamageReceiver> _callback_remove_receiver)
    {
        callback_remove_receiver = _callback_remove_receiver;
    }

    public void ReceiveDamage(Damage damage)
    {

        Debug.Log(damage.DamageType);
        if (invulnerabilities.Contains(damage.DamageType) || IsInvulnerable(damage)
            || onlyVulnerableActive && damage.DamageType == onlyVulnerableTo)
        {
            InvulnerabilityFeedback(damage.DamageType);
            return;
        }

        if (damage.Physical_damage > 0)
        {
            physical_damage.Invoke(damage.Physical_damage);
            physicalDamageEvent(damage.Physical_damage);
        }
        if (damage.HasKnokback)
        {
            KnockBack.Invoke(damage.KnockBackForce, damage.Owner_position);
            knockBackEvent(damage.KnockBackForce, damage.Owner_position);
        }

        if (lifecomponent.HP <= 0)
        {
            callback_remove_receiver.Invoke(this);
            deathEvent(damage);
        }
    }
}