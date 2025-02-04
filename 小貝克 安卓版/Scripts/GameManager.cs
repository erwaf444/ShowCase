using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;


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
    public bool loadDayIsReady;

    void Awake()
    {
    }
    async void Start()
    {
        timeSystem = FindObjectOfType<TimeSystem>(); 
    
        LoadDays();

        InitializeStats();
        await LoadMoney();
        timeSystem.LoadCleanValue();
        timeSystem.LoadFoodFreshValue();
        timeSystem.LoadCustomerHappyValue();
        timeSystem.LoadMakedBreads();
        // playerScript.LoadExp();
          

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
        yield return new WaitUntil(() => loadDayIsReady);
        DayPanelText.text = "Day " + dayText;
        Debug.Log("dayTExt" + dayText);
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
            TimeSystem.AccumulateDay == 100 ||
            TimeSystem.AccumulateDay == 120 ||
            TimeSystem.AccumulateDay == 150
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
        
    }   

    public void SaveMoney()
    {
      
    }

    public async Task LoadMoney()
    {
        try
        {
            // 從雲端加載數據
            var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "PlayerMoney" });

            // 檢查是否有儲存的金錢數據
            if (data.ContainsKey("PlayerMoney"))
            {
                money = int.Parse(data["PlayerMoney"].ToString());
                Debug.Log("Money loaded from cloud: " + money);
            }
            else
            {
                Debug.Log("No money data found in cloud.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load money from cloud: " + e.Message);
        }
    }


  

    public void GameOver()
    {
        gameOverAnimator.SetTrigger("GameOverPanelDown");

    }

    public void ExitToMainMenu()
    {
        loadingManager.LoadToMainMenu(0);
    }

    public void TryAgain()
    {
        loadingManager.LoadToGame(1);
    }


    #region unitycloud
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

    
    public void RestSteamStats()
    {
     
    }

    public void ResetDayStat(string statKey)
    {
        
    }


    public async void LoadDays()
    {
        dayText = await LoadData("day", dayText);
        Debug.Log("Day loaded from cloud: " + dayText);
        loadDayIsReady = true;
    }


    #endregion


}
