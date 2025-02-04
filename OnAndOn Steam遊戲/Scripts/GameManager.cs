using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class GameManager : MonoBehaviour
{
    public GameObject settingPanel;
    public Transform player;        // 玩家 Transform
    public Player playerScript;
    public GameObject theEnd;
    public Image youWinPanel;
    public TextMeshProUGUI spentTimeText;
    public TextMeshProUGUI youWinPanelTitle;
    public TextMeshProUGUI youWinPanelSubTitle;
    // public TextMeshProUGUI worldBestRecordTime;
    public TextMeshProUGUI yourBestRecordTime;
    private float startTime; // 記錄遊玩開始時間
    public bool isGameStarted = false; // 記錄遊玩是否已經開始
    public bool isGameEnded = false; // 記錄遊玩是否已經結束
    private float elapsedTime; // 記錄遊戲進行的時間
    private string timeString;
    private string bestTimeString;
    private int storedTime;
    public int totalSeconds;
    public LeaderBoard leaderBoard;

    #region 相機屬性
    public Camera gameCamera;
    Rigidbody2D playerRb;
    public float zoomSpeed = 0.1f;
    public float waitTime = 8f;
    public float waitCounter; 
    public float zoomSize = 2f;       // 縮小後的視野大小
    public float defaultZoomSize = 8f; // 原始視野大小
    private bool isZoomedIn = false;  // 是否已縮小
    

    #endregion
    
  
    public Color[] colors = new Color[3]; // 儲存三種顏色
    public GameObject teachPanel;
    public TextMeshProUGUI teachText;
    public bool panelOn = false;
    public Transform beginDoor;
    public Speaker speaker;
    public GameObject hideStar;
    public float hideStarTime;
    public GameObject languagePanel; 
    public LocaleSelector localeSelector;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public LoadingManager loadingManager;
    public TextMeshProUGUI languageTitleText;
    public TextMeshProUGUI settingTitleText;
    public TextMeshProUGUI teachTitleText;
    public TextMeshProUGUI backToStartText;
    public TextMeshProUGUI backToMenuText;

    


    void Start()
    {
        languagePanel.SetActive(false);
        hideStar.SetActive(false);
        UnlockAchievement("Just_Here");
        teachPanel.SetActive(false);
        player.position = beginDoor.position;
        Vector3 loadPosition = LoadPlayerPositionFromSteamCloud();
        player.position = loadPosition;

        // 獲取所有帶有 "obstacle" 標籤的物體
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        // 遍歷所有帶有 "obstacle" 標籤的物體並為它們設置顏色
        for (int i = 0; i < obstacles.Length; i++)
        {
            Renderer renderer = obstacles[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                // 循環顏色數組，使用餘數運算來確保顏色重複使用
                renderer.material.color = colors[i % colors.Length];
                // Debug.Log($"為物體 {obstacles[i].name} 設置顏色 {colors[i % colors.Length]}");
            }
            else
            {
                Debug.LogWarning($"物體 {obstacles[i].name} 沒有 Renderer 組件，無法設置顏色！");
            }
        }
        // 為每個物體分配顏色
        // AssignColorsToObstacles(obstacles);

        playerRb = player.GetComponent<Rigidbody2D>();
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
			Debug.Log(name);
            CSteamID steamID = SteamUser.GetSteamID();
            string userId = steamID.ToString();
            Debug.Log("Current Steam User ID: " + userId);
          
            LoadTimeFromSteamCloud();
            // leaderBoard.ResetLeaderboard();
        }
        startTime = Time.time;
        isGameStarted = true;
        youWinPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        hideStarTime += Time.deltaTime;

        if (hideStarTime > 60f)
        {
            hideStar.SetActive(true);
        }
        HandleZoomLogic();
        // loadingManager.loadingPanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
        youWinPanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
        settingPanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
        teachPanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
        languagePanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel != null && !settingPanel.gameObject.activeSelf && !teachPanel.activeSelf)
            {
                settingPanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
                panelOn = true;
                settingPanel.SetActive(true);
                playerScript.canMove = false;
            } 
            else
            {
                panelOn = false;
                settingPanel.SetActive(false);
                playerScript.canMove = true;
            }
        }

        // 檢查玩家是否碰撞到 TheEnd
        if (isGameStarted && !isGameEnded)
        {
            // 計算遊戲進行時間
            elapsedTime = Time.time - startTime;
        }

        if (isGameEnded)
        {
            Time.timeScale = 0f;
        }

        SavePlayerPositionToSteamCloud(player.position);

        if (Input.GetKeyDown(KeyCode.Tab) && !teachPanel.activeSelf && !settingPanel.activeSelf)
        {
            teachPanel.transform.position = new Vector3(player.position.x, player.position.y, 0f);
            teachPanel.SetActive(true);
            panelOn = true;
            playerScript.canMove = false;
        } else if (Input.GetKeyDown(KeyCode.Tab) && teachPanel.activeSelf && !settingPanel.activeSelf)
        {
            teachPanel.SetActive(false);
            panelOn = false;
            playerScript.canMove = true;
        }
        if (localeSelector.localID == 0)
        {
            teachText.font = TraditionalFontAsset; // 如果有專屬英文字體，可改為對應的英文字體資源
            teachText.text =
                "First: Pressing the left mouse button allows you to charge and launch.\n\n" +
                "Second: Every object can be attached, but it cannot be attached consecutively more than once. When attaching an object, you can launch it without charging.\n\n" +
                "Third: Consecutively attaching different objects increases the launch force.\n\n" +
                "End: Enjoy!";
            languageTitleText.font = TraditionalFontAsset;
            languageTitleText.text = "Language";
            settingTitleText.font = TraditionalFontAsset;
            settingTitleText.text = "Setting";
            teachTitleText.font = TraditionalFontAsset;
            teachTitleText.text  =  "Teach";
            backToStartText.font = TraditionalFontAsset;
            backToStartText.text = "Back to Start";
            backToMenuText.font = TraditionalFontAsset;
            backToMenuText.text = "Back to Menu";
        }
        else if (localeSelector.localID == 1)
        {
            teachText.font = TraditionalFontAsset;
            teachText.text =
                "第一點：按下左鍵即可進行蓄力發射。\n\n" +
                "第二點：所有物件都可以被附著，但不能連續附著相同物件超過一次。當附著物件時，可以直接發射而不需要蓄力。\n\n" +
                "第三點：連續附著不同物件會增加發射力量。\n\n" +
                "結束：祝您遊戲愉快！";
            languageTitleText.font = TraditionalFontAsset;
            languageTitleText.text = "語言";
            settingTitleText.font = TraditionalFontAsset;
            settingTitleText.text = "設置";
            teachTitleText.font = TraditionalFontAsset;
            teachTitleText.text  =  "教學";
            backToStartText.font = TraditionalFontAsset;
            backToStartText.text = "回到起點";
            backToMenuText.font = TraditionalFontAsset;
            backToMenuText.text = "回到選單";
        }
        else if (localeSelector.localID == 2)
        {
            teachText.font = SimplifiedFontAsset;
            teachText.text =
                "第一点：按下左键即可进行蓄力发射。\n\n" +
                "第二点：所有物件都可以被附着，但不能连续附着相同物件超过一次。当附着物件时，可以直接发射而不需要蓄力。\n\n" +
                "第三点：连续附着不同物件会增加发射力量。\n\n" +
                "结束：祝您游戏愉快！";
            languageTitleText.font = SimplifiedFontAsset;
            languageTitleText.text = "语言";
            settingTitleText.font = SimplifiedFontAsset;
            settingTitleText.text = "设置";
            teachTitleText.font = SimplifiedFontAsset;
            teachTitleText.text  =  "教学";
            backToStartText.font = SimplifiedFontAsset;
            backToStartText.text = "回到起点";
             backToMenuText.font = SimplifiedFontAsset;
            backToMenuText.text = "返回选单";
        }

    }

    void AssignColorsToObstacles(GameObject[] obstacles)
    {
        // // 如果場景中的 "obstacle" 物體數量多於三個
        // if (obstacles.Length > 3)
        // {
        //     Debug.LogWarning("場景中有多於三個 obstacle 物體，超過部分將不會顯示顏色！");
        // }

        // 為每個物體分配顏色
        for (int i = 0; i < obstacles.Length && i < colors.Length; i++)
        {
            Renderer renderer = obstacles[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                // 為每個物體設置不同的顏色
                renderer.material.color = colors[i];
            }
        }
    }

   
    public void GameEnd()
    {
        UnlockAchievement("Thank_You");
        Time.timeScale = 0f;
        youWinPanel.gameObject.SetActive(true);
        StartCoroutine(FadeInYouWinPanel());
        speaker.audioSource.Pause();
        speaker.narrationTextUI.text = "";
        panelOn = true;
        totalSeconds = Mathf.FloorToInt(elapsedTime);
        Debug.Log(totalSeconds);
        // 計算分鐘和秒
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        // 格式化顯示
        if (minutes > 0)
        {
            timeString = minutes + "/m " + seconds + "s";
        }
        else
        {
            timeString = seconds + "/s";
        }

        Debug.Log("遊戲時間: " + timeString);
        spentTimeText.text = timeString;

      

        // 更新最佳紀錄時間
        yourBestRecordTime.text = bestTimeString;
     
        if (totalSeconds < storedTime || storedTime == 0)
        {
            SaveTimeToSteamCloud(totalSeconds);
            Debug.Log("Updated best time: " + bestTimeString);
        }

        player.position = beginDoor.position;
        SavePlayerPositionToSteamCloud(player.position);
        // leaderBoard.FetchLeaderboard();
        // leaderBoard.UploadScore(totalSeconds);

    }

    IEnumerator FadeInYouWinPanel()
    {
        float fadeDuration = 1f;
        float elapsedTime = 0f;

        youWinPanel.color = new Color(
            youWinPanel.color.r, 
            youWinPanel.color.g, 
            youWinPanel.color.b, 
            0f
        );

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            youWinPanel.color = new Color(
                youWinPanel.color.r,
                youWinPanel.color.g,
                youWinPanel.color.b,
                alpha
            );

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        youWinPanel.color = new Color(
            youWinPanel.color.r,
            youWinPanel.color.g,
            youWinPanel.color.b,
            1f
        );

    }

    public void PlayAgain()
    {
        loadingManager.LoadToGameScene(1);
        player.position = beginDoor.position;
        
        startTime = Time.time; 
        isGameStarted = true;
        isGameEnded = false;
        elapsedTime = 0f; 
        timeString = ""; 
        spentTimeText.text = timeString; 

        youWinPanel.gameObject.SetActive(false);
        teachPanel.SetActive(false);
        settingPanel.SetActive(false);
        youWinPanel.color = new Color(youWinPanel.color.r, youWinPanel.color.g, youWinPanel.color.b, 0f); // Reset the alpha
        panelOn = false; 

        Time.timeScale = 1f;
        
        // speaker.audioSource.Pause();
        speaker.narrationTextUI.text = ""; 



    }

    

    public void SaveTimeToSteamCloud(int totalSeconds)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat("Time", totalSeconds);
            bool success = SteamUserStats.StoreStats();
            
            if (success)
            {
                Debug.Log("Time saved to Steam Cloud: " + totalSeconds);
            }
            else
            {
                Debug.LogWarning("Failed to store stats to Steam Cloud.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to save Time.");
        }
    }

    public void LoadTimeFromSteamCloud()
    {

        if (SteamManager.Initialized)
        {
            // 從Steam Cloud中獲取金錢數據
            bool hasStat = SteamUserStats.GetStat("Time", out storedTime);
        
            if (hasStat && storedTime > 0)
            {
                int minutes = storedTime / 60;
                int seconds = storedTime % 60;
                bestTimeString = minutes + "/m " + seconds + "s";
                Debug.Log("Loaded time from Steam Cloud: " + bestTimeString);
            }
            else
            {
                // 如果沒有數據，設置默認值為 0
                storedTime = 0;
                bestTimeString = "0";
                Debug.LogWarning("No time record found. Setting default time to 0.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to load Time.");
        }
    }

    public void SavePlayerPositionToSteamCloud(Vector3 playerPosition)
    {
        if (SteamManager.Initialized)
        {
            // 将玩家的 X、Y、Z 坐标分开保存
            SteamUserStats.SetStat("PlayerPos_X", playerPosition.x);
            SteamUserStats.SetStat("PlayerPos_Y", playerPosition.y);
            SteamUserStats.SetStat("PlayerPos_Z", playerPosition.z);
            // 保存统计数据到 Steam Cloud
            bool success = SteamUserStats.StoreStats();
            
            if (success)
            {
                // Debug.Log("Player position saved to Steam Cloud: " + playerPosition);
            }
            else
            {
                // Debug.LogWarning("Failed to store player position to Steam Cloud.");
            }
       }
       else
       {
            Debug.LogWarning("Steam is not initialized. Unable to save player position.");
       }
    }

    public Vector3 LoadPlayerPositionFromSteamCloud()
    {
        if (SteamManager.Initialized)
        {
            float x, y, z;
            
            // 获取保存的坐标数据
            if (SteamUserStats.GetStat("PlayerPos_X", out x) &&
                SteamUserStats.GetStat("PlayerPos_Y", out y) &&
                SteamUserStats.GetStat("PlayerPos_Z", out z))
            {
                Vector3 playerPosition = new Vector3(x, y, z);
                Debug.Log("Player position loaded from Steam Cloud: " + playerPosition);
                return playerPosition;
            }
            else
            {
                Debug.LogWarning("Failed to load player position from Steam Cloud.");
                return Vector3.zero;  // 如果加载失败，可以返回一个默认位置
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to load player position.");
            return Vector3.zero;
        }
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

    #region 相機控制
    private void ZoomCamera(float targetSize)
    {
        // 如果當前視野大小接近目標值，則停止縮放
        if (Mathf.Abs(gameCamera.orthographicSize - targetSize) > 0.01f)
        {
            gameCamera.orthographicSize = Mathf.Lerp(gameCamera.orthographicSize, targetSize, zoomSpeed);
        }
    } 
    
    private void HandleZoomLogic()
    {
        // 檢查速度是否低於閾值
        if (playerRb.velocity.magnitude < 3f)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter > waitTime && !isZoomedIn)
            {
                isZoomedIn = true;
            }
        }
        else
        {
            isZoomedIn = false;
            waitCounter = 0f; // 重置等待時間
        }

        // 根據狀態縮放攝影機
        if (isZoomedIn)
        {
            ZoomCamera(zoomSize);
        }
        else
        {
            ZoomCamera(defaultZoomSize);
        }
    }
    #endregion

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

    public void BackToStart()
    {
        player.position = beginDoor.position;
    }
}
