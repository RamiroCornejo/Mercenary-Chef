using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IngredientEditorWindow : EditorWindow
{
    IngredientNote note;
    Vector2 scrollPos;
    Texture[] notesImg = new Texture[4];

    Rect lastRect;

    float noteHeight = 85;
    float noteWidth = 150;

    int sections = 3;
    float sectionSeparation = 20;
    float timeSeparationTick = 0.05f;

    public static void OpenWindow(IngredientNote ingredient)
    {
        IngredientEditorWindow window = (IngredientEditorWindow)GetWindow(typeof(IngredientEditorWindow));
        window.note = ingredient;
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


        if (note.setters.Length == 0)
        {
            if (GUI.Button(new Rect(10, 10, 200, 100), "Add First Note"))
            {
                Add(0);
                Repaint();
                return;
            }

            return;
        }

        EditorGUILayout.BeginVertical(GUILayout.Height(position.height - lastRect.height - 20));
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        lastRect = new Rect();


        for (int i = note.setters.Length - 1; i >= 0; i--)
        {
            int sect = note.setters[i].section;
            float sep = i - 1 < 0 ? 1 : note.setters[i - 1].timeSeparation;
            var xPos = (noteWidth + sectionSeparation) * sect;

            EditorGUI.DrawRect(new Rect(xPos, lastRect.y + lastRect.height, noteWidth, noteHeight), GUI.color);
            EditorGUI.DrawTextureTransparent(new Rect(xPos += noteWidth / 2 - 50 / 2, lastRect.y += lastRect.height, 50, 50), notesImg[sect], ScaleMode.ScaleToFit);

            EditorGUILayout.Space(10);

            if (note.setters[i].section > 0)
            {
                if (GUI.Button(new Rect(xPos + 2, lastRect.y + 50, 20, 30), "<"))
                {
                    note.setters[i].section -= 1;
                    Repaint();
                    return;
                }
            }

            if (note.setters[i].section < sections)
            {
                if (GUI.Button(new Rect(xPos + 22, lastRect.y + 50, 20, 30), ">"))
                {
                    note.setters[i].section += 1;
                    Repaint();
                    return;
                }
            }

            if(i != note.setters.Length - 1)
            {
                if (note.setters[i].timeSeparation < 5)
                {
                    if (GUI.RepeatButton(new Rect(xPos + 55, lastRect.y + 22, 30, 20), "v"))
                    {
                        note.setters[i].timeSeparation += timeSeparationTick;
                        if (note.setters[i].timeSeparation > 5)
                            note.setters[i].timeSeparation = 5;
                        Repaint();
                    }
                }

                if (note.setters[i].timeSeparation > 0)
                {
                    if (GUI.RepeatButton(new Rect(xPos + 55, lastRect.y, 30, 20), "^"))
                    {
                        note.setters[i].timeSeparation -= timeSeparationTick;
                        if (note.setters[i].timeSeparation < 0)
                            note.setters[i].timeSeparation = 0;
                        Repaint();
                    }
                }
            }
            xPos = (noteWidth + sectionSeparation) * sect;

            if (GUI.Button(new Rect(xPos + 2, lastRect.y, 20, 20), "x"))
            {
                Remove(i);
                return;
            }

            if (GUI.Button(new Rect(xPos + 2, lastRect.y + 22, 50, 20), "+ Before"))
            {
                Add(i + 1);
                return;
            }

            if (GUI.Button(new Rect(xPos + 2, lastRect.y + 44, 50, 20), "+ After"))
            {
                Add(i);
                return;
            }

            EditorGUILayout.Space(100 * sep);
            lastRect = GUILayoutUtility.GetLastRect();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorUtility.SetDirty(note);
    }

    void Remove(int indexToRemove)
    {
        var tempArray = new NoteSetter[note.setters.Length - 1];

        int index = 0;
        for (int i = 0; i < tempArray.Length; i++)
        {

            if (i == indexToRemove) { index += 1; }
            tempArray[i] = new NoteSetter();
            tempArray[i].section = note.setters[index].section;
            tempArray[i].timeSeparation = note.setters[index].timeSeparation;
            index += 1;
        }

        note.setters = new NoteSetter[tempArray.Length];

        for (int i = 0; i < note.setters.Length; i++)
        {
            note.setters[i] = tempArray[i];
        }
        Repaint();
    }

    void Add(int indexToAdd)
    {
        var tempArray = new NoteSetter[note.setters.Length + 1];

        int index = 0;

        for (int i = 0; i < tempArray.Length; i++)
        {

            if (i == indexToAdd)
            {
                tempArray[i] = new NoteSetter();
                continue;
            }
            else {
                tempArray[i] = note.setters[index];
                index += 1;
            }
        }

        note.setters = tempArray;
        Repaint();
    }
}
