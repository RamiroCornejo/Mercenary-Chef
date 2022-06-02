using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIAnim_ImageTransition : MonoBehaviour
{
    public enum Anim_Direction { left, right, up, down }
    public Anim_Direction direction;
    public float offset_quantity = 20;

    bool anim;
    float timer;
    public float animation_time;
    public bool isEnter;
    public bool debug;
    public bool animate_by_editor_prefs;

    RectTransform myRect;
    Image myImage;

    Action OnEnd;

    Vector2 centerpos;
    Vector2 offsetpos;

    Color white = new Color(1,1,1,1);
    Color transparent = new Color(1, 1, 1, 0);

    Vector2 Pos
    {
        set => myRect.anchoredPosition = value;
        get => myRect.anchoredPosition;
    }

    private void Awake()
    {
        myRect = this.GetComponent<RectTransform>();
        myImage = this.GetComponent<Image>();
        centerpos = myRect.anchoredPosition;
    }

    public UIAnim_ImageTransition Show()
    {
        myImage.enabled = true;
        return this;
    }
    public UIAnim_ImageTransition Hide()
    {
        myImage.enabled = false;
        return this;
    }

    public UIAnim_ImageTransition Anim(Anim_Direction dir, bool isEnter, Action _OnEnd, Sprite sp = null)
    {
        Show();
        if (sp) myImage.sprite = sp;
        anim = true;
        timer = 0;
        this.isEnter = isEnter;
        OnEnd = _OnEnd;

        switch (dir)
        {
            case Anim_Direction.left:   offsetpos = new Vector2(centerpos.x - offset_quantity, centerpos.y); break;
            case Anim_Direction.right:  offsetpos = new Vector2(centerpos.x + offset_quantity, centerpos.y); break;
            case Anim_Direction.up:     offsetpos = new Vector2(centerpos.x, centerpos.y + offset_quantity); break;
            case Anim_Direction.down:   offsetpos = new Vector2(centerpos.x, centerpos.y - offset_quantity); break;
        }

        return this;
    }
    public UIAnim_ImageTransition Anim_Enter(Action _OnEnd, Sprite sp = null)
    {
        Show();
        if (sp) myImage.sprite = sp;
        anim = true;
        timer = 0;
        isEnter = true;
        OnEnd = _OnEnd;

        switch (direction)
        {
            case Anim_Direction.left: offsetpos = new Vector2(centerpos.x - offset_quantity, centerpos.y); break;
            case Anim_Direction.right: offsetpos = new Vector2(centerpos.x + offset_quantity, centerpos.y); break;
            case Anim_Direction.up: offsetpos = new Vector2(centerpos.x, centerpos.y + offset_quantity); break;
            case Anim_Direction.down: offsetpos = new Vector2(centerpos.x, centerpos.y - offset_quantity); break;
        }

        return this;
    }
    public UIAnim_ImageTransition Anim_Exit(Action _OnEnd, Sprite sp = null)
    {
        Show();
        if (sp) myImage.sprite = sp;
        anim = true;
        timer = 0;
        isEnter = false;
        OnEnd = _OnEnd;

        switch (direction)
        {
            case Anim_Direction.left: offsetpos = new Vector2(centerpos.x - offset_quantity, centerpos.y); break;
            case Anim_Direction.right: offsetpos = new Vector2(centerpos.x + offset_quantity, centerpos.y); break;
            case Anim_Direction.up: offsetpos = new Vector2(centerpos.x, centerpos.y + offset_quantity); break;
            case Anim_Direction.down: offsetpos = new Vector2(centerpos.x, centerpos.y - offset_quantity); break;
        }

        return this;
    }
    public void Anim()
    {
        anim = true;
        timer = 0;

        switch (direction)
        {
            case Anim_Direction.left: offsetpos = new Vector2(centerpos.x - offset_quantity, centerpos.y); break;
            case Anim_Direction.right: offsetpos = new Vector2(centerpos.x + offset_quantity, centerpos.y); break;
            case Anim_Direction.up: offsetpos = new Vector2(centerpos.x, centerpos.y + offset_quantity); break;
            case Anim_Direction.down: offsetpos = new Vector2(centerpos.x, centerpos.y - offset_quantity); break;
        }
    }


    void TICK_Animate(float lerp_value)
    {
        if (isEnter)
        {
            //entro, voy al centro y me opaco
            Pos = Vector3.Lerp(offsetpos, centerpos, lerp_value);
            myImage.color = Color.Lerp(transparent, white, lerp_value);
        }
        else
        {
            //salgo, voy afuera y me transparento
            Pos = Vector3.Lerp(centerpos, offsetpos, lerp_value);
            myImage.color = Color.Lerp(white, transparent, lerp_value);
        }
    }

    private void Update()
    {
        if(debug)
        {
            if (animate_by_editor_prefs && Input.GetKeyDown(KeyCode.K))
            {
                Anim();
            }

            //if (Input.GetKeyDown(KeyCode.LeftArrow)) Anim(Anim_Direction.left, false, null);
            //if (Input.GetKeyDown(KeyCode.RightArrow)) Anim(Anim_Direction.right, false, null);
            //if (Input.GetKeyDown(KeyCode.UpArrow)) Anim(Anim_Direction.up, false, null);
            //if (Input.GetKeyDown(KeyCode.DownArrow)) Anim(Anim_Direction.down, false, null);
        }

        if (anim)
        {
            if (timer < animation_time)
            {
                timer = timer + 1 * Time.deltaTime;
                TICK_Animate(timer / animation_time);
            }
            else
            {
                timer = 0;
                anim = false;
                myImage.sprite = null;
                OnEnd.Invoke();
                Hide();
            }
        }
    }
}
