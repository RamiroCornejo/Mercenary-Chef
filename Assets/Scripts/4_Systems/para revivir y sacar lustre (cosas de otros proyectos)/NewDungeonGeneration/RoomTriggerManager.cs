﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using DungeonGenerator;
using DungeonGenerator.Components;
using Tools.Extensions;


#region LEEME
/*
 * ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄
 * ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀

    ROOM TRIGGER MANAGER

        Esta es una clase con Instancia estática, tiene que haber una sola por escena

    ¿Por que es una Instancia Estatica? porque no quiero guardar esta referencia 
    en todos los RoomTrigger


    Tiene 2 tipos de Inicializacion:

    >>> DUNGEON <<<
    Esta pensado para que al inicializar le pasemos la lista de rooms y este se
    encargue de buscar por dentro los RoomTriggers y los DungeonElements

    >>> NO DUNGEON <<<
    Esta pensado para que por editor le pasemos directamente los parents RoomTrigger
    Estos tienen que estar en el parent de la habitacion, para que busque todos
    los IDungeonElements

 * ▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄
 * ▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀
 */
#endregion

public class RoomTriggerManager : MonoBehaviour
{
    public static RoomTriggerManager instancia;

    private void Awake() { instancia = this; }

    public RoomTrigger current;

    [SerializeField] List<RoomTrigger> roomtriggers = new List<RoomTrigger>();
    public List<IDungeonElement> dungeonElements = new List<IDungeonElement>();

    public void Initialize(List<NewRoom> rooms)
    {
        //estos clear no estan al pedo
        //estan pensados para que se resetee
        //la dungeon en la misma escena
        dungeonElements.Clear();
        roomtriggers.Clear();

        foreach (var r in rooms)
        {
            foreach (var d in r.myRoomtrigger.GetDungeonElements()) dungeonElements.Add(d);
            roomtriggers.Add(r.myRoomtrigger);
        }

        DungeonGenerationFinallized();
    }

    public void DungeonGenerationFinallized()
    {
        dungeonElements.ForEach(x => x.OnDungeonGenerationFinallized());
    }

    public void Update()
    {
        if (current)
        {
            foreach (var c in current.myDungeonElements)
            {
                if (c != null) c.OnUpdateInThisRoom();
            }
        }
    }

    public void PlayerEnterIn(RoomTrigger roomselected, GameObject go)
    {
        if (current != null) current.GetDungeonElements().ForEach(x => x.OnPlayerExitInThisRoom());

        current = roomselected;

        if (current != null)
        {
            var dunelems = current.GetDungeonElements();
            foreach (var e in dunelems)
            {

                e.OnPlayerEnterInThisRoom(go.transform);
            }
        }
    }

    public void Alert()
    {
        //var enems = GetComponentsInChildren<Enem>();

        //foreach (var e in enems)
        //{
        //    e.EnemyConfirmed();
        //}

    }
}
