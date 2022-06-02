using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class PhotoContainterAnimController : MonoBehaviour
{
    public const int LEFT = 0;
    public const int UP = 1;
    public const int DOWN = 2;
    public const int RIGHT = 3;

    public Animator myAnim;

    public Image myImage;

    void Show() => myImage.enabled = true;
    void Hide() => myImage.enabled = false;

    public Sprite current => myImage.sprite;
    Action NONE = delegate { };

    public Sprite DEBUG_IMG1;
    public Sprite DEBUG_IMG2;

    [Header("Anims component by code")]
    public UIAnim_ImageTransition left_comp;
    public UIAnim_ImageTransition right_comp;
    public UIAnim_ImageTransition up_comp;
    public UIAnim_ImageTransition down_comp;

    bool canAnimate = true;

    public void Awake()
    {
        myAnim = GetComponent<Animator>();
    }

    public void Anim_Use()
    {
        myAnim.Play("UseAItem");
    }
    public void Anim_Negate()
    {
        myAnim.Play("CanNotUseItem");
    }
    public void Anim_Go_Left(Sprite from)
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AnimDir(UP, LEFT, DEBUG_IMG1);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            AnimDir(LEFT, UP, DEBUG_IMG2);
        }
    }

    public bool AnimDir(int fromdir, int todir, Sprite fromsp)
    {
        if (!canAnimate) return false;
        Hide();

        if (fromdir == LEFT) LeftExit();
        if (fromdir == UP) UpExit();
        if (fromdir == RIGHT) RightExit();
        if (fromdir == DOWN) DownExit();

        if (todir == LEFT) LeftEnter(fromsp);
        if (todir == UP) UpEnter(fromsp);
        if (todir == RIGHT) RigthEnter(fromsp);
        if (todir == DOWN) DownEnter(fromsp);

        canAnimate = false;
        return true;
    }


    void RigthEnter(Sprite from) { if (right_comp) right_comp.Anim_Enter(() => { myImage.sprite = from; Show(); canAnimate = true; }, from); }
    void RightExit() { if (right_comp) right_comp.Anim_Exit(NONE, current); }

    void LeftEnter(Sprite from) { if (left_comp) left_comp.Anim_Enter(() => { myImage.sprite = from; Show(); canAnimate = true; }, from); }
    void LeftExit() { if (left_comp) left_comp.Anim_Exit(NONE, current); }

    void UpEnter(Sprite from) { if (up_comp) up_comp.Anim_Enter(() => { myImage.sprite = from; Show(); canAnimate = true; }, from); }
    void UpExit() { if (up_comp) up_comp.Anim_Exit(NONE, current); }

    void DownEnter(Sprite from) { if (down_comp) down_comp.Anim_Enter(() => { myImage.sprite = from; Show(); canAnimate = true; }, from); }
    void DownExit() { if (down_comp) down_comp.Anim_Exit(NONE, current); }

}
