using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System;

public class PetScript : MonoBehaviour
{
    #region 價錢
    public GameObject notEnoughMoneyPanel; // 錢不夠的提示框
    private Dictionary<string, ItemData> itemData = new Dictionary<string, ItemData>()
    {
        { "雞塊麵包", new ItemData { price = 5, hungerIncrease = 20f, expIncrease = 5, animationTrigger = "isEating" } },
        { "麵包", new ItemData { price = 1, hungerIncrease = 5f, expIncrease = 2, animationTrigger = "isEating" } },
        { "小魚麵包", new ItemData { price = 2, hungerIncrease = 10f, expIncrease = 2, animationTrigger = "isEating" } },
        { "叉燒麵包", new ItemData { price = 2, hungerIncrease = 5f, expIncrease = 2, animationTrigger = "isEating" } },
        { "紙杯咖啡", new ItemData { price = 2, drinkIncrease = 10f, expIncrease = 2, animationTrigger = "isEating" } },
        { "小杯子咖啡", new ItemData { price = 1, drinkIncrease = 10f, expIncrease = 2, animationTrigger = "isEating" } },
        { "珍珠奶茶", new ItemData { price = 5, drinkIncrease = 20f, expIncrease = 2, animationTrigger = "isEating" } },
        { "能量飲料", new ItemData { price = 5, drinkIncrease = 30f, expIncrease = 2, animationTrigger = "isEating" } },
        { "啞鈴", new ItemData { price = 5, playIncrease = 20f, expIncrease = 10, animationTrigger = "isPlay" } },
        { "橄欖球", new ItemData { price = 4, playIncrease = 15f, expIncrease = 5, animationTrigger = "isPlay" } },
        { "網球", new ItemData { price = 3, playIncrease = 10f, expIncrease = 3, animationTrigger = "isPlay" } },
        { "袜子", new ItemData { price = 1, playIncrease = 5f, expIncrease = 1, animationTrigger = "isPlay" } }
    };

    // 物品數據結構
    private class ItemData
    {
        public int price;            // 價格
        public float hungerIncrease; // 飢餓值增加
        public float drinkIncrease;  // 飲料值增加
        public float playIncrease;   // 娛樂值增加
        public int expIncrease;      // 經驗值增加
        public string animationTrigger; // 動畫觸發器
    }
    #endregion
    public int virtualCurrency;
    public TextMeshProUGUI  virtualCurrencyText;
    public GameObject pet;
    public Sprite eatingSprite; // 吃東西時的圖片
    public Sprite defaultSprite; // 默認圖片
    public Sprite awakeSprite;  // 醒來的圖片
    public Sprite asleepSprite;  // 睡覺的圖片
    public float playValue = 100f;  // 玩耍值
    public float hungerValue = 100f;  // 飢餓值
    public float sleepValue = 100f;  // 睡眠值
    public float drinkValue = 100f;  // 飲水值

    public float playDecayRate = 0.3f;  // 玩耍值減少速率
    public float hungerDecayRate = 0.2f;  // 飢餓值減少速率
    public float sleepDecayRate = 0.1f;  // 睡眠值減少速率
    public float drinkDecayRate = 0.1f;  // 飲水值減少速率

    // UI Image 進度條
    public Image playBarImage;
    public Image hungerBarImage;
    public Image sleepBarImage;
    public Image drinkBarImage;



    //工具箱
    public GameObject playToolBox;
    public GameObject hungerToolBox;
    public GameObject drinkToolBox;
    public GameObject sleepToolBox;

    //等級
    public int level;
    public int exp;
    public TextMeshProUGUI levelText;
    public Slider expSlider;
    public TextMeshProUGUI expText;

    // public Image levelBarImage;

    public GameObject foodScrollView;
    public GameObject drinkScrollView;
    public GameObject playScrollView;

