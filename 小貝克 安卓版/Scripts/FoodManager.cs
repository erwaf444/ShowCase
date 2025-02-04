using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    // public Dictionary<string, int> foodCompletionCounts = new Dictionary<string, int>();
    public int completedFoods= 0;

  
    // public void IncrementCompletionCount(string foodName)
    // {
    //     if (foodCompletionCounts.ContainsKey(foodName))
    //     {
    //         foodCompletionCounts[foodName]++;
    //     }
    //     else
    //     {
    //         foodCompletionCounts[foodName] = 1; // 如果字典中没有，初始化为1
    //     }
    // }

    public void AddToCompletedFoods(int count)
    {
        completedFoods += count; // 增加已完成食物的数量
    }

    public void MinusToCompletedFoods(int count)
    {
        completedFoods -= count;
    }

}
