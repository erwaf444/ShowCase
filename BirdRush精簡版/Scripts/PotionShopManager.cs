// using UnityEngine.UI;
// using UnityEngine;
// using TMPro;

// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine.SceneManagement;
// using Unity.Services.Core;
// using Unity.Services.CloudSave;
// using System;

// public class PotionShopManager : MonoBehaviour
// {
//     public static PotionShopManager instance;

//     public int coins;
//     public Potion[] potions;
//     public TextMeshProUGUI coinsText;
//     public GameObject shopUI;
//     public Transform potionShopContent;
//     public GameObject itemPrefab;
//     public TextMeshProUGUI MainMenuText;



  


    

//     private async void Awake() 
//     {
//         if (instance == null) {
//             instance = this;
//         } else {
//             Destroy(gameObject);
//         }

//         DontDestroyOnLoad(gameObject);

//         try
//         {
//             await UnityServices.InitializeAsync();
//             Debug.Log("Unity Services initialized successfully");
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
//         }

//     }

//     async void Start()
//     {
//         int MainMenuCoinsText = bouncyBallCoins.coins;
//         MainMenuText.text =  "Coins: " + MainMenuCoinsText.ToString();
//         coins = bouncyBallCoins.coins;
//         coinsText.text = coins.ToString("00000");

        
        

//         foreach (Potion potion in potions) 
//         {
//             GameObject item = Instantiate(itemPrefab, potionShopContent);

//             potion.itemRef = item;

//             foreach (Transform child in item.transform)
//             {
//                 if (child.gameObject.name == "Cost"){
//                     child.gameObject.GetComponent<TextMeshProUGUI>().text = "$" + potion.cost.ToString();
//                 } else if (child.gameObject.name == "Name"){
//                     child.gameObject.GetComponent<TextMeshProUGUI>().text = potion.name;
//                 } else if (child.gameObject.name == "Image"){
//                     child.gameObject.GetComponent<Image>().sprite = potion.image;
//                 }
//             }

//             item.GetComponent<Button>().onClick.AddListener(() => {
//                 BuyPotion(potion);
//             });
//         }


//     }

//     void Update()
//     {
//         coins = bouncyBallCoins.coins;
//         coinsText.text = coins.ToString("00000");
//     }

//     private string FormatTimeSpan(TimeSpan timeSpan)
//     {
//         // 获取小时和分钟
//         int hours = timeSpan.Days * 24 + timeSpan.Hours;
//         int minutes = timeSpan.Minutes;

//         // 格式化时间字符串
//         string timeString = $"{hours} 小时 {minutes} 分钟";

//         return timeString;
//     }

//     private TimeSpan CalculateRemainingCooldownFor24Hours(TimeSpan elapsed)
//     {
//         // 总冷却时间为24小时
//         TimeSpan totalCooldown = TimeSpan.FromHours(24);

//         // 计算剩余时间
//         TimeSpan remaining = totalCooldown - elapsed;

//         return remaining;
//     }

//     private TimeSpan CalculateRemainingCooldownFor8Hours(TimeSpan elapsed)
//     {
//         TimeSpan totalCooldown = TimeSpan.FromHours(8);

//         // 计算剩余时间
//         TimeSpan remaining = totalCooldown - elapsed;

//         return remaining;
//     }

//     private TimeSpan CalculateRemainingCooldownFor1Hours(TimeSpan elapsed)
//     {
//         TimeSpan totalCooldown = TimeSpan.FromHours(1);

//         // 计算剩余时间
//         TimeSpan remaining = totalCooldown - elapsed;

//         return remaining;
//     }

//     private TimeSpan CalculateRemainingCooldownForHalfHours(TimeSpan elapsed)
//     {
//         TimeSpan totalCooldown = TimeSpan.FromHours(0.5);

//         // 计算剩余时间
//         TimeSpan remaining = totalCooldown - elapsed;

//         return remaining;
//     }

