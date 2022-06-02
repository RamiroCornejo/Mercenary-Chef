using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Cooking/Ingredient", order = 1)]
public class IngredientNote : UsableData
{
    [Header("IngredientNote")]
    public NoteSetter[] setters = new NoteSetter[0];
    public Mesh ingredientMesh;
    public Material ingredientMat;
}

[Serializable]
public class NoteSetter
{
    [Range(0, 3)] public int section = 0;

    public float timeSeparation;

    [HideInInspector]
    public Mesh meshIngredient;

    [HideInInspector]
    public Material matIngredient;
}
