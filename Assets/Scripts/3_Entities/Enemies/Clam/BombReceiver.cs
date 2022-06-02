using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BombReceiver : MonoBehaviour
{
    public Action OnGetBomb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ObjectThrowable>())
        {
            OnGetBomb.Invoke();
            Destroy(other.gameObject);
        }
    }
}
