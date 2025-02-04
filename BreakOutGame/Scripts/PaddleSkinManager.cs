using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Threading.Tasks;
 

[CreateAssetMenu(fileName = "PaddleSkinManager", menuName = "Paddle Skin Manager")]
public class PaddleSkinManager : ScriptableObject
{
  [SerializeField] public PaddleSkin[] paddleSkins;
  private const string Prefix = "PaddleSkin_";
  private const string SelectedPaddleSkin = "SelectedPaddleSkin";


  public void SelectSkin(int skinIndex) => PlayerPrefs.SetInt(SelectedPaddleSkin, skinIndex);

   

  public PaddleSkin GetSelectedPaddleSkin()
  {
    int skinIndex = PlayerPrefs.GetInt(SelectedPaddleSkin, 0);
    if (skinIndex >= 0 && skinIndex < paddleSkins.Length)
    {
      return paddleSkins[skinIndex];
    }
    else
    {
      // 如果找不到有效皮肤，可以返回默认皮肤或者其他处理逻辑
        Debug.LogWarning("Invalid skin index, returning default skin.");
        return paddleSkins[0]; // 返回默认皮肤或者根据具体需求返回其他皮肤
    }
  }


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


  // public void Unlock(int skinIndex) => PlayerPrefs.SetInt(Prefix + skinIndex, 1);


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

  // public bool IsUnlocked(int skinIndex) => PlayerPrefs.GetInt(Prefix + skinIndex, 0) == 1;
}