//     private TimeSpan CalculateRemainingCooldownFor10Minutes(TimeSpan elapsed)
//     {
//         TimeSpan totalCooldown = TimeSpan.FromMinutes(10);

//         // 计算剩余时间
//         TimeSpan remaining = totalCooldown - elapsed;

//         return remaining;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor24HoursForTimeSpan(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 24)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor8HoursForTimeSpan(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 8)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor1HoursForTimeSpan(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 1)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownForHalfHoursForTimeSpan(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 0.5)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownForHalfHoursForTimeSpanTwo(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 0.5)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor10MinutesForTimeSpanIncreaseMaxLives(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor10MinutesForTimeSpanIncreasePaddleWidthWeak(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor10MinutesForTimeSpanIncreasePaddleWidthStrong(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor10MinutesForTimeSpanDecreasePaddleWidthWeak(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownFor10MinutesForTimeSpanDecreasePaddleWidthStrong(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }

//     private async Task<TimeSpan?> CheckPurchaseCooldownForHalfHoursForTimeSpanAddBall(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 0.5)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return timeSinceLastPurchase;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return null;
//     }



//     public async void BuyPotion(Potion potion) {
//         if (coins >= potion.cost) {
//             if (potion.name == "增加生命最大值" && bouncyBallCoins.lives >= 5) {
//                 ShowPotionCoolDownMessage("生命已经达到最大值。");
//                 return;
//             }

//             if (potion.name == "销毁场景全部砖块")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor24HoursForTimeSpan(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor24Hours(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor24Hours(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "销毁场景一半砖块")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor8HoursForTimeSpan(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor8Hours(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor8Hours(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "销毁场景5个砖块")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor1HoursForTimeSpan(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor1Hours(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor1Hours(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "每秒+10金币")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownForHalfHoursForTimeSpan(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownForHalfHours(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownForHalfHours(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "增加100金币")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownForHalfHoursForTimeSpanTwo(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownForHalfHoursTwo(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownForHalfHours(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "增加生命最大值")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor10MinutesForTimeSpanIncreaseMaxLives(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor10MinutesForIncreaseMaxLives(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor10Minutes(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "增加平台宽度/效果弱")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor10MinutesForTimeSpanIncreasePaddleWidthWeak(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor10MinutesForIncreasePaddleWidthWeak(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor10Minutes(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "增加平台宽度/效果强")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor10MinutesForTimeSpanIncreasePaddleWidthStrong(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor10MinutesForIncreasePaddleWidthStrong(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor10Minutes(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "减少平台宽度/效果弱")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor10MinutesForTimeSpanDecreasePaddleWidthWeak(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor10MinutesForDecreasePaddleWidthWeak(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor10Minutes(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "减少平台宽度/效果强")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownFor10MinutesForTimeSpanDecreasePaddleWidthStrong(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownFor10MinutesForDecreasePaddleWidthStrong(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownFor10Minutes(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }

//             if (potion.name == "增加一颗球")
//             {
//                 TimeSpan? coolDown = await CheckPurchaseCooldownForHalfHoursForTimeSpanAddBall(potion.name);
//                 bool canBuy = await CheckPurchaseCooldownForHalfHourForAddBall(potion.name);
//                 if (!canBuy)
//                 {
//                     ShowPotionCoolDownMessage("目前还不能购买此道具。请等待冷却。");
//                     TimeSpan remainingCooldown = CalculateRemainingCooldownForHalfHours(coolDown.Value);
//                     string formattedTime = FormatTimeSpan(remainingCooldown);
//                     Debug.Log($"{potion.name} 冷却时间还剩下 {formattedTime}。");
//                     bouncyBallCoins.ShowPotionCoolDownTime("冷却时间还剩下:" + formattedTime);
//                     return;
//                 }
//             }
          
        
//             coins -= potion.cost;
//             bouncyBallCoins.coins -= potion.cost;

          
            
