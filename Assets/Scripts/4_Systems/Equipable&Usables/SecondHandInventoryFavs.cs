using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondHandInventoryFavs : MonoBehaviour
{
    public const int MAX_FAVS = 3;

    int[] favs = new int[MAX_FAVS];
    private void Awake()
    {
        for (int i = 0; i < MAX_FAVS; i++) favs[i] = -1;
    }

    public void AddFavs(int _inventory_index_to_add)
    {
        for (int i = 0; i < MAX_FAVS; i++)
        {

        }
    }
}
