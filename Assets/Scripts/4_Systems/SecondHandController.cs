using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondHandController : MonoBehaviour
{

    [SerializeField] SecondHandSlotRotable the_master;
    [SerializeField] SecondHandSlotRotable left;
    [SerializeField] SecondHandSlotRotable right;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            the_master.RotateLeft(() => { });
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            the_master.RotateRight(() => { });
        }
    }

    public void AddSlot(Slot slot)
    {


    }
    public void RemoveSlot(Slot slot)
    {

    }
}
