﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputImageDatabase : MonoBehaviour
{
    public static InputImageDatabase instance;
    private void Awake()
    {
        instance = this;
        current = MouseKeyboard;
    }

    public enum InputImageCode { joystick, mouse }
    public enum InputImageType
    {
        move,
        rotate,
        normal_attack,
        interact,
        jump
    }
    SpriteDataBaseInput current;
    public SpriteDataBaseInput Joystick;
    public SpriteDataBaseInput MouseKeyboard;

    public Sprite GetSprite(InputImageType type)
    {
        if (type == InputImageType.move) return current.move;
        if (type == InputImageType.rotate) return current.rotate;
        if (type == InputImageType.normal_attack) return current.normal_attack;
        if (type == InputImageType.interact) return current.interact;
        if (type == InputImageType.jump) return current.jump;
        return null;
    }
    public void ChangeInput(InputImageCode inputImageCode)
    {
        if (inputImageCode == InputImageCode.joystick) current = Joystick;
        if (inputImageCode == InputImageCode.mouse) current = MouseKeyboard;
    }
}

[System.Serializable]
public class SpriteDataBaseInput
{
    public Sprite move;
    public Sprite rotate;
    public Sprite normal_attack;
    public Sprite interact;
    public Sprite jump;
}
