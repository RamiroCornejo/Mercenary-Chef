using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_ContainerManager : MonoBehaviour
{
    [SerializeField] RectTransform myParent;

    [SerializeField] UI_Slot[] mySlots;

    public void Open()
    {
        mySlots[0].Select();
    }

    public void Build(Container container)
    {
        int cap = container.Capacity;

        mySlots = new UI_Slot[cap];

        for (int i = 0; i < cap; i++)
        {
            var slot = GameObject.Instantiate(UI_InventoryDataBase.SlotUIModel, myParent);
            slot.ConfigureSlot(container.GetSlotByIndex(i));
            slot.Refresh();

            mySlots[i] = slot;
        }
    }

    public void Refresh()
    {
        foreach (var s in mySlots)
        {
            s.Refresh();
        }
    }
}
