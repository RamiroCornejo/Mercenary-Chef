using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RecipeEditorWindow : EditorWindow
{
    Recipe recipe;
    Vector2 scrollPos;
    Texture[] notesImg = new Texture[4];

    Rect lastRect;

    //float noteHeight = 85;
    //float noteWidth = 150;

    //int sections = 3;
    //float sectionSeparation = 20;
    //float timeSeparationTick = 0.05f;

    public static void OpenWindow(Recipe recipe)
    {
        RecipeEditorWindow window = (RecipeEditorWindow)GetWindow(typeof(RecipeEditorWindow));
        window.recipe = recipe;
        window.notesImg[0] = Resources.Load("GuitarGreen") as Texture;
        window.notesImg[1] = Resources.Load("GuitarRed") as Texture;
        window.notesImg[2] = Resources.Load("GuitarYellow") as Texture;
        window.notesImg[3] = Resources.Load("GuitarBlue") as Texture;
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("SUPER ULTIMATUM FINAL EDITOR V 2.13", EditorStyles.boldLabel);
        lastRect = GUILayoutUtility.GetLastRect();

        EditorGUILayout.Space(4);


        if (recipe.ingredientsOrder.Count == 0)
        {
            EditorGUILayout.HelpBox("Acá sólo posicionas los ingredientes, los tenes que agregar desde inspector", MessageType.Info);

            return;
        }

        EditorGUILayout.BeginVertical(GUILayout.Height(position.height - lastRect.height - 20));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        lastRect = new Rect();


        //for (int i = recipe.setters.Length - 1; i >= 0; i--)
        //{
        //    int sect = recipe.setters[i].section;
        //    float sep = i - 1 < 0 ? 1 : recipe.setters[i - 1].timeSeparation;
        //    var xPos = (noteWidth + sectionSeparation) * sect;

        //    EditorGUI.DrawRect(new Rect(xPos, lastRect.y + lastRect.height, noteWidth, noteHeight), GUI.color);
        //    EditorGUI.DrawTextureTransparent(new Rect(xPos += noteWidth / 2 - 50 / 2, lastRect.y += lastRect.height, 50, 50), notesImg[sect], ScaleMode.ScaleToFit);

        //    EditorGUILayout.Space(10);

        //    if (recipe.setters[i].section > 0)
        //    {
        //        if (GUI.Button(new Rect(xPos + 2, lastRect.y + 50, 20, 30), "<"))
        //        {
        //            recipe.setters[i].section -= 1;
        //            Repaint();
        //            return;
        //        }
        //    }

        //    if (recipe.setters[i].section < sections)
        //    {
        //        if (GUI.Button(new Rect(xPos + 22, lastRect.y + 50, 20, 30), ">"))
        //        {
        //            recipe.setters[i].section += 1;
        //            Repaint();
        //            return;
        //        }
        //    }

        //    if (i != recipe.setters.Length - 1)
        //    {
        //        if (recipe.setters[i].timeSeparation < 5)
        //        {
        //            if (GUI.RepeatButton(new Rect(xPos + 55, lastRect.y + 22, 30, 20), "v"))
        //            {
        //                recipe.setters[i].timeSeparation += timeSeparationTick;
        //                if (recipe.setters[i].timeSeparation > 5)
        //                    recipe.setters[i].timeSeparation = 5;
        //                Repaint();
        //            }
        //        }

        //        if (recipe.setters[i].timeSeparation > 0)
        //        {
        //            if (GUI.RepeatButton(new Rect(xPos + 55, lastRect.y, 30, 20), "^"))
        //            {
        //                recipe.setters[i].timeSeparation -= timeSeparationTick;
        //                if (recipe.setters[i].timeSeparation < 0)
        //                    recipe.setters[i].timeSeparation = 0;
        //                Repaint();
        //            }
        //        }
        //    }
        //    xPos = (noteWidth + sectionSeparation) * sect;

        //    if (GUI.Button(new Rect(xPos + 2, lastRect.y, 20, 20), "x"))
        //    {
        //        Remove(i);
        //        return;
        //    }

        //    if (GUI.Button(new Rect(xPos + 2, lastRect.y + 22, 50, 20), "+ Before"))
        //    {
        //        Add(i + 1);
        //        return;
        //    }

        //    if (GUI.Button(new Rect(xPos + 2, lastRect.y + 44, 50, 20), "+ After"))
        //    {
        //        Add(i);
        //        return;
        //    }

        //    EditorGUILayout.Space(100 * sep);
        //    lastRect = GUILayoutUtility.GetLastRect();
        //}

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorUtility.SetDirty(recipe);
    }

    //void Remove(int indexToRemove)
    //{
    //    var tempArray = new NoteSetter[recipe.setters.Length - 1];

    //    int index = 0;
    //    for (int i = 0; i < tempArray.Length; i++)
    //    {

    //        if (i == indexToRemove) { index += 1; }
    //        tempArray[i] = new NoteSetter();
    //        tempArray[i].section = recipe.setters[index].section;
    //        tempArray[i].timeSeparation = recipe.setters[index].timeSeparation;
    //        index += 1;
    //    }

    //    recipe.setters = new NoteSetter[tempArray.Length];

    //    for (int i = 0; i < recipe.setters.Length; i++)
    //    {
    //        recipe.setters[i] = tempArray[i];
    //    }
    //    Repaint();
    //}

    //void Add(int indexToAdd)
    //{
    //    var tempArray = new NoteSetter[recipe.setters.Length + 1];

    //    int index = 0;

    //    for (int i = 0; i < tempArray.Length; i++)
    //    {

    //        if (i == indexToAdd)
    //        {
    //            tempArray[i] = new NoteSetter();
    //            continue;
    //        }
    //        else
    //        {
    //            tempArray[i] = recipe.setters[index];
    //            index += 1;
    //        }
    //    }

    //    recipe.setters = tempArray;
    //    Repaint();
    //}
}
