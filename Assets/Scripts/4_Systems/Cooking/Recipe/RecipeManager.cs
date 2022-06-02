using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance { get; private set; }

    [SerializeField] List<Recipe> recipesUnlocked = new List<Recipe>();

    [Header("UI_Things")]
    [SerializeField] GameObject book = null;
    [SerializeField] RecipeUI[] myRecipes = new RecipeUI[0];
    [SerializeField] GameObject prevButton = null;
    [SerializeField] GameObject nextButton = null;

    int currentStartIndex = 0;
    bool open;

    private void Awake()
    {
        instance = this;
    }

    public void OpenRecipeBook()
    {
        if (open) return;
        book.SetActive(true);

        RefreshRecipes();

        if (recipesUnlocked.Count > myRecipes.Length)
            nextButton.SetActive(true);

        open = true;
        currentRecipe = 0;
        myRecipes[currentRecipe].Select();
    }

    public void CloseRecipeBook()
    {
        if (!open) return;
        currentStartIndex = 0;
        prevButton.SetActive(false);
        nextButton.SetActive(false);
        book.SetActive(false);
        open = false;
    }

    public void NextPage()
    {
        prevButton.SetActive(true);

        currentStartIndex += myRecipes.Length;

        if (currentRecipe != 0)
        {
            myRecipes[currentRecipe].Unselect();
            currentRecipe = 0;
            myRecipes[currentRecipe].Select();
        }
        RefreshRecipes();

        if (currentStartIndex + myRecipes.Length >= recipesUnlocked.Count)
            nextButton.SetActive(false);

    }

    public void PrevPage()
    {
        nextButton.SetActive(true);

        currentStartIndex -= myRecipes.Length;
        if (currentRecipe != 0)
        {
            myRecipes[currentRecipe].Unselect();
            currentRecipe = 0;
            myRecipes[currentRecipe].Select();
        }
        RefreshRecipes();

        if (currentStartIndex <= 0)
            prevButton.SetActive(false);
    }


    public void UnlockRecipe(Recipe recipe)
    {
        if (recipesUnlocked.Contains(recipe)) return;

        recipesUnlocked.Add(recipe);
    }

    void MakeRecipe(Recipe recipe)
    {
        if (!recipe.ContainsRecipe)
        {
            Debug.Log("No tenés suficiente pá");
        }
        else
        {
            recipe.ConsumeIngredients();
            NotesManager.instance.ReciveRecipe(recipe);
            CloseRecipeBook();
        }
    }

    void RefreshRecipes()
    {
        for (int i = 0; i < myRecipes.Length; i++)
        {
            if (i + currentStartIndex >= recipesUnlocked.Count)
            {
                myRecipes[i].gameObject.SetActive(false);
                continue;
            }
            myRecipes[i].gameObject.SetActive(true);
            var recipe = recipesUnlocked[i + currentStartIndex];
            myRecipes[i].SetRecipe(recipesUnlocked[i + currentStartIndex], () => { MakeRecipe(recipe); } );
        }
    }

    float timerCD;
    int currentRecipe = 0;

    private void Update()
    {
        if (!open) return;

        if (Input.GetButtonDown("NextPage") && nextButton.activeSelf)
            NextPage();
        else if (Input.GetButtonDown("PrevPage") && prevButton.activeSelf)
            PrevPage();

        if (timerCD < 0.5f)
        {
            timerCD += Time.deltaTime;
            return;
        }

        float vertical = Input.GetAxis("Vertical");

        if (vertical > 0.5f) vertical = 1;
        else if (vertical < -0.5f) vertical = -1;
        else vertical = 0;
        if (vertical != 0)
        {
            Navigate(-(int)vertical);
        }
    }

    void Navigate(int dir)
    {
        if (currentRecipe + dir < 0 || currentRecipe + dir >= myRecipes.Length) return;
        myRecipes[currentRecipe].Unselect();
        currentRecipe += dir;
        myRecipes[currentRecipe].Select();

        timerCD = 0;
    }
}
