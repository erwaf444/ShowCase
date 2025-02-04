using System.Collections;
using TMPro;
using UnityEngine;
using Steamworks;

public class GameManager : MonoBehaviour
{
    public GameObject NotePanel;
    public GameObject OvenPanel;
    public GameObject MakeBreadPanel;

    public GameObject[] toolTipObjects;
    public int money;
    public TextMeshProUGUI moneyText;
    public GameObject[] interactiveObjects;
    public bool canInteract = true;

    public GameObject accountBook;
    public GameObject settingPanel;
    public Animator settingPanelAnimator;

    //洗手方法
    public GameObject player;
    public GameObject backPlayer;
    public BookFIirstPageScript bookFIirstPageScript;
    public OvenScript ovenScript;
    public bool isWashingHands = false;
    private float washingStartTime = 0f; // 開始洗手的時間
    private float washDuration = 120f; // 2分鐘（120秒）
    public Animator DayPanelAnimator;
    // public int day = 1;
    public TextMeshProUGUI DayPanelText;
    private TimeSystem timeSystem;
    public RecipeInfo recipeInfo;
    public OpenAnim openAnimScript;
    public GameAnim gameAnimScript;
    public Player playerScript;
    public int makedBreads;
    // public GameObject gameOverPanel;
    public LoadingManager loadingManager;
    public Animator gameOverAnimator;

    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset SimplifiedFontAsset2; //這是當第一個asset不能使用的時候 這是簡體asset
    public TMP_FontAsset TraditionalFontAsset;
    public LocaleSelector localeSelector;
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI selectEngButtonText;
    public TextMeshProUGUI selectChiTButtonText;
    public TextMeshProUGUI selectChiSButtonText;
    public TextMeshProUGUI selectLanguageTitleText;
    public TextMeshProUGUI ResolutionText;
    public int dayText = 1;

