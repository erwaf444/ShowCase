using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
   
 
    public Player player;

    public bool CanCraftRecipe(Recipe recipe)
    {
        return player.IsSkillUnlocked(recipe.requiredSkillValue);
    }

    public void CraftRecipe(Recipe recipe)
    {
        if (CanCraftRecipe(recipe))
        {
            Debug.Log("Recipe crafted: " + recipe.name);
            // 进行制作食谱的相关逻辑
        }
        else
        {
            Debug.Log("Skill required: " + recipe.requiredSkillValue);
        }
    }
}



