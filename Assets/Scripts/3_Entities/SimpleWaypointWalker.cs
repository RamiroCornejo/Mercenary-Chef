using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Extensions;

public class SimpleWaypointWalker : MonoBehaviour
{
    public Transform[] positions;

    Rigidbody rig;

    public bool animate;
    

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (animate)
        {

        }
    }
}
