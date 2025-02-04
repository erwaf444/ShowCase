using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Steamworks;

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

    void Start()
    {
        experiencePoints = 0;
    }

    void Update()
    {
        xpText.text = experiencePoints.ToString();
    }

    public void AddExp(float amount)
    {
        experiencePoints += amount;
        Debug.Log("XP added: " + amount + ", Total XP: " + experiencePoints);
    }

    // Check if player has enough XP to unlock the skill
    public bool IsSkillUnlocked(int requiredSkillValue)
    {
        return experiencePoints >= requiredSkillValue;
    }

    public void SaveExp()
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat("Exp", experiencePoints);
            bool success = SteamUserStats.StoreStats();
            
            if (success)
            {
                Debug.Log("Exp saved to Steam Cloud: " + experiencePoints);
            }
            else
            {
                Debug.LogWarning("Failed to store stats to Steam Cloud.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to save Exp.");
        }
    }

    public void LoadExp()
    {

        if (SteamManager.Initialized)
        {
            // 從Steam Cloud中獲取金錢數據
            float storedExp;
            bool hasStat = SteamUserStats.GetStat("Exp", out storedExp);
        
            if (hasStat)
            {
                experiencePoints = storedExp;
                Debug.Log("Exp loaded from Steam Cloud: " + experiencePoints);
            }
            else
            {
                Debug.LogWarning("Stat 'Exp' not found or could not be retrieved.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to load Exp.");
        }
    }

  
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



