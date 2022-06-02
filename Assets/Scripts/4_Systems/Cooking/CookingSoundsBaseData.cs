using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSoundsBaseData : MonoBehaviour
{
    public static CookingSoundsBaseData Instance { get; private set; } 
    [SerializeField] AudioClip C5 = null;
    [SerializeField] AudioClip D5 = null;
    [SerializeField] AudioClip E5 = null;
    [SerializeField] AudioClip G5 = null;

    [SerializeField] AudioClip[] perfectSounds = new AudioClip[0];
    [SerializeField] AudioClip[] goodSounds = new AudioClip[0];
    [SerializeField] AudioClip[] failSounds = new AudioClip[0];

    Dictionary<SoundNotes, string> baseData = new Dictionary<SoundNotes, string>();
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < perfectSounds.Length; i++)
            AudioManager.instance.GetSoundPool(perfectSounds[i].name, AudioManager.SoundDimesion.TwoD, perfectSounds[i]);
        for (int i = 0; i < goodSounds.Length; i++)
            AudioManager.instance.GetSoundPool(goodSounds[i].name, AudioManager.SoundDimesion.TwoD, goodSounds[i]);
        for (int i = 0; i < perfectSounds.Length; i++)
            AudioManager.instance.GetSoundPool(failSounds[i].name, AudioManager.SoundDimesion.TwoD, failSounds[i]);

        AudioManager.instance.GetSoundPool(C5.name, AudioManager.SoundDimesion.TwoD, C5);
        AudioManager.instance.GetSoundPool(D5.name, AudioManager.SoundDimesion.TwoD, D5);
        AudioManager.instance.GetSoundPool(E5.name, AudioManager.SoundDimesion.TwoD, E5);
        AudioManager.instance.GetSoundPool(G5.name, AudioManager.SoundDimesion.TwoD, G5);

        baseData.Add(SoundNotes.C5, C5.name);
        baseData.Add(SoundNotes.D5, D5.name);
        baseData.Add(SoundNotes.E5, E5.name);
        baseData.Add(SoundNotes.G5, G5.name);
    }


    public static void PlaySound(SoundNotes note)
    {
        Instance.PlaySoundNote(note);
    }

    public static void PlayPerfectNote()
    {
        Instance.PlayPerfect();
    }

    public static void PlayGoodNote()
    {
        Instance.PlayGood();
    }

    public static void PlayFailNote()
    {
        Instance.PlayFail();
    }

    void PlayPerfect()
    {
        int random = UnityEngine.Random.Range(0, perfectSounds.Length);

        AudioManager.instance.StopAllSounds(perfectSounds[random].name);
        AudioManager.instance.PlaySound(perfectSounds[random].name);
    }

    void PlayGood()
    {
        int random = UnityEngine.Random.Range(0, goodSounds.Length);

        AudioManager.instance.StopAllSounds(goodSounds[random].name);
        AudioManager.instance.PlaySound(goodSounds[random].name);
    }

    void PlayFail()
    {
        int random = UnityEngine.Random.Range(0, failSounds.Length);

        AudioManager.instance.StopAllSounds(failSounds[random].name);
        AudioManager.instance.PlaySound(failSounds[random].name);
    }

    void PlaySoundNote(SoundNotes note)
    {
        AudioManager.instance.PlaySound(baseData[note]);
    }
}

public enum SoundNotes { C5, D5, E5, G5 }