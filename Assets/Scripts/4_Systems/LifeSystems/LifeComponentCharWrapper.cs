using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeComponentCharWrapper : LifeComponent
{
    protected override void Start()
    {
        // hacemos este wrapper para poder desacoplar el FrontEndStatBase del Character
        // y asi tenerlo por fuera, para no andar creando un canvas dentro del character

        ui_life = CharacterLifeFrontEnd.instance;

        base.Start();
    }
}