//             Debug.Log("PotionShopManager coins: " + coins);
//             Debug.Log("coins: " + bouncyBallCoins.coins);
//             ApplyBuyPotion(potion);
//             UpdateCoinsText();

//             if (potion.name == "销毁场景全部砖块")
//             {
//                 await SavePurchaseTimeFor24Hours(potion.name);
//             }
//             if (potion.name == "销毁场景一半砖块")
//             {
//                 await SavePurchaseTimeFor8Hours(potion.name);
//             }
//             if (potion.name == "销毁场景5个砖块")
//             {
//                 await SavePurchaseTimeFor1Hours(potion.name);
//             }
//             if (potion.name == "每秒+10金币")
//             {
//                 await SavePurchaseTimeForHalfHours(potion.name);
//             }
//             if (potion.name == "增加100金币")
//             {
//                 await SavePurchaseTimeForHalfHoursTwo(potion.name);
//             }
//             if (potion.name == "增加生命最大值")
//             {
//                 await SavePurchaseTimeFor10MinutesForIncreaseMaxLives(potion.name);
//             }
//             if (potion.name == "增加平台宽度/效果弱")
//             {
//                 await SavePurchaseTimeFor10MinutesForIncreasePaddleWidthWeak(potion.name);
//             }
//             if (potion.name == "增加平台宽度/效果强")
//             {
//                 await SavePurchaseTimeFor10MinutesForIncreasePaddleWidthStrong(potion.name);
//             }
//             if (potion.name == "减少平台宽度/效果弱")
//             {
//                 await SavePurchaseTimeFor10MinutesForDecreasePaddleWidthWeak(potion.name);
//             }
//             if (potion.name == "减少平台宽度/效果强")
//             {
//                 await SavePurchaseTimeFor10MinutesForDecreasePaddleWidthStrong(potion.name);
//             }
//             if (potion.name == "增加一颗球")
//             {
//                 await SavePurchaseTimeForHalfHourForAddBall(potion.name);
//             }
//         } 

//         if (coins < potion.cost)
//         {
//             bouncyBallCoins.NotEnoughCoins();
//         }
//     }

//     public void ShowPotionCoolDownMessage(string message)
//     {
//         potionCollDownText.text = message;
//         bouncyBallCoins.coolDownAnimObject.SetActive(true);

//         potionAnimator.SetTrigger("Show");

//     }

//     public void HidePotionCoolDownAnim()
//     {
//         bouncyBallCoins.coolDownAnimObject.SetActive(false);
//         potionAnimator.SetTrigger("Hide");

//     }

//     public void HideUsePotionAnim()
//     {
//         bouncyBallCoins.usePotionAnimObject.SetActive(false);
//         usePotionAnimator.SetTrigger("Hide");
//     }

//     public void HideUsePotionWithCoinsAnim()
//     {
//         bouncyBallCoins.usePotionAnimObjectWithCoins.SetActive(false);
//         usePotionAnimatorWithCoins.SetTrigger("Hide");
//     }

//     public void HideUsePotionWithHeartAnim()
//     {
//         bouncyBallCoins.UsePotionAnimObjectWithHeart.SetActive(false);
//         usePotionAnimatorWithHeart.SetTrigger("Hide");
//     }

//     public void HideUsePotionWithPaddleBigAnim()
//     {
//         bouncyBallCoins.UsePotionAnimObjectWithPaddleBig.SetActive(false);
//         usePotionAnimatorWithPaddleBig.SetTrigger("Hide");
//     }

//     public void HideUsePotionWithPaddleSmallAnim()
//     {
//         bouncyBallCoins.UsePotionAnimObjectWithPaddleSmall.SetActive(false);
//         usePotionAnimatorWithPaddleSmall.SetTrigger("Hide");
//     }

//     public void HideUsePotionWithAddBallAnim()
//     {
//         bouncyBallCoins.UsePotionAnimObjectWithAddBall.SetActive(false);
//         usePotionAnimatorWithAddBall.SetTrigger("Hide");
//     }

