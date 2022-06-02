using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Tools.Extensions;

public class SituationalDropper : Dropper
{
    [SerializeField] DamageType_ModelsList models = new DamageType_ModelsList();
    List<Tuple<int, GameObject>> catch_tuple = new List<Tuple<int, GameObject>>();
    List<ProbModel> selectedList;
    [SerializeField] bool uniqueSpawn = false;

    [SerializeField] bool hasOneShot = true;
    bool oneShoot = false;

    private void Awake()
    {
        GetComponent<DamageReceiver>().deathEvent += SelectSpawnList;
    }

    void SelectSpawnList(Damage dmg)
    {
        selectedList = models[dmg.DamageType];
        catch_tuple = selectedList.Select(x => x.GetTuple()).ToList();
    }

    public override void Spawn()
    {
        if (hasOneShot)
        {
            if (oneShoot) return;
            oneShoot = true;
        }
        var interations = UnityEngine.Random.Range(minDrop, maxDrop + 1);
        for (int i = 0; i < interations; i++)
        {
            GameObject go = null;
            if (!uniqueSpawn) go = Instantiate(ExtensionsAndUtils.WheelSelection(catch_tuple));
            else go = Instantiate(selectedList[0].model);
            go.transform.position = this.transform.position;
            var rig = go.GetComponent<Rigidbody>();
            if (rig)
            {
                rig.AddForce(Vector3.up * 4, ForceMode.VelocityChange);
            }
        }

        SoundFX.Play_Object_Drop();
    }

    public override void SpawnWithPositions(List<Transform> posList)
    {
        var interations = UnityEngine.Random.Range(minDrop, maxDrop + 1);
        for (int i = 0; i < interations; i++)
        {
            if (posList.Count <= 0) break;
            GameObject go = null;
            if (!uniqueSpawn) go = Instantiate(ExtensionsAndUtils.WheelSelection(catch_tuple));
            else go = Instantiate(selectedList[0].model);
            Transform posSelected = posList[UnityEngine.Random.Range(0, posList.Count)];
            posList.Remove(posSelected);

            go.transform.position = posSelected.position;
            var rig = go.GetComponent<Rigidbody>();
            if (rig)
            {
                rig.AddForce(Vector3.up * 4, ForceMode.VelocityChange);
            }
        }
    }
}
