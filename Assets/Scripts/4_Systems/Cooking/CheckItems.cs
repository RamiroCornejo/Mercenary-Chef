using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckItems : MonoBehaviour
{
    [SerializeField] Recipe objetiveRecipe = null;

    public bool CheckIngredients()
    {
        return objetiveRecipe.ContainsRecipe;
    }
}
