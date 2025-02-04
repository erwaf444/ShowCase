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

    public int coins;
    public Upgrade[] upgrades;

    public TextMeshProUGUI coinsText;
    public GameObject shopUI;
    public Transform shopContent;
    public GameObject itemPrefab;
    public PlayerMovement paddle;
    public BouncyBall bouncyBallCoins;
    // public PlayerMovement playerMovement;
    public TextMeshProUGUI MainMenuText;
    public BouncyBall bouncyBall;
    public PaddleAutoMinusCloudSave paddleAutoMinusCloudSave;
    public GameObject openBox;


    

    private async void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        // DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();

    }

    private async void Start() 
    {
        
        // coins = bouncyBallCoins.coins + StaticData.valueToKeep;
        coins = StaticData.valueToKeep;
        coins = bouncyBallCoins.coins;
        int MainMenuCoinsText = bouncyBallCoins.coins;
        MainMenuText.text =  "Coins: " + MainMenuCoinsText.ToString();
        // float speed = 20;

        foreach (Upgrade upgrade in upgrades) 
        {
            GameObject item = Instantiate(itemPrefab, shopContent);

            upgrade.itemRef = item;

            foreach (Transform child in item.transform)
            {
                if (child.gameObject.name == "Quantity"){
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = upgrade.quantity.ToString();
                } else if (child.gameObject.name == "Cost"){
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = "$" + upgrade.cost.ToString();
                } else if (child.gameObject.name == "Name"){
                    child.gameObject.GetComponent<TextMeshProUGUI>().text = upgrade.name;
                } else if (child.gameObject.name == "Image"){
                    child.gameObject.GetComponent<Image>().sprite = upgrade.image;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() => {
                BuyUpgrade(upgrade);
            }); 
        }

        
    }



    public void BuyUpgrade(Upgrade upgrade) {
        coins = bouncyBallCoins.coins;
        if (coins >= upgrade.cost) {
            coins -= upgrade.cost;
            bouncyBallCoins.coins -= upgrade.cost;
            upgrade.quantity++;
            upgrade.itemRef.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgrade.quantity.ToString();
            
            Debug.Log("ShopManager coins: " + coins);
            Debug.Log("BouncyBall coins: " + bouncyBallCoins.coins);
            ApplyUpgrade(upgrade);
            UpdateCoinsText();
            SaveUpgradeQuantity(upgrade);
        }
    }

    public void UpdateCoinsText()
    {   
        if (coinsText != null)
        {
            coinsText.text = bouncyBallCoins.coins.ToString("00000");

        } else
        {
            Debug.Log("No coinsText");
        }
        if (bouncyBallCoins != null)
        {
            bouncyBallCoins.coinsText.text = coinsText.text;
        } else 
        {
            Debug.Log("No bouncyBallCoins");
        }
    }
   
    public async Task ApplyUpgrade(Upgrade upgrade) 
    {
        switch (upgrade.name){
            case "增加平台速度":
                await bouncyBallCoins.LoadSpeed();
                paddle.speed += 0.5f;
                await bouncyBallCoins.SaveSpeed();
                await paddleAutoMinusCloudSave.SaveLastReductionTimeForSpeed();
                // paddleAutoMinusCloudSave.SavePaddleSpeedBuyTime();
                bouncyBall.SaveCoins();
                Debug.Log("Paddle speed: " + paddle.speed);

                
                break;
            case "增加平台宽度":
                await paddle.LoadPaddleWidth();
                await paddle.LoadPaddleHeight();
                paddle.AddPaddleWidthForShopManager();
                paddle.AddPaddleHeightForShopManager();
                await paddle.SavePaddleWidth();
                await paddle.SavePaddleHeight();
                await paddleAutoMinusCloudSave.SaveLastReductionTimeForWidthAndHeight();
                // paddleAutoMinusCloudSave.SavePaddleWidthAndHeightBuyTime();
                bouncyBall.SaveCoins();
                Debug.Log("Paddle width add ");
                break;
            case "增加球的大小":
                await bouncyBall.LoadBallSize();
                bouncyBall.AddBallSizeForShopManager();
                await bouncyBall.SaveBallSize();
                await paddleAutoMinusCloudSave.SaveLastReductionTimeForBallSize();
                // paddleAutoMinusCloudSave.SaveBallSizeBuyTime();
                bouncyBall.SaveCoins();
                Debug.Log("Ball size: "+ bouncyBall.BallSizeValueForShopManager);
                break;
            default:
                Debug.Log("No upgrade");
                break;
        }
    }

    public void ToggleShop() {
        shopUI.SetActive(!shopUI.activeSelf);
    }

    public void OnGUI() {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + coins.ToString();

        }
        UpdateCoinsText();
        
    }


    
    public async void ShowCoins()
    {
        coins = await LoadCoins();
        coinsText.text = coins.ToString("00000");
        openBox.SetActive(true);
        coinsText.gameObject.SetActive(true);
        StartCoroutine(HideCoinsAfterDelay(1f));
    }

    private IEnumerator HideCoinsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        coinsText.gameObject.SetActive(false);
        openBox.SetActive(false);
    }

    public async Task<int> LoadCoins()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "coins" });

        if (data.ContainsKey("coins"))
        {
            coins = int.Parse(data["coins"]);
            Debug.Log("Loaded coins: " + coins);
            return coins;
        }
        else
        {
            Debug.Log("No coins data found");
            return 0;
        }
    }

    public static class UpgradeNameMapping
    {
        public static readonly Dictionary<string, string> NameToEnglish = new Dictionary<string, string>
        {
            { "增加平台速度", "IncreasePaddleSpeed" },
            { "增加平台宽度", "IncreasePaddleWidth" },
            { "增加球的大小", "IncreaseBallSize" }
            // 添加更多的映射
        };

        public static readonly Dictionary<string, string> EnglishToName = new Dictionary<string, string>
        {
            { "IncreasePaddleSpeed", "增加平台速度" },
            { "IncreasePaddleWidth", "增加平台宽度" },
            { "IncreaseBallSize", "增加球的大小" }
            // 添加更多的映射
        };
    }

    public async Task SaveUpgradeQuantity(Upgrade upgrade)
    {
        var englishName = UpgradeNameMapping.NameToEnglish[upgrade.name];
        // Create a dictionary to hold upgrade quantities
        var upgradeData = new Dictionary<string, object>
        {
            { $"upgrade_{englishName}_quantity", upgrade.quantity.ToString() }
        };

        // Save the dictionary to cloud
        await CloudSaveService.Instance.Data.ForceSaveAsync(upgradeData);

        Debug.Log($"Saved upgrade '{englishName}' quantity: {upgrade.quantity}");
    }

    public async Task LoadUpgrades()
    {
        var keys = new HashSet<string>();
        foreach (Upgrade upgrade in upgrades)
        {
            var englishName = UpgradeNameMapping.NameToEnglish[upgrade.name];
            keys.Add($"upgrade_{englishName}_quantity");
        }

        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(keys);

        foreach (Upgrade upgrade in upgrades)
        {
            var englishName = UpgradeNameMapping.NameToEnglish[upgrade.name];
            if (data.TryGetValue($"upgrade_{englishName}_quantity", out string quantityString))
            {
                if (int.TryParse(quantityString, out int quantity))
                {
                    upgrade.quantity = quantity;
                }
                else
                {
                    Debug.LogWarning($"Failed to parse quantity for upgrade '{englishName}'");
                }
            }
            else
            {
                Debug.Log($"No quantity data found for upgrade '{englishName}'");
            }
        }
    }

    // 這個load是為了能夠返回一個int
    public async Task<int> LoadUpgradesForPaddleAutoMinusScript(string targetEnglishName)
    {
        var keys = new HashSet<string>();
        foreach (Upgrade upgrade in upgrades)
        {
            var englishName = UpgradeNameMapping.NameToEnglish[upgrade.name];
            keys.Add($"upgrade_{englishName}_quantity");
            Debug.Log("LoadUpgradesForPaddleAutoMinusScript: " + $"upgrade_{englishName}_quantity");
        }

        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(keys);

        foreach (var kvp in data)
        {
            Debug.Log($"Loaded key: {kvp.Key}, value: {kvp.Value}");
        }
        foreach (Upgrade upgrade in upgrades)
        {
            var englishName = UpgradeNameMapping.NameToEnglish[upgrade.name];
            if (englishName == targetEnglishName){
                if (data.TryGetValue($"upgrade_{englishName}_quantity", out string quantityString))
                {
                    Debug.Log($"Target English Name: {targetEnglishName}");
                    Debug.Log($"Current English Name: {englishName}");
                    
                    if (int.TryParse(quantityString, out int quantity))
                    {
                        upgrade.quantity = quantity;
                        return upgrade.quantity;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to parse quantity for upgrade '{englishName}'");
                    }
                }
                else
                {
                    Debug.Log($"No quantity data found for upgrade '{englishName}'");
                }
            }
        }
        return 0;
    }


}


[System.Serializable]
public class Upgrade {
    public string name;
    public int cost;
    public Sprite image;
    [HideInInspector] public int quantity;
    [HideInInspector] public GameObject itemRef;
}