    void Awake()
    {
        SteamAPI.Init();
    }
    void Start()
    {
        timeSystem = FindObjectOfType<TimeSystem>(); 
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
			Debug.Log(name);
            CSteamID steamID = SteamUser.GetSteamID();
            string userId = steamID.ToString();
            Debug.Log("Current Steam User ID: " + userId);
            LoadDays();


            // SteamUserStats.GetStat("Player_Money", out money);
            // money++;
            // SteamUserStats.SetStat("Player_Money", money);
            // Debug.Log("Money set to: " + money);
            // SteamUserStats.StoreStats();
            // Debug.Log("Saved money to Steam Cloud: " + money);

            UnlockAchievement("FIRST_IN_GAME");
            InitializeStats();
            LoadMoney();
            timeSystem.LoadCleanValue();
            timeSystem.LoadFoodFreshValue();
            timeSystem.LoadCustomerHappyValue();
            timeSystem.LoadMakedBreads();
            playerScript.LoadExp();
          
        }

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized. Please check your Steam configuration.");
        }

        GameObject gameAnimObject = GameObject.Find("GameAnimation");
        if (gameAnimObject != null)
        {
            gameAnimScript = gameAnimObject.GetComponent<GameAnim>();
        }
        GameObject openAnimObject = GameObject.Find("OpenAnimation");
        if (openAnimObject != null)
        {
            openAnimScript = openAnimObject.GetComponent<OpenAnim>();
        }
        StartCoroutine(DayPanelRoutine());
        GameObject ovenScriptObject = GameObject.Find("OvenPanel");
        if (ovenScriptObject != null)
        {
            ovenScript = ovenScriptObject.GetComponent<OvenScript>();
        }
        backPlayer.SetActive(false);
        NotePanel.SetActive(false);
        accountBook.SetActive(false);
        MakeBreadPanel.SetActive(false);
    }

    void Update()
    {
        if (playerScript.experiencePoints == 10)
        {
            UnlockAchievement("NEWBIE");;
        }
        if (playerScript.experiencePoints == 20)
        {
            UnlockAchievement("GET_BETTER");;
        }
        if (playerScript.experiencePoints == 50)
        {
            UnlockAchievement("SENIOR");;
        }
        if (playerScript.experiencePoints == 100)
        {
            UnlockAchievement("PRO");;
        }
        if (playerScript.experiencePoints == 200)
        {
            UnlockAchievement("MASTER");;
        }
        if (playerScript.experiencePoints == 300)
        {
            UnlockAchievement("WORLD_CLASS");;
        }
        if (playerScript.experiencePoints == 400)
        {
            UnlockAchievement("UNIVERSE_LEVEL");;
        }
        DayPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        moneyText.text = money.ToString();
        if (canInteract && Input.GetMouseButtonDown(0) && openAnimScript.OpenAnimIsEnd) // 0 是左鍵
        {
            DetectMouseClick();
        }

        if (localeSelector.localID == 0)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = TraditionalFontAsset;
                text.SetAllDirty();
            }
            selectEngButtonText.text = "English";
            selectEngButtonText.font = TraditionalFontAsset;
            selectChiTButtonText.text = "Chinese (Traditional)";
            selectChiTButtonText.font = TraditionalFontAsset;
            selectChiSButtonText.text = "Chinese (Simplified)";
            selectChiSButtonText.font = TraditionalFontAsset;
            selectLanguageTitleText.text = "Choose Language";
            selectLanguageTitleText.font = TraditionalFontAsset;
            ResolutionText.font = TraditionalFontAsset;
            ResolutionText.text = "Resolution";
        } else if (localeSelector.localID == 1)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = TraditionalFontAsset;
                text.SetAllDirty();
            }
            selectEngButtonText.text = "英文";
            selectEngButtonText.font = TraditionalFontAsset;
            selectChiTButtonText.text = "中文（繁體）";
            selectChiTButtonText.font = TraditionalFontAsset;
            selectChiSButtonText.text = "中文（簡體）";
            selectChiSButtonText.font = TraditionalFontAsset;
            selectLanguageTitleText.text = "選擇語言";
            selectLanguageTitleText.font = TraditionalFontAsset;
            ResolutionText.font = TraditionalFontAsset;
            ResolutionText.text = "分辨率";
        } else if (localeSelector.localID == 2)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = SimplifiedFontAsset;
                text.SetAllDirty();
            }
            selectEngButtonText.text = "英文";
            selectEngButtonText.font = SimplifiedFontAsset2;
            selectChiTButtonText.text = "中文（繁体）";
            selectChiTButtonText.font = SimplifiedFontAsset2;
            selectChiSButtonText.text = "中文（简体）";
            selectChiSButtonText.font = SimplifiedFontAsset2;
            selectLanguageTitleText.text = "选择语言";
            selectLanguageTitleText.font = SimplifiedFontAsset2;
            ResolutionText.font = SimplifiedFontAsset2;
            ResolutionText.text = "分辨率";
        }
    }

    public IEnumerator DayPanelRoutine()
    {
        LoadDays();
        openAnimScript.canStartOpenAnim = false;
        DayPanelAnimator.enabled = true;
        canInteract = false;
        Time.timeScale = 0;
        DayPanelText.text = "Day " + dayText;
        yield return new WaitForSecondsRealtime(2f);
        if (
            TimeSystem.AccumulateDay == 20 ||
            TimeSystem.AccumulateDay == 40 ||
            TimeSystem.AccumulateDay == 60 ||
            TimeSystem.AccumulateDay == 80 ||
            TimeSystem.AccumulateDay == 100 ||
            TimeSystem.AccumulateDay == 120 ||
            TimeSystem.AccumulateDay == 150
        ) {
            DayPanelAnimator.SetTrigger("DayPanelDownWithGameAnim");
        } else
        {
            DayPanelAnimator.SetTrigger("DayPanelDown");
        }
        yield return new WaitForSecondsRealtime(3f);
        if (
            TimeSystem.AccumulateDay == 20 ||
            TimeSystem.AccumulateDay == 40 ||
            TimeSystem.AccumulateDay == 60 ||
            TimeSystem.AccumulateDay == 80 ||
            TimeSystem.AccumulateDay == 100 ||
            TimeSystem.AccumulateDay == 120 ||
            TimeSystem.AccumulateDay == 150
        ) {
            DayPanelAnimator.SetTrigger("DayPanelUpWithGameAnim");

        } else
        {
            DayPanelAnimator.SetTrigger("DayPanelUp");

        }
        yield return new WaitForSecondsRealtime(2f);
        DayPanelAnimator.enabled = false;

        if (
            TimeSystem.AccumulateDay == 20 ||
            TimeSystem.AccumulateDay == 40 ||
            TimeSystem.AccumulateDay == 60 ||
            TimeSystem.AccumulateDay == 80 ||
            TimeSystem.AccumulateDay == 100
        ) 
        {
            Time.timeScale = 0;
            gameAnimScript.gameAnimIsEnd = false;
        } 
        else
        {
            Time.timeScale = 1;
            gameAnimScript.gameAnimIsEnd = true;
        }
        canInteract = true;
        openAnimScript.canStartOpenAnim = true;
    }

    void DetectMouseClick()
    {
        RaycastHit2D hit; // 使用 RaycastHit2D 來檢查 2D 碰撞器
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 從滑鼠位置發射射線

        hit = Physics2D.Raycast(ray.origin, ray.direction); // 使用 Physics2D.Raycast

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name == "Cat")
            {
                Debug.Log("Clicked on Cat!");
                AudioManager.instance.PlaySFX("Cat");
            }
            else if (hit.collider.gameObject.name == "CashRegister")
            {
                Debug.Log("Clicked on CashRegister!");
                AudioManager.instance.PlaySFX("CashRegister");
                OpenAccountBook();
            }
            else if (hit.collider.gameObject.name == "Kitchen")
            {
                Debug.Log("Wash Hands!");
                AudioManager.instance.PlaySFX("WashHands");
                StartCoroutine(WashHandsRoutine());
            }
        }
    }

    public void OpenAccountBook()
    {
        if (canInteract && openAnimScript.OpenAnimIsEnd)
        {
            accountBook.SetActive(true);
            canInteract = false; 
        }
    }

    public void CloseAccountBook()
    {
        accountBook.SetActive(false);
        canInteract = true; 
    }
 
    public void OpenNote()
    {
        if (canInteract && openAnimScript.OpenAnimIsEnd)
        {
            NotePanel.SetActive(true);
            SetToolTipsEnabled(false);
            canInteract = false;
            recipeInfo.ChangeTipMaterialRandomly();
        }
    }

    public void CloseNote()
    {
        NotePanel.SetActive(false);
        SetToolTipsEnabled(true);
        canInteract = true;
    }

    public void OpenMakeBreadPnael()
    {
        if (canInteract && openAnimScript.OpenAnimIsEnd)
        {
            MakeBreadPanel.SetActive(true);
            SetToolTipsEnabled(false);
            canInteract = false;
        }
    }

    public void CloseMakeBreadPnael()
    {
            MakeBreadPanel.SetActive(false);
            SetToolTipsEnabled(true);
            canInteract = true;
    }

    public void OpenOvenPanel()
    {
        if (canInteract && openAnimScript.OpenAnimIsEnd)
        {
            OvenPanel.SetActive(true);
            SetToolTipsEnabled(false);
            canInteract = false;
        }
    }

    public void CloseOvenPanel()
    {
        OvenPanel.SetActive(false);
        SetToolTipsEnabled(true);
        canInteract = true;
    }

    public void OpenSettingPanel()
    {
        if (canInteract)
        {
            AudioManager.instance.PlaySFX("TurnPage");
            settingPanelAnimator.SetTrigger("Down");
            canInteract = false;
        }
    }

    public void CloseSettingPanel()
    {
        AudioManager.instance.PlaySFX("TurnPage");
        settingPanelAnimator.SetTrigger("Up");
        canInteract = true;
    }

    public void InteractWithObjects()
    {
        foreach (GameObject obj in interactiveObjects)
        {
            if(obj.name == "Cat")
            {
                AudioManager.instance.PlaySFX("Cat");
            }
           
        }
    }

    void SetToolTipsEnabled(bool isEnabled)
    {
        foreach (GameObject toolTipObject in toolTipObjects)
        {
            ToolTip toolTip = toolTipObject.GetComponent<ToolTip>();
            if (toolTip != null)
            {
                toolTip.isToolTipEnabled = isEnabled;

            }
        }
    }

    IEnumerator WashHandsRoutine()
    {
        WashHands();
        yield return new WaitForSeconds(2.0f);
        WashHandsDone();
        isWashingHands = true;
        washingStartTime = Time.time; // 記錄洗手開始的時間
    }

    public void WashHands()
    {
        canInteract = false;
        StartCoroutine(FadeOutIn(player, backPlayer, 1.0f)); // 1秒的淡入淡出時間
    }

    public void WashHandsDone()
    {
        canInteract = true;
        StartCoroutine(FadeOutIn(backPlayer, player, 1.0f)); 
    }

    // 協程：淡出 player 並淡入 backPlayer
    IEnumerator FadeOutIn(GameObject fadeOutObject, GameObject fadeInObject, float duration)
    {
        SpriteRenderer fadeOutRenderer = fadeOutObject.GetComponent<SpriteRenderer>();
        SpriteRenderer fadeInRenderer = fadeInObject.GetComponent<SpriteRenderer>();

        if (fadeOutRenderer == null || fadeInRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on one of the objects!");
            yield break;
        }

        // 激活淡入物體
        fadeInObject.SetActive(true);
        
        float timer = 0f;

        Color fadeOutColor = fadeOutRenderer.color;
        Color fadeInColor = fadeInRenderer.color;

        // 同時進行淡出和淡入
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // 控制透明度
            fadeOutRenderer.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, 1f - progress); // 淡出
            fadeInRenderer.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, progress);           // 淡入

            yield return null;
        }

        // 保證最終的透明度為正確值
        fadeOutRenderer.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, 0f); // 完全透明
        fadeInRenderer.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, 1f);    // 完全顯示

        // 淡出後可以選擇隱藏物體（如果需要）
        fadeOutObject.SetActive(false);
    }


    //這裡是減少玩家value的地方
    public void CalculatePlayerCleanValue()
    {
        float currentTime = Time.time;
        // 如果超過2分鐘，重設洗手狀態
        if (currentTime - washingStartTime > washDuration)
        {
            isWashingHands = false; // 洗手狀態設置為 false
        } 

        // 檢查自上次洗手以來的時間
        if (isWashingHands)
        {
            bookFIirstPageScript.cleanValue += 1; // 增加清潔值
            if (bookFIirstPageScript.cleanValue > 100)
            {
                bookFIirstPageScript.cleanValue = 100;
            }
        }
        else
        {
            bookFIirstPageScript.cleanValue -= 1;
            if (bookFIirstPageScript.cleanValue < 0)
            {
                bookFIirstPageScript.cleanValue = 0;
            }
        }
    }

    public void MinusPlayerFoodFreshValue()
    {
        bookFIirstPageScript.foodFreshValue -= 1;
        if (bookFIirstPageScript.foodFreshValue < 0)
        {
            bookFIirstPageScript.foodFreshValue = 0;
        }
    }

    public void MinusPlayerFoodFreshValueWithValue(int value)
    {
        bookFIirstPageScript.foodFreshValue -= value;
        if (bookFIirstPageScript.foodFreshValue < 0)
        {
            bookFIirstPageScript.foodFreshValue = 0;
        }
    }

    public void AddPlayerFoodFreshValue()
    {
        bookFIirstPageScript.foodFreshValue += 1;
        if (bookFIirstPageScript.foodFreshValue > 100)
        {
            bookFIirstPageScript.foodFreshValue = 100;
        }
    }

    public void AddPlayerCustomerHappyValue()
    {
        bookFIirstPageScript.customerHappyValue += 1;
        if (bookFIirstPageScript.customerHappyValue > 100)
        {
            bookFIirstPageScript.customerHappyValue = 100;
        }
    }

    public void MinusPlayerCustomerHappyValue()
    {
        bookFIirstPageScript.customerHappyValue -= 1;
        if (bookFIirstPageScript.customerHappyValue < 0)
        {
            bookFIirstPageScript.customerHappyValue = 0;
        }
    }

    public void EndTheDay()
    {
        timeSystem.NextDay();
    }


    //SteamApi
    public void InitializeStats()
    {
        if (SteamManager.Initialized)
        {
            // 檢查 Player_Money 是否已存在，如果不存在則初始化
            int storedMoney;
            if (!SteamUserStats.GetStat("Player_Money", out storedMoney))
            {
                // 如果統計數據不存在，則將其設置為 0
                SteamUserStats.SetStat("Player_Money", 0);
                SteamUserStats.StoreStats();
                Debug.Log("Initialized Player_Money to 0");
                money = storedMoney;
                moneyText.text = money.ToString();
            }
        }
    }   

    public void SaveMoney()
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat("Player_Money", money);
            bool success = SteamUserStats.StoreStats();
            
            if (success)
            {
                Debug.Log("Money saved to Steam Cloud: " + money);
            }
            else
            {
                Debug.LogWarning("Failed to store stats to Steam Cloud.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to save money.");
        }
    }

    public void LoadMoney()
    {

        if (SteamManager.Initialized)
        {
            // 從Steam Cloud中獲取金錢數據
            int storedMoney;
            bool hasStat = SteamUserStats.GetStat("Player_Money", out storedMoney);
        
            if (hasStat)
            {
                money = storedMoney;
                moneyText.text = money.ToString();
                Debug.Log("Money loaded from Steam Cloud: " + money);
            }
            else
            {
                Debug.LogWarning("Stat 'Player_Money' not found or could not be retrieved.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to load money.");
        }
    }

    public void UnlockAchievement(string achievementID)
    {
        if (SteamManager.Initialized)
        {
            bool achieved = false;
            SteamUserStats.GetAchievement(achievementID, out achieved);
            if (achieved)
            {
                Debug.Log("Achievement " + achievementID + " is already unlocked.");
                return; // 如果成就已解锁，直接返回
            }
            // 解鎖成就
            SteamUserStats.SetAchievement(achievementID);
            // 提交成就數據到 Steam 雲端
            bool success = SteamUserStats.StoreStats();
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log("Achievement " + achievementID + " unlocked and stored to Steam.");
            }
            else
            {
                Debug.LogWarning("Failed to store achievement stats.");
            }
        }
    }

    public void GameOver()
    {
        gameOverAnimator.SetTrigger("GameOverPanelDown");
        UnlockAchievement("YOU_FISRT_DIE");

    }

    public void ExitToMainMenu()
    {
        loadingManager.LoadToMainMenu(0);
    }

    public void TryAgain()
    {
        loadingManager.LoadToGame(1);
    }


    public void RestSteamStats()
    {
        if (SteamUserStats.ResetAllStats(true))
        {
            // 提交重置请求以同步到 Steam
            SteamUserStats.StoreStats();
            Debug.Log("Steam 统计数据已成功清空");
        }
        else
        {
            Debug.LogError("重置 Steam 统计数据失败");
        }
    }

    public void ResetDayStat(string statKey)
    {
        if (SteamUserStats.SetStat(statKey, 0))
        {
            SteamUserStats.StoreStats();  // 提交更改到 Steam
            Debug.Log($"Statistic {statKey} has been reset to 0.");
        }
        else
        {
            Debug.LogError($"Failed to reset statistic {statKey}.");
        }
    }


    public void LoadDays()
    {

        if (SteamManager.Initialized)
        {
            // 從Steam Cloud中獲取金錢數據
            int storedDays;
            bool hasStat = SteamUserStats.GetStat("Days", out storedDays);
        
            if (hasStat)
            {
                dayText = storedDays;
                Debug.Log("day loaded from Steam Cloud: " + dayText);
            }
            else
            {
                Debug.LogWarning("Stat 'day' not found or could not be retrieved.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to load day.");
        }
    }

}
