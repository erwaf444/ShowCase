using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

[System.Serializable]
public class IncomeDialogue
{
    public string[] incomeDialogue;
}

[System.Serializable]
public class OutlayDialogue
{
    public string[] outlayDialogue;
}




public class TimeSystem : MonoBehaviour
{
    public IncomeDialogue incomeDialogue;
    public TextMeshProUGUI incomeDialogueText;
    public OutlayDialogue outlayDialogue;
    public TextMeshProUGUI outlayDialogueText;
    // float TIMESCALE = 0.01f;
    private float fastTimeScale = 0.001f; // 快速时间缩放值
    private float normalTimeScale = 0.01f; // 正常时间缩放值
    private float currentTimeScale; // 当前时间缩放值
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI yearText;
    public GameObject leafParticle;
    public GameObject snowParticle;
    public GameObject summerLeafParticle;
    public GameObject rainParticle;

    public static int minute, hour, day, month, year;
    public static double second;
    public static int AccumulateDay;

    public bool isDayEnded = false;
    public Animator shopClosingAnimator;
    public Animator shopClosingAccountAnimator;
    public GameManager gameManagerScript;
    public Transform player;
    public Passerby passerbyScript;
    public GameObject frontPlayer;
    public GameObject backPlayer;

    //記錄收入和支出區域
    //收入
    private int todaysBalance; //記錄今天的餘額
    private int todaysTotoalIncome;  //記錄今天的總收入
    private int todaysBreadIncome; 
    private int todaysOtherIncome; 
    //支出
    private int todaysStuffOutlay;
    private int todaysEquipmentOutlay;
    private int todaysOtherOutlay;
    private int todaysTotalOutlay; //記錄今天的總支出
 
    public GameObject DayEndAccountBook;
    public ShopCloseAccountBookScript shopCloseAccountBookScript;
    public int monthlyRent = 0; // 每月租金金額
    public OpenAnim openAnimScript;
    public GameAnim gameAnimScript;
    public BookFIirstPageScript bookFirstPageScript;
    public Player playerScript;
    public OvenScript ovenScript;
    public int foodImagesEnabledCount = 0;
    public int foodBagsActiveCount = 0;
    public static bool isReady;

