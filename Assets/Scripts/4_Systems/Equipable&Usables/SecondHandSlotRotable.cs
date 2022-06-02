using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SecondHandSlotRotable : MonoBehaviour
{
    public const int LEFT = 0;
    public const int UP = 1;
    public const int DOWN = 2;
    public const int RIGHT = 3;

    [System.NonSerialized] public Slot slot_current;
    [Header("Solo el master necesita las referencias")]
    [SerializeField] bool iAmTheMaster;
    [SerializeField] SecondHandSlotRotable myleft;
    [SerializeField] SecondHandSlotRotable myright;

    [System.NonSerialized] public PhotoContainterAnimController photocontainer;
    public Sprite current_sprite => slot_current.Element.Element_Image;

    private void Start()
    {
        photocontainer = GetComponentInChildren<PhotoContainterAnimController>();
    }

    public void RotateLeft(Action OnEndAnimation)
    {
        if (iAmTheMaster)
        {
            var current = slot_current;
            var left = myleft.slot_current;
            var right = myright.slot_current;

            myleft.slot_current = current;
            slot_current = right;
            myright.slot_current = left;

            if (slot_current != null) photocontainer.AnimDir(RIGHT, DOWN, current_sprite);
            if (myright.slot_current != null) myright.photocontainer.AnimDir(DOWN, LEFT, myright.slot_current.Element.Element_Image);
            if (myleft.slot_current != null) myleft.photocontainer.AnimDir(UP, RIGHT, myleft.slot_current.Element.Element_Image);
        }
    }
    public void RotateRight(Action OnEndAnimation)
    {
        if (iAmTheMaster)
        {
            var current = slot_current;
            var left = myleft.slot_current;
            var right = myright.slot_current;

            myright.slot_current = slot_current;
            slot_current = left;
            myleft.slot_current = right;

            if (slot_current != null) photocontainer.AnimDir(DOWN, RIGHT, current_sprite);
            if (myleft.slot_current != null) myleft.photocontainer.AnimDir(RIGHT, UP, myleft.slot_current.Element.Element_Image);
            if (myright.slot_current != null) myright.photocontainer.AnimDir(LEFT, DOWN, myright.slot_current.Element.Element_Image);

        }
    }
}
