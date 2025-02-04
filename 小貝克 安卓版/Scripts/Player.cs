using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;

[System.Serializable]
public class Recipe
{
    public string name;
    public int requiredSkillValue; // 技能名称或ID
}

[System.Serializable]
public class Skill
{
    public string name;
    public bool isUnlocked;
}
 

public class Player : MonoBehaviour
{
    public float experiencePoints;
    public Image expPointsTitle;
    public TextMeshProUGUI xpText;

    public List<Skill> unlockedSkills = new List<Skill>();
    public List<Recipe> recipes = new List<Recipe>();

    float max = 100;

    async void Start()
    {
        // experiencePoints = 0;
        experiencePoints = await LoadExp("experiencePoints", 0f);

    }

    void Update()
    {
        xpText.text = experiencePoints.ToString();
    }

    public void AddExp(float amount)
    {
        experiencePoints += amount;
        Debug.Log("XP added: " + amount + ", Total XP: " + experiencePoints);
        SaveExp();
    }

    // Check if player has enough XP to unlock the skill
    public bool IsSkillUnlocked(int requiredSkillValue)
    {
        return experiencePoints >= requiredSkillValue;
    }


    #region unityCloud
    private async Task SaveExp(string key, float value)
    {
        try
        {
            var data = new Dictionary<string, object> { { key, value } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log($"Saved {key}: {value}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save {key}: {e.Message}");
        }
    }

    private async Task<float> LoadData(string key, float defaultValue)
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

            if (savedData != null && savedData.ContainsKey(key))
            {
                var value = savedData[key]; // 讀取儲存的數據
                
                if (float.TryParse(value.ToString(), out float parsedValue))
                {
                    Debug.Log($"Loaded {key}: {parsedValue}");
                    return parsedValue; // 轉換成功，回傳讀取值
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load {key}: {e.Message}");
        }

        return defaultValue; // 若無法讀取，則回傳預設值
    }


    public async void SaveExp()
    {
        await SaveExp("experiencePoints", experiencePoints);
    }

    public async Task<float> LoadExp(string key, float defaultValue)
    {
        return await LoadData(key, defaultValue);
    }


    #endregion

    



  
    // public void UnlockSkill(string skillName)
    // {
    //     Skill skill = unlockedSkills.Find(s => s.name == skillName);
    //     if (skill != null)
    //     {
    //         skill.isUnlocked = true;
    //     }
    // }

    // public bool IsSkillUnlocked(string skillName)
    // {
    //     Skill skill = unlockedSkills.Find(s => s.name == skillName);
    //     return skill != null && skill.isUnlocked;
    // }
}



