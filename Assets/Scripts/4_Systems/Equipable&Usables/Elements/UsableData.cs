using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Tool", menuName = "RPG/Inventory/UsablesAndEquipables", order = 1)]
public class UsableData : ElementData
{
    //herramientas : Ej: tijeras lo uso, y rompo lianas
    //ingredientes : Ej: los hongos los uso para cocinar, pero tambien me lo puedo comer asi nomas y me da pocos beneficios
    //consumibles : EJ: Alguna pota o algo que me dieron o compre
    //comidas o preparados : EJ: Comer o Usar algo crafteado
    [Header("Usables & Equipables")]
    [SerializeField] public Usable equiped_item;
    public bool isConsumible = false;
}
