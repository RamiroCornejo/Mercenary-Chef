using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BunnyPositions : MonoBehaviour
{
    [SerializeField] Transform[] food_positions;
    [SerializeField] Transform[] hide_positions;

    private void Awake()
    {
        food_positions = this.gameObject.GetComponentsInChildren<BunnyCarrot>().Select(x => x.transform).ToArray();
        hide_positions = this.gameObject.GetComponentsInChildren<BunnyCave>().Select(x => x.transform).ToArray();
        var bunnies = GetComponentsInChildren<SimpleFlee>();
        foreach (var b in bunnies)
        {
            b.SetZone(this);
        }
    }

    public Transform RandomHidePosition()
    {
        if (hide_positions.Length > 0)
            return hide_positions[Random.Range(0, hide_positions.Length - 1)];
        else return null;
    }
    public Transform RandomFoodPosition()
    {
        if (food_positions.Length > 0)
            return food_positions[Random.Range(0, food_positions.Length - 1)];
        else return null;

    }
}
