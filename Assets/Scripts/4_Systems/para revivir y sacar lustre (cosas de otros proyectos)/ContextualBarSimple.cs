using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContextualBarSimple : MonoBehaviour
{
    public static ContextualBarSimple instance;
    private void Awake()
    {
        instance = this;
        myRect = GetComponent<RectTransform>();
        Reset_All();
    }
    private void Start()
    {
        canvas_group.alpha = 0;
        generic_bar_fill.Configure(1);
        generic_bar_fill.SetValue(0);
        //Main.instance.eventManager.SubscribeToEvent(GameEvents.CHANGE_INPUT, OnChange);
    }

    [SerializeField] GenericBar_Sprites generic_bar_fill = null;
    [Header("Auxiliar Components")]
    [SerializeField] Image photo_image = null;

    [Header("Animation")]
    public CanvasGroup canvas_group = null;
    public float transition_animation_time = 1f;

    bool modif_type = false;
    InputImageDatabase.InputImageType type;

    RectTransform myRect;

    void Reset_All()
    {
        generic_bar_fill.Configure(1);
        generic_bar_fill.SetValue(0);
        photo_image.sprite = null;
    }

    public ContextualBarSimple Show()
    {
        myRect.ForceUpdateRectTransforms();
        canvas_group.alpha = 1;
        return this;
    }

    public ContextualBarSimple Hide()
    {
        canvas_group.alpha = 0;
        Reset_All();
        return this;
    }



    private void OnChange(params object[] p) 
    { 
        if (!modif_type) return; 
        photo_image.sprite = InputImageDatabase.instance.GetSprite(type);
    }


    public void ResetBar() { generic_bar_fill.Configure(1); generic_bar_fill.SetValue(0); }
    public ContextualBarSimple Set_Sprite_Photo(Sprite val, bool hasBackground = true) { photo_image.sprite = val; return this; }
    public ContextualBarSimple Set_Values_Load_Bar(float max, float current) { generic_bar_fill.Configure(max); generic_bar_fill.SetValue(current); return this; }
    public ContextualBarSimple Set_Sprite_Button_Custom(InputImageDatabase.InputImageType val) { type = val; modif_type = true; photo_image.sprite = InputImageDatabase.instance.GetSprite(type); return this; }


}
