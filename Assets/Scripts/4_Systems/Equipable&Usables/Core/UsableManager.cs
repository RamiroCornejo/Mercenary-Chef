using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableManager : MonoBehaviour
{
    public static UsableManager instance;
    
    public Transform parent_behaviours;
    [SerializeField] SlotUsable[] places;
    public Dictionary<SpotType, SlotUsable> registry = new Dictionary<SpotType, SlotUsable>();
    UsableManager_UI frontend;
    SecondHandInventoryFavs favs;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        InitialRegisterPlaces();
        frontend = GetComponent<UsableManager_UI>();
        frontend.Initialize();
    }

    private void Update()
    {
        if (Input.GetButtonDown("SecondHand")) SecondHand_PRESS();
        if (Input.GetButtonUp("SecondHand")) SecondHand_RELEASE();
    }

    #region Second Hand
    public void SecondHand_PRESS()
    {
        SlotUsable slot_usable = registry[SpotType.Secondhand];
        if (slot_usable == null) return;

        if (registry[SpotType.Secondhand].Current)
        {
            registry[SpotType.Secondhand].Current.Basic_PressDown();
        }
    }
    public void SecondHand_RELEASE()
    {
        SlotUsable slot_usable = registry[SpotType.Secondhand];
        if (slot_usable == null) return;

        if (slot_usable.Current)
        {
            if (slot_usable.Quantity > 0)
            {
                registry[SpotType.Secondhand].Current.Basic_PressUp();
            }
        }
    }
    #endregion

    //esto, viene del character, es el custom usable
    public void UseFirstHand(Usable.UsableHitInfo<DamageReceiver> hit_info)
    {
        if (registry[SpotType.Firsthand].Current)
        {
            registry[SpotType.Firsthand].Current.Basic_CustomUse(hit_info);
        }
        else
        {
            Debug.Log("No tengo nada en la primer mano");
        }
    }

    

    public static void Equip(int _position) => instance.Equip_Usable(_position);
    void Equip_Usable(int _position)
    {
        UsableData usable_data = (UsableData)PlayerInventory.GetSlotByIndexPosition(_position).Element;
        SpotType spot = usable_data.equiped_item.Spot;

        
        if (registry.ContainsKey(spot))
        {
            registry[spot].TryEquip(_position, usable_data.equiped_item, spot, usable_data.Model_Visual_Equipable);
        }

        frontend.RefreshAll(registry);
    }
    void AddToFavs()
    {

    }

    #region Registro
    void InitialRegisterPlaces()
    {
        for (int i = 0; i < places.Length; i++)
        {
            if (!registry.ContainsKey(places[i].spot)) registry.Add(places[i].spot, places[i]);
            else throw new System.Exception("Ojo que en el array de diseño estas repitiendo dos spots iguales");
        }
    }
    #endregion
}

[System.Serializable]
public class SlotUsable
{
    //NO TIENE CONSTRUCTOR, porque la idea es usar la funcion serializable y asignarlo por editor
    private int index_position = -1;
    public Transform parent = null;
    public SpotType spot = SpotType.NoWereable;
    Usable current = null;

    #region Public Getters
    public int Index_position => index_position;
    public int Quantity => mySlot.Quantity;
    public Usable Current => current;
    public Slot mySlot
    {
        get
        {
            if (ContainsSomething) return PlayerInventory.GetSlotByIndexPosition(index_position);
            else return null;
        }
    }
   
    public bool ContainsSomething
    {
        get
        {
            if (index_position != -1 && current != null) return true;
            else return false;
        }
    }
    #endregion

    GameObject to_destroy;

    public bool Try_Press()
    {
        if (ContainsSomething)
        {
            if (Quantity > 0)
            {
                current.Basic_PressDown();
                return true;
            }
        }
        return false;
    }
    public bool Try_Release()
    {
        if (ContainsSomething)
        {
            if (Quantity > 0)
            {
                current.Basic_PressDown();
                return true;
            }
        }
        return false;
    }

    public void TryEquip(int _index_position, Usable usable, SpotType spot, GameObject model)
    {
        if (current != null)
        {
            Debug.Log("tengo algo");
            if (usable.KeyName == current.KeyName)
            {
                to_destroy = current.gameObject;
                current.UnEquip();
                current = null;
                GameObject.Destroy(parent.GetComponentInChildren<Usable>().gameObject);
                Character.instance.visualEquip.Empty(usable.Spot);
                index_position = -1;
            }
            else
            {
                Debug.Log("Es distinto, lo voy a remplazar");

                current.UnEquip();
                GameObject.Destroy(parent.GetComponentInChildren<Usable>().gameObject);
                current = null;

                current = GameObject.Instantiate(usable, parent);
                current = usable;
                current.Equip();

                Character.instance.visualEquip.Equip(spot, model);

                index_position = _index_position;
            }
        }
        else
        {
            Debug.Log("No tengo nada");
            current = GameObject.Instantiate(usable, parent);
            current = usable;
            current.Equip();
            Character.instance.visualEquip.Equip(spot, model);
            index_position = _index_position;
        }
    }
}
