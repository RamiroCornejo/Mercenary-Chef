using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public event Action<GameObject> callBack;

    public void AddEventListener(Action<GameObject> _callback) { callBack += _callback; }

    public void Initialize()
    {
        //var sensor = gameObject
        //         .AddComponent<Sensor>()
        //         .Initialize()
        //         .Configure(GameManager.instancia.playerlayermask)
        //         .SubscribeAction(OnTriggerEnterSensor);
    }

    void OnTriggerEnterSensor(GameObject go)
    {

        callBack(go);
    }
}