    private bool isAddingSleepValue = false;
    public bool isSleeping;
    public GameObject wakeUpRemindPanel;
    public Animator PetAnim;
    public GameObject petPanel;

    public void openPetPanel()
    {
        petPanel.SetActive(true);
        if (!isSleeping)
        {
            PetAnim.SetTrigger("isBlink");
        }
    }


    async void Start()
    {
        await UnityServices.InitializeAsync();
        await UpdatePlayerVirtualCurrency();
        LoadPlayerLevel();
        LoadPlayerData();
        await LoadIsSleeping();
        
        // 初始化 Image.fillAmount 的值
        playBarImage.fillAmount = playValue / 100f;
        hungerBarImage.fillAmount = hungerValue / 100f;
        sleepBarImage.fillAmount = sleepValue / 100f;
        drinkBarImage.fillAmount = drinkValue / 100f;
        


        playToolBox.SetActive(false);
        hungerToolBox.SetActive(false);
        drinkToolBox.SetActive(false);
        sleepToolBox.SetActive(false);

        foodScrollView.SetActive(false);
        drinkScrollView.SetActive(false);
        playScrollView.SetActive(false);

        wakeUpRemindPanel.SetActive(false);
        
        UpdatePetImage();
        virtualCurrencyText.text = "x" + virtualCurrency.ToString();
       

    }

  
    void Update()
    {
        // 減少各個數值
        playValue -= playDecayRate * Time.deltaTime;
        hungerValue -= hungerDecayRate * Time.deltaTime;
        sleepValue -= sleepDecayRate * Time.deltaTime;
        drinkValue -= drinkDecayRate * Time.deltaTime;

        // 限制數值範圍
        playValue = Mathf.Clamp(playValue, 0f, 100f);
        hungerValue = Mathf.Clamp(hungerValue, 0f, 100f);
        sleepValue = Mathf.Clamp(sleepValue, 0f, 100f);
        drinkValue = Mathf.Clamp(drinkValue, 0f, 100f);

        // 更新 Image.fillAmount
        playBarImage.fillAmount = playValue / 100f;
        hungerBarImage.fillAmount = hungerValue / 100f;
        sleepBarImage.fillAmount = sleepValue / 100f;
        drinkBarImage.fillAmount = drinkValue / 100f;

        UpdatePetImage();

        if (isSleeping)
        {
            sleepValue += 3 * Time.deltaTime;
        }
        RemindToWakeUp();
    

        if (exp >= 100)
        {
            exp = 0;
            level += 1;
            SavePlayerLevel();

        }

        // int expThreshold = GetExpThreshold();
        // if (exp >= expThreshold)
        // {
        //     exp -= expThreshold; // 保留多餘的經驗值
        //     level += 1;          // 等級提升
        // }
        levelText.text = level.ToString();
        // expSlider.value = (float)exp / expThreshold; // Slider 表示進度的百分比
        // expText.text = $"{exp}/{expThreshold}";

        expSlider.value = exp; // 將 Slider 的值設為當前經驗值
        // levelBarImage.fillAmount = exp / 100;
    }

    public int GetExpThreshold()
    {
        // 公式：每個等級需要的經驗值逐步增加
        return 100 + (level - 1) * 50; // 每升一級，經驗值上限增加 50
        // 或者用指數增長公式，例如：
        // return (int)(100 * Mathf.Pow(1.2f, level - 1)); 
    }

    public void AddSleepValue()
    {
        isSleeping = !isSleeping;
        if(isSleeping == false)
        {
            PetAnim.SetTrigger("isBlink");
        }
    }


    public void AddPlayValue()
    {
        playValue += 1;
    }


