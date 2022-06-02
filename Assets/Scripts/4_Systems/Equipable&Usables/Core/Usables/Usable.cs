using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Tools.EventClasses;

public abstract class Usable : Equipable
{
    bool canUpdateUse = false;

    #region Modulos Opcionales
    CooldownModule cooldown = null;
    public CooldownModule CooldownModule => cooldown;
    NormalCastModule normal_cast = null;
    public NormalCastModule NormalCasting => normal_cast;
    ChargeModule charge_module = null;
    public ChargeModule CargeModule => charge_module;
    #endregion

    Action CallbackOnUse = delegate { };
    public void Subscribe_Callback_OnUse(Action _callback) => CallbackOnUse = _callback;

    ///////////////////////////////////////////////////////////////
    ///      U S E   F U N C T I O N S
    ///////////////////////////////////////////////////////////////
    public bool CanUse() 
    {
        bool cooldownActive = false;
        if (cooldown != null) cooldownActive = cooldown.IsRunning;
        return OnCanUse() && predicate.Invoke() && !cooldownActive; 
    }

    public void Basic_PressDown(bool force = false)
    {
        OnPress();

        if (force)
        {
            RealUse();
            return;
        }

        if (normal_cast != null && charge_module != null) throw new Exception("Hay dos modulos activos incompatibles entre si");

        if (normal_cast != null || charge_module != null)
        {
            
            if (normal_cast != null)
            {
                normal_cast.Subscribe_Sucess(RealUse);
                normal_cast.StartCast();
            }
            else
            {
                charge_module.Subscribe_Feedback_OnRelease(RealUse);
                charge_module.BeginPress();
            }
        }
        else
        {
            RealUse();
        }        
    }

    public void Basic_PressUp() 
    {
        OnRelease();

        if (normal_cast != null && charge_module != null) throw new Exception("Hay dos modulos activos incompatibles entre si");

        if (normal_cast != null || charge_module != null)
        {
            
            if (normal_cast != null)
                normal_cast.StopCast();
            else
                charge_module.StopPress();
        }
        
        canUpdateUse = false; 
    }

    public void Basic_CustomUse(UsableHitInfo<DamageReceiver> hit_info)
    {
        OnHit(hit_info);
    }

    void RealUse(int charges = 0)
    {
        if (cooldown != null) cooldown.StartCooldown();
        canUpdateUse = true;
        CallbackOnUse.Invoke();
        OnExecute(charges);
    }

    ///////////////////////////////////////////////////////////////
    ///      E Q U I P    F U N C T I O N S
    ///////////////////////////////////////////////////////////////
    public override void Equip()
    {
        base.Equip();
        cooldown = GetComponent<CooldownModule>();
        normal_cast = GetComponent<NormalCastModule>();
        charge_module = GetComponent<ChargeModule>();
    }
    public override void UnEquip()
    {
        base.UnEquip();
        if (cooldown != null)
        {
            cooldown.Stop();
        }
    }

    ///////////////////////////////////////////////////////////////
    ///      O T H E R S
    ///////////////////////////////////////////////////////////////
    #region abstracts
    protected abstract void OnPress();
    protected abstract void OnRelease();
    protected abstract void OnExecute(int charges);
    protected abstract void OnTick();
    protected abstract bool OnCanUse();
    #endregion
    protected virtual void OnHit(UsableHitInfo<DamageReceiver> hit_info)
    {

    }
    #region Update & LoopGame
    protected override void Update() { base.Update(); if (canUpdateUse) OnTick(); }
    public override void Pause() { base.Pause(); /*canUpdateUse = false;*/ }
    public override void Resume() { base.Resume(); /*canUpdateUse = true;*/ }
    #endregion
    #region Predicate
    #region PREDICADO OPCIONAL - 2 formas de usar - (Leeme)
    /*
    █████████████████████████████████████████████████████████████████████████████████████████████████████████████ <READ_START>
               PREDICADO OPCIONAL - 2 formas de usar - (Leeme) 
    * ¿para que sirve el predicado opcional? este predicado siempre va a estar devolviendole un true al AND
    * de CanUse, entonces desde otro lado, incluso desde editor le pasamos un predicado personalizado
    * con esta funcion "SetModelFunction(Func<bool> _predicate)"
    * por ejemplo, un modulo que me chequee que si tengo tanta salud no pueda usar este Item
    * 
    * En el caso de que se quiera hacer por Unity Events, dejé preparado en "using Tools.EventClasses"
    * Una clase "EventCounterPredicate", que justamente es un Event que hay que ejecutarlo desde un Start
    * y pasarle el predicado que queramos chekear Ej: 
    * 
    *       using Tools.EventClasses
    *       class MiClassZaraza : Monovehaviour 
    *       {
    *           public EventCounterPredicate MiUnityEvent;
    *           bool Mi_Predicado_Personalizado() => return !LifeSystem.Tengo_La_Salud_Llena; 
    *           void Start() => MiUnityEvent.Invoke(Mi_Predicado_Personalizado); 
    *       }
    * 
    * Luego desde editor se le asigna a este SetModelFunction "Func<bool>"... 
    * MUCHA ATENCION, tiene que ser "Dynamic Func" cuando lo asignes
    █████████████████████████████████████████████████████████████████████████████████████████████████████████████ <READ_END>
    */
    #endregion
    Func<bool> predicate = delegate { return true; };
    public void SetModelFunction(Func<bool> _predicate) => predicate = _predicate;
    #endregion

    public override string ToString()
    {
        return this.gameObject.name;
    }

    [System.Serializable]
    public class Usable_Unity_Events
    {
        [Header("States")]
        public EventInt EV_Execute;
        public UnityEvent EV_Usable_Press;
        public UnityEvent EV_Usable_Release;
        public UnityEvent EV_UpdateUse;
    }

    public struct UsableHitInfo<T> where T : Component
    {
        private Usable_World_Info world_info;
        private T[] elements;

        public UsableHitInfo(Usable_World_Info world_info, T[] elements)
        {
            this.world_info = world_info;
            this.elements = elements;
        }

        public Usable_World_Info World_info { get => world_info; }
        public T[] Objects { get => ContainSometing ? elements : null; }
        public T First { get => ContainSometing ? elements[0] : null; }
        public T Last { get => ContainSometing ? elements[elements.Length - 1] : null; }
        public bool ContainSometing { get => elements.Length > 0; }

        public struct Usable_World_Info
        {
            private Vector3 origin;
            private Vector3 destiny;
            private Vector3 normalized_Vdir;

            public Vector3 Origin { get => origin; }
            public Vector3 Destiny { get => destiny; }
            public Vector3 Normalized_Vdir { get => normalized_Vdir; }

            public Usable_World_Info(Vector3 _origin, Vector3 _destiny, Vector3 _normalized_Vdir)
            {
                this.origin = _origin;
                this.destiny = _destiny;
                this.normalized_Vdir = _normalized_Vdir;
            }
        }
    }
}
