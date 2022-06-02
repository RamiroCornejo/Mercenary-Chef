using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tools.Structs;
using System.Linq;

public class PlayerDamageComponentByTrigger : PlayerComponent
{
    HashSet<DamageReceiver> elems = new HashSet<DamageReceiver>();
    public LayerMask layers_to_damage;

    [SerializeField] Damage damage; //esto lo pongo re asi nomas, en el futuro puede venir de cualquier parte, por ejemplo un sistema de weapons o usables, con sus daños customizados

    public Action<Transform> callback_victim_position;
    [SerializeField] int physicalDamage = 5;
    [SerializeField] bool hasKnockback = true;
    [SerializeField] float KnockbackForce = 5;

    [SerializeField] bool UseAuxiliarUsableSystem = true;
    Action<Usable.UsableHitInfo<DamageReceiver>> callback_to_Usable_System;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        //hardcodeo super fuerte para testear... la idea es que por ejemplo... una espada tenga este script. 
        //y que luego quien me use, use este behaviour de daño a travez de la espada
        damage = new Damage(physicalDamage, () => GetOwnerTransform.position, hasKnockback, KnockbackForce, DamageType.Normal);
    }

    public void Set_CustomOnHit(Action<Usable.UsableHitInfo<DamageReceiver>> callback_usable_system) => callback_to_Usable_System = callback_usable_system;
    public void Set_Callback_Victim_Position(Action<Transform> callback) => callback_victim_position = callback;


    #region TICK
    protected override void OnTick(float DeltaTime)
    {
        Update_CD(DeltaTime);
    }
    #endregion

    #region [LOGIC] Execution logic
    float timer;
    public float CD = 0.1f;
    bool CD_Is_Active;
    public bool DoDamage()
    {
        if (!CanUseThisComponent) return false;

        //if (CD_Is_Active) return false;
        DamageLogic();
        CD_Is_Active = true;
        return true;
    }
    void Update_CD(float DeltaTime)
    {
        if (CD_Is_Active)
        {
            if (timer < CD)
            {
                timer = timer + 1 * DeltaTime;
            }
            else
            {
                CD_Is_Active = false;
                timer = 0;
            }
        }
    }
    #endregion

    #region [LOGIC] Deliver Damage



    private void OnTriggerEnter(Collider other)
    {
        var dmgreceiver = other.GetComponent<DamageReceiver>();
        if (dmgreceiver)
        {

            elems.Add(dmgreceiver);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var dmgreceiver = other.GetComponent<DamageReceiver>();
        if (dmgreceiver)
        {
            elems.Remove(dmgreceiver);
        }
    }

    HashSet<DamageReceiver> to_remove = new HashSet<DamageReceiver>();
    void DamageLogic()
    {
        Vector3 position_averge = new Vector3();
        int sum = 0;
        foreach (var e in elems)
        {
            if (e)
            {
                if (callback_to_Usable_System == null)
                {
                    //esto es en el caso de que no tengamos el custom usable que aplique un daño, es solo un ejemplo, no deberia entrar aca
                    damage.SetDamage(UnityEngine.Random.Range(6, 16));//el daño random tambien pasarlo por Action
                    Debug.Log(" <color=yellow> ¡CUIDADO! </color> Si entro acá es porque no tiene un sistema de usables atachado");
                }

                callback_victim_position?.Invoke(e.transform); // esto podria no estar, es solo para el view, los custom usables tendran su propio view
                e.ConfigureCallback_to_remove_me(x => to_remove.Add(x)); //esto es para que cuando su vida llega a 0 (CERO) se auto-remueva

                //para sacar el promedio de los objetos a golpear, podria hacer uno por uno, pero paja, ademas es mas óptimo
                position_averge = new Vector3(
                    e.transform.position.x + position_averge.x,
                    e.transform.position.y + position_averge.y,
                    e.transform.position.z + position_averge.z);
                sum++;
            }
        }

        #region OBTENGO PARAM 1: Info de hit en el mundo
        Vector3 origin = this.transform.position;
        Vector3 destiny = new Vector3(position_averge.x / sum, position_averge.y / sum, position_averge.z / sum);
        Vector3 vdir = (destiny - origin).normalized;
        var world = new Usable.UsableHitInfo<DamageReceiver>.Usable_World_Info(origin, destiny, vdir);
        #endregion

        #region OBTENGO PARAM 2: Array de los elementos
        DamageReceiver[] receivers = elems.ToArray();
        #endregion

        #region Se lo mando a mi custom Callback
        var hitinfo = new Usable.UsableHitInfo<DamageReceiver>(world, receivers);
        callback_to_Usable_System.Invoke(hitinfo);
        #endregion

        //remuevo al final los marcados como para remover
        foreach (var r in to_remove)
        {
            elems.Remove(r);
        }
        to_remove.Clear();
    }
    #endregion
}
