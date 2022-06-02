using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevelopTools;
using System;

public class NotesManager : MonoBehaviour
{
    public static NotesManager instance { get; private set; }

    [SerializeField] BasicNote basicNotePrefab = null;
    [SerializeField] Transform spawnPos = null;
    [SerializeField] CookTable table = null;

    [SerializeField] float perfectScore = 1f;
    [SerializeField] float goodScore = 0.5f;
    [SerializeField] FoodResultUI resultUI = null;
    float currentScore;

    float speed = 8;

    [SerializeField] GameObject notesCanvas = null;

    NotePool poolNotes;
    Queue<NoteSetter> myQueue = new Queue<NoteSetter>();
    Recipe recipeOnUse;
    RecipeResult result;

    public int currentNotes;
    float currentSpacing;
    float timer;
    bool songStart;
    float maxScorePossible;

    public List<Note> myNotes = new List<Note>();
    private void Awake()
    {
        instance = this;

        poolNotes = new GameObject($"{basicNotePrefab.name} pool").AddComponent<NotePool>();
        poolNotes.transform.SetParent(transform);
        poolNotes.Configure(basicNotePrefab);
        poolNotes.Initialize(3);
    }

    private void Update()
    {
        if (!songStart) return;

        SongWithRecipe();
    }

    public void ReciveRecipe(Recipe recipe)
    {
        myQueue = recipe.GetNoteQueue();
        currentSpacing = recipe.timeToStart;
        speed = recipe.recipeSpeed;
        songStart = true;
        recipeOnUse = recipe;
        notesCanvas.gameObject.SetActive(true);
        result = recipe.plateResult;
        maxScorePossible = myQueue.Count * perfectScore;
        table.StartRecipe(recipe);
    }

    void SongWithRecipe()
    {
        if (myQueue.Count > 0)
        {
            timer += Time.deltaTime * speed;
            if (timer >= currentSpacing)
            {
                timer = 0;
                SpawnNoteToQueue();
            }
        }
        else if (myQueue.Count == 0 && myNotes.Count == 0)
        {
            timer += Time.deltaTime;
            if (timer >= recipeOnUse.timeToEnd)
            {
                timer = 0;
                ShowResult();
            }
        }
        if (myNotes.Count == 0) return;

        for (int i = 0; i < myNotes.Count; i++)
        {
            myNotes[i].transform.position -= myNotes[i].transform.forward * Time.deltaTime * speed;
        }
    }

    void SpawnNoteToQueue()
    {
        var noteInfo = myQueue.Dequeue();
        var newNote = poolNotes.Get();
        table.PosisionateNote(newNote, noteInfo.section);
        newNote.transform.position = new Vector3(newNote.transform.position.x, spawnPos.position.y, spawnPos.position.z);

        newNote.SetColor(noteInfo.meshIngredient, noteInfo.matIngredient);

        myNotes.Add(newNote);

        currentSpacing = noteInfo.timeSeparation;
    }

    public void DeleteNote(Note note, TimingResult result)
    {
        if(result != TimingResult.Fail) table.SliceIngredient(note.model.gameObject);
        myNotes.Remove(note);
        poolNotes.ReturnParticle(note);
    }

    public void AddScore(bool isPerfect)
    {
        if (isPerfect)
            currentScore += perfectScore;
        else
            currentScore += goodScore;
    }

    void ShowResult()
    {
        table.EndRecipe();
        songStart = false;
        notesCanvas.gameObject.SetActive(false);
        string prov = "";
        int quality;
        TimingResult resultTiming = TimingResult.Fail;
        if(currentScore >= recipeOnUse.scoreToPerfect)
        {
            Debug.Log("Te salio pipi cucu");
            prov = "pipi cucu";
            resultTiming = TimingResult.Perfect;
            quality = 3;
        }
        else if(currentScore >= recipeOnUse.scoreToGood)
        {
            Debug.Log("Podría estar mejor");
            prov = "meh";
            resultTiming = TimingResult.Good;
            quality = 2;
        }
        else
        {
            Debug.Log("Te salió mal banana");
            prov = "pete";
            quality = 1;
        }

        var process = PlayerInventory.Add(result, 1, quality);
        if (!process.Process_Successfull)
        {
            //¿que hacemos aca? lo dropeamos? te dejo ahi abajo una variable que devuelve la cantidad de objetos que no se pudieron agregar al inventario
            var quantity_to_drop = process.Remainder_Quantity;
        }
        resultUI.Open(result, currentScore, maxScorePossible, prov, (int)resultTiming);
        currentScore = 0;
    }
}