    async void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        frontPlayer.SetActive(true);
        backPlayer.SetActive(false);
        currentTimeScale = normalTimeScale; // 初始化为正常时间缩放值
        await UnityServices.InitializeAsync();
    }

    void Start()
    {
        GameObject openAnimObject = GameObject.Find("OpenAnimation");
        if (openAnimObject != null)
        {
            openAnimScript = openAnimObject.GetComponent<OpenAnim>();
        }
        GameObject gameAnimObject = GameObject.Find("GameAnimation");
        if (gameAnimObject != null)
        {
            gameAnimScript = gameAnimObject.GetComponent<GameAnim>();
        }
        day = 1;
        month = 1;
        year = 2025;
        AccumulateDay = 150;
        Debug.Log("AcuumulateDay" + AccumulateDay);
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        }
 
        InitializeTime();
        LoadDays();
        LoadMonth();
        LoadYear();
        LoadCleanValue();
        LoadFoodFreshValue();
        LoadCustomerHappyValue();
        // LoadAccumulateDay();
    }

    void Update()
    {
        // Debug.Log(gameAnimScript.gameAnimIsEnd);
        shopClosingAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        shopClosingAccountAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        if (!isDayEnded)
        {
            if (openAnimScript.OpenAnimIsEnd && gameAnimScript.gameAnimIsEnd)
            {
                CalculateTime();
            }
            UpdateUI();
            CheckDayEnd();
        }

     

        // 检查 K 键的按下状态
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleFastForward(); // 切换时间加速
        }
    }

    void InitializeTime()
    {
        hour = 8; // 每天从 8:00 开始
        minute = 00;
        second = 0;
        isDayEnded = false;
        UpdateParticles();
    }

    void CalculateTime()
    {
        second += Time.deltaTime / currentTimeScale;
        // Debug.Log("currentTimeScale: "+currentTimeScale);

        if (second >= 60)
        {
            second = 0;
            minute++;

            if (minute >= 60)
            {
                minute = 0;
                hour++;
            }
        }
    }

    void CheckDayEnd()
    {
        if (hour >= 17 && !isDayEnded)
        {
            EndDay();
        }
    }

    void EndDay()
    {
        isDayEnded = true;
        StartCoroutine(EndDayRoutine());
    }

    public void NextDay()
    {
        if (isDayEnded)
        {
            gameManagerScript.money -= todaysBalance;

            if (gameManagerScript.money < 0)
            {
                
            }

            //重置收入和支出
            //收入
            todaysTotoalIncome = 0;
            todaysBreadIncome = 0;
            todaysOtherIncome = 0;
            //支出
            todaysStuffOutlay = 0;
            todaysEquipmentOutlay = 0;
            todaysOtherOutlay = 0;
            todaysTotalOutlay  = 0;
            //餘額
            todaysBalance = 0;

            
            StopAllCoroutines();
           

            // shopClosingAnimator.updateMode = AnimatorUpdateMode.Normal;
            // shopClosingAccountAnimator.updateMode = AnimatorUpdateMode.Normal;

            isDayEnded = false;
            day++;
            SaveDays();
            SaveMonth();
            SaveYear();
            CaculateMonth();

            ResetGameState();
            // 在重置完成後，才初始化時間和更新UI
            InitializeTime();
            UpdateUI();
            gameManagerScript.canInteract = true;
            // Debug.Log("Next day: " + day);

            // 開始新的一天
            LoadDays();
            LoadMonth();
            LoadYear();
            StartCoroutine(StartNewDay());

            
            

       

            LoadFoodFreshValue();
            LoadCleanValue();
            LoadCustomerHappyValue();
            
            SaveMakedBreads();
            AccumulateDay++;
            SaveAccumulateDay();
            gameAnimScript.canStartGameAnim = false;
            gameAnimScript.canRespondToRKey = true;

            // 恢復時間縮放
            Time.timeScale = 1;
        }
    }

    
    void ResetGameState()
    {
       StartCoroutine(ResetGameStateRoutine());
    }

    IEnumerator ResetGameStateRoutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        shopClosingAnimator.ResetTrigger("ShopClosing");
        shopClosingAccountAnimator.ResetTrigger("ShopClosingAccount");


        // 在这里重置所有需要在新的一天开始时重置的游戏状态
        shopClosingAccountAnimator.SetTrigger("ShopClosingAccountUp");
        // yield return new WaitForSecondsRealtime(2f);
        shopClosingAnimator.SetTrigger("ShopClosingUp");
        yield return new WaitForSecondsRealtime(0.1f);
        shopClosingAccountAnimator.ResetTrigger("ShopClosingAccountUp");
        shopClosingAnimator.ResetTrigger("ShopClosingUp");

    
        // - 重置玩家位置
        player.localPosition  = new Vector3(6, -2f, 3.53f);
        frontPlayer.SetActive(true);
        backPlayer.SetActive(false);
        // - 重置库存或资源
        // - 重置任何每日任务或事件
        
        // 示例：
        // playerScript.ResetPosition();
        // inventoryScript.ResetDailyItems();
        // questSystem.ResetDailyQuests();
    }

    IEnumerator StartNewDay()
    {
        yield return new WaitForSeconds(0.1f); // 短暂延迟以确保所有系统都已更新
        
        // 在这里触发新一天开始时的事件
        // 例如：
        // - 生成每日任务
        // - 更新商店库存
        // - 触发每日事件
        
        // 示例：
        // questSystem.GenerateDailyQuests();
        // shopSystem.UpdateDailyInventory();
        // eventSystem.TriggerDailyEvent();
        AudioManager.instance.musicSource.Play();
        StartCoroutine(gameManagerScript.DayPanelRoutine());
        
     
    }


    IEnumerator EndDayRoutine()
    {
        // AudioManager.instance.PlaySFX("ShopClosing");
        AudioManager.instance.musicSource.Pause();
        Time.timeScale = 0;
        gameManagerScript.canInteract = false;
        AudioManager.instance.PlaySFX("TheDayEnd");
        shopClosingAnimator.SetTrigger("ShopClosing");
        yield return new WaitForSecondsRealtime(3f);
        shopClosingAccountAnimator.SetTrigger("ShopClosingAccount");
        yield return new WaitForSecondsRealtime(2f);

        //記錄今天沒有做完的麵包
        for (int i = 0; i < ovenScript.foodImages.Length; i++)
        {
            if (ovenScript.foodImages[i].enabled)  // 檢查foodImages[i]是否啟用
            {
                foodImagesEnabledCount++;  // 如果啟用，增加計數
            }
        }

        for (int i = 0; i < ovenScript.foodBags.Length; i++)
        {
            if (ovenScript.foodBags[i].activeInHierarchy)  // 檢查foodImages[i]是否啟用
            {
                foodBagsActiveCount++;  // 如果啟用，增加計數
            }
        }
        Debug.Log("今天沒有做完的麵包數量:" + foodImagesEnabledCount);
        Debug.Log("今天沒有做完的麵包數量:" + foodBagsActiveCount);

        gameManagerScript.MinusPlayerFoodFreshValueWithValue(foodImagesEnabledCount);
        gameManagerScript.MinusPlayerFoodFreshValueWithValue(foodBagsActiveCount);

        SaveCleanValue();
        SaveFoodFreshValue();
        SaveCustomerHappyValue();
        ovenScript.ResetFood();

        //記錄今天的日期
        // SaveDays();
        // SaveMonth();
        // SaveYear();


        //記錄今天的收入
        //麵包收入
        todaysBreadIncome = gameManagerScript.money;
        shopCloseAccountBookScript.breadIncomeText.text = todaysBreadIncome.ToString();
        //其他收入
        if (AccumulateDay > 31)
        {
            TriggerRandomOtherIncomeEvent(); //計算隨機收入
        }
        //今天的總收入
        todaysTotoalIncome = todaysBreadIncome + todaysOtherIncome;
        shopCloseAccountBookScript.totalIncomeText.text = todaysTotoalIncome.ToString();

        //記錄今天的支出
        CheckRentPayment(); //檢查和計算租金
        //其他收入
        if (AccumulateDay > 31)
        {
            TriggerRandomStuffOutlayEvent(); //檢查和計算隨機物品支出
        }
        if (AccumulateDay > 61)
        {
            TriggerRandomEquipmentOutlayEvent(); //檢查和計算隨機設備支出
        }
        if (AccumulateDay > 80)
        {
            TriggerRandomOtherOutlayEvent(); //檢查和計算隨機其他支出
        }
        //今天的總支出
        todaysTotalOutlay = monthlyRent + todaysStuffOutlay + todaysEquipmentOutlay + todaysOtherOutlay;
        Debug.Log(monthlyRent + " " + todaysStuffOutlay + " " + todaysEquipmentOutlay + " " + todaysOtherOutlay);
        shopCloseAccountBookScript.totalOutlayText.text = todaysTotalOutlay.ToString();
        
        incomeDialogueText.text = incomeDialogue.incomeDialogue[UnityEngine.Random.Range(0, incomeDialogue.incomeDialogue.Length)];
        outlayDialogueText.text = outlayDialogue.outlayDialogue[UnityEngine.Random.Range(0, outlayDialogue.outlayDialogue.Length)];
        todaysBalance = todaysTotoalIncome - todaysTotalOutlay;

        // 不再自动进入下一天，等待 NextDay 方法被调用
    }




    void UpdateUI()
    {
        dayText.text = day.ToString();
        clockText.text = string.Format("{0:00}:{1:00}", (int)hour, (int)minute);
        yearText.text = year.ToString();
        monthText.text = month.ToString();
        CalculateSeason();
    }

    void CalculateSeason()
    {
        if (month == 12 || month == 1 || month == 2)
        {
            seasonText.text = "冬天";
        }
        else if (month == 3 || month == 4 || month == 5)
        {
            seasonText.text = "春天";
        }
        else if (month == 6 || month == 7 || month == 8)
        {
            seasonText.text = "夏天";
        }
        else if (month == 9 || month == 10 || month == 11)
        {
            seasonText.text = "秋天";
        }
        UpdateParticles();
    }

    void UpdateParticles()
    {
        leafParticle.SetActive(seasonText.text == "秋天");
        snowParticle.SetActive(seasonText.text == "冬天");
        summerLeafParticle.SetActive(seasonText.text == "夏天");
        rainParticle.SetActive(seasonText.text == "春天");
    }

    void CaculateMonth()
    {
        int daysInMonth = 31;

        if (month == 4 || month == 6 || month == 9 || month == 11)
        {
            daysInMonth = 30;
        }
        else if (month == 2)
        {
            daysInMonth = ((year % 4 == 0 && year % 100 != 0) || year % 400 == 0) ? 29 : 28;
        }

        if (day > daysInMonth)
        {
            day = 1;
            month++;

            if (month > 12)
            {
                month = 1;
                year++;
            }
        }
    }

    void CheckRentPayment()
    {
        // 如果是每月的 5 號，支付租金
        if (day == 5)
        {
            // 確認玩家有足夠的金錢支付租金
            // if (gameManagerScript.money >= monthlyRent)
            // {
            monthlyRent = 100;
            shopCloseAccountBookScript.rentOutlayText.text = monthlyRent.ToString();

            // }
            // else
            // {
            //     // 如果金錢不足，提示金額不足或觸發其他事件
            //     Debug.Log("Not enough money to pay rent!");
            // }
        }
    }

    void TriggerRandomOtherIncomeEvent()
    {
        // 生成一个随机收入数值，例如在 10 到 100 之间
        int randomIncome = UnityEngine.Random.Range(0, 500);

        // 将随机收入加入 today's other income
        todaysOtherIncome += randomIncome;

        // 更新 UI 显示新的其他收入
        shopCloseAccountBookScript.otherIncomeText.text = todaysOtherIncome.ToString();

    }

    void TriggerRandomStuffOutlayEvent()
    {
        // 生成一个随机收入数值，例如在 10 到 100 之间
        int randomOutlay = UnityEngine.Random.Range(0, 200);

        // 将随机收入加入 today's other income
        todaysStuffOutlay += randomOutlay;

        // 更新 UI 显示新的其他收入
        shopCloseAccountBookScript.stuffOutlayText.text = todaysStuffOutlay.ToString();
    }

    void TriggerRandomEquipmentOutlayEvent()
    {
        // 生成一个随机收入数值，例如在 10 到 100 之间
        int randomOutlay = UnityEngine.Random.Range(0, 200);

        // 将随机收入加入 today's other income
        todaysEquipmentOutlay += randomOutlay;

        // 更新 UI 显示新的其他收入
        shopCloseAccountBookScript.equipmentOutlayText.text = todaysEquipmentOutlay.ToString();
    }

    void TriggerRandomOtherOutlayEvent()
    {
        // 生成一个随机收入数值，例如在 10 到 100 之间
        int randomOutlay = UnityEngine.Random.Range(0, 200);

        // 将随机收入加入 today's other income
        todaysOtherOutlay += randomOutlay;

        // 更新 UI 显示新的其他收入
        shopCloseAccountBookScript.otherOutlayText.text = todaysOtherOutlay.ToString();
    }

    #region unityCloud
    private async Task SaveData(string key, int value)
    {
        try
        {
            var data = new Dictionary<string, object> { { key, value } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log($"Saved {key}: {value}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save {key}: {e.Message}");
        }
    }

    private async Task<int> LoadData(string key, int defaultValue)
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });

            if (savedData.TryGetValue(key, out var value))
            {
                // 由于 Cloud Save 将数字存储为字符串形式的 JSON
                // 我们需要先解析这个值
                if (int.TryParse(value.ToString(), out int parsedValue))
                {
                    Debug.Log($"Loaded {key}: {parsedValue}");
                    return parsedValue;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load {key}: {e.Message}");
        }

        return defaultValue;
    }

    public async void SaveDays()
    {
        await SaveData("day", day);
    }

    public async void LoadDays()
    {
        day = await LoadData("day", day);
        isReady = true;
    }

    public async void SaveMonth()
    {
        await SaveData("month", month);
    }

    public async void LoadMonth()
    {
        month = await LoadData("month", month);
    }

    public async void SaveYear()
    {
        await SaveData("year", year);
    }

    public async void LoadYear()
    {
        year = await LoadData("year", year);
    }

    public async void SaveCleanValue()
    {
        await SaveData("cleanValue", bookFirstPageScript.cleanValue);
    }

    public async void LoadCleanValue()
    {
        bookFirstPageScript.cleanValue = await LoadData("cleanValue", bookFirstPageScript.cleanValue);
    }

    public async void SaveFoodFreshValue()
    {
        await SaveData("foodFreshValue", bookFirstPageScript.foodFreshValue);
    }

    public async void LoadFoodFreshValue()
    {
        bookFirstPageScript.foodFreshValue = await LoadData("foodFreshValue", bookFirstPageScript.foodFreshValue);
    }

    public async void SaveCustomerHappyValue()
    {     
        await SaveData("customerHappyValue", bookFirstPageScript.customerHappyValue);
    }

    public async void LoadCustomerHappyValue()
    {
        bookFirstPageScript.customerHappyValue = await LoadData("customerHappyValue", bookFirstPageScript.customerHappyValue);
    }

    public async void SaveMakedBreads()
    {
        await SaveData("bread", gameManagerScript.makedBreads);
    }

    public async void LoadMakedBreads()
    {
        gameManagerScript.makedBreads = await LoadData("bread", gameManagerScript.makedBreads);
    }

    public async void SaveAccumulateDay()
    {
        await SaveData("accumulateDay", AccumulateDay);
    }

    public async void LoadAccumulateDay()
    {
        AccumulateDay = await LoadData("accumulateDay", AccumulateDay);

    }
    #endregion
   
   



    //時間變快測試方法
    void ToggleFastForward()
    {
        if (currentTimeScale == normalTimeScale)
        {
            currentTimeScale = fastTimeScale; // 切换到加速状态
            Debug.Log("Switched to fast time scale: " + currentTimeScale);
        }
        else
        {
            currentTimeScale = normalTimeScale; // 切换回正常状态
            Debug.Log("Switched back to normal time scale: " + currentTimeScale);

        }
    }
}