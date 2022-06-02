using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class SampleEntityDetector<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected float distance = 2;
    [SerializeField] protected LayerMask mask = 0 << 0;
    [SerializeField] protected float angle = 180;
    public T owner = null;

    public void DetecteEntitie(Vector3 rotPos, Vector3 forwardDir, QueryTriggerInteraction triggersInteract)
    {
        var entities = Physics.OverlapSphere(rotPos, distance, mask, triggersInteract)
            .Where(x =>
            {
                var entity = x.GetComponent<T>();
                if(entity != null)Debug.Log(entity.name);
                return entity != null && entity != owner;
            })
            //.Where(x =>
            //{
            //    Vector3 dir = x.transform.position - rotPos;
            //    float angleTemp = Vector3.Angle(forwardDir, dir);

            //    return dir.magnitude <= distance && angleTemp < angle;
            //})
            .Select(x => x.GetComponent<T>())
            .ToArray();

        if (entities.Length > 0) DoSomethingWithEntities(entities);
        else NoEntitiesOnRange();
    }

    public K[] DetecteEntitie<K>(Vector3 rotPos, Vector3 forwardDir, float _distance, LayerMask _mask, K _owner, float _angle,
                               QueryTriggerInteraction triggersInteract) where K : MonoBehaviour
    {
        var entities = Physics.OverlapSphere(rotPos, _distance, _mask, triggersInteract)
            .Where(x =>
            {
                var entity = x.GetComponent<K>();
                return entity != null && entity != _owner;
            })
            .Where(x =>
            {
                Vector3 dir = x.transform.position - rotPos;
                float angleTemp = Vector3.Angle(forwardDir, dir);

                return angleTemp < _angle;
            })
            .Select(x => x.GetComponent<K>())
            .ToArray();

        return entities;
    }

    protected abstract void DoSomethingWithEntities(T[] entities);
    protected abstract void NoEntitiesOnRange();
}
