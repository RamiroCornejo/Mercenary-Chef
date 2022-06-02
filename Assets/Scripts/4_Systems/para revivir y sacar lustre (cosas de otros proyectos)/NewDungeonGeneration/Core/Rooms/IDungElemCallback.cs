using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGenerator.Components;
using UnityEngine.Events;

public class IDungElemCallback : MonoBehaviour, IDungeonElement
{
    public UnityEvent PlayerEnterInThisRoom;
    public UnityEvent PlayerExitTheRoom;
    public UnityEvent UpdateThisRoom;
    public UnityEvent PlayerDeath;

    public void OnDungeonGenerationFinallized() { }
    public void OnPlayerEnterInThisRoom(Transform who) => PlayerEnterInThisRoom.Invoke();
    public void OnPlayerExitInThisRoom() => PlayerExitTheRoom.Invoke();
    public void OnUpdateInThisRoom() => UpdateThisRoom.Invoke();
    public void OnPlayerDeath() => PlayerDeath.Invoke();
}
