using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physical_Entity : MonoBehaviour
{
    Rigidbody myrig;
    protected Rigidbody MyRig => myrig;
    protected Transform myTransform => myrig.transform;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        myrig = GetComponent<Rigidbody>();
    }


    public void KnockBack(float owner_force, Vector3 owner_position)
    {
        myrig = GetComponent<Rigidbody>();
        if (myrig == null) return;

        var dir = this.transform.position - owner_position;
        dir.Normalize();

        myrig.AddForce(dir * owner_force, ForceMode.VelocityChange);
    }
}
