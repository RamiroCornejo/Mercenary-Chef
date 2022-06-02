using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dropper : MonoBehaviour
{
    [SerializeField] protected int maxDrop = 3;
    [SerializeField] protected int minDrop = 1;
    public abstract void Spawn();

    public abstract void SpawnWithPositions(List<Transform> posList);
}
