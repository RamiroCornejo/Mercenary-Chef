using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DistanceSensor : MonoBehaviour
{
    Transform target;
    public Action EnterToCombat = delegate { };
    public Action ExitToCombat = delegate { };
    public bool InCombat { get; private set; }

    [SerializeField] float distanceToCombat = 10;
    [SerializeField] float distanceToLeaveCombat = 20;

    public Transform GetTarget() => target;
    public void SetTarget(Transform newTarget) => target = newTarget;

    public void Refresh()
    {
        if(target == null)
        {
            if (InCombat)
            {
                ExitToCombat();
                InCombat = false;
            }
            return;
        }

        float distance = (transform.position - target.position).sqrMagnitude;

        if (InCombat && distance > distanceToLeaveCombat * distanceToLeaveCombat)
        {
            ExitToCombat();
            InCombat = false;
        }
        else if (!InCombat && distance <= distanceToCombat * distanceToCombat)
        {
            EnterToCombat();
            InCombat = true;
        }
    }

    public bool HasTargetInDistance(float dis, Transform _target = null)
    {
        if (!_target) _target = target;
        float distance = (transform.position - _target.position).sqrMagnitude;

        if (distance <= dis * dis) return true;
        else return false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceToCombat);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToLeaveCombat);
    }
}
