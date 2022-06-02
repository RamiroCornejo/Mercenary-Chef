using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnerWithProb : Dropper
{
    [SerializeField] EnemyBase entityToSpawn = null;
    [SerializeField] Transform posToSpawn = null;
    [SerializeField, Range(0, 1)] float probToSpawn = 0.5f;

    public override void Spawn()
    {
        var interations = UnityEngine.Random.Range(minDrop, maxDrop + 1);
        for (int i = 0; i < interations; i++)
        {
            var randomValue = Random.value;

            if (randomValue > probToSpawn) continue;

            var newEntity = Instantiate(entityToSpawn);

            newEntity.transform.position = posToSpawn.position;
        }
    }

    public override void SpawnWithPositions(List<Transform> posList)
    {
        var interations = UnityEngine.Random.Range(minDrop, maxDrop + 1);
        for (int i = 0; i < interations; i++)
        {
            var randomValue = Random.value;

            if (randomValue > probToSpawn) continue;
            if (posList.Count <= 0) break;
            var newEntity = Instantiate(entityToSpawn);
            Transform posSelected = posList[UnityEngine.Random.Range(0, posList.Count)];
            posList.Remove(posSelected);

            newEntity.transform.position = posSelected.position;
        }
    }
}