    private void UpdatePetImage()
    {
        // Debug.Log(isSleeping + " " + sleepValue + " " + pet.GetComponent<Image>().sprite);

        if (sleepValue > 20f && !isSleeping)
        {
            pet.GetComponent<Image>().sprite = awakeSprite;
            PetAnim.enabled = true; 
        }
        if (sleepValue < 20f && !isSleeping)
        {
            pet.GetComponent<Image>().sprite = asleepSprite;
            PetAnim.enabled = false; 
        }
        if (sleepValue < 20f && isSleeping)
        {
            pet.GetComponent<Image>().sprite = asleepSprite;
            PetAnim.enabled = false; 
        }
        if (sleepValue > 20f && isSleeping)
        {
            pet.GetComponent<Image>().sprite = asleepSprite;
            PetAnim.enabled = false; 
        }
    }

    public void RemindToWakeUp()
    {
        if(sleepValue > 80f && isSleeping)
        {
            wakeUpRemindPanel.SetActive(true);
        }
        else if(sleepValue > 80f && !isSleeping)
        {
            wakeUpRemindPanel.SetActive(false);
        }
    }

    public void ToolBoxClick()
    {
        playToolBox.SetActive(!playToolBox.activeSelf);
        hungerToolBox.SetActive(!hungerToolBox.activeSelf);
        drinkToolBox.SetActive(!drinkToolBox.activeSelf);
        sleepToolBox.SetActive(!sleepToolBox.activeSelf);
    }

    public void FoodScrollViewClick()
    {
        foodScrollView.SetActive(!foodScrollView.activeSelf);
        drinkScrollView.SetActive(false);
        playScrollView.SetActive(false);
    }

    public void DrinkScrollViewClick()
    {
        foodScrollView.SetActive(false);
        drinkScrollView.SetActive(!drinkScrollView.activeSelf);
        playScrollView.SetActive(false);
    }

    public void PlayScrollViewClick()
    {
        foodScrollView.SetActive(false);
        drinkScrollView.SetActive(false);
        playScrollView.SetActive(!playScrollView.activeSelf);
    }

    private IEnumerator PlayNextAnimationAfterDelay(string currentAnim, string nextAnim)
    {
        // 等待 "isPlay" 動畫播放完畢
        AnimatorStateInfo stateInfo = PetAnim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length + 0.6f;  // 動畫的長度

        // 等待動畫完成
        yield return new WaitForSeconds(animationLength);
        
        // 播放 "isBlink" 動畫
        PetAnim.SetTrigger(nextAnim);
    }

    // 購買物品的通用方法
    public async void BuyItem(string itemName)
    {
        // 檢查玩家是否在睡覺
        if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // 檢查物品是否存在於字典中
        if (itemData.ContainsKey(itemName))
        {
            ItemData item = itemData[itemName];

            // 檢查玩家是否有足夠的錢
            if (virtualCurrency >= item.price)
            {
                // 扣錢
                virtualCurrency -= item.price;

                // 根據物品效果增加對應的數值
                if (item.hungerIncrease > 0)
                {
                    hungerValue += item.hungerIncrease;
                }
                if (item.drinkIncrease > 0)
                {
                    drinkValue += item.drinkIncrease;
                }
                if (item.playIncrease > 0)
                {
                    playValue += item.playIncrease;
                }
                if (item.expIncrease > 0)
                {
                    exp += item.expIncrease;
                }

                // 保存玩家數據
                SavePlayerLevel();
                SavePlayerData();
                await SaveCurrency(virtualCurrency);

                // 播放動畫（如果有動畫觸發器）
                if (!string.IsNullOrEmpty(item.animationTrigger))
                {
                    PetAnim.SetTrigger(item.animationTrigger);
                    StartCoroutine(PlayNextAnimationAfterDelay(item.animationTrigger, "isBlink"));
                }

                Debug.Log($"購買 {itemName} 成功！");
                virtualCurrencyText.text = "x" + virtualCurrency.ToString();
            }
            else
            {
                // 如果錢不夠，顯示提示訊息
                Debug.LogWarning($"錢不夠，無法購買 {itemName}！");
                ShowNotEnoughMoneyMessage();
            }
        }
        else
        {
            Debug.LogWarning($"未找到 {itemName} 的數據！");
        }
    }

