using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Unity.Services.Core;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System;

public class LevelMenu : MonoBehaviour
{

    public Button[] buttons;
    public LoadingManager loadingManager;
    

    // private void Awake(){
    //     int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
    //     for (int i = 0; i < buttons.Length; i++)
    //     {
    //         buttons[i].interactable = false;
    //     }
    //     for (int i = 0; i < unlockedLevel; i++)
    //     {
    //         buttons[i].interactable = true;
    //     }
    // }

    private async void Awake(){
        await UnityServices.InitializeAsync();

        Debug.Log("UnlockedLevelValue: " + await LoadLevel());
        int unlockedLevel = await LoadLevel();

        // if (unlockedLevel == 0)
        // {
        //     Debug.Log("UnlockedLevelValue: " + unlockedLevel);
        //     buttons[0].interactable = true;
        // }
        
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = i < unlockedLevel;
        }
        // for (int i = 0; i < buttons.Length; i++)
        // {
        //     buttons[i].interactable = false;
        // }
        // for (int i = 0; i < unlockedLevel; i++)
        // {
        //     buttons[i].interactable = true;
        // }
    }

    public async Task<int> LoadLevel()
    {
        try
        {
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "UnlockedLevelValue" });
            if(data.ContainsKey("UnlockedLevelValue"))
            {
                return int.Parse(data["UnlockedLevelValue"]);
            }
            else
            {
                return 0; // Default value if no data is found
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data: {ex.Message}");
            return 0;
        }
    }

    public async Task FirstPlaySaveLevel(int currentLevel)
    {   
        var data = new Dictionary<string, object> { {"UnlockedLevelValue", currentLevel} };

        try
        {
            // if (data.ContainsKey("UnlockedLevelValue"))
            // {
            //     Debug.Log("UnlockedLevelValue already exists, skipping save.");
            //     return;
            // }
            // await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            int savedLevel = await LoadLevel();
            if (currentLevel > savedLevel || savedLevel == 0)
            {
                await CloudSaveService.Instance.Data.ForceSaveAsync(data);
                Debug.Log("Level progress saved: " + currentLevel);
            }
            else
            {
                Debug.Log("Level progress already at " + savedLevel + ", skipping save.");
            }
            
        }
        catch (Exception ex)
        {
            // Debug.LogError($"Error saving data: {ex.Message}");
            Debug.Log("Data already exists, skipping save.");
        }
    }

    public async void FirstPlaySaveCurrentLevel()
    {   
        
        
        int currentLevel = 1; // Replace with your logic to get the current level
        await FirstPlaySaveLevel(currentLevel);
        Debug.Log("Key Save");
    }

  

    async void Start()
    {

        

    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        // SceneManager.LoadScene(levelName);


        loadingManager.LoadLevel(levelName);

    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
