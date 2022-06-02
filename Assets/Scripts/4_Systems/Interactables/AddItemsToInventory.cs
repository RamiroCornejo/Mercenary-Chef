using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Extensions;
using System;
using UnityEngine.Events;

public class AddItemsToInventory : MonoBehaviour
{
    [SerializeField] KeyValue<ElementData, int>[] items = new KeyValue<ElementData, int>[0];

    List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();

    [SerializeField] Transform myTransform;

    [SerializeField] float anim_speed = 5;

    [SerializeField] UnityEvent OnTakeSucessfull;

    float timer;
    bool anim;
    Vector3 anim_initial_pos;

    public bool hasOneShot = true;
    bool oneShot;

    private void Start()
    {
        tuples.Add(Tuple.Create(8, 1));
        tuples.Add(Tuple.Create(4, 2));
        tuples.Add(Tuple.Create(2, 3));
    }

    public void AddItems()
    {
        if (hasOneShot)
        {
            if (oneShot) return;
            oneShot = true;
        }

        for (int i = 0; i < items.Length; i++)
        {
            //viejo
            //ProvisionalInventory.instance.AddElement(items[i].key, items[i].value);

            //nuevo
            var process_result = PlayerInventory.Add(items[i].key, items[i].value, ExtensionsAndUtils.WheelSelection<int>(tuples));

            if (process_result.Process_Successfull)
            {
                OnTakeSucessfull.Invoke();
            }
            else
            {
                var quantity_to_drop = process_result.Remainder_Quantity;
                return;
            }
            
        }

        anim = true;
        anim_initial_pos = myTransform.position;
    }

    private void Update()
    {
        if (anim)
        {
            if (timer < 1)
            {
                timer = timer + anim_speed * Time.deltaTime;
                myTransform.position = Vector3.Lerp(anim_initial_pos, Character.instance.transform.position, timer);
            }
            else
            {
                anim = false;
                timer = 0;
                GameObject.Destroy(myTransform.gameObject);//o regresar al pool
            }
        }
    }


}
