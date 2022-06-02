using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Extensions;


[ExecuteInEditMode]
public class ClampCollapse : MonoBehaviour
{
    public bool execute;

    private void Update()
    {
        if (execute)
        {
            var childs = transform.GetComponentsInChildren<Transform>();

            foreach (var c in childs)
            {
                c.position = c.position.ClampV3ZeroDotFive();
                c.eulerAngles = c.eulerAngles.ClampV3ZeroDotFive();
                c.localScale = c.localScale.ClampV3ZeroDotFive();
            }

            
        }
    }
}