    // 顯示錢不夠的提示
    public void ShowNotEnoughMoneyMessage()
    {
        notEnoughMoneyPanel.SetActive(true);
        StartCoroutine(HideNotEnoughMoneyMessageAfterDelay(3f)); 
    }

    private IEnumerator HideNotEnoughMoneyMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notEnoughMoneyPanel.SetActive(false);
    }

    #region 麵包數值
    public void ChickenNuggetBread() //雞塊麵包
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;
        // hungerValue += 20f;
        // exp += 5;
        // SavePlayerLevel();
        // PetAnim.SetTrigger("isEating");
        // StartCoroutine(PlayNextAnimationAfterDelay("isEating", "isBlink"));
        // SavePlayerData();
        // virtualCurrency -= 5;
        BuyItem("雞塊麵包");
    }

    public void Bread() //麵包
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // hungerValue += 5f;
        // exp += 2;
        // SavePlayerLevel();
        // PetAnim.SetTrigger("isEating");
        // StartCoroutine(PlayNextAnimationAfterDelay("isEating", "isBlink"));
        // SavePlayerData();
        // virtualCurrency -= 1;
        BuyItem("麵包");

    }


    public void FishStyleBread() //小魚麵包
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // hungerValue += 10f;
        // exp += 2;
        // SavePlayerLevel();
        // PetAnim.SetTrigger("isEating");
        // StartCoroutine(PlayNextAnimationAfterDelay("isEating", "isBlink"));
        // SavePlayerData();
        // virtualCurrency -= 2;
        BuyItem("小魚麵包");
    }


    public void CrossBread() //叉燒麵包
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // hungerValue += 5f;
        // exp += 2;
        // SavePlayerLevel();
        // PetAnim.SetTrigger("isEating");
        // StartCoroutine(PlayNextAnimationAfterDelay("isEating", "isBlink"));
        // SavePlayerData();
        // virtualCurrency -= 2;
        BuyItem("叉燒麵包");
    }


    #endregion


    #region 飲料數值
    public void CupOfCoffee() //紙杯咖啡
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // drinkValue += 10f;
        // exp += 2;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("紙杯咖啡");
    }

    public void CupOfLittleCoffee() //小杯子咖啡
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // drinkValue += 10f;
        // exp += 2;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("小杯子咖啡");
    }

    public void CupOfBubbleTea() //珍珠奶茶
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // drinkValue += 20f;
        // exp += 2;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("珍珠奶茶");
    }

    public void CupOfPowerDrink() //能量飲料
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // drinkValue += 30f;
        // exp += 2;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("能量飲料");
    }

    #endregion


    #region 玩具數值
    public void DumbBell() //啞鈴
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;

        // playValue += 20f;
        // exp += 10;
        // SavePlayerLevel();
        // PetAnim.SetTrigger("isPlay");
        // StartCoroutine(PlayNextAnimationAfterDelay("isPlay", "isBlink"));
        // SavePlayerData();
        BuyItem("啞鈴");
    }

    public void USAFootBall() //橄欖球
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;
        // playValue += 15f;
        // exp += 5;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("橄欖球");
    }


    public void TennisBall() //網球
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;
        // playValue += 10f;
        // exp += 3;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("網球");
    }

    public void Sock() //袜子
    {
        // if (isSleeping || pet.GetComponent<Image>().sprite == asleepSprite) return;
        // playValue += 5f;
        // exp += 1;
        // SavePlayerLevel();
        // SavePlayerData();
        BuyItem("袜子");
    }
    #endregion



    #region unityCloud
    public async void SavePlayerLevel()
    {
        var data = new Dictionary<string, object>
        {
            { "level", level },
            { "exp", exp }
        };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Player data saved successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save player data: {ex.Message}");
        }
    }

    public async void LoadPlayerLevel()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "level", "exp" });

            if (savedData.ContainsKey("level"))
            {
                level = int.Parse(savedData["level"].ToString());
            }
            if (savedData.ContainsKey("exp"))
            {
                exp = int.Parse(savedData["exp"].ToString());
            }

            Debug.Log($"Player data loaded: Level = {level}, EXP = {exp}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load player data: {ex.Message}");
        }
    }

    public async void SavePlayerData()
    {
        var data = new Dictionary<string, object>
        {
            { "playValue", playValue },
            { "hungerValue", hungerValue },
            { "sleepValue", sleepValue },
            { "drinkValue", drinkValue }
        };

        try
        {
            // 使用 Cloud Save 儲存數據
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Player data saved successfully!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save player data: {ex.Message}");
        }
    }

    public async void LoadPlayerData()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "playValue", "hungerValue", "sleepValue", "drinkValue" });

        
            if (savedData.ContainsKey("playValue"))
            {
                playValue = float.Parse(savedData["playValue"].ToString());
            }
            if (savedData.ContainsKey("hungerValue"))
            {
                hungerValue = float.Parse(savedData["hungerValue"].ToString());
            }
            if (savedData.ContainsKey("sleepValue"))
            {
                sleepValue = float.Parse(savedData["sleepValue"].ToString());
            }
            if (savedData.ContainsKey("drinkValue"))
            {
                drinkValue = float.Parse(savedData["drinkValue"].ToString());
            }

            Debug.Log($"Player data loaded: PlayValue = {playValue}, HungerValue = {hungerValue}, SleepValue = {sleepValue}, DrinkValue = {drinkValue}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load player data: {ex.Message}");
        }
    }

    public async void SaveIsSleeping(bool isSleeping)
    {
        try
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "isSleeping", isSleeping }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("isSleeping has been saved to the cloud.");

        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save isSleeping: {e.Message}");
        }
    }

    public async Task<bool> LoadIsSleeping()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "isSleeping" });

            if (data.TryGetValue("isSleeping", out string isSleepingValue))
            {
                isSleeping = bool.Parse(isSleepingValue);
                Debug.Log($"Loaded isSleeping: {isSleeping}");
                return isSleeping;
            }
            else
            {
                Debug.Log("isSleeping not found in the cloud.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load isSleeping: {e.Message}");
            return false;
        }
    }

    public async Task SaveCurrency(int amount)
    {
        try 
        {
            var saveData = new Dictionary<string, object>
            {
                { "virtualCurrency", amount }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);
            Debug.Log($"貨幣已成功儲存：{amount}");
        }
        catch (Exception e)
        {
            Debug.LogError($"儲存貨幣失敗：{e.Message}");
        }
    }

    public async Task<int> GetVirtualCurrency()
    {
        try 
        {
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(
                new HashSet<string> { "virtualCurrency" }
            );

            if (data.ContainsKey("virtualCurrency"))
            {
                return Convert.ToInt32(data["virtualCurrency"]);
            }

            return 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"獲取虛擬貨幣失敗：{e.Message}");
            return 0;
        }
    }

    public async Task UpdatePlayerVirtualCurrency()
    {
        try
        {
            // 獲取虛擬貨幣
            int currency = await GetVirtualCurrency();
            
            // 賦值到 playerData
            virtualCurrency = currency;

            Debug.Log($"玩家虛擬貨幣已更新：{virtualCurrency}");
        }
        catch (Exception e)
        {
            Debug.LogError($"更新玩家虛擬貨幣失敗：{e.Message}");
        }
    }

    #endregion


    #region 遊戲結束時儲存數據
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SavePlayerData(); // 當遊戲被暫停時儲存數據
            SaveIsSleeping(isSleeping);
        }
    }

    private void OnApplicationQuit()
    {
        SavePlayerData(); // 當應用退出時儲存數據
        SaveIsSleeping(isSleeping);
    }
    #endregion

}
