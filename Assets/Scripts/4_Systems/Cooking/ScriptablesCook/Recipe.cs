using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Recipe", menuName = "Cooking/Recipe", order = 0)]
public class Recipe : ScriptableObject
{
    public KeyValue<IngredientNote, int>[] necesaryIngredients = new KeyValue<IngredientNote, int>[0];

    public string recipeName;

    public List<KeyValue<IngredientNote, float>> ingredientsOrder = new List<KeyValue<IngredientNote, float>>();

    public float timeToStart;
    public float timeToEnd;

    public float recipeSpeed;

    public RecipeResult plateResult;

    public float scoreToPerfect;
    public float scoreToGood;

    public SoundNotes[] notes = new SoundNotes[4];

    public AudioClip baseSound;

    public Queue<NoteSetter> GetNoteQueue()
    {
        Queue<NoteSetter> noteQueue = new Queue<NoteSetter>();

        for (int i = 0; i < ingredientsOrder.Count; i++)
        {
            int count = ingredientsOrder[i].key.setters.Length;
            for (int x = 0; x < count; x++)
            {
                var newSetter = new NoteSetter();
                newSetter.section = ingredientsOrder[i].key.setters[x].section;
                newSetter.timeSeparation = ingredientsOrder[i].key.setters[x].timeSeparation;
                newSetter.meshIngredient = ingredientsOrder[i].key.ingredientMesh;
                newSetter.matIngredient = ingredientsOrder[i].key.ingredientMat;
                if (x == count - 1) newSetter.timeSeparation = ingredientsOrder[i].value;
                noteQueue.Enqueue(newSetter);
            }
        }

        return noteQueue;
    }

    public bool ContainsRecipe
    {
        get
        {
            bool isEnough = true;
            for (int i = 0; i < necesaryIngredients.Length; i++)
            {
                isEnough = PlayerInventory.QueryElement(necesaryIngredients[i].key, necesaryIngredients[i].value, 0, false);

                if (!isEnough)
                    break;
            }

            return isEnough;
        }
    }

    public void ConsumeIngredients()
    {
        for (int i = 0; i < necesaryIngredients.Length; i++)
        {
            PlayerInventory.Remove(necesaryIngredients[i].key, necesaryIngredients[i].value, 0, false);
        }
    }
}

[System.Serializable]
public class KeyValue<T, K>
{
    public T key;
    public K value;
}