using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public static PlayerStart instance;
    private void Awake()
    {
        if (instance) { Destroy(this.gameObject); return; }
        else { instance = this; }
    }

    private void Start()
    {
        Character.instance.gameObject.transform.position = transform.position;
    }

    public static Vector3 Position => instance.transform.position;
}
