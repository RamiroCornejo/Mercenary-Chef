using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientUI : MonoBehaviour
{
    [SerializeField] Image ingredientImg = null;
    [SerializeField] TextMeshProUGUI cantText = null;


    public void SetUI(Sprite sprite, int cant, bool isEnough)
    {
        ingredientImg.sprite = sprite;
        cantText.text = cant.ToString();

        cantText.color = isEnough ? Color.white : Color.red;
    }
}
