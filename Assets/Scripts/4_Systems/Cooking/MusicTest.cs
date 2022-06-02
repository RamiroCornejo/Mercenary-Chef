using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTest : MonoBehaviour
{
    [SerializeField] Recipe recipeToTest = null;

    [SerializeField] bool play;

    bool songPlaying;

    float timer;
    float speed;
    float currentSpacing;
    Queue<NoteSetter> myQueue = new Queue<NoteSetter>();
    SoundNotes[] myNotes = new SoundNotes[4];
    [SerializeField] AudioSource baseSource = null;

    private void Update()
    {
        if (play && !songPlaying)
        {
            StartTest();
            play = false;
            songPlaying = true;
        }

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
            if (timer >= recipeToTest.timeToEnd)
            {
                timer = 0;
                EndTest();
            }
        }
    }


    void StartTest()
    {
        for (int i = 0; i < myNotes.Length; i++)
        {
            myNotes[i] = recipeToTest.notes[i];
        }

        baseSource.clip = recipeToTest.baseSound;
        baseSource.Play();

        myQueue = recipeToTest.GetNoteQueue();

        speed = recipeToTest.recipeSpeed;
        currentSpacing = recipeToTest.timeToStart + 3;
    }

    void SpawnNoteToQueue()
    {
        var obj = myQueue.Dequeue();
        currentSpacing = obj.timeSeparation;
        CookingSoundsBaseData.PlaySound(myNotes[obj.section]);
    }

    void EndTest()
    {
        play = false;
        songPlaying = false;
        baseSource.Stop();
    }
}
