using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookSection : MonoBehaviour
{
    [SerializeField] Animator animKey = null;
    [SerializeField] CookTable table = null;

    [SerializeField] SensorClick goodSensor = null;
    [SerializeField] SensorClick perfectSensor = null;

    private void Start()
    {
        goodSensor.OnNoteExit += FailNote;
    }

    public bool ActionDown()
    {
        var note = perfectSensor.HasANote();
        bool isTrue = false;
        if(note != null)
        {
            table.NoteResult(TimingResult.Perfect);
            note.PerfectNote();
            isTrue = true;
        }
        else
        {
            note = goodSensor.HasANote();
            if (note != null)
            {
                table.NoteResult(TimingResult.Good);
                note.GoodNote();
                isTrue = true;
            }
        }

        perfectSensor.Refresh(note);
        goodSensor.Refresh(note);
        animKey.Play("PressDown");

        return isTrue;
    }

    public void Action()
    {

    }

    public void ActionUp()
    {

    }


    public void FailNote(Note note)
    {
        table.NoteResult(TimingResult.Fail);
        note.FailNote();
    }
}
