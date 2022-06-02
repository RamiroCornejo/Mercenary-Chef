using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBookAux : DefaultMachinery
{
    protected override void OnBeginExecute()
    {
        RecipeManager.instance.OpenRecipeBook();
    }

    protected override void OnEndExecute()
    {
        RecipeManager.instance.CloseRecipeBook();
    }
}