//     // public void ShowPotionCoolDownTime(string time)
//     // {
//     //     bouncyBallCoins.coolDownTimeText.text = time;
//     //     bouncyBallCoins.coolDownTimeTip.SetActive(true);
//     //     Debug.Log("Starting HidePotionCoolDownTime coroutine.");
//     //     StartCoroutine(HidePotionCoolDownTime());
//     // }

//     // IEnumerator HidePotionCoolDownTime()
//     // {
//     //     Debug.Log("HidePotionCoolDownTime");
//     //     yield return new WaitForSeconds(3f);
//     //     bouncyBallCoins.coolDownTimeTip.SetActive(false);
//     //     Debug.Log("End");

//     // }

  

//     public void UpdateCoinsText()
//     {   
//         if (coinsText != null)
//         {
//             coinsText.text = bouncyBallCoins.coins.ToString("00000");

//         }
//         if (bouncyBallCoins != null)
//         {
//             bouncyBallCoins.coinsText.text = coinsText.text;
//         }
//     }

//     public void ApplyBuyPotion(Potion potion) 
//     {
//         switch (potion.name){
//             case "销毁场景5个砖块":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.usePotionAnimObject.SetActive(true);
//                     usePotionAnimator.SetTrigger("GoAnim");
//                     playerMovement.AutoDestroy5Brick();
//                 }

//                 break;
//             case "销毁场景一半砖块":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.usePotionAnimObject.SetActive(true);
//                     usePotionAnimator.SetTrigger("GoAnim");
//                     playerMovement.AutoDestroyHalfBrick();
//                 }
//                 break;
//             case "销毁场景全部砖块":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.usePotionAnimObject.SetActive(true);
//                     usePotionAnimator.SetTrigger("GoAnim");
//                     playerMovement.AutoDestroyAllBrick();
//                 }
//                 break;
//             case "每秒+10金币":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.usePotionAnimObjectWithCoins.SetActive(true);
//                     usePotionAnimatorWithCoins.SetTrigger("GoAnim");
//                     playerMovement.AutoAddCoins10Sec();
//                 }
//                 break;
//             case "增加100金币":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.usePotionAnimObjectWithCoins.SetActive(true);
//                     usePotionAnimatorWithCoins.SetTrigger("GoAnim");
//                     playerMovement.Add100Coins();
//                 }
//                 break;
//             case "增加生命最大值":
//                 if (playerMovement != null )
//                 {
//                     if (bouncyBallCoins.lives < 5 )
//                     {
//                         bouncyBallCoins.UsePotionAnimObjectWithHeart.SetActive(true);
//                         usePotionAnimatorWithHeart.SetTrigger("GoAnim");
//                         playerMovement.AddLives();
//                     } 
//                 }
//                 break;
//             case "增加平台宽度/效果弱":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.UsePotionAnimObjectWithPaddleBig.SetActive(true);
//                     usePotionAnimatorWithPaddleBig.SetTrigger("GoAnim");
//                     playerMovement.AddPaddleWidth();
//                 }
//                 break;
//             case "增加平台宽度/效果强":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.UsePotionAnimObjectWithPaddleBig.SetActive(true);
//                     usePotionAnimatorWithPaddleBig.SetTrigger("GoAnim");
//                     playerMovement.AddPaddleBigWidth();
//                 }
//                 break;
//             case "减少平台宽度/效果弱":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.UsePotionAnimObjectWithPaddleSmall.SetActive(true);
//                     usePotionAnimatorWithPaddleSmall.SetTrigger("GoAnim");
//                     playerMovement.MinusPaddleWidth();
//                 }
//                 break;
//             case "减少平台宽度/效果强":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.UsePotionAnimObjectWithPaddleSmall.SetActive(true);
//                     usePotionAnimatorWithPaddleSmall.SetTrigger("GoAnim");
//                     playerMovement.MinusPaddleBigWidth();
//                 }
//                 break;
//             case "增加一颗球":
//                 if (playerMovement != null)
//                 {
//                     bouncyBallCoins.UsePotionAnimObjectWithAddBall.SetActive(true);
//                     usePotionAnimatorWithAddBall.SetTrigger("GoAnim");
//                     playerMovement.AddBall();
//                 }
//                 break;
//             default:
//                 Debug.Log("No potionBuy");
//                 break;
//         }
//     }

