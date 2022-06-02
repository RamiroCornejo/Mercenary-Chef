using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrap : Trap
{
    [SerializeField] MinMax<int> spawnAmmount = new MinMax<int>();

    [SerializeField] MinMax<float> timeToSpawn = new MinMax<float>();

    [SerializeField] RockDamage prefab = null;
    RockPool rockPool;

    [SerializeField] Transform startPosition = null;
    [SerializeField] Transform endPosition = null;

    [SerializeField] float width = 3;
    [SerializeField] Mesh meshOnlyForGizmo = null;

    float currentTime;
    float timer;
    protected override void OnInitialize()
    {
        ActivateTrap();
        rockPool = new GameObject($"{prefab.name} pool").AddComponent<RockPool>();
        rockPool.transform.SetParent(transform);
        rockPool.Configure(prefab);
        rockPool.Initialize(3);
    }

    protected override void OnActivate()
    {
        currentTime = Random.Range(timeToSpawn.min, timeToSpawn.max);
    }

    protected override void OnDesactivate()
    {
        timer = 0;
    }

    protected override void OnUpdate()
    {
        timer += Time.deltaTime;

        if(timer >= currentTime)
        {
            SpawnRocks();
        }
    }

    public void SpawnRocks()
    {
        int spawnAmmountCurrent = Random.Range(spawnAmmount.min, spawnAmmount.max);
        for (int i = 0; i < spawnAmmountCurrent; i++)
        {
            var newRock = ActiveRock();
            Vector3 center = Vector3.Lerp(startPosition.position, endPosition.position, Random.Range(0f, 1f));

            Vector3 pointA = center + startPosition.right * width;
            Vector3 pointB = center - startPosition.right * width;

            Vector3 point = Vector3.Lerp(pointA, pointB, Random.Range(0f, 1f));
            newRock.transform.position = point;
        }
        timer = 0;
        currentTime = Random.Range(timeToSpawn.min, timeToSpawn.max);
    }
    private void OnDrawGizmos()
    {
        Vector3 center = Vector3.Lerp(startPosition.position, endPosition.position, 0.5f);
        float y = (startPosition.position - endPosition.position).magnitude;

        Vector3 pointA = center + startPosition.right * width;
        Vector3 pointB = center - startPosition.right * width;
        float x = (pointA - pointB).magnitude;

        Gizmos.DrawWireMesh(meshOnlyForGizmo, center, startPosition.rotation, new Vector3(x, 1, y));

        Gizmos.DrawSphere(pointA, 1);
        Gizmos.DrawSphere(pointB, 1);
    }

    public RockDamage ActiveRock()
    {
        RockDamage aS = rockPool.Get();
        aS.Initialize(this);
        return aS;
    }

    public void ReturnRock(RockDamage corpse)
    {
        rockPool.ReturnToPool(corpse);
    }

}
