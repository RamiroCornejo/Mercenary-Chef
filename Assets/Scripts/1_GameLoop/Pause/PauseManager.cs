using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance { get; private set; }

    bool paused;

    private void Awake()
    {
        instance = this;
    }

    public void Pause()
    {
        if (paused) return;

        paused = true;
        Character.TrackInput(false);
    }

    public void Resume()
    {
        if (!paused) return;

        paused = false;
        Character.TrackInput(true);
    }
}
