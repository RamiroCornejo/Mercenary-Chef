using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponUsable : EquipedItem
{
    [SerializeField] int physical_damage;
    [SerializeField] bool hasKnokback;
    [SerializeField] float knockBackForce;
    [SerializeField] DamageType dmgType;

    [SerializeField] Damage damage;

    public override void Start()
    {
        base.Start();
        damage.SetOwnerPosition(CharacterPosition);
    }

    Vector3 CharacterPosition() => Character.instance.transform.position;

    public override void Equip()
    {
        base.Equip();
        OnEquip();
    }
    public override void UnEquip()
    {
        base.UnEquip();
        OnUnequip();
    }
    protected override void OnTick()
    {
        base.OnTick();
        Tick();
    }

    protected override void OnHit(UsableHitInfo<DamageReceiver> hit_info)
    {
        //este Hit info tiene toda la info que necesitamos
        //en este caso es del tipo DamageReceiver
        //a su vez DamageReciver es un Monovehaviour

        //asi que en esta funcion vamos a hacer toooodo
        //desde obtener el punto de golpe, hasta hacerle da�o al damage receiver

        base.OnHit(hit_info);

        if (hit_info.ContainSometing)
        {
            var receiver = hit_info.First;
            receiver.ReceiveDamage(damage);
            
            Debug.Log(receiver.name + " recibió " + damage.Physical_damage + " de daño físico con " + MyName);
            Hit(receiver);
        }
        else
        {
            Debug.Log(KeyName + " dando golpes al aire");
            Miss();
        }
    }

    protected abstract void Hit(DamageReceiver target);
    protected abstract void Miss();
    protected abstract void OnEquip();
    protected abstract void OnUnequip();
    protected abstract void Tick();


}
