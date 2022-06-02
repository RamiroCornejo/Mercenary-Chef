using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Tools.EventClasses;
using UnityEngine.SceneManagement;
using Tools;

public class Inputs : MonoBehaviour
{
    public enum InputType { Joystick, Mouse, Other }
    public InputType input_type;

    JoystickBasicInput joystickhelper;

    public InputControl inputControlCheck;
    bool isJoystick;

    private void Awake() => ConfigureJoystickHelper();

    private void Update()
    {
        if (!isJoystick)
        {
            if (inputControlCheck.GetInputState() == InputControl.eInputState.Controler)
            {
                input_type = InputType.Joystick;
                isJoystick = true;

                InputImageDatabase.instance.ChangeInput(InputImageDatabase.InputImageCode.joystick);

                //avisar a todos que se cambio el input
                //Main.instance.eventManager.TriggerEvent(GameEvents.CHANGE_INPUT, "Joystick");
                SendMessage(isJoystick);
            }
        }
        else
        {
            if (inputControlCheck.GetInputState() == InputControl.eInputState.MouseKeyboard)
            {
                input_type = InputType.Mouse;
                isJoystick = false;

                InputImageDatabase.instance.ChangeInput(InputImageDatabase.InputImageCode.mouse);

                //avisar a todos que se cambio el input
                //Main.instance.eventManager.TriggerEvent(GameEvents.CHANGE_INPUT, "Mouse");
                SendMessage(isJoystick);
            }
        }

        RefreshHelper();
    }

    #region Change Input & Message
    public JoystickMessage joystickMessage;
    public void SendMessage(bool _isJoystick)
    {
        joystickMessage.Open();
        joystickMessage.Message(_isJoystick);
    }
    #endregion

    #region JoystickHelper
    void ConfigureJoystickHelper()
    {
        joystickhelper = new JoystickBasicInput();
        joystickhelper

            .SUBSCRIBE_PRESS_DPAD_UP(EV_PRESS_DPAD_UP)
            .SUBSCRIBE_PRESS_DPAD_DOWN(EV_PRESS_DPAD_DOWN)
            .SUBSCRIBE_PRESS_DPAD_RIGHT(EV_PRESS_DPAD_RIGHT)
            .SUBSCRIBE_PRESS_DPAD_LEFT(EV_PRESS_DPAD_LEFT)

            .SUBSCRIBE_RELEASE_DPAD_UP(EV_RELEASE_DPAD_UP)
            .SUBSCRIBE_RELEASE_DPAD_DOWN(EV_RELEASE_DPAD_DOWN)
            .SUBSCRIBE_RELEASE_DPAD_RIGHT(EV_RELEASE_DPAD_RIGHT)
            .SUBSCRIBE_RELEASE_DPAD_LEFT(EV_RELEASE_DPAD_LEFT)

            .SUBSCRIBE_PRESS_LTRIGGER(EV_DPAD_RTRIGGER)
            .SUBSCRIBE_PRESS_RTRIGGER(EV_DPAD_LTRIGGER)
            .SUBSCRIBE_RELEASE_LTRIGGER(EV_DPAD_RTRIGGER_RELEASE)
            .SUBSCRIBE_RELEASE_RTRIGGER(EV_DPAD_LTRIGGER_RELEASE)
            .SUBSCRIBE_R_STICK_BTN_CENTRAL(EV_RSTICKCENTRAL)
            .SUBSCRIBE_L_STICK_BTN_CENTRAL(EV_LSTICKCENTRAL)
            ;
    }
    void RefreshHelper() => joystickhelper.Refresh();
    void EV_PRESS_DPAD_UP() { }
    void EV_PRESS_DPAD_DOWN() { }
    void EV_PRESS_DPAD_LEFT() { }
    void EV_PRESS_DPAD_RIGHT() { }
    void EV_RELEASE_DPAD_UP() { }
    void EV_RELEASE_DPAD_DOWN() { }
    void EV_RELEASE_DPAD_LEFT() { }
    void EV_RELEASE_DPAD_RIGHT() { }
    void EV_DPAD_LTRIGGER() { }
    void EV_DPAD_LTRIGGER_RELEASE() { }
    void EV_DPAD_RTRIGGER() { }
    void EV_DPAD_RTRIGGER_RELEASE() { }
    void EV_RSTICKCENTRAL() { }
    void EV_LSTICKCENTRAL() { }
    #endregion
}
