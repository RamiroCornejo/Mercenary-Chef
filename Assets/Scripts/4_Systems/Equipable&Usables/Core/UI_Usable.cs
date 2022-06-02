using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Usable : MonoBehaviour
{
    public SpotType spot_type;
    public Image item_photo;
    public Image image_to_fill;
    public TextMeshProUGUI amount_info;

    public void SetImage(Sprite _sp)
    {
        item_photo.sprite = _sp;
    }

    public void Hide()
    {
        item_photo.sprite = null;
    }

    public void HideFillValue()
    {
        image_to_fill.enabled = false;
    }

    public void SetAmount(int _amount)
    {
        if(amount_info) amount_info.text = _amount.ToString();
    }

    public void SetFill(float fill)
    {
        image_to_fill.fillAmount = fill;
    }

    public void TouchUseAnimation()
    {
        //anim
    }
}
