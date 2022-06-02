using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Recipe))]
public class RecipeEditor : Editor
{
    Recipe recipe;

    const float perfect = 1;
    float maxResult;

    private void OnEnable()
    {
        recipe = target as Recipe;

        RefreshMaxResult();
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor"))
        {
            RecipeEditorWindow.OpenWindow(target as Recipe);
        }
        base.OnInspectorGUI();

        EditorGUILayout.Space(2);

        EditorGUILayout.LabelField("Máxima puntuación posible: " + maxResult.ToString(), EditorStyles.helpBox);

        EditorGUILayout.Space(5);

        recipe.scoreToPerfect = EditorGUILayout.Slider(recipe.scoreToPerfect, 0, maxResult);

        EditorGUILayout.LabelField("Puntuación para Perfect: " + recipe.scoreToPerfect.ToString(), EditorStyles.helpBox);

        EditorGUILayout.Space(5);

        recipe.scoreToGood = EditorGUILayout.Slider(recipe.scoreToGood, 0, recipe.scoreToPerfect);

        EditorGUILayout.LabelField("Puntuación para Good: " + recipe.scoreToGood.ToString(), EditorStyles.helpBox);

        if (GUI.changed)
        {
            int value = 0;

            for (int i = 0; i < recipe.necesaryIngredients.Length; i++)
            {
                value += recipe.necesaryIngredients[i].value;
            }
            if(value != recipe.ingredientsOrder.Count)
                RefreshList();


            RefreshMaxResult();
            Repaint();
            EditorUtility.SetDirty(recipe);
        }
    }

    void RefreshMaxResult()
    {
        maxResult = 0;
        for (int i = 0; i < recipe.necesaryIngredients.Length; i++)
        {
            maxResult += recipe.necesaryIngredients[i].key.setters.Length * recipe.necesaryIngredients[i].value;
        }
    }

    void RefreshList()
    {
    }
}
