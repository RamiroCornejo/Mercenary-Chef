using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SensorClick : MonoBehaviour
{
    List<Note> currentNotes = new List<Note>();
    public Action<Note> OnNoteExit = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Note>())
        {
            if (currentNotes.Contains(other.GetComponent<Note>())) return;

            currentNotes.Add(other.GetComponent<Note>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var note = other.GetComponent<Note>();
        if (note && currentNotes.Contains(note))
        {
            if(note.gameObject.activeSelf)
            {
                OnNoteExit(note);
            }

            currentNotes.Remove(note);
        }
    }

    public void Refresh(Note note)
    {
        if(currentNotes.Contains(note)) currentNotes.Remove(note);
    }

    public Note HasANote()
    {
        Note currentNote = null;
        if (currentNotes.Count > 0)
        {
            currentNote = currentNotes[0];
            currentNotes.RemoveAt(0);
        }

        return currentNote;
    }
}
