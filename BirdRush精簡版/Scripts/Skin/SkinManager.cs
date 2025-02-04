//在Sprite/bird資料夾中
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Threading.Tasks;
 

[CreateAssetMenu(fileName = "SkinManager", menuName = "Skin Manager")]
public class SkinManager : ScriptableObject
{
    [SerializeField] public Skin[] skins;
    private const string Prefix = "Skin_";
    private const string SelectedSkin = "SelectedSkin";

    public void SelectSkin(int skinIndex) => PlayerPrefs.SetInt(SelectedSkin, skinIndex);

    // public async Task SaveSelectSkin(int skinIndex)
    // {   
    //     var data = new Dictionary<string, object> { {SelectedSkin, skinIndex} };

    //     try
    //     {
    //         await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    //         Debug.Log("speeddata saved successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError($"Error saving speeddata: {ex.Message}");
    //     }
    // }

    



    public Skin GetSelectedSkin()
    {
        int skinIndex = PlayerPrefs.GetInt(SelectedSkin, 0);
        if (skinIndex >= 0 && skinIndex < skins.Length)
        {
            return skins[skinIndex];
        }
        else
        {
            return null;
        } 
    }

    // public async Task<Skin> LoadSelectSkin()
    // {
    //     try
    //     {
    //         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { SelectedSkin });
    //         if (data.ContainsKey(SelectedSkin))
    //         {
    //             int skinIndex = int.Parse(data[SelectedSkin]);
    //             if (skinIndex >= 0 && skinIndex < skins.Length)
    //             {
    //                 return skins[skinIndex];;
    //             }
    //         }
           
    //         return null; // Default value if no data is found
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError($"Error loading selected skin: {ex.Message}");
    //         return null; // Or handle the error differently
    //     }
    // }

    // public async Task<Sprite> LoadSkin()
    // {
    //     Task<Skin> skinTask = LoadSelectSkin();
    //     Skin skin = await skinTask; // Wait for the task to complete
    //     Sprite sprite = null;
    //     if (skin != null)
    //     {
    //         sprite = skin.sprite; // Access the sprite property after loading
    //     }

    //     return sprite;
    // }





    // public void Unlock(int skinIndex) => PlayerPrefs.SetInt(Prefix + skinIndex, 1);


    public async Task SaveUnlock(int skinIndex)
    {   
        var data = new Dictionary<string, object> { {Prefix + skinIndex, 1} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("IsUnlock data saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving IsUnlock data: {ex.Message}");
        }
    } 


    // public bool IsUnlocked(int skinIndex) => PlayerPrefs.GetInt(Prefix + skinIndex, 0) == 1;
 
    public async Task<bool> LoadIsUnlock(int skinIndex)
    {
        try
        {
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { Prefix + skinIndex });
            if (data.ContainsKey(Prefix + skinIndex))
            {
                return int.Parse(data[Prefix + skinIndex]) == 1;
            }
            else
            {
                return false; // Default value if no data is found
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading selected skin: {ex.Message}");
            return false; // Or handle the error differently
        }
    }



    public void ResetAllUnlocks()
    {
        for (int i = 0; i < skins.Length; i++)
        {
            PlayerPrefs.DeleteKey(Prefix + i);
        }
    }
}