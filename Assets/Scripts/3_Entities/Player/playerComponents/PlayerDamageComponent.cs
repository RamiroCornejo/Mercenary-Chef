﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tools.Structs;
using System.Linq;

public class PlayerDamageComponent : PlayerComponent
{

   // ConeObtainer<DamageReceiver> cone;
   // public LayerMask layers_to_damage;

   // protected override void OnInitialized()
   // {
   //     base.OnInitialized();
   //     //query.Configure(GetOwnerTransform);
   //     cone = new ConeObtainer<DamageReceiver>();
   //     cone.Configure(GetOwnerTransform, layers_to_damage);

   //     damage = new Damage(5, () => GetOwnerTransform.position, true, 5 , new State_Effect("Fuego", true, 5, true, 0.1f, 1));

   // }

   // private void OnDrawGizmos()
   // {
   //     if (cone != null)
   //         cone.OnDrawGizmos();
   // }

   // #region TICK
   // protected override void OnTick(float DeltaTime)
   // {
   //     Update_CD(DeltaTime);
   // }
   // #endregion

   // #region [LOGIC] Execution logic
   // float timer;
   // public float CD = 0.1f;
   // bool CD_Is_Active;
   // public bool DoDamage()
   // {
   //     if (!CanUseThisComponent) return false;

   //     if (CD_Is_Active) return false;
   //     DamageLogic();
   //     CD_Is_Active = true;
   //     return true;
   // }
   // void Update_CD(float DeltaTime)
   // {
   //     if (CD_Is_Active)
   //     {
   //         if (timer < CD)
   //         {
   //             timer = timer + 1 * DeltaTime;
   //         }
   //         else
   //         {
   //             CD_Is_Active = false;
   //             timer = 0;
   //         }
   //     }
   // }
   // #endregion

   // #region [LOGIC] Deliver Damage
   // [SerializeField] Damage damage;
   //// [SerializeField] ObserverQuery query;
   // void DamageLogic()
   // {
   //     var elems = cone.Query();

   //     foreach (var e in elems)
   //     {
   //         damage.SetDamage(UnityEngine.Random.Range(6, 16));
   //         e.ReceiveDamage(damage, x => { });
   //     }

   //     //esto funcionaba con la grilla del proyecto anterior

   //     //if (query == null) { throw new Exception("[ERROR] I do not have a " + ObserverQuery.Static_ToString()); }

   //     //var col = query
   //     //    .Query() //IA2-P2 [SpatialGrid - PlayerDamage]
   //     //    .OfType<GridComponent>()
   //     //    .Select(x => x.Grid_Object.GetComponent<LivingEntity>()) //IA-P3 [Select]
   //     //    .Where(x => !x.gameObject.GetComponent<PlayerModel>() && (x.gameObject.GetComponent<Enemy>() || x.gameObject.GetComponent<TowerEntity>())) //IA2-P3 [Where]
   //     //    .DefaultIfEmpty(null);
   //     //if (col == null) return;
   //     //foreach (var liv_ent in col)
   //     //{
   //     //    if (liv_ent != null)
   //     //    {
   //     //        Debug.Log("Do DAMAGE: {" + damage.Physical_damage + "} => " + liv_ent.gameObject.name);
   //     //        liv_ent.ReceiveDamage(damage);
   //     //    }
   //     //}
   // }
   // #endregion
}
