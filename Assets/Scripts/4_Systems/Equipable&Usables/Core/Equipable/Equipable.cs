using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MonoScripts.Core;
using UnityEngine.Events;

public abstract class Equipable : MonoBehaviour, IPausable, IResumeable
{
    [SerializeField] private new string name = "default";
    public string MyName => name;
    public string KeyName => this.gameObject.name + "::" + name;
    bool equiped = false;
    bool tick = false;
    [SerializeField] SpotType spotType;
    public SpotType Spot => spotType;
    public virtual void Start() { }
    public bool Equiped { get { return equiped; } }
    public virtual void Equip() { equiped = true; tick = true; }
    public virtual void UnEquip() { equiped = false; tick = false; }
    protected virtual void Update() { if (tick) OnUpdateEquipation(); }
    protected abstract void OnUpdateEquipation();
    public virtual void Pause() { tick = false; }
    public virtual void Resume() { tick = true; }

    [System.Serializable]
    public class Equipable_Unity_Events
    {
        [Header("States")]
        public UnityEvent EV_Equipable_Equip;
        public UnityEvent EV_Equipable_Unequip;
        public UnityEvent EV_Equipable_Update;
    }
}
