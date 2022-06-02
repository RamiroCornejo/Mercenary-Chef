using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RecipeUI : MonoBehaviour
{
    Action onClickEvent;
    [SerializeField] TextMeshProUGUI recipeNameText = null;
    [SerializeField] Animator anim = null;
    [SerializeField] IngredientUI[] myIngredients = new IngredientUI[0];
    [SerializeField] GameObject[] addSigns = new GameObject[0];

    public void SetRecipe(Recipe recipe, Action onClick)
    {
        for (int i = 0; i < myIngredients.Length; i++)
        {
            myIngredients[i].gameObject.SetActive(false);
            if (i < addSigns.Length) addSigns[i].SetActive(false);
        }

        recipeNameText.text = recipe.recipeName;
        int length = recipe.necesaryIngredients.Length;
        recipeNameText.color = Color.white;
        bool isEnough;

        for (int i = 0; i < length; i++)
        {
            var ingredient = recipe.necesaryIngredients[i];

            myIngredients[i].gameObject.SetActive(true);

            isEnough = PlayerInventory.QueryElement(ingredient.key, ingredient.value, 0, false);

            if (recipeNameText.color != Color.red && !isEnough) recipeNameText.color = Color.red;

            myIngredients[i].SetUI(ingredient.key.Element_Image, ingredient.value, PlayerInventory.QueryElement(ingredient.key, ingredient.value, 0, false));

            if (i < length - 1)
                addSigns[i].SetActive(true);
        }

        onClickEvent = onClick;
    }

    public void CreateRecipe()
    {
        onClickEvent?.Invoke();
    }

    public void Select()
    {
        anim.SetTrigger("Highlighted");
    }

    public void Unselect()
    {
        anim.SetTrigger("Normal");
    }
}
