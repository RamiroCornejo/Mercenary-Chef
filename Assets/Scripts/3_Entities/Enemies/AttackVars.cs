using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AttackVars
{
    public float anticipationTime;
    public float damage;
    public float knockBack;
    public Transform root;

    public Vector3 OwnerPos() => root.position;
}
