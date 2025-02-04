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

public class SkinShopItem : MonoBehaviour
{
  [SerializeField] private SkinManager skinManager;
  [SerializeField] private int skinIndex;
  [SerializeField] private Button buyButton;
  [SerializeField] private TextMeshProUGUI costText;
  private Skin skin;
//   public SkinShopController skinShopController;
  public int score;


  async void Start()
  {
    skin = skinManager.skins[skinIndex];

    GetComponent<Image>().sprite = skin.sprite;
    await CheckSkinUnlock(skinIndex);
    // if (skinManager.IsUnlocked(skinIndex))
    // {
    //     buyButton.gameObject.SetActive(false);    
    // }
    // else
    // {
    //     buyButton.gameObject.SetActive(true);
    //     costText.text = skin.cost.ToString();
    // }
  }
   
  public async Task CheckSkinUnlock(int skinIndex)
  {
      bool IsUnlocked = await skinManager.LoadIsUnlock(skinIndex);

      if (IsUnlocked)
      {
        buyButton.gameObject.SetActive(false);
      } else 
      {
        buyButton.gameObject.SetActive(true);
        costText.text = skin.cost.ToString();
      }
  }


  public async void OnSkinPressed()
  {
    bool IsUnlocked = await skinManager.LoadIsUnlock(skinIndex);
    if (IsUnlocked)
    {
        // skinManager.SaveSelectSkin(skinIndex);
        skinManager.SelectSkin(skinIndex);
    }
  }

  public async void OnBuyButtonPressed()
  {
        score = await LoadScore();
        bool isUnlocked = await skinManager.LoadIsUnlock(skinIndex);

        // Unlock the skin
        if (score >= skin.cost && !isUnlocked)
        { 
            score = score - skin.cost;
            Debug.Log("New coins after deduction: " + score); 
            // skinShopController.score = skinShopController.score - skin.cost;
            // Debug.Log("New skin shop coins: " + skinShopController.score); 
            // skinShopController.scoreText.text = skinShopController.score.ToString("00000");
            // Debug.Log("Coins text updated to: " + skinShopController.scoreText.text);
            // bouncyBall.coins = score;
            // bouncyBall.SaveCoins();
            SaveScore();
            await skinManager.SaveUnlock(skinIndex);
            buyButton.gameObject.SetActive(false);
            // skinManager.SaveSelectSkin(skinIndex);
            skinManager.SelectSkin(skinIndex);
        }
        else
        {
            Debug.Log("Not enough coins :(");
        }
  }

   public async void SaveScore()
    {   
        var data = new Dictionary<string, object> { {"score", score} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("save score: " + score);
            Debug.Log("scoredata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving scoredata: {ex.Message}");
        }
    }

    public async Task<int> LoadScore()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "score" });
        if (data.ContainsKey("score"))
        {
            score = int.Parse(data["score"]);
            Debug.Log("Loaded score: " + score);
            return score;
        }
        else
        {
            Debug.Log("No score data found");
            return 0;
        }
    }
  
 
  
}