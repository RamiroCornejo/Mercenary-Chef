using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.Extensions;
using UnityEngine;

public abstract class RoomBase : MonoBehaviour
{
    [Header("RoomBase")]
   // List<Entity> entities;
    protected List<IRoomElementable> elements;
    

    public void Initialize()
    {
       // entities = GetComponentsInChildren<Entity>().ToList();
        elements = GetComponentsInChildren<IRoomElementable>().ToList();//

       // foreach (var e in entities) e.Initialize();
        foreach (var e in elements) e.SetmanualRoom(this);
        

        OnInitialize();
    }

    public abstract void OnInitialize();
}
