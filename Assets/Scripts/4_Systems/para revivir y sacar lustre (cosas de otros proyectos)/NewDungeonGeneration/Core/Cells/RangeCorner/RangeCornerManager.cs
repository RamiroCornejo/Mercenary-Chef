using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class RangeCornerManager : MonoBehaviour
{
    public List<RangeCorners> ranges;

    public bool rebuild;

    private void Update()
    {
        if (rebuild)
        {
            rebuild = false;
            ranges = this.GetComponentsInChildren<RangeCorners>().ToList() ;
        }
    }

    public void Initialize()
    {
        foreach (var r in ranges)
        {
            r.Initialize();
        }
    }
}