//     private string GetEnglishKey(string potionName)
//     {
//         switch (potionName)
//         {
//             case "销毁场景5个砖块":
//                 return "Destroy5Bricks";
//             case "销毁场景一半砖块":
//                 return "DestroyHalfBricks";
//             case "销毁场景全部砖块":
//                 return "DestroyAllBricks";
//             case "每秒+10金币":
//                 return "Add10CoinsPerSec";
//             case "增加100金币":
//                 return "Add100Coins";
//             case "增加生命最大值":
//                 return "IncreaseMaxLives";
//             case "增加平台宽度/效果弱":
//                 return "IncreasePaddleWidthWeak";
//             case "增加平台宽度/效果强":
//                 return "IncreasePaddleWidthStrong";
//             case "减少平台宽度/效果弱":
//                 return "DecreasePaddleWidthWeak";
//             case "减少平台宽度/效果强":
//                 return "DecreasePaddleWidthStrong";
//             case "增加一颗球":
//                 return "AddBall";
//             default:
//                 return "UnknownPotion";
//         }
//     }

//     private async Task SavePurchaseTimeFor24Hours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor24Hours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 24)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor8Hours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor8Hours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 8)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor1Hours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor1Hours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 1)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeForHalfHours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownForHalfHours(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 0.5)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeForHalfHoursTwo(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownForHalfHoursTwo(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 0.5)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor10MinutesForIncreaseMaxLives(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor10MinutesForIncreaseMaxLives(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor10MinutesForIncreasePaddleWidthWeak(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor10MinutesForIncreasePaddleWidthWeak(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor10MinutesForIncreasePaddleWidthStrong(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor10MinutesForIncreasePaddleWidthStrong(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor10MinutesForDecreasePaddleWidthWeak(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor10MinutesForDecreasePaddleWidthWeak(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeFor10MinutesForDecreasePaddleWidthStrong(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownFor10MinutesForDecreasePaddleWidthStrong(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalMinutes < 10)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }

//     private async Task SavePurchaseTimeForHalfHourForAddBall(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         var data = new Dictionary<string, object>
//         {
//             { $"{englishKey}_purchase_time", DateTime.UtcNow.ToString("g") }
//         };

//         try
//         {
//             await CloudSaveService.Instance.Data.ForceSaveAsync(data);
//             Debug.Log($"{englishKey} purchase time saved successfully");
//         }
//         catch
//         {
//             Debug.LogError($"Error saving {englishKey} purchase time");
//         }
//     }

//     private async Task<bool> CheckPurchaseCooldownForHalfHourForAddBall(string potionName)
//     {
//         string englishKey = GetEnglishKey(potionName);
//         Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { $"{englishKey}_purchase_time" });
//         if (data.ContainsKey($"{englishKey}_purchase_time"))
//         {
//             if (DateTime.TryParse(data[$"{englishKey}_purchase_time"], out DateTime lastPurchaseTime))
//             {
//                 TimeSpan timeSinceLastPurchase = DateTime.UtcNow - lastPurchaseTime;
//                 if (timeSinceLastPurchase.TotalHours < 0.5)
//                 {
//                     Debug.Log($"{englishKey} was purchased {timeSinceLastPurchase.TotalHours} hours ago. Cannot purchase yet.");
//                     return false;
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"Failed to parse {englishKey} purchase time: {data[$"{potionName}_purchase_time"]}");
//             }
//         }
//         return true;
//     }



// }




// [System.Serializable]
// public class Potion {
//     public string name;
//     public int cost;
//     public Sprite image;
//     // [HideInInspector] public int quantity;
//     [HideInInspector] public GameObject itemRef;
// }