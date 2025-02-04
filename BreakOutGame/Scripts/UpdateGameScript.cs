using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;


public class UpdateGameScript : MonoBehaviour
{
    
    private const string SpeedKey = "speed";
    private const string UpdateFlagKey = "speedUpdated";
    private int speed = 50;
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await UpdateInitialSpeed();
    }

    private async Task UpdateInitialSpeed()
    {
        try
        {
            var keys = new HashSet<string> { UpdateFlagKey };
            var data = await CloudSaveService.Instance.Data.LoadAsync(keys);
            
            if (data.ContainsKey(UpdateFlagKey))
            {
                bool isSpeedUpdated;
                // 已经更新过，不做任何更改
                if (bool.TryParse(data[UpdateFlagKey]?.ToString(), out isSpeedUpdated) && isSpeedUpdated)
                {
                    // 已经更新过，不做任何更改
                    Debug.Log("Speed has already been updated. Skipping update.");
                    return;
                }
            }

            // 执行更新逻辑
            await SaveSpeed();
            
            // 保存标志表示已经更新过
            var updateFlag = new Dictionary<string, object> { { UpdateFlagKey, true } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(updateFlag);
            
            Debug.Log("Speed data updated and flag saved.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error checking or saving speed data: {ex.Message}");
        }
    }

    public async Task SaveSpeed()
    {   
        var data = new Dictionary<string, object> { {SpeedKey, speed} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("speeddata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving speeddata: {ex.Message}");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
