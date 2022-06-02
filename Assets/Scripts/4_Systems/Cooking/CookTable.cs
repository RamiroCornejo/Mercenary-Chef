using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Assets.Scripts;

public class CookTable : MonoBehaviour
{
    [SerializeField] CookInputBind[] inputBind = new CookInputBind[0];

    [SerializeField] Animator textAnim = null;
    [SerializeField] TextMeshPro text = null;
    [SerializeField] Animator knife = null;

    [SerializeField] Transform _triggerExitTipPosition = null;
    [SerializeField] Transform _triggerEnterTipPosition = null;
    [SerializeField] Transform _triggerEnterBasePosition = null;
    [SerializeField] float _forceAppliedToCut = 4;

    [SerializeField] AudioSource baseSource = null;

    [SerializeField] AudioClip[] cutSounds = new AudioClip[0];

    bool on;

    private void Start()
    {
        for (int i = 0; i < cutSounds.Length; i++)
        {
            AudioManager.instance.GetSoundPool(cutSounds[i].name, AudioManager.SoundDimesion.TwoD, cutSounds[i]);
        }
    }

    public void StartRecipe(Recipe recipe)
    {
        on = true;
        for (int i = 0; i < inputBind.Length; i++)
        {
            inputBind[i].noteSound = recipe.notes[i];
        }
        baseSource.clip = recipe.baseSound;
        baseSource.Play();
        MusicPlay.instance.StartMusic(recipe);
    }

    public void EndRecipe()
    {
        on = false;

        baseSource.Stop();
    }

    private void Update()
    {
        if (!on) return;

        for (int i = 0; i < inputBind.Length; i++)
        {
            var bind = inputBind[i];

            if (Input.GetButtonDown(bind.input))
            {
                PositionateKnife(bind.section.transform.position);

                if (!bind.section.ActionDown())
                {
                    PlayCutSound();
                }
                break;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    PositionateKnife(inputBind[0].section.transform.position);
        //    SliceIngredient(ingredientToCut);
        //}
    }

    public void PosisionateNote(Note note, int section)
    {
        note.transform.position = inputBind[section].section.transform.position;
    }

    void PositionateKnife(Vector3 knifePos)
    {
        knife.transform.position = new Vector3(knifePos.x, knife.transform.position.y, knife.transform.position.z);
        float rotX = UnityEngine.Random.Range(-24.297f, 11.354f);

        knife.transform.localEulerAngles = new Vector3(rotX, knife.transform.localEulerAngles.y, knife.transform.localEulerAngles.z);

        knife.Rebind();
        knife.Play("Cut");
    }

    public void NoteResult(TimingResult result)
    {
        string resultString = Enum.GetName(typeof(TimingResult), result);
        if (result == TimingResult.Good)
        {
            NotesManager.instance.AddScore(false);
            CookingSoundsBaseData.PlayPerfectNote();
        }
        else if (result == TimingResult.Perfect)
        {
            NotesManager.instance.AddScore(true);
            CookingSoundsBaseData.PlayGoodNote();
        }
        else
            CookingSoundsBaseData.PlayFailNote();
        textAnim.Play("ScaleAnim");
        text.text = resultString;
    }

    public void SliceIngredient(GameObject ingredient)
    {
        Vector3 side1 = _triggerExitTipPosition.position - _triggerEnterTipPosition.position;
        Vector3 side2 = _triggerExitTipPosition.position - _triggerEnterBasePosition.position;

        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(ingredient.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = ingredient.transform.InverseTransformPoint(_triggerEnterTipPosition.position);

        Plane plane = new Plane();

        plane.SetNormalAndPosition(
                transformedNormal,
                transformedStartingPoint);

        var direction = Vector3.Dot(Vector3.up, transformedNormal);

        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
        {
            plane = plane.flipped;
        }

        GameObject[] slices = Slicer.Slice(plane, ingredient);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * _forceAppliedToCut;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);
    }

    void PlayCutSound()
    {
        int random = UnityEngine.Random.Range(0, cutSounds.Length);

        AudioManager.instance.StopAllSounds(cutSounds[random].name);
        AudioManager.instance.PlaySound(cutSounds[random].name);
    }
}

[Serializable]
public class CookInputBind
{
    public CookSection section;
    public string input;
    public SoundNotes noteSound;
}

public enum TimingResult { Fail, Good, Perfect }