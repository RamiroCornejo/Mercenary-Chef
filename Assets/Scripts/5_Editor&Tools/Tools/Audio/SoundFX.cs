using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public static SoundFX instance;
    private void Awake() => instance = this;

    //Maquinas
    [SerializeField] AudioClip sarten_hit;
    [SerializeField] AudioClip object_drop;

    [SerializeField] AudioClip potion_consume;
    [SerializeField] AudioClip potion_pick_up;

    public void Start()
    {
        if (sarten_hit) AudioManager.instance.GetSoundPoolFastRegistry2D(sarten_hit);
        if (object_drop) AudioManager.instance.GetSoundPoolFastRegistry2D(object_drop);
        if (potion_consume) AudioManager.instance.GetSoundPoolFastRegistry2D(potion_consume);
        if (potion_pick_up) AudioManager.instance.GetSoundPoolFastRegistry2D(potion_pick_up);
    }

    public static void Play_Sarten_Hit() => AudioManager.instance.PlaySound(instance.sarten_hit.name);
    public static void Play_Object_Drop() => AudioManager.instance.PlaySound(instance.object_drop.name);
    public static void Play_Potion_Consume() => AudioManager.instance.PlaySound(instance.potion_consume.name);
    public static void Play_Potion_PickUp() => AudioManager.instance.PlaySound(instance.potion_pick_up.name);


}
