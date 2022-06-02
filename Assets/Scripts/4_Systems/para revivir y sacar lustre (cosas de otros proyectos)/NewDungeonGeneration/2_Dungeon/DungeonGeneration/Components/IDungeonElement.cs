using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator.Components
{
    public interface IDungeonElement
    {
        void OnDungeonGenerationFinallized();
        void OnPlayerEnterInThisRoom(Transform who);
        void OnPlayerExitInThisRoom();
        void OnUpdateInThisRoom();
        void OnPlayerDeath(); 
    }
}
