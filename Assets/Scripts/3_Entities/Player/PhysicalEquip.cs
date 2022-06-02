using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEquip : MonoBehaviour
{
    Spot[] spots = new Spot[0];
    Dictionary<SpotType, Spot> registry = new Dictionary<SpotType, Spot>();

    private void Awake()
    {
        spots = this.GetComponentsInChildren<Spot>();

        for (int i = 0; i < spots.Length; i++)
        {
            if (!registry.ContainsKey(spots[i].spotType))
            {
                registry.Add(spots[i].spotType, spots[i]);
            }
            else
            {
                Debug.LogWarning("OJO que hay dos spots iguales");
            }
        }
        spots = null;
    }

    public void Empty(SpotType spot_type)
    {
        if (registry.ContainsKey(spot_type))
        {
            registry[spot_type].Empty();
        }
    }

    public void Equip(SpotType spot_type, GameObject model)
    {
        if (registry.ContainsKey(spot_type))
        {
            registry[spot_type].Equip_Visuals(model);
        }
    }
}
