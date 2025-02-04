using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSystem : MonoBehaviour
{
    public StoryManager storyManager;
    public List<StoryManager.Item> playerInventory = new List<StoryManager.Item>(); // 玩家擁有的物品
    public List<StoryManager.Recipe> recipes = new List<StoryManager.Recipe>(); // 所有合成配方
    public TextMeshProUGUI resultText; // 顯示合成結果
    public StoryManager.Item ironOre;
    public StoryManager.Item wood;
    public StoryManager.Item ironSword;

    void Start()
    {
        // 創建材料
        ironOre = new StoryManager.Item { itemName = "Iron Ore", itemType = StoryManager.ItemType.Material };
        wood = new StoryManager.Item { itemName = "Wood", itemType = StoryManager.ItemType.Material };
        // 創建裝備
        ironSword = new StoryManager.Item { itemName = "Iron Sword", itemType = StoryManager.ItemType.Equipment };

    }

    // 檢查是否可以合成
    public bool CanCraft(StoryManager.Recipe recipe)
    {
        foreach (StoryManager.Item requiredItem in recipe.requiredItems)
        {
            if (!playerInventory.Contains(requiredItem))
                return false;
        }
        return true;
    }

    // 執行合成
    public void CraftItem(StoryManager.Recipe recipe)
    {
        if (CanCraft(recipe))
        {
            // 移除材料
            foreach (StoryManager.Item requiredItem in recipe.requiredItems)
            {
                playerInventory.Remove(requiredItem);
            }

            // 添加新物品
            playerInventory.Add(recipe.resultItem);
            resultText.text = "成功合成：" + recipe.resultItem.itemName;
        }
        else
        {
            resultText.text = "材料不足，無法合成！";
        }
    }

    public void SetupRecipes()
    {
        StoryManager.Recipe swordRecipe = new StoryManager.Recipe
        {
            requiredItems = new List<StoryManager.Item> { ironOre, wood },
            resultItem = ironSword
        };

        recipes.Add(swordRecipe);
    }
}
