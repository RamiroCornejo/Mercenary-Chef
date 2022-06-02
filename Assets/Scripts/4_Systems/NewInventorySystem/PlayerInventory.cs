using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;
    private void Awake() => instance = this;

    public int capacity = 10;
    GenericInventory inventory;

    [SerializeField] CanvasGroup canvasGroup;
    bool isOpen = false;

    public static Slot GetSlotByIndexPosition(int slot_position) => instance.inventory.GetSlotByIndex(slot_position);

    const bool FORCE_USE = true;

    private void Start()
    {
        inventory = GetComponentInChildren<GenericInventory>();
        inventory.Build(capacity);
        canvasGroup.alpha = isOpen ? 1 : 0;
        canvasGroup.blocksRaycasts = isOpen ? true : false;
    }

    // PUBLIC STATICS
    public static void Remove(ElementData elem, int quant, int quality, bool respectQuality = false)
    {
        instance.Remove_To_Inventory(elem, quant, quality, respectQuality);
    }

    public static CPResult Add(ElementData elem, int quant, int quality)
    {
        return instance.Add_To_Inventory(elem, quant, quality);
    }

    public static CPResult Add_One_And_Try_Equip(ElementData elem, int quality)
    {
        CPResult result = instance.Add_To_Inventory(elem, 1, quality);
        List<int> position = result.SlotsEquiped;
        instance.Equip(position[0], elem);
        return result;
    }

    public static bool QueryElement(ElementData elem, int quant, int quality, bool respectQuality)
    {
        return instance.Query_Element(elem, quant, quality, respectQuality);
    }

    public static void UseBuff(BuffResult buff)
    {
        instance.Use(buff);
    }

    // PRIVATE FUNCTIONS
    CPResult Add_To_Inventory(ElementData elem, int quant, int quality)
    {
        return inventory.AddElement(elem, quant, quality);
    }
    CPResult Remove_To_Inventory(ElementData elem, int quant, int quality, bool respectQuality = false)
    {
        return inventory.RemoveElement(elem, quant, quality, respectQuality);
       
    }
    bool Query_Element(ElementData elem, int quant, int quality, bool respectQuality)
    {
        return inventory.QueryElement(elem, quant, quality, respectQuality);
    }

    void Use(UsableData _data)
    {
        if (Query_Element(_data, 1, 0, false))
        {
            Usable usable = Instantiate(_data.equiped_item);

            if (usable.CanUse())
            {
                if (_data.isConsumible)
                {
                    var cpresult = Remove_To_Inventory(_data, 1, 0, false);
                    if (cpresult.Process_Successfull)
                    {
                        usable.Basic_PressDown(FORCE_USE);
                        Destroy(usable.gameObject);
                    }
                    else
                    {
                        throw new System.Exception("ERROR: " + cpresult.Message);
                    }
                }
                else
                {
                    usable.Basic_PressDown(FORCE_USE);
                }
            }
        }
    }

    public void Select(UI_Slot ui_slot)
    {
        if (ui_slot.Element)
        {
            inventory.result_message.text = "Select: " + ui_slot;
        }
        else
        {
            inventory.result_message.text = "Select: VACIO";
        }
    }

    public void Use(UI_Slot ui_slot)
    {
        if (ui_slot.Element != null)
        {
            var casted_usable_data = ui_slot.Element as UsableData;

            if (casted_usable_data != null)
            {
                Debug_Inventory_Message("Accept/Usar: " + ui_slot);
                Use(casted_usable_data);
            }
            else
            {
                Debug_Inventory_Message("estas intentando usar algo que no es usable");
            }
        }
        else
        {
            Debug_Inventory_Message("Accept/Usar: VACIO");
        }
    }

    public void Cancel(UI_Slot ui_slot) => Debug_Inventory_Message(ui_slot.Element ? "Cancel: " + ui_slot : "Cancel: VACIO");

    public void Equip(UI_Slot ui_slot)
    {
        int index_position = ui_slot.Slot.Position;

        ElementData element = ui_slot.Element;
        if (element)
        {
            UsableData usabledata = (UsableData)element;
            if (usabledata)
            {
                var usable = usabledata.equiped_item;
                if (usable)
                {
                    UsableManager.Equip(index_position);
                }
            }
        }
    }
    public void Equip(int _position, ElementData _element)
    {
        int index_position = _position;
        ElementData element = _element;

        if (element)
        {
            UsableData usabledata = (UsableData)element;
            if (usabledata)
            {
                var usable = usabledata.equiped_item;
                if (usable)
                {
                    UsableManager.Equip(index_position);
                }
            }
        }
    }

    void Debug_Inventory_Message(string msg) => inventory.result_message.text = msg;

    private void Update()
    {
        if (InputManagerShortcuts.PRESS_OpenInventory)
        {
            isOpen = !isOpen;
            canvasGroup.alpha = isOpen ? 1 : 0;
            canvasGroup.blocksRaycasts = isOpen ? true : false;
            if (isOpen)
            {
                inventory.ui_manager.Open();
                InputManagerShortcuts.OpenMenues();
            }
            else
            {
                InputManagerShortcuts.CloseMenues();
            }
        }

        if (InputManagerShortcuts.PRESS_Equip)
        {
            var slot = MyEventSystem.instance.current.GetComponent<UI_Slot>();
            if (slot != null) Equip(slot);
        }

        if (InputManagerShortcuts.PRESS_Accept)
        {
            var slot = MyEventSystem.instance.current.GetComponent<UI_Slot>();
            if (slot != null) Use(slot);
        }

        if (InputManagerShortcuts.PRESS_Cancel)
        {
            var slot = MyEventSystem.instance.current.GetComponent<UI_Slot>();
            if (slot != null) Cancel(slot);
        }
    }
}
