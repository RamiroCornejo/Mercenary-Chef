using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IngredientNote))]
public class IngredientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open Editor"))
        {
            IngredientEditorWindow.OpenWindow(target as IngredientNote);
        }
        base.OnInspectorGUI();

    }
}
