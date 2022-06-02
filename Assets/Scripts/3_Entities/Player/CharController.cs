using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum KeyEventButon
{
    KeyDown = 0,
    Key = 1,
    KeyUp = 2
}

public class CharController : MonoBehaviour
{

    [SerializeField] UnityEvFloat AxisHorizontal;
    [SerializeField] UnityEvFloat AxisVertical;

    [SerializeField] InputBind dashBind;
    [SerializeField] InputBind attackBind;
    [SerializeField] InputBind interactBind;

    bool dead;

    private void Start()
    {
    }

    private void Update()
    {
        if (dead) return;
        float axisHorizontalOne = Input.GetAxis("Horizontal");
        float axisVerticalOne = Input.GetAxis("Vertical");

        AxisHorizontal?.Invoke(axisHorizontalOne);
        AxisVertical?.Invoke(axisVerticalOne);

        InputManager.GetButton(dashBind, KeyEventButon.KeyDown);
        InputManager.GetButton(attackBind, KeyEventButon.KeyDown);
        InputManager.GetButton(interactBind, KeyEventButon.KeyDown);
    }

    void CharacterDead()
    {
        AxisHorizontal?.Invoke(0);
        AxisVertical?.Invoke(0);
        dead = true;
    }

}

