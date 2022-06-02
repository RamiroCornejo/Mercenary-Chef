using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DungeonGenerator.Components;

[System.Serializable]
public class RoomTrigger : MonoBehaviour
{
    List<DoorTrigger> triggers;
    public List<IDungeonElement> myDungeonElements = new List<IDungeonElement>();

    public void Initialize()
    {
        triggers = GetComponentsInChildren<DoorTrigger>().ToList();
        myDungeonElements = GetComponentsInChildren<IDungeonElement>().ToList();

        foreach (var t in triggers)
        {
            t.Initialize();
            t.AddEventListener(IsInside);
        }
    }

    public List<IDungeonElement> GetDungeonElements()
    {
        return myDungeonElements;
    }

    public void PlayerIsDeath()
    {
        foreach (var e in myDungeonElements)
        {
            e.OnPlayerDeath();
        }
    }

    public void IsInside(GameObject go)
    {
        if (go.GetComponent<Character>())
        {
            NewRoomGenerator.instancia.SetCurrentRoom(this.GetComponent<NewRoom>());
            RoomTriggerManager.instancia.PlayerEnterIn(this, go);
        }
    }
}
