using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Note : MonoBehaviour
{
    [SerializeField] public MeshFilter model = null;

    public void FailNote()
    {
        NotesManager.instance.DeleteNote(this, TimingResult.Fail);
    }

    public virtual void GoodNote()
    {
        NotesManager.instance.DeleteNote(this, TimingResult.Good);
    }

    public virtual void PerfectNote()
    {
        NotesManager.instance.DeleteNote(this, TimingResult.Perfect);
    }

    public abstract void SetColor(Mesh mesh, Material mat);
}
