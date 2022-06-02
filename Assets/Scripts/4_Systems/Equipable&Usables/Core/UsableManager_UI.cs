using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableManager_UI : MonoBehaviour
{
    Dictionary<SpotType, UI_Usable> ui_usables = new Dictionary<SpotType, UI_Usable>();
    [SerializeField] RectTransform parent;

    public void Initialize()
    {
        Intialize_Get_UI_Usables();
    }

    void Intialize_Get_UI_Usables()
    {
        UI_Usable[] usables = parent.GetComponentsInChildren<UI_Usable>();
        for (int i = 0; i < usables.Length; i++)
        {
            SpotType spot = usables[i].spot_type;

            if (!ui_usables.ContainsKey(spot))
            {
                ui_usables.Add(spot, usables[i]);
            }
            else throw new System.Exception("hay dos items con el mismo slot");
        }
    }

    public void RefreshAll(Dictionary<SpotType, SlotUsable> registry)
    {
        foreach (var r in registry)
        {
            RefreshOneSpot(r.Key, r.Value);
        }
    }
    public void RefreshOneSpot(SpotType spot, SlotUsable slot)
    {
        if (slot.Index_position == -1) return;

        if (ui_usables.ContainsKey(spot))
        {
            UI_Usable ui = ui_usables[spot];
            
            Slot slot_info = PlayerInventory.GetSlotByIndexPosition(slot.Index_position);

            ui.SetAmount(slot_info.Quantity);
            ui.SetImage(slot_info.Element.Element_Image);

            if (slot.Current)
            {
                if (slot.Current.CooldownModule != null)
                {
                    ui.SetFill(slot.Current.CooldownModule.Cooldown);
                }
                else
                {
                    ui.HideFillValue();
                }
            }
        }
        else
        {
            throw new System.Exception("No tengo ese spot en los registros del UI");
        }
    }
}
