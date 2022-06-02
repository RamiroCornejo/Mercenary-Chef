using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum DamageType { Normal, Explosion, Cutting, Fire }
[System.Serializable]
public struct Damage
{
    [SerializeField] int physical_damage;
    [SerializeField] bool hasKnokback;
    [SerializeField] float knockBackForce;
    [SerializeField] Func<Vector3> ownerPosition;
    [SerializeField] DamageType dmgType;

    public Damage(int physical_damage, Func<Vector3> Owner_Position, bool hasKnokback, float knockBackForce, DamageType _dmgType)
    {
        this.physical_damage = physical_damage;
        this.ownerPosition = Owner_Position;
        this.hasKnokback = hasKnokback;
        this.knockBackForce = knockBackForce;
        dmgType = _dmgType;
    }

    public void SetDamage(int damage) => physical_damage = damage;
    public int Physical_damage { get => physical_damage; }
    public void SetOwnerPosition(Func<Vector3> _OwnerPosition) => ownerPosition = _OwnerPosition;
    public Vector3 Owner_position { get => ownerPosition.Invoke(); }
    public bool HasKnokback { get => hasKnokback; }
    public float KnockBackForce => knockBackForce;

    public DamageType DamageType => dmgType;

}


public struct DamageResult
{
    [SerializeField] bool damage_successful;
    [SerializeField] Vector3 hit_position;
    [SerializeField] bool has_resistance;
    [SerializeField] bool is_blocked;
    [SerializeField] bool is_parried;
    [SerializeField] bool is_reflected;

    public DamageResult(bool damage_successful, Vector3 hit_position, bool has_resistance, bool is_blocked, bool is_parried, bool is_reflected)
    {
        this.damage_successful = damage_successful;
        this.hit_position = hit_position;
        this.has_resistance = has_resistance;
        this.is_blocked = is_blocked;
        this.is_parried = is_parried;
        this.is_reflected = is_reflected;
    }

    public bool Damage_successful { get => damage_successful; }
    public Vector3 Hit_position { get => hit_position; }
    public bool Has_resistance { get => has_resistance; }
    public bool Is_blocked { get => is_blocked; }
    public bool Is_parried { get => is_parried; }
    public bool Is_reflected { get => is_reflected; }
}


