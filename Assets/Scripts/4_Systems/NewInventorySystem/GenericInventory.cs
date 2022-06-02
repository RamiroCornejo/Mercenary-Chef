using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenericInventory : MonoBehaviour
{
    [SerializeField] int capacity;
    [SerializeField] Container container;
    public UI_ContainerManager ui_manager;
    public TextMeshProUGUI result_message;

    public void Build(int capacity)
    {
        container = new Container(capacity);
        ui_manager.Build(container);
    }

    public Slot GetSlotByIndex(int index_position) => container.GetSlotByIndex(index_position);

    public CPResult AddElement(ElementData data, int quantity, int quality)
    {
        var result = container.Add_Element(data, quantity, quality);

        result_message.color = result.Process_Successfull ? Color.green : Color.red;
        result_message.text = "[" + result.Remainder_Quantity + "] " + result.Message;

        ui_manager.Refresh();

        return result;
    }

    public CPResult RemoveElement(ElementData data, int quantity, int quality, bool respectQuality = false)
    {

        var result = container.Remove_Element(data, quantity, quality, respectQuality);

        result_message.color = result.Process_Successfull ? Color.green : Color.red;
        result_message.text = "[" + result.Remainder_Quantity + "] " + result.Message;

        ui_manager.Refresh();

        return result;
    }

    public bool QueryElement(ElementData data, int quantity, int quality, bool respectQuality = false)
    {
        return container.QueryElement(data, quantity, quality, respectQuality);
    }

}
