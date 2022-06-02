using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlay : MonoBehaviour
{
    public static MusicPlay instance { get; private set; }

    Recipe recipeToPlay = null;

    bool songPlaying;

    float timer;
    float speed;
    float currentSpacing;
    Queue<NoteSetter> myQueue = new Queue<NoteSetter>();
    SoundNotes[] myNotes = new SoundNotes[4];
    [SerializeField] AudioSource baseSource = null;
    [SerializeField] float initTime = 4;

    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        if (!songPlaying) return;

        if (myQueue.Count > 0)
        {
            timer += Time.deltaTime * speed;
            if (timer >= currentSpacing)
            {
                timer = 0;
                SpawnNoteToQueue();
            }
        }
        else if (myQueue.Count == 0)
        {
            timer += Time.deltaTime;
            if (timer >= recipeToPlay.timeToEnd)
            {
                timer = 0;
                EndTest();
            }
        }
    }

    public void StartMusic(Recipe _recipeToPlay)
    {
        recipeToPlay = _recipeToPlay;
        for (int i = 0; i < myNotes.Length; i++)
        {
            myNotes[i] = recipeToPlay.notes[i];
        }

        songPlaying = true;
        baseSource.clip = recipeToPlay.baseSound;
        baseSource.Play();

        myQueue = recipeToPlay.GetNoteQueue();

        speed = recipeToPlay.recipeSpeed;
        currentSpacing = recipeToPlay.timeToStart + initTime;
    }

    void SpawnNoteToQueue()
    {
        var obj = myQueue.Dequeue();
        currentSpacing = obj.timeSeparation;
        CookingSoundsBaseData.PlaySound(myNotes[obj.section]);
    }

    void EndTest()
    {
        songPlaying = false;
        baseSource.Stop();
    }
}
