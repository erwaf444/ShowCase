using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using TMPro;

public class PaddleAutoMinusCloudSave : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private float initialSpeed;
    private float initialWidth;
    private float initialHeight;
    private float initialBallSize;
    private TimeSpan reductionInterval = TimeSpan.FromSeconds(10);
    
    public BouncyBall bouncyBall;
    public TextMeshProUGUI timeLeftTextForSpeedText;
    public TextMeshProUGUI timeLeftTextForWidthAndHeightText;
    public TextMeshProUGUI timeLeftTextForBallSizeText; 

    // public GameObject timeLeftPanel;
    public ShopManager shopManager;
    

    async void Awake()
    {
        Debug.Log("Awake called");

        initialSpeed = 50;
        initialHeight = 0.06f;
        initialWidth = 0.07f;
        initialBallSize = 0.05f;

        
        // await TimeLeftPanelAwakeDelay();

        await shopManager.LoadUpgrades();
        

    }

    // public async Task TimeLeftPanelAwakeDelay()
    // {
    //     Debug.Log("TimeLeftPanelAwakeDelay Start");
    //     float speed = await LoadSpeed();
    //     float ballSize = await LoadBallSize();
    //     float paddleWidth = await LoadPaddleWidth();
    //     // Vector3 ballScale = bouncyBall.transform.localScale;
    //     // Vector3 paddleScale = playerMovement.transform.localScale;
    //     // await Task.Delay(1000);
    //     await TimeLeftForSpeed();
    //     // await Task.Delay(1000);
    //     await TimeLeftForWidthAndHeight();
    //     // await Task.Delay(1000);
    //     await TimeLeftForBallSize();

    //     if (
    //         speed > initialSpeed || 
    //         ballSize > initialBallSize ||
    //         paddleWidth > initialWidth
    //         // ballScale.x > initialBallSize || 
    //         // ballScale.y > initialBallSize || 
    //         // paddleScale.x > initialWidth ||
    //         // paddleScale.y > initialHeight
    //     ) {
    //         Debug.Log("TimeLeftPanelAwakeDelay SetPanelTrue");
    //         timeLeftPanel.SetActive(true);
    //         StartCoroutine(TimeLeftPanelRoutine());
    //     } else {
    //         timeLeftPanel.SetActive(false);
    //     }
    // }

    // IEnumerator TimeLeftPanelRoutine()
    // {
    //     yield return new WaitForSeconds(3);
    //     timeLeftPanel.SetActive(false);
    // }

    async void Update()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
   

        
    }

    public async Task CheckAndReduceSpeedRoutine()
    {
        // Debug.Log("Checking and reducing speed...");
        // while (true)
        // {

            // await Task.Delay(TimeSpan.FromSeconds(30));
            await CheckAndReduceSpeed();

        // }
        
     
    }

    public async Task SaveLastReductionTimeForSpeed()
    {
        var data = new Dictionary<string, object> { {"LastReductionTime", DateTime.UtcNow.ToString("g")} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("LastReductionTime" + data["LastReductionTime"]);
            // Debug.Log("LastReductionTime saved successfully");
        }
        catch 
        {
            Debug.Log($"Error saving LastReductionTime: ");
        }
    }

    public async Task<DateTime> LoadLastReductionTimeForSpeed()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "LastReductionTime" });
        if (data.ContainsKey("LastReductionTime"))
        {
            if (DateTime.TryParse(data["LastReductionTime"], out DateTime lastReductionTime))
            {
                Debug.Log("Loaded LastReductionTime: " + lastReductionTime);
                return lastReductionTime;
            }
            else
            {
                Debug.LogError("Failed to parse LastReductionTime data: " + data["LastReductionTime"]);
                return DateTime.MinValue;
            }
        }
        else
        {
            Debug.Log("No LastReductionTime data found");
            return DateTime.MinValue;
        }
    }

    private async Task CheckAndReduceSpeed()
    {
        // Debug.Log("Checking and reducing speed...");
        DateTime lastReductionTime = await LoadLastReductionTimeForSpeed();
        // Debug.Log("LastReductionTime: " + lastReductionTime);
        if (lastReductionTime == DateTime.MinValue)
        {
            // 如果没有找到上次更新时间，则将当前时间设置为上次更新时间
            await SaveLastReductionTimeForSpeed();
            return;
        }
        // Debug.Log("DateTime.UtcNow: " + DateTime.UtcNow);
        TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastReductionTime;
        // Debug.Log("Time since last update: " + timeSinceLastUpdate.TotalSeconds);
        if (playerMovement.speed > initialSpeed)
        {
            int upgrade = await shopManager.LoadUpgradesForPaddleAutoMinusScript("IncreasePaddleSpeed");
            Debug.Log("Loaded upgrade value: " + upgrade);
            int intervalsPassed = (int)(timeSinceLastUpdate.TotalDays / 1); // 使用秒数计算间隔
            if (intervalsPassed > 0)
            {
                playerMovement.speed -= intervalsPassed * 0.5f;
                upgrade = upgrade - intervalsPassed;
                if (upgrade < 0)
                {
                    upgrade = 0;
                }
                SaveUpgrade(0, upgrade);
                
                if (playerMovement.speed < initialSpeed)
                {
                    playerMovement.speed = initialSpeed;
                }
                Debug.Log("Reduced speed by " + (intervalsPassed * 0.5f) + ". New speed: " + playerMovement.speed);

                // 保存新的speed值和当前时间
                await bouncyBall.SaveSpeed();
                await SaveLastReductionTimeForSpeed();
            }
        }
    }

    public async Task CheckAndReduceWidthAndHeightRoutine()
    {   
        // Debug.Log("Checking and reducing width and height...");
        // while (true)
        // {
            // await Task.Delay(TimeSpan.FromHours(24)); 
            // Debug.Log("Interval passed, checking and reducing width and height...");
            await CheckAndReduceWidthAndHeight();
        // }
    }

    public async Task SaveLastReductionTimeForWidthAndHeight()
    {
        var data = new Dictionary<string, object> { {"LastReductionTimeForWidthAndHeight", DateTime.UtcNow.ToString("g")} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("LastReductionTimeForWidthAndHeight saved successfully");
        }
        catch 
        {
            Debug.Log($"Error saving LastReductionTimeForWidthAndHeight: ");
        }
    }

    public async Task<DateTime> LoadLastReductionTimeForWidthAndHeight()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "LastReductionTimeForWidthAndHeight" });
        if (data.ContainsKey("LastReductionTimeForWidthAndHeight"))
        {
            if (DateTime.TryParse(data["LastReductionTimeForWidthAndHeight"], out DateTime lastReductionTimeForWidthAndHeight))
            {
                Debug.Log("Loaded LastReductionTimeForWidthAndHeight: " + lastReductionTimeForWidthAndHeight);
                return lastReductionTimeForWidthAndHeight;
            }
            else
            {
                Debug.LogError("Failed to parse LastReductionTimeForWidthAndHeight data: " + data["LastReductionTimeForWidthAndHeight"]);
                return DateTime.MinValue;
            }
        }
        else
        {
            Debug.Log("No LastReductionTimeForWidthAndHeight data found");
            return DateTime.MinValue;
        }
    }

    private async Task CheckAndReduceWidthAndHeight()
    {
        // Debug.Log("Checking and reducing width and height...");
        DateTime lastReductionTime = await LoadLastReductionTimeForWidthAndHeight();
        if (lastReductionTime == DateTime.MinValue)
        {
            // 如果没有找到上次更新时间，则将当前时间设置为上次更新时间
            await SaveLastReductionTimeForWidthAndHeight();
            return;
        }

        TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastReductionTime;
        Debug.Log("Time since last update: " + timeSinceLastUpdate.TotalSeconds);

        if (playerMovement != null)
        {
            // Debug.Log("PlayerMovement found, checking and reducing width and height...");
            if 
            (
                playerMovement.transform.localScale.x > initialWidth 
                && playerMovement.transform.localScale.y > initialHeight 
            )
                {
                    int upgrade = await shopManager.LoadUpgradesForPaddleAutoMinusScript("IncreasePaddleWidth");
                    Debug.Log("Loaded upgrade value: " + upgrade);
                    // Debug.Log("Started reducing width and height...");
                    int intervalsPassed = (int)(timeSinceLastUpdate.TotalDays/ 1); // 使用秒数计算间隔
                    if (intervalsPassed > 0)
                    {
                        Vector3 newScale = playerMovement.transform.localScale;
                        newScale.x -= intervalsPassed * 0.01f;
                        newScale.y -= intervalsPassed * 0.008f;
                        upgrade = upgrade - intervalsPassed;
                        if (upgrade < 0)
                        {
                            upgrade = 0;
                        }
                        SaveUpgrade(1, upgrade);
                        if (newScale.x < initialWidth)
                        {
                            newScale.x = initialWidth;
                        }
                        if (newScale.y < initialHeight)
                        {
                            newScale.y = initialHeight;
                        }
                        playerMovement.transform.localScale = newScale;
                        // Debug.Log("Reduced width and height by " + (intervalsPassed * 0.01f) + " and " + (intervalsPassed * 0.008f) + ". New width and height: " + playerMovement.transform.localScale);
                        // 保存新的speed值和当前时间
                        await playerMovement.SavePaddleWidth();
                        await playerMovement.SavePaddleHeight();
                        await SaveLastReductionTimeForWidthAndHeight();
                    }
                }
        }
     
        // Debug.Log("AutoMinus Done");
    }



    public async Task CheckAndReduceBallSizeRoutine()
    {
        Debug.Log("Checking and reducing ballSize...");
        // while (true)
        // {
            // await Task.Delay(TimeSpan.FromHours(24)); 
            await CheckAndReduceBallSize();
        // }
    }

    public async Task SaveLastReductionTimeForBallSize()
    {
        var data = new Dictionary<string, object> { {"SaveLastReductionTimeForBallSize", DateTime.UtcNow.ToString("g")} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("SaveLastReductionTimeForBallSize saved successfully");
        }
        catch 
        {
            Debug.Log($"Error saving SaveLastReductionTimeForBallSize: ");
        }
    }

    public async Task<DateTime> LoadLastReductionTimeForBallSize()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "SaveLastReductionTimeForBallSize" });
        if (data.ContainsKey("SaveLastReductionTimeForBallSize"))
        {
            if (DateTime.TryParse(data["SaveLastReductionTimeForBallSize"], out DateTime lastReductionTimeForBallSize))
            {
                Debug.Log("Loaded SaveLastReductionTimeForBallSize: " + lastReductionTimeForBallSize);
                return lastReductionTimeForBallSize;
            }
            else
            {
                Debug.LogError("Failed to parse SaveLastReductionTimeForBallSize data: " + data["SaveLastReductionTimeForBallSize"]);
                return DateTime.MinValue;
            }
        }
        else
        {
            Debug.Log("No SaveLastReductionTimeForBallSize data found");
            return DateTime.MinValue;
        }
    }

    private async Task CheckAndReduceBallSize()
    {
        Debug.Log("Checking and reducing ballSize...");
        DateTime lastReductionTime = await LoadLastReductionTimeForBallSize();
        if (lastReductionTime == DateTime.MinValue)
        {
            // 如果没有找到上次更新时间，则将当前时间设置为上次更新时间
            await SaveLastReductionTimeForBallSize();
            return;
        }

        TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastReductionTime;
        Debug.Log("Time since last update: " + timeSinceLastUpdate.TotalSeconds);
        
        if 
        (
            bouncyBall.transform.localScale.x > initialBallSize 
            && bouncyBall.transform.localScale.y > initialBallSize 
        )
        {
            int upgrade = await shopManager.LoadUpgradesForPaddleAutoMinusScript("IncreaseBallSize");
            Debug.Log("Loaded upgrade value: " + upgrade);
            Vector3 newScale = bouncyBall.transform.localScale;
            int intervalsPassed = (int)(timeSinceLastUpdate.TotalDays / 1);
            if (intervalsPassed > 0)
            {
                newScale.x -= intervalsPassed * 0.001f;
                newScale.y -= intervalsPassed * 0.001f;
                upgrade = upgrade - intervalsPassed;
                if (upgrade < 0)
                {
                    upgrade = 0;
                }
                SaveUpgrade(2, upgrade);
                if (newScale.x < initialBallSize)
                {
                    newScale.x = initialBallSize;
                }
                if (newScale.y < initialBallSize)
                {
                    newScale.y = initialBallSize;
                }
                bouncyBall.transform.localScale = newScale;
                Debug.Log("Reduced ballSize by " + (intervalsPassed * 0.001f) + ". New ballSize: " + newScale);
                // 保存新的ballSize值和当前时间
                await bouncyBall.SaveBallSize();
                await SaveLastReductionTimeForBallSize();
            }
        } 
   
        Debug.Log("AutoMinus Done");

    }

    // 自動減少剩餘時間PaddleSpeed

    // public void SavePaddleSpeedBuyTime()
    // {
    //     DateTime now = DateTime.UtcNow;
    //     PlayerPrefs.SetString("PaddleSpeedBuyTime", now.ToString("g"));
    //     PlayerPrefs.Save();

    // }

    // public DateTime? LoadPaddleSpeedBuyTime()
    // {
    //     if (PlayerPrefs.HasKey("PaddleSpeedBuyTime"))
    //     {
    //         string storedTime = PlayerPrefs.GetString("PaddleSpeedBuyTime");
    //         if (DateTime.TryParse(storedTime, out DateTime buyTime))
    //         {
    //             return buyTime;
    //         }
    //     }
    //     return null;
    // }

    public async Task<float> LoadSpeed()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "speed" });
        if (data.ContainsKey("speed"))
        {
            playerMovement.speed = float.Parse(data["speed"]);
            Debug.Log("Loaded speed: " + playerMovement.speed);
            return playerMovement.speed;
        }
        else
        {
            Debug.Log("No speed data found");
            return 0;
        }
    }

    public async Task TimeLeftForSpeed()
    {
        float speed = await LoadSpeed();
        if (speed > 50)
        {
            int upgrade = await shopManager.LoadUpgradesForPaddleAutoMinusScript("IncreasePaddleSpeed");
            DateTime lastReductionTime  = await LoadLastReductionTimeForSpeed();
            Debug.Log("上次購買的時間: " + lastReductionTime);
            if (lastReductionTime != null)
            {
                TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastReductionTime;
                Debug.Log("Time since last update: " + timeSinceLastUpdate.TotalSeconds);
                if (playerMovement.speed > initialSpeed)
                {
                  
                    float currentSpeed = playerMovement.speed; // 或者使用 y 或 z
                    timeLeftTextForSpeedText.text = $"当前平台的速度等级:\n{upgrade}级";
                }
            }   
        } 
        else
        {
            timeLeftTextForSpeedText.text = "你还没有升级！";
        }     
    }

    

    // 自動減少剩餘時間PaddleWidthAndHeight

    
    public void SavePaddleWidthAndHeightBuyTime()
    {
        DateTime now = DateTime.UtcNow;
        PlayerPrefs.SetString("PaddleWidthAndHeightBuyTime", now.ToString("g"));
        PlayerPrefs.Save();
    }

    public DateTime? LoadPaddleWidthAndHeightBuyTime()
    {
        if (PlayerPrefs.HasKey("PaddleWidthAndHeightBuyTime"))
        {
            string storedTime = PlayerPrefs.GetString("PaddleWidthAndHeightBuyTime");
            if (DateTime.TryParse(storedTime, out DateTime buyTime))
            {
                return buyTime;
            }
        }
        return null;
    }

    public async Task<float> LoadPaddleWidth()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "paddleWidth" });
        if (data.ContainsKey("paddleWidth"))
        {
            // transform.localScale.x = float.Parse(data["paddleWidth"]);
            // Debug.Log("Loaded paddleWidth: " + transform.localScale.x);
            // return transform.localScale.x;
            if (float.TryParse(data["paddleWidth"], out float paddleWidth))
            {
                Debug.Log("Loaded paddleWidth: " + paddleWidth);
                return paddleWidth;
            }
            else
            {
                Debug.LogError("Failed to parse paddleWidth data: " + data["paddleWidth"]);
                return 0f;
            }
        }
        else
        {
            Debug.Log("No paddleWidth data found");
            return 0;
        }
    }

    public async Task<float> LoadPaddleHeight()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "paddleHeight" });
        if (data.ContainsKey("paddleHeight"))
        {
            // transform.localScale.x = float.Parse(data["paddleWidth"]);
            // Debug.Log("Loaded paddleWidth: " + transform.localScale.x);
            // return transform.localScale.x;
            if (float.TryParse(data["paddleHeight"], out float paddleHeight))
            {
                Debug.Log("Loaded paddleHeight: " + paddleHeight);
                return paddleHeight;
            }
            else
            {
                Debug.LogError("Failed to parse paddleHeight data: " + data["paddleHeight"]);
                return 0f;
            }
        }
        else
        {
            Debug.Log("No paddleHeight data found");
            return 0;
        }
    }

    public async Task TimeLeftForWidthAndHeight()
    {   
        float width = await LoadPaddleWidth();
        float height = await LoadPaddleHeight();
        if (width > 0.07 && height > 0.06)
        {
            DateTime lastReductionTime  = await LoadLastReductionTimeForWidthAndHeight();

            if (lastReductionTime != null)
            {
                int upgrade = await shopManager.LoadUpgradesForPaddleAutoMinusScript("IncreasePaddleWidth");
                TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastReductionTime;
                Debug.Log("Time since last update: " + timeSinceLastUpdate.TotalSeconds);
                float currentPaddleWidth = playerMovement.transform.localScale.x; // 或者使用 y 或 z
                float currentPaddleHeight = playerMovement.transform.localScale.y;
                timeLeftTextForWidthAndHeightText.text = $"当前平台的大小:\n{upgrade}级";
            }
        } else
        {
            timeLeftTextForWidthAndHeightText.text = "你还没有升级！";
        }
        
    }


    // public void SaveBallSizeBuyTime()
    // {
    //     DateTime now = DateTime.UtcNow;
    //     PlayerPrefs.SetString("BallSizeBuyTime", now.ToString("g"));
    //     PlayerPrefs.Save();
    // }

    // public DateTime? LoadBallSizeBuyTime()
    // {
    //     if (PlayerPrefs.HasKey("BallSizeBuyTime"))
    //     {
    //         string storedTime = PlayerPrefs.GetString("BallSizeBuyTime");
    //         if (DateTime.TryParse(storedTime, out DateTime buyTime))
    //         {
    //             return buyTime;
    //         }
    //     }
    //     return null;
    // }

    public async Task<float> LoadBallSize()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "ballSize" });
        if (data.ContainsKey("ballSize"))
        {
            // transform.localScale.x = float.Parse(data["paddleWidth"]);
            // Debug.Log("Loaded paddleWidth: " + transform.localScale.x);
            // return transform.localScale.x;
            if (float.TryParse(data["ballSize"], out float ballSize))
            {
                Debug.Log("Loaded ballSize: " + ballSize);
                return ballSize;
            }
            else
            {
                Debug.LogError("Failed to parse ballSize data: " + data["ballSize"]);
                return 0f;
            }
        }
        else
        {
            Debug.Log("No ballSize data found");
            return 0;
        }
    }

    
    
    
    public async Task TimeLeftForBallSize()
    {   
        float ballSize = await LoadBallSize();
        Debug.Log("BallSize: " + ballSize);
        if (ballSize > 0.05f)
        {
            Debug.Log("Ball size is upgraded");
            DateTime lastReductionTime  = await LoadLastReductionTimeForBallSize();

            if (lastReductionTime != null)
            {
                int upgrade = await shopManager.LoadUpgradesForPaddleAutoMinusScript("IncreaseBallSize");
                // Debug.Log("Ball size is upgraded");

                TimeSpan timeSinceLastUpdate = DateTime.UtcNow - lastReductionTime;
                Debug.Log("Time since last update: " + timeSinceLastUpdate.TotalSeconds);
                float currentBallSize = bouncyBall.transform.localScale.x; // 或者使用 y 或 z
                timeLeftTextForBallSizeText.text = $"当前球的大小:\n{upgrade}级";
            }
        } 
        else
        {
            Debug.Log("Ball size is not upgraded");
            timeLeftTextForBallSizeText.text = "你还没有升级！";
        }     
    }



    public async void SaveUpgrade(int upgradeType, int upgradeQuantity)
    {   
        string key = "";

        switch (upgradeType)
        {
            case 0:
                key = "upgrade_IncreasePaddleSpeed_quantity";
                break;
            case 1:
                key = "upgrade_IncreasePaddleWidth_quantity";
                break;
            case 2:
                key = "upgrade_IncreaseBallSize_quantity";
                break;
            default:
                Debug.LogWarning("Invalid upgrade type");
                return;
        }
        var data = new Dictionary<string, object> { {key, upgradeQuantity} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log($"Saved upgrade: {key} with quantity: {upgradeQuantity}");
            Debug.Log("upgrade data saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving upgrade data: {ex.Message}");
        }
    }
   
}
