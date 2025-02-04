using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public GameManager gameManager;
    public bool isPlayerTurn; // 當前是否是玩家回合
    // public float turnTime = 30f; // 每回合的時間限制
    public float remainingTime; // 當前回合的剩餘時間

    public TextMeshProUGUI timerText; // 顯示計時器的 UI
    public Slider timerSlider; // 顯示計時器的進度條
    public int currentTurn; // 用來計算目前回合數
    public TextMeshProUGUI turnText; //回合數UI
    public AllAnimAnimatorScript allAnimAnimatorScript;
    public bool isOnPlay = true; //記錄此時是否在玩家選擇出卡片的時間
    public TextMeshProUGUI turnTextOnBattleScene; //回合數在戰鬥畫面的: Round 1
    public TextMeshProUGUI turnVictoryText; //回合數在戰鬥畫面的: Round 1
    public VideoPlayer turnTextOnBattleSceneVideoPlayer;
    public GameObject turnTextOnBattleSceneQuad;
    public GameObject turnTextOnBattleSceneMask;
    public GameObject drawCardNotice;//第4、7、10回合抽牌提醒
    public bool hasDrawCardForRound = false;
    public GameObject EndLevelGameObject;
    public TextMeshProUGUI EndLevelGameObjectText;
    public LoadingManager loadingManager;
    // public LevelManager levelManager;
    // public StoryManager storyManager;
    public int currentLevel = 1;

    void Start()
    {
        drawCardNotice.SetActive(false);

        currentTurn = 1; // 初始化為第一回合
        IsPlayerTurn();
        UpdateTurnText(); // 更新回合文字

        // 確保初始透明度為 0
        if (turnTextOnBattleSceneQuad != null)
        {
            RawImage  image = turnTextOnBattleSceneQuad.GetComponent<RawImage >();
            if (image != null)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, 0f); // 設置透明度為 0
            }
        }
        if (turnVictoryText != null)
        {
            Color color = turnVictoryText.color;
            turnVictoryText.color = new Color(color.r, color.g, color.b, 0f); // 設置透明度為 0
        }

        turnVictoryText.gameObject.SetActive(false);
        turnTextOnBattleSceneVideoPlayer.loopPointReached += OnTurnTextVideoEnd;
        currentTurn -= 1; //讓目前回合 = 1
    }

    void Update()
    {
        if (currentTurn == 4 || currentTurn == 7 || currentTurn == 10)
        {
            if (!hasDrawCardForRound)
            {
                drawCardNotice.SetActive(true);
                gameManager.canCountTime = false;
            } else 
            {
                drawCardNotice.SetActive(false);
                //接著這裡有一個 gameManager.canCountTime = true;，寫在EndTurn()裡面
            }
        }
        if (remainingTime > 0 && gameManager.canCountTime) //canCountTime是等待玩家的牌翻面
        {
            remainingTime -= Time.deltaTime;
            timerSlider.value = remainingTime;

            // 更新文字（如果有）
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(remainingTime).ToString() + " / 秒";
                // 漸進更改字體顏色
                float t = Mathf.Clamp01(remainingTime / 24); // 剩餘時間的比例 (0 到 1)
                // 線性插值從紅色 (時間少) 到白色 (時間多)
                timerText.color = Color.Lerp(Color.red, Color.black, t);
            }
        }
        if (currentTurn == 14)
        {
            turnTextOnBattleScene.text = "";
        } else 
        {
            turnTextOnBattleScene.text = "Round " + currentTurn;
        }
        


    }

    private void PlayTurnTextVideo()
    {
        if (turnTextOnBattleSceneQuad != null)
        {
            turnTextOnBattleSceneQuad.SetActive(true); 
            turnTextOnBattleSceneMask.SetActive(true);
            turnVictoryText.gameObject.SetActive(true);
        }
        StartCoroutine(FadeIn(turnTextOnBattleSceneQuad.GetComponent<RawImage>(), 1f)); // 1 秒內淡入
        StartCoroutine(FadeIn(turnVictoryText, 1f)); // 1 秒內淡入

        turnTextOnBattleSceneVideoPlayer.Play();
    }

    private void OnTurnTextVideoEnd(VideoPlayer vp)
    {
        Debug.Log("影片播放結束");
        StartCoroutine(FadeOut(turnTextOnBattleSceneQuad.GetComponent<RawImage>(), 1f)); // 1 秒內淡出
        StartCoroutine(FadeOut(turnVictoryText, 1f, () =>
        {
            // 淡出完成後隱藏物件
            turnTextOnBattleSceneQuad.SetActive(false);
            turnTextOnBattleSceneMask.SetActive(false);
            turnVictoryText.gameObject.SetActive(false);
        }));

        NextTurn();
      
    }

    

    void UpdateTurnText()
    {
        turnText.text = "第" + ConvertNumberToChinese(currentTurn) + "回合";
    }

    string ConvertNumberToChinese(int number)
    {
        string[] chineseNumbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

        if (number < 10)
        {
            // 單位數字直接轉換
            return chineseNumbers[number];
        }

        string result = "";
        int tens = number / 10; // 計算十位數
        int units = number % 10; // 計算個位數

        if (tens == 1)
        {
            result += "十"; // 十位數為1時，只顯示「十」
        }
        else
        {
            result += chineseNumbers[tens] + "十"; // 顯示「二十」、「三十」等
        }

        if (units > 0)
        {
            result += chineseNumbers[units]; // 加上個位數
        }

        return result;
    }

    public void NextTurn()
    {
        Debug.Log("下一回合");
        Debug.Log("调用堆栈: " + Environment.StackTrace);

        currentTurn++; // 增加回合數
        Debug.Log(currentTurn);
        remainingTime = 24; // 重設剩餘時間
        timerSlider.value = remainingTime;
        timerText.text = Mathf.Ceil(remainingTime).ToString() + " / 秒";
        UpdateTurnText(); // 更新顯示文字
        TurnTextOnBattleSceneShowUp();
        gameManager.canCountTime = true;
        gameManager.isPlayedCard = false;
        hasDrawCardForRound = false;

        // 檢查遊戲是否結束
        if (currentTurn == 14)
        {
            Debug.Log("遊戲結束");
            EndGame();
        }
    }




    public void IsPlayerTurn()
    {
        isPlayerTurn = true;
    }

    public void IsEnemyTurn()
    {
        isPlayerTurn = false;
    }

    public void EndTurn()
    {
        if (isPlayerTurn)
        {
            // 玩家出牌後結束回合，轉換到敵人回合
            isPlayerTurn = false;
            Debug.Log("EnemyPlayCard開始");
            gameManager.EnemyPlayCard();  // 讓敵人出牌
            isOnPlay = false;
            gameManager.canCountTime = false;
        }

        
    }

    public void TurnTextOnBattleSceneShowUp()
    {
        // turnTextOnBattleScene.gameObject.SetActive(true);
        allAnimAnimatorScript.turnTextAnimator.SetTrigger("FadeIn");
        StartCoroutine(WaitAndCloseTurnTextOnBattleScene());
    }

    IEnumerator WaitAndCloseTurnTextOnBattleScene()
    {
        yield return new WaitForSeconds(3f);
        allAnimAnimatorScript.turnTextAnimator.SetTrigger("FadeOut");
        // turnTextOnBattleScene.gameObject.SetActive(false);
    }
 
    public void OnTimeUp()
    {
      
        // 玩家和敵人出牌
        GameManager.CardType playerCard = gameManager.GetPlayerPlayedCard();
        GameManager.CardType enemyCard = gameManager.GetEnemyPlayedCard();

        int result = gameManager.CompareCards(playerCard, enemyCard);

        if (result == 1)
        {
            Debug.Log("玩家贏得了本回合！");
            // 更新玩家分數或其他邏輯
            gameManager.PlayerWinRound();
            gameManager.UpdateStarColor(currentTurn - 1, gameManager.playerWinColor);
            turnVictoryText.text = "You Win!";
        }
        else if (result == -1)
        {
            Debug.Log("敵人贏得了本回合！");
            // 更新敵人分數或其他邏輯
            gameManager.EnemyWinRound();
            gameManager.UpdateStarColor(currentTurn - 1, gameManager.enemyWinColor);
            turnVictoryText.text = "You Lose!";
        }
        else
        {
            Debug.Log("本回合平局！");
            gameManager.BothWinRound();
            gameManager.UpdateStarColor(currentTurn - 1, gameManager.drawColor);
            turnVictoryText.text = "Oh! Draw!";
            // 處理平局邏輯（如加時或直接進入下一回合）
        }

        PlayTurnTextVideo();
        // currentTurn++;
        // Debug.Log("現在是第" + currentTurn + "回合");
    }

    public void EndGame()
    {
        Debug.Log("EndGame");
        if (gameManager.playerScore > gameManager.enemyScore)
        {
            EndLevelGameObjectText.text = "You Win!";
            StartCoroutine(WaitAndClose());
            currentLevel += 1;
            SaveCurrentLevel(currentLevel);
        } else if (gameManager.playerScore < gameManager.enemyScore)
        {
            EndLevelGameObjectText.text = "You Lose!";
            StartCoroutine(WaitAndClose());

        } else if (gameManager.playerScore == gameManager.enemyScore)
        {
            EndLevelGameObjectText.text = "Draw!";
            StartCoroutine(WaitAndClose());
        }
        gameManager.canCountTime = false;
        
    }
    

    IEnumerator WaitAndClose()
    {
        yield return new WaitForSeconds(3f);
        allAnimAnimatorScript.gameLevelEndAnimator.SetTrigger("WinShow");

    }
   
    #region FadeIn&Out
    // Fade In 協程（用於 Image）
    private IEnumerator FadeIn(RawImage image, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // 目標透明度為 1

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        image.color = targetColor; // 確保最終透明度為 1
    }

    // Fade In 協程（用於 TextMeshProUGUI）
    private IEnumerator FadeIn(TextMeshProUGUI text, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // 目標透明度為 1

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        text.color = targetColor; // 確保最終透明度為 1
    }

    // Fade Out 協程（用於 Image）
    private IEnumerator FadeOut(RawImage image, float duration, Action onComplete = null)
    {
        float elapsedTime = 0f;
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // 目標透明度為 0

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        image.color = targetColor; // 確保最終透明度為 0
        onComplete?.Invoke(); // 執行回調函數
    }

    // Fade Out 協程（用於 TextMeshProUGUI）
    private IEnumerator FadeOut(TextMeshProUGUI text, float duration, Action onComplete = null)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // 目標透明度為 0

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        text.color = targetColor; // 確保最終透明度為 0
        onComplete?.Invoke(); // 執行回調函數
    }
    #endregion


    #region steamAPI
    // 儲存當前等級
    public static void SaveCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("currentLevel", level);
        PlayerPrefs.Save(); // 確保資料存儲
        Debug.Log("Level saved: " + level);
    }

    public  int LoadCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        Debug.Log("Loaded level: " + currentLevel);
        return currentLevel;
    }

    #endregion
}
