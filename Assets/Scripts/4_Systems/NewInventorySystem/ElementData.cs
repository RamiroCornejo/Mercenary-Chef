using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "ScriptableObjects/Element", order = 1)]
public class ElementData : ScriptableObject
{
    #region Editor variables
    [Header("Item Data")]
    [SerializeField] new string name = "default_element";
    [SerializeField] int element_ID = -1;
    [SerializeField] int max_stack = 10;
    [SerializeField] [Multiline(4)] string description = "this is a element description";
    [SerializeField] float weight = 1.0f;
    [SerializeField] int max_quality;

    [Header("Representation")]
    [SerializeField] Sprite image;
    [SerializeField] GameObject world_model;
    [SerializeField] GameObject equipable_model;
    #endregion

    #region Getters
    public int Element_ID => element_ID;
    public string Element_Name => this.name;
    public int MaxStack => max_stack;
    public string Description => description;
    public float Weight => weight;
    public Sprite Element_Image => image;
    public int MaxQuality => max_quality;
    public GameObject Model => world_model;
    public GameObject Model_Visual_Equipable => equipable_model;

    #endregion

    #region Object override
    public override bool Equals(object other)
    {
        var elem = (ElementData)other;
        return elem.name == this.name &&
            elem.max_stack == max_stack &&
            elem.description == description &&
            elem.weight == weight;
    }

    public override int GetHashCode()
    {
        var hashCode = 2056222142;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + element_ID.GetHashCode();
        hashCode = hashCode * -1521134295 + max_stack.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(description);
        hashCode = hashCode * -1521134295 + weight.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(image);
        return hashCode;
    }

    #endregion

}
