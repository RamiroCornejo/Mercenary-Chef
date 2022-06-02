using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.Extensions;
using UnityEngine;

public class ManualRoom : RoomBase
{


    public override void OnInitialize()
    {
        var trigger = GetComponent<RoomTrigger>();
        trigger.Initialize();
        RoomTriggerManager.instancia.dungeonElements.AddListToList(trigger.GetDungeonElements());
        RoomTriggerManager.instancia.DungeonGenerationFinallized();

        foreach (var e in elements) e.SetmanualRoom(this);
    }
}
