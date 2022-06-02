using System;
using UnityEngine;
using Tools.Structs;
using UnityEngine.Events;

public class LifeComponent : MonoBehaviour
{
    [Header("--- Living Entity Vars ---")]
    [SerializeField] int initial_hp = 100;
    [SerializeField] LifeSystemBase life;
    [SerializeField] protected FrontendStatBase ui_life;

    protected Func<int,bool> Protection = delegate { return false; };

    [SerializeField] protected UnityEvent EV_OnHit;
    [SerializeField] UnityEvent EV_OnHeal;
    [SerializeField] UnityEvent EV_OnDeath;

    public LifeSystemBase GetLifeSystemBase() => life;

    protected virtual void Start()
    {
        life = new LifeSystemBase(
            initial_hp,
            Feedback_ReceiveDamage,
            Feedback_OnHeal,
            OnDeath,
            ui_life,
            initial_hp);
    }

    public float HP { get => life.Life; }

    public bool Hit_Can_Kill_Him(int val) => life.CanThisBlowKillHim(val);

    public bool LifeIsFull => life.IsFull();

    public bool IsAlive => !life.IsEmpty();

    public void Heal(int val)
    {
        life.Heal(val);
    }
    public void Add_To_Max(int val)
    {
        life.IncreaseLife(val);
    }

    public void ReceiveDamage(int dmg = 5)
    {
        if (!Protection(dmg))
        {
            life.Hit(dmg);
        }
        
    }
    public void ReceiveDamage(Damage damage)
    {
        if (!Protection(damage.Physical_damage))
        {
            life.Hit(damage.Physical_damage);
        }
    }
    public void Resurrect()
    {
        life.ResetLife();
    }

    protected virtual void Feedback_ReceiveDamage()
    {
        EV_OnHit.Invoke();
    }

    protected virtual void Feedback_OnHeal()
    {
        EV_OnHeal.Invoke();
    }

    protected virtual void OnDeath()
    {
        EV_OnDeath.Invoke();
    }
}