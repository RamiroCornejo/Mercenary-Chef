using UnityEngine;
public class InventoryDataBase : MonoBehaviour
{
    // 1 Add carne
    // 2 Add zanahoria
    // 3 Add zapallo
    private void Awake() => instance = this;
    public static InventoryDataBase instance;
    public ElementData[] database;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift)) { PlayerInventory.Remove(database[0], 1, UnityEngine.Random.Range(1, 4)); }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) {  PlayerInventory.Add(database[0], 1, UnityEngine.Random.Range(1, 4)); }

        if (Input.GetKeyDown(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftShift)) { PlayerInventory.Remove(database[1], 1, UnityEngine.Random.Range(1, 4)); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {  PlayerInventory.Add(database[1], 1, UnityEngine.Random.Range(1, 4)); }

        if (Input.GetKeyDown(KeyCode.Alpha3) && Input.GetKey(KeyCode.LeftShift)) { PlayerInventory.Remove(database[2], 1, UnityEngine.Random.Range(1, 4)); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { PlayerInventory.Add(database[2], 1, UnityEngine.Random.Range(1, 4)); }

        if (Input.GetKeyDown(KeyCode.Alpha4) && Input.GetKey(KeyCode.LeftShift)) { PlayerInventory.Remove(database[3], 1, UnityEngine.Random.Range(1, 4)); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { PlayerInventory.Add_One_And_Try_Equip(database[3], UnityEngine.Random.Range(1, 4)); }
    }
}

