using UnityEngine.UI;
using UnityEngine;
using TMPro;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;


public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public int score;
    public Potion[] potions;

    public TextMeshProUGUI scoreText;
    public GameObject shopUI;
    public Transform shopContent;
    public GameObject itemPrefab;
    public PotionScript potionScript;
    public GameManager gameManager;

    

    private async void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        // DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();
        score = await LoadScore();
        UpdateScoreText();

    }

    private async void Start() 
    {
        
       

        foreach (Potion potion in potions) 
        {
            GameObject item = Instantiate(itemPrefab, shopContent);

            potion.itemRef = item;

            foreach (Transform child in item.transform)
            {
                if (child.gameObject.name == "Cost"){
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = "$" + potion.cost.ToString();
                } else if (child.gameObject.name == "Name"){
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = potion.name;
                } else if (child.gameObject.name == "Image"){
                    child.gameObject.GetComponent<Image>().sprite = potion.image;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() => {
                BuyPotion(potion);
            }); 
        }

        
    }

    void Update()
    {
        score = gameManager.score;
    }


    public async void BuyPotion(Potion potion) {
        if (await CanBuyPotion(potion.name)) {
            score = await LoadScore();
            if (score >= potion.cost) {
                score -= potion.cost;
                gameManager.score -= potion.cost;
                SaveScore();
                gameManager.SaveScore();
                
                Debug.Log(" Score: " + score);
                await ApplyBuyPotion(potion);
                UpdateScoreText();
            }
        } else {
            Debug.Log("You must wait before buying this potion again.");
        }
      
    }

    public void UpdateScoreText()
    {   
        if (scoreText != null)
        {
            scoreText.text = score.ToString("00000");

        } else
        {
            Debug.Log("No coinsText");
        }
 
    }
   
    public async Task ApplyBuyPotion(Potion potion) 
    {
        Debug.Log("ApplyBuyPotion called with potion: " + potion.name);
        string potionNameInEnglish = NameMapping.NameToEnglish.TryGetValue(potion.name, out var englishName) ? englishName : null;
        
        if (potionNameInEnglish == null)
        {
            Debug.Log("Potion name not recognized: " + potion.name);
            return;
        }

        switch (potionNameInEnglish){
            case "Shield30Seconds":
                potionScript.AddShield(30);
                break;
            case "SpeedBoost":
                potionScript.AddSpeed();
                break;
            case "SpeedReduce":
                potionScript.MinusSpeed();
                break;
            case "DoubleScore":
                potionScript.DoubleScore();
                break;
            case "ShrinkItem":
                potionScript.TurnSmallFunc();
                Debug.Log("TurnSmall start");
                break;
            case "InvisibilityMode":
                potionScript.TurnOpacity();
                break;
            default:
                Debug.Log("No upgrade");
                break;
        }
        await SavePurchaseTime(potionNameInEnglish);


    }

    public void ToggleShop() {
        shopUI.SetActive(!shopUI.activeSelf);
    }

    public void OnGUI() {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();

        }
        UpdateScoreText();
        
    }


    

    public static class NameMapping
    {
        public static readonly Dictionary<string, string> NameToEnglish = new Dictionary<string, string>
        {
            { "盾牌30秒", "Shield30Seconds" },
            { "速度提升", "SpeedBoost" },
            { "速度減慢", "SpeedReduce" },
            { "雙倍分數", "DoubleScore" },
            { "縮小道具", "ShrinkItem" },
            { "隱形模式", "InvisibilityMode" }
            // 添加更多的映射
        };

        public static readonly Dictionary<string, string> EnglishToName = new Dictionary<string, string>
        {
            { "Shield30Seconds", "盾牌30秒" },
            { "SpeedBoost", "速度提升" },
            { "SpeedReduce", "速度減慢" },
            { "DoubleScore", "雙倍分數" },
            { "ShrinkItem", "縮小道具" },
            { "InvisibilityMode", "隱形模式" }
            // 添加更多的映射
        };

        // public static string GetEnglishName(string chineseName)
        // {
        //     return NameToEnglish.TryGetValue(chineseName, out string englishName) ? englishName : null;
        // }

        // public static string GetChineseName(string englishName)
        // {
        //     return EnglishToName.TryGetValue(englishName, out string chineseName) ? chineseName : null;
        // }
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



    //檢查購買時間
    public async Task SavePurchaseTime(string potionName)
    {
        var data = new Dictionary<string, object>
        {
            { potionName + "_LastPurchaseTime", DateTime.UtcNow.ToString("g") }  // 使用道具名称作为 key
        };
        
        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log($"Saved purchase time for {potionName}");
        }
        catch (CloudSaveValidationException ex)
        {
            Debug.LogError($"Validation error saving purchase time: {ex.Message}");
            Debug.LogError($"Details: {ex.Details}");
        }
    }

   
    public async Task<bool> CanBuyPotion(string potionNameInChinese)
    {
        Debug.Log("Checking potion: " + potionNameInChinese);
        string potionNameInEnglish = NameMapping.NameToEnglish.TryGetValue(potionNameInChinese, out var englishName) ? englishName : null;
        Debug.Log("Converted to English name: " + potionNameInEnglish);

        if (potionNameInEnglish == null)
        {
            Debug.Log("Potion name not recognized: " + potionNameInChinese);
            return false;  // 如果找不到英文名称，则不允许购买
        }

        try
        {
            // 從Cloud Save加載購買時間
            var result = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { potionNameInEnglish + "_LastPurchaseTime" });

            if (result.TryGetValue(potionNameInEnglish + "_LastPurchaseTime", out string lastPurchaseTimeObj))
            {
                Debug.Log("Last purchase time retrieved: " + lastPurchaseTimeObj);

                // 使用 TryParse 以安全地解析日期时间
                if (DateTime.TryParse(lastPurchaseTimeObj, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime lastPurchaseTime))
                {
                    TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;

                    if (timeSinceLastPurchase.TotalMinutes < 10)
                    {
                        Debug.Log("Cannot buy yet. Wait for " + (10 - timeSinceLastPurchase.TotalMinutes) + " more minutes.");
                        return false;  // 间隔不足10分钟，不能购买
                    }
                }
                    else
                    {
                        Debug.LogError("Invalid DateTime format: " + lastPurchaseTimeObj);
                        return false;  // 如果解析失败，视为不允许购买
                    }
            }
            else
            {
                // 没有记录，允许购买
                Debug.Log("No last purchase time found. Allowing purchase.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error checking last purchase time: " + ex.Message);
        }
        
        return true;  // 沒有記錄或間隔超過5分鐘，可以購買
    }



}


[System.Serializable]
public class Potion {
    public string name;
    public int cost;
    public Sprite image;
    // [HideInInspector] public int quantity;
    [HideInInspector] public GameObject itemRef;
}