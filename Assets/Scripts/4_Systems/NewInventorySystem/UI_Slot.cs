using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Slot : Selectable, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] Image myBkgSlot;
    [SerializeField] Image myElementImage;
    [SerializeField] Image elementQuality;
    [SerializeField] TextMeshProUGUI myCant;
    [SerializeField] CanvasGroup content;

    Slot slot;
    public ElementData Element => slot.Element ? slot.Element : null;
    public Slot Slot => slot;


    [SerializeField] GameObject Tab_Object;

    public void ConfigureSlot(Slot slot)
    {
        this.slot = slot;
    }

    public void Refresh()
    {
        if (slot == null) return;
        if (slot.Stack == null)
        {
            Hide();
            return;
        }
        var stack = slot.Stack;
        if (!slot.Stack.IsEmpty)
        {
            var element = slot.Stack.Element;

            Show();
            myElementImage.sprite = element.Element_Image;
            myCant.enabled = true;
            myCant.text = stack.Quantity.ToString();
            elementQuality.sprite = UI_InventoryDataBase.GetElementByQuality(stack.Quality);
            Tab_Object.SetActive(stack.IsTabbed);

            //if (element != null)
            //{
               
            //}
            //else
            //{
            //    Hide();
            //}
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        content.alpha = 0;
    }
    void Show()
    {
        content.alpha = 1;
        content.blocksRaycasts = false;
    }

    

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop: " + gameObject.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Debug.Log(eventData.pointerId);
        if (eventData.pointerId == -1) PlayerInventory.instance.Equip(this);
        if (eventData.pointerId == -2) PlayerInventory.instance.Use(this);
        if (eventData.pointerId == -3) PlayerInventory.instance.Cancel(this);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {

        base.OnPointerEnter(eventData);
        
        myBkgSlot.sprite = UI_InventoryDataBase.SlotEnter;
        ElementInfo.Show(slot.Stack);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        myBkgSlot.sprite = UI_InventoryDataBase.SlotNormal;
        ElementInfo.Hide();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        myBkgSlot.sprite = UI_InventoryDataBase.SlotEnter;
        ElementInfo.Show(slot.Stack);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        myBkgSlot.sprite = UI_InventoryDataBase.SlotNormal;
        ElementInfo.Hide();
    }

}
