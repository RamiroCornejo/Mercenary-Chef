using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public static class InputManager
{
    static bool IsJoystick;

    private static Dictionary<KeyEventButon, Func<KeyCode, bool>> ButtonEventDic = new Dictionary<KeyEventButon, Func<KeyCode, bool>>(3)
    {
        { KeyEventButon.KeyDown, Input.GetKeyDown },
        { KeyEventButon.Key, Input.GetKey },
        { KeyEventButon.KeyUp, Input.GetKeyUp }
    };

    private static Dictionary<KeyEventButon, Func<string, bool>> ButtonEventDicString = new Dictionary<KeyEventButon, Func<string, bool>>(3)
    {
        { KeyEventButon.KeyDown, Input.GetButtonDown },
        { KeyEventButon.Key, Input.GetButton },
        { KeyEventButon.KeyUp, Input.GetButtonUp }
    };

    public static bool GetInput(InputBind bind, KeyEventButon eventButton)
    {
        if (ButtonEventDic[eventButton](bind.joystickKey))
        {
            IsJoystick = true;
            bind.EventAction.Invoke(eventButton);
            return true;
        }
        else if (ButtonEventDic[eventButton](bind.keyboardKey))
        {
            IsJoystick = false;
            bind.EventAction.Invoke(eventButton);
            return true;
        }

        return false;
    }

    public static bool GetButton(InputBind bind, KeyEventButon eventButton)
    {
        if (ButtonEventDicString[eventButton](bind.buttonName))
        {
            IsJoystick = true;
            bind.EventAction.Invoke(eventButton);
            return true;
        }
        else if (ButtonEventDicString[eventButton](bind.buttonName))
        {
            IsJoystick = false;
            bind.EventAction.Invoke(eventButton);
            return true;
        }

        return false;
    }
}

[Serializable]
public struct InputBind
{
    public string buttonName;
    public KeyCode joystickKey;
    public KeyCode keyboardKey;
    public UnityEvKeyButton EventAction;
}

[Serializable] public class UnityEvFloat : UnityEvent<float> { }

[Serializable] public class UnityEvKeyButton : UnityEvent<KeyEventButon> { }
