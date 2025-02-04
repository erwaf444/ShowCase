using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public int currentLevel;
    private Dictionary<int, string> levelNames = new Dictionary<int, string>
    {
        { 1, "The Awakening" },           // 覺醒
        { 2, "Forest of Trials" },        // 試煉森林
        { 3, "Whispering Woods" },        // 低語森林
        { 4, "Desert of Secrets" },       // 秘密沙漠
        { 5, "Ruins of the Ancients" },   // 遺跡
        { 6, "Cave of Darkness" },        // 黑暗洞穴
        { 7, "Frozen Abyss" },            // 冰封深淵
        { 8, "Temple of the Lost" },      // 失落神殿
        { 9, "Echoing Catacombs" },       // 迴響地下墓穴
        { 10, "Crimson Valley" },         // 緋紅谷地
        { 11, "The Haunted Keep" },       // 詛咒城堡
        { 12, "Stormcaller’s Peak" },     // 風暴巔峰
        { 13, "The Sunken Ruins" },       // 沉沒遺跡
        { 14, "Eclipse Citadel" },        // 日蝕城堡
        { 15, "The Shattered Realm" },    // 破碎境界
        { 16, "Tower of Illusions" },     // 幻象之塔
        { 17, "Celestial Gate" },         // 天界之門
        { 18, "Abyssal Labyrinth" },      // 深淵迷宮
        { 19, "Infernal Sanctum" },       // 地獄聖堂
        { 20, "Chrono Spire" },           // 時間尖塔
        { 21, "The Forgotten Realm" },    // 被遺忘的世界
        { 22, "The Eternal Warden" },     // 永恆守衛
        { 23, "The Final Judgment" },     // 最終審判
        { 24, "Ascension" }               // 神聖升華
    };
    public LoadingManager loadingManager;
    private bool isCountingDown = true;
    // public Slider timerSlider; // 連結到 UI 的 Slider
    public bool canCountTime = false;
    // public TextMeshProUGUI timerText;     // 可選：連結到顯示時間的 Text
    private float remainingTime = 24f;
    private bool isPaused = false; // 是否處於暫停狀態
    public GameObject pauseMenu;
    public GameObject videoButton;
    public GameObject audioButton;
    public GameObject ExitButton;
    public GameObject videoArea;
    public GameObject audioArea;
    public GameObject exitArea;
    public GameObject videoButtonArrow;
    public GameObject audioButtonArrow;
    public GameObject ExitButtonArrow;

    public VideoPlayer videoPlayer; // 影片播放器
    public GameObject videoQuad;
    private bool hasPlayedVideo = false; // 確保影片只播放一次
    public TextMeshProUGUI videoLevelText;
    public TextMeshProUGUI videoLevelNameText;
    public GameObject drawCardMask;
    public TextMeshProUGUI drawCardText;
    public AllAnimAnimatorScript allAnimAnimatorScript;

    
    #region 卡牌結構
    // 定義卡片種類
    public enum CardType
    {
        stoneCard,  //石頭牌 
        scissorsCard,  //剪刀牌
        paperCard, //布牌
        middleFingerCard, //中指牌
        actuallyScissorsCard, //其實是剪刀牌
        loveCard, //愛心牌
        fingerGunCard, //指槍牌
        lieCard //騙人牌
    }

    public class Card
    {
        public CardType type; // 卡牌類型
        public int id;      // 卡牌編號

        public Card(CardType type, int id)
        {
            this.type = type;
            this.id = id;
        }

        public override string ToString()
        {
            return $"{type} {id}";
        }

        //重寫Equals、GetHashCode方法才能夠比較卡牌，然後移除卡牌
        public override bool Equals(object obj)
        {
            if (obj is Card otherCard)
            {
                return this.type == otherCard.type && this.id == otherCard.id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, id);
        }
    }
    //卡牌屬性
    public int enemyCardLeft;
    public int playerCardLeft;
    public TextMeshProUGUI enemyCardLeftText; //顯示剩餘可抽牌的雙眸
    public TextMeshProUGUI playerCardLeftText;
    public int totalCards = 104; // 總共的卡牌數量 (8種，每種13張，兩副牌)
    public int playerHandSize = 13; // 每位玩家的手牌數
    public int numPlayers = 2; // 玩家人數
    private List<Card> deck = new List<Card>(); // 卡牌堆
    private List<Card>[] playerHands; // 每位玩家的手牌
    // private int currentPlayer = 0; // 當前回合玩家
    public bool hasFirstDrawCard = false;
    // public GameObject cardPrefab;
    public Transform cardContainer; //玩家卡牌區域
    public Transform enemyCardContainer; //敵人卡牌區域
    public Transform playerPlayCardContainer; //玩家出牌區
    public Transform enemyPlayCardContainer; //敵人出牌區
    // 卡牌類型對應的權重
    private Dictionary<CardType, int> cardWeights = new Dictionary<CardType, int>
    {
        { CardType.stoneCard, 25 },          // 石頭牌
        { CardType.scissorsCard, 25 },       // 剪刀牌
        { CardType.paperCard, 25 },          // 布牌
        { CardType.middleFingerCard, 3 },   // 中指牌
        { CardType.actuallyScissorsCard, 10 }, // 其實是剪刀牌
        { CardType.loveCard, 4 },            // 愛心牌
        { CardType.fingerGunCard, 2 },       // 指槍牌
        { CardType.lieCard, 6 }              // 騙人牌
    };
    //不同種類的牌的prefab
    public GameObject stoneCardPrefab;
    public GameObject scissorsCardPrefab;
    public GameObject paperCardPrefab;
    public GameObject middleFingerCardPrefab;
    public GameObject actuallyScissorsCardPrefab;
    public GameObject loveCardPrefab;
    public GameObject fingerGunCardPrefab;
    public GameObject lieCardPrefab;
    public GameObject closeCardPrefab;
    public Dictionary<CardType, GameObject> cardPrefabs = new Dictionary<CardType, GameObject>();
    public Animator firstDrawCardAnim;
    public Animator enemyFirstDrawCardAnim;
    public GameObject arrowPrefab; // 箭頭的預設
    public GameObject playCardChoicePrefab;
    public GameObject cardPropertyChoicePrefab;
    public GameObject cardInfoPrefab; 
    public Animator playCardAnimator;
    public bool isPlayedCard = false;
    // private bool isPlayerTurn = true;  // 假設開始時是玩家回合
    public TurnManager turnManager;
    public CardType lastPlayedPlayerCard;
    public CardType lastPlayedEnemyCard;
    #endregion

    #region 勝負屬性
    // 敵人台詞
    private List<string> playerWinLines = new List<string>
    {
        "不可能！我怎麼會輸給你……",
        "這次是你贏了，但別得意！",
        "哼，這次算你走運，下次可沒這麼簡單了！",
        "你……？看來我低估你了。",
        "勝利只是暫時的，不要得意忘形！"
    };

    private List<string> enemyWinLines = new List<string>
    {
        "哈哈哈！這就是你的極限嗎？太弱了！",
        "你以為你能贏我？真是天真！",
        "失敗的滋味如何？下次再來挑戰我吧！",
        "這就是你的實力？真是令人失望！",
        "遊戲結束了，弱者不配站在我面前！"
    };

    private List<string> drawLines = new List<string>
    {
        "哼，看來我們勢均力敵……但下次你不會這麼幸運了！",
        "有趣，竟然能和我打成平手。不過，這只是開始！",
        "平局？這只是熱身而已，真正的戰鬥還在後面！",
        "你的實力不錯，但還不足以擊敗我！",
        "看來我們都需要再磨練一下技巧，下次再見分曉！"
    };
    public TextMeshProUGUI enemyLineText;
    public TextMeshProUGUI playerLineText;
    public GameObject enemyLineDialog;
    public GameObject playerLineDialog;
    public int playerScore;
    public int enemyScore;
    public GameObject scorePanel;
    public List<Image> stars;
    public Color playerWinColor = new Color(0.5f, 0.7f, 1f); // 玩家勝利顏色
    public Color enemyWinColor = Color.red;  // 敵人勝利顏色
    public Color drawColor = Color.green;  // 平手顏色
    public Color defaultColor = Color.white;  // 初始顏色
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;
    
    #endregion


    

    void Start()
    {
        LoadCurrentLevel();
        //播放戰鬥開場動畫
        if (videoPlayer != null && !hasPlayedVideo)
        {
            videoLevelText.gameObject.SetActive(false);
            videoLevelNameText.gameObject.SetActive(false);
            Time.timeScale = 0f;
            PlayOpeningVideo(); // 播放影片
        }

        cardPrefabs = new Dictionary<CardType, GameObject>
        {
            { CardType.stoneCard, stoneCardPrefab },
            { CardType.scissorsCard, scissorsCardPrefab },
            { CardType.paperCard, paperCardPrefab },
            { CardType.middleFingerCard, middleFingerCardPrefab },
            { CardType.actuallyScissorsCard, actuallyScissorsCardPrefab },
            { CardType.loveCard, loveCardPrefab },
            { CardType.fingerGunCard, fingerGunCardPrefab },
            { CardType.lieCard, lieCardPrefab }
        };

        // timerSlider.maxValue = remainingTime;
        // timerSlider.value = remainingTime;
        videoButtonArrow.SetActive(true);
        audioButtonArrow.SetActive(false);
        ExitButtonArrow.SetActive(false);
        InitializeDeck();
        // 測試：統計各種類型的卡牌數量
    var cardTypeCounts = new Dictionary<CardType, int>();
    foreach (var card in deck)
    {
        if (!cardTypeCounts.ContainsKey(card.type))
        {
            cardTypeCounts[card.type] = 0;
        }
        cardTypeCounts[card.type]++;
    }

    foreach (var kvp in cardTypeCounts)
    {
        Debug.Log($"CardType: {kvp.Key}, Count: {kvp.Value}");
    }
        ShuffleDeck();
        DealCards();
        enemyCardLeft = playerHandSize; //默認每位玩家都有13張牌
        playerCardLeft = playerHandSize;
        enemyCardLeftText.text = enemyCardLeft.ToString();
        playerCardLeftText.text = playerCardLeft.ToString();
        drawCardMask.SetActive(false);
        drawCardText.gameObject.SetActive(false);
    }

    // 播放開場影片
    private void PlayOpeningVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(true); // 顯示影片的 3D Quad
        }

        videoPlayer.loopPointReached += OnVideoEnd; // 設置影片播放完成事件
        videoPlayer.Play();
    }

    private void UpdateTextOverlay()
    {
        if (videoPlayer != null)
        {
            if (videoPlayer.time >= 5.6f) // 當影片播放到第 6 秒
            {
                ShowLevelText();
            }
        }
    }

    private void ShowLevelText()
    {
        if (videoLevelText != null)
        {
            videoLevelText.gameObject.SetActive(true);
            videoLevelText.text = currentLevel.ToString(); // 直接設定關卡數字
        }

        if (videoLevelNameText != null)
        {
            videoLevelNameText.gameObject.SetActive(true);

            if (levelNames.TryGetValue(currentLevel, out string levelName))
            {
                videoLevelNameText.text = levelName; // 透過 Dictionary 設定關卡名稱
            }
            else
            {
                videoLevelNameText.text = "Unknown Level"; // 預設值，避免出錯
            }
        }
    }

    // 當影片播放結束
    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("影片播放結束");
        if (videoQuad != null)
        {
            videoQuad.SetActive(false); // 隱藏影片的 3D Quad
            videoLevelText.gameObject.SetActive(false);
            videoLevelNameText.gameObject.SetActive(false);
        }
        
        hasPlayedVideo = true; // 標記影片已播放

        StartCoroutine(WaitForFirstDrawCard()); // 啟動等待協程
    }

    private IEnumerator WaitForFirstDrawCard()
    {
        drawCardMask.SetActive(true);
        drawCardText.gameObject.SetActive(true);
        Debug.Log("等待第一次抽牌完成...");

        // 持續檢查直到 hasFirstDrawCard 為 true
        while (!hasFirstDrawCard)
        {
            yield return null; // 每幀繼續檢查
        }

        // 抽牌完成後恢復遊戲
        Debug.Log("第一次抽牌完成，遊戲繼續。");
        drawCardMask.SetActive(false);
        drawCardText.gameObject.SetActive(false);
        Time.timeScale = 1f; // 繼續遊戲

    }


    void Update()
    {
        UpdateTextOverlay();
        // 倒數計時
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // 恢復遊戲
            }
            else
            {
                Pause(); // 暫停遊戲
            }
        }
        
        playerScoreText.text = playerScore.ToString();
        enemyScoreText.text = enemyScore.ToString();
        // for (int i = 0; i < playerHands.Length; i++)
        // {
        //     Debug.Log($"Player {i + 1} initial hand size: {playerHands[i].Count}");
        // }
        // Debug.Log("playerHands.Length: " + playerHands.Length);
    }

    // 暫停遊戲
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f; // 停止遊戲時間
        // 在這裡可以啟用暫停菜單或其他 UI
        Debug.Log("遊戲暫停");
        pauseMenu.SetActive(true);
    }

    // 恢復遊戲
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 恢復遊戲時間
        // 在這裡可以關閉暫停菜單或其他 UI
        Debug.Log("遊戲恢復");
        pauseMenu.SetActive(false);
    }

   
    public void VideoButtonPress()
    {
        videoButtonArrow.SetActive(true);
        audioButtonArrow.SetActive(false);
        ExitButtonArrow.SetActive(false);
        videoArea.SetActive(true);
        audioArea.SetActive(false);
        exitArea.SetActive(false);
    }

    public void AudioButtonPress()
    {
        videoButtonArrow.SetActive(false);
        audioButtonArrow.SetActive(true);
        ExitButtonArrow.SetActive(false);
        videoArea.SetActive(false);
        audioArea.SetActive(true);
        exitArea.SetActive(false);
    }

    public void ExitButtonPress()
    {
        videoButtonArrow.SetActive(false);
        audioButtonArrow.SetActive(false);
        ExitButtonArrow.SetActive(true);
        videoArea.SetActive(false);
        audioArea.SetActive(false);
        exitArea.SetActive(true);
    }

    public void ExitBattle()
    {
        loadingManager.LoadToMainMenu(0);
    }




    #region 卡牌區域
    // 根據權重隨機抽取卡牌類型
    private CardType GetRandomCardTypeByWeight()
    {
        int totalWeight = cardWeights.Values.Sum(); // 計算總權重
        int randomValue = UnityEngine.Random.Range(0, totalWeight); // 在 0 到總權重之間生成隨機數

        int cumulativeWeight = 0;
        foreach (var kvp in cardWeights)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue < cumulativeWeight)
            {
                return kvp.Key; // 返回對應的卡牌類型
            }
        }

        // 如果沒找到（理論上不會發生）
        throw new System.Exception("Error in weighted random selection.");
    }

    // 初始化卡牌堆
    void InitializeDeck()
    {
        // foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
        // {
        //     for (int j = 1; j <= 13; j++) // 每種卡有13張
        //     {
        //         deck.Add(new Card(cardType, j));
        //         deck.Add(new Card(cardType, j)); // 假設兩副牌
        //     }
        // }
        for (int i = 0; i < totalCards; i++) // 根據總卡牌數生成
        {
            CardType randomType = GetRandomCardTypeByWeight();
            int cardId = (i % 13) + 1; // 假設每種卡牌有 13 張（1 到 13 編號）
            deck.Add(new Card(randomType, cardId));
        }
        Debug.Log($"Deck initialized with {deck.Count} cards.");

    }

    // 洗牌
    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, deck.Count);
            Card temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // 發牌
    void DealCards()
    {
        playerHands = new List<Card>[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            playerHands[i] = new List<Card>();
        }

        for (int i = 0; i < playerHandSize; i++)
        {
            for (int j = 0; j < numPlayers; j++)
            {
                if (deck.Count > 0)
                {
                    playerHands[j].Add(deck[0]);
                    deck.RemoveAt(0);
                }
            }
        }

        // 檢查玩家手牌和牌堆
        for (int i = 0; i < playerHands.Length; i++)
        {
            Debug.Log($"Player {i + 1} initial hand size: {playerHands[i].Count}");
        }
        Debug.Log($"Deck remaining cards: {deck.Count}");
    }

    // 顯示玩家手牌
    void DisplayPlayerHands()
    {
        for (int i = 0; i < playerHands.Length; i++)
        {
            Debug.Log($"Player {i + 1} Hand:");
            foreach (var card in playerHands[i])
            {
                Debug.Log(card.ToString());
            }
        }
    }

    // 玩家抽牌
    // public void DrawCard(int playerIndex)
    // {
    //     // 確認玩家手上剩下一張牌時才進行抽牌操作
    //     if (playerHands[playerIndex].Count == 1)
    //     {
    //         int cardsToDraw = Mathf.Min(3, deck.Count); // 確保不會超出剩餘牌數
            
    //         int currentHandSize = playerHands[playerIndex].Count;
    //         if (currentHandSize < 13)
    //         {
    //             cardsToDraw = Mathf.Min(cardsToDraw, 13 - currentHandSize);
    //         }
            
    //         for (int i = 0; i < cardsToDraw; i++)
    //         {
    //             Card drawnCard = deck[0];
    //             deck.RemoveAt(0);
    //             playerHands[playerIndex].Add(drawnCard);

    //             // 根據卡牌類型選擇對應的 Prefab
    //             if (cardPrefabs.TryGetValue(drawnCard.type, out GameObject selectedPrefab))
    //             {
    //                 GameObject cardObject = Instantiate(selectedPrefab, cardContainer);
    //                 // cardObject.name = $"{drawnCard.type} {drawnCard.id}";

    //                 // // 更新卡牌顯示
    //                 // TextMeshProUGUI cardText = cardObject.GetComponentInChildren<TextMeshProUGUI>();
    //                 // if (cardText != null)
    //                 // {
    //                 //     cardText.text = $"{drawnCard.type} {drawnCard.id}";
    //                 // }
    //             }
    //             else
    //             {
    //                 Debug.LogWarning($"No prefab found for card type: {drawnCard.type}");
    //             }
               
    //         }

    //         // 更新剩餘卡牌數並同步到 UI
    //         if (playerIndex == 0) // 假設玩家是索引 0
    //         {
    //             playerCardLeft -= cardsToDraw;
    //             playerCardLeftText.text = playerCardLeft.ToString();
    //         }
    //         else if (playerIndex == 1) // 假設敵人是索引 1
    //         {
    //             enemyCardLeft -= cardsToDraw;
    //             enemyCardLeftText.text = enemyCardLeft.ToString();
    //         }

    //         Debug.Log($"Player {playerIndex + 1} drew {cardsToDraw} cards");
    //     }
    //     else
    //     {
    //         Debug.Log($"Player {playerIndex + 1} cannot draw cards because they have more than 1 card in hand.");
    //     }
    // }

    public void DrawCard(int playerIndex)
    {
        if ( turnManager.currentTurn != 4 && turnManager.currentTurn != 7 && turnManager.currentTurn != 10)
        {
            Debug.Log(turnManager.currentTurn);
            Debug.Log("不是第四、七、十回合");
            return;
        }
        if (turnManager.hasDrawCardForRound == true)
        {
            return;
        }
        int cardsToDraw = Mathf.Min(3, playerHands[playerIndex].Count); // 確保不會超出剩餘牌數
        int enemyIndex = (playerIndex == 0) ? 1 : 0; // 假設敵人是另一位玩家

        // 玩家抽牌
        for (int i = 0; i < cardsToDraw; i++)
        {
            if (playerHands[playerIndex].Count > 0)
            {
                // 從玩家的剩餘牌中抽取一張卡牌
                Card playerDrawCard = playerHands[playerIndex][0]; // 假設我們從手上的第一張牌抽取
                playerHands[playerIndex].RemoveAt(0); // 移除這張卡

                // 显示玩家的 closeCardPrefab
                GameObject closeCardObject = Instantiate(closeCardPrefab, cardContainer);
                // 播放动画
                firstDrawCardAnim.SetTrigger("FirstDrawCard");
                // 使用协程延迟显示真正的卡牌
                StartCoroutine(RevealPlayerCardAfterAnimation(closeCardObject, playerDrawCard));
            }
        }

        // 敵人抽牌
        for (int i = 0; i < cardsToDraw; i++)
        {
            if (playerHands[enemyIndex].Count > 0)
            {
                // 從敵人的剩餘牌中抽取一張卡牌
                Card enemyDrawCard = playerHands[enemyIndex][0]; // 假設我們從手上的第一張牌抽取
                playerHands[enemyIndex].RemoveAt(0); // 移除這張卡

                // 敵人卡牌顯示為 closeCardPrefab
                GameObject enemyCardObject = Instantiate(closeCardPrefab, enemyCardContainer);
                enemyFirstDrawCardAnim.SetTrigger("FirstDrawCard");
                // 可以在需要時保存這些卡片對象，方便後續替換
                enemyCardObject.AddComponent<CardDisplay>().Initialize(enemyDrawCard, false); // 使用自訂的 `CardDisplay` 類處理邏輯
            }
        }

        // 更新剩餘卡牌數並同步到 UI
        playerCardLeft -= cardsToDraw;
        enemyCardLeft -= cardsToDraw;

        playerCardLeftText.text = playerCardLeft.ToString();
        enemyCardLeftText.text = enemyCardLeft.ToString();
        turnManager.hasDrawCardForRound = true;

        Debug.Log($"Player {playerIndex + 1} and enemy drew {cardsToDraw} cards each");
    }

    public void FirstDrawCards(int playerIndex)
    {
        if (hasFirstDrawCard) return;
        int cardsToDraw = Mathf.Min(4, deck.Count); // 確保不會超出剩餘牌數
        int enemyIndex = (playerIndex == 0) ? 1 : 0; // 假設敵人是另一位玩家
        
       
        for (int i = 0; i < cardsToDraw; i++)
        {
            if (playerHands[playerIndex].Count > 0)
            {
               // 從玩家的 13 張卡中抽取一張卡牌
                Card playerDrawCard = playerHands[playerIndex][0]; // 假設我們從手上的第一張牌抽取
                playerHands[playerIndex].RemoveAt(0); // 移除這張卡

                // 显示玩家的 closeCardPrefab
                GameObject closeCardObject = Instantiate(closeCardPrefab, cardContainer);
                // 播放动画
                firstDrawCardAnim.SetTrigger("FirstDrawCard");
                // 使用协程延迟显示真正的卡牌
                StartCoroutine(RevealPlayerCardAfterAnimation(closeCardObject, playerDrawCard));
            }
        }
           

        for (int i = 0; i < cardsToDraw; i++)
        {
            if (playerHands[enemyIndex].Count > 0)
            {
                // 從敵人的 13 張卡中抽取一張卡牌
                Card enemyDrawCard = playerHands[enemyIndex][0]; // 假設我們從手上的第一張牌抽取
                playerHands[enemyIndex].RemoveAt(0); // 移除這張卡

                // 敵人卡牌顯示為 closeCardPrefab
                GameObject enemyCardObject = Instantiate(closeCardPrefab, enemyCardContainer);
                enemyFirstDrawCardAnim.SetTrigger("FirstDrawCard");
                // 可以在需要時保存這些卡片對象，方便後續替換
                enemyCardObject.AddComponent<CardDisplay>().Initialize(enemyDrawCard, false); // 使用自訂的 `CardDisplay` 類處理邏輯
            }   
        }
        // 更新剩餘卡牌數並同步到 UI
        playerCardLeft -= cardsToDraw;
        enemyCardLeft -= cardsToDraw;

        playerCardLeftText.text = playerCardLeft.ToString();
        enemyCardLeftText.text = enemyCardLeft.ToString();

        Debug.Log($"Player {playerIndex + 1} and enemy drew {cardsToDraw} cards each");
        hasFirstDrawCard = true;
        turnManager.TurnTextOnBattleSceneShowUp();
    }

    public void OnDrawButtonClicked(int playerIndex)
    {
        if (hasFirstDrawCard)
        {
            DrawCard(playerIndex); // 后续抽牌
        }
        else
        {
            FirstDrawCards(playerIndex); // 第一次抽牌
        }
    }

    private IEnumerator RevealPlayerCardAfterAnimation(GameObject closeCardObject, Card playerDrawCard)
    {
        // 等待动画播放完成
        yield return new WaitForSeconds(firstDrawCardAnim.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1f);
        // 删除 closeCardPrefab
        Destroy(closeCardObject);

        // 根据卡牌类型显示真实的卡牌
        if (cardPrefabs.TryGetValue(playerDrawCard.type, out GameObject playerCardPrefab))
        {
            GameObject playerCardObject = Instantiate(playerCardPrefab, cardContainer);
            // playerCardObject.name = $"{playerDrawCard.type} {playerDrawCard.id}";
            playerCardObject.AddComponent<CardDisplay>().Initialize(playerDrawCard, true); 
        }
        else
        {
            Debug.LogWarning($"No prefab found for card type: {playerDrawCard.type}");
        }

        yield return new WaitForSeconds(1f);
        canCountTime = true;
    }


    public void RevealEnemyCards()
    {
        foreach (Transform child in enemyCardContainer)
        {
            CardDisplay cardDisplay = child.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.RevealCard(cardPrefabs, enemyCardContainer);
            }
        }
    }

    public void PlayCard(Card card, GameObject cardObject)
    {
        // 從玩家手牌中移除
        // int currentPlayerIndex = 0; // 假設玩家是 0 號

        // 更新出牌區的顯示
        // Debug.Log($"Player played: {card}");

        lastPlayedPlayerCard = card.type;
        Vector3 originalPosition = cardObject.transform.position;
        Quaternion originalRotation = cardObject.transform.rotation;

        // 紀錄原始的父物件（如果有的話）
        Transform parent = cardObject.transform.parent;

        // 銷毀當前物件
        Destroy(cardObject);

        // 根據卡牌數據選擇正確的 prefab
        if (cardPrefabs.TryGetValue(card.type, out GameObject cardPrefab))
        {
            // 實例化新物件
            GameObject newCard = Instantiate(cardPrefab, originalPosition, originalRotation);
            newCard.transform.SetParent(playerPlayCardContainer); // 保留世界空間位置
            
            // 設置其他屬性（例如大小、比例等）
            newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localScale = new Vector3(1.5f, 1.8f, 1f);

            Debug.Log($"Created new card: {card.type}");
            isPlayedCard = true;
        }
        else
        {
            Debug.LogWarning($"No prefab found for card type: {card.type}");
        }
        turnManager.EndTurn();
    }

    public CardType GetPlayerPlayedCard()
    {
        return lastPlayedPlayerCard;
    }
  
    // 敵人自動出牌邏輯
    public void EnemyPlayCard()
    {
        // if (playerHands[1].Count == 0) 
        // {
        //     Debug.Log("敵人沒有卡牌");
        //     return; // 如果敵人沒有卡牌則不執行
        // }
        // 隨機選擇敵人桌上的一張卡
        int randomIndex = UnityEngine.Random.Range(0, enemyCardContainer.childCount);
        Transform selectedCardTransform = enemyCardContainer.GetChild(randomIndex);
        Card selectedCard = selectedCardTransform.GetComponent<CardDisplay>().GetCard();

        lastPlayedEnemyCard = selectedCard.type;
        // 從桌面上移除這張卡（這樣就不會再重複使用）
        Destroy(selectedCardTransform.gameObject);

        // 從敵人的手牌中移除這張卡牌
        // playerHands[1].RemoveAt(randomIndex);

        // 根據卡牌的類型選擇正確的 prefab 並顯示在出牌區
        if (cardPrefabs.TryGetValue(selectedCard.type, out GameObject cardPrefab))
        {
            // 顯示敵人出牌
            GameObject closeCardObject = Instantiate(closeCardPrefab, enemyPlayCardContainer);
            closeCardObject.transform.localPosition = Vector3.zero;
            closeCardObject.transform.localScale = new Vector3(1.5f, 1.8f, 1f);
            // 你可以在這裡添加一個動畫，讓卡片顯示更流暢
            Debug.Log($"Enemy played: {selectedCard.type}");
            
            // 更新剩餘的敵人卡牌數
            // enemyCardLeft--;
            // enemyCardLeftText.text = enemyCardLeft.ToString();
            StartCoroutine(ShowBothPlayedCard(closeCardObject, selectedCard, cardPrefab));
        }
        else
        {
            Debug.LogWarning($"No prefab found for card type: {selectedCard.type}");
        }
        turnManager.IsPlayerTurn();
    }

    public CardType GetEnemyPlayedCard()
    {
        return lastPlayedEnemyCard;
    }

   IEnumerator ShowBothPlayedCard(GameObject closeCardObject, Card selectedCard, GameObject cardPrefab)
    {
        // 時間結束處理邏輯
        canCountTime = false;
        // 延遲顯示
        allAnimAnimatorScript.turnEnemyCardAnimator.SetTrigger("ShowEnemyCard");
        yield return new WaitForSeconds(4f);
        allAnimAnimatorScript.turnEnemyCardAnimator.ResetTrigger("ShowEnemyCard");

        // 替換為真實的卡片
        Destroy(closeCardObject);

        GameObject realCardObject = Instantiate(cardPrefab, enemyPlayCardContainer);
        realCardObject.transform.localPosition = Vector3.zero;
        realCardObject.transform.localScale = new Vector3(1.5f, 1.8f, 1f);

        // 初始化卡片顯示（假設 CardDisplay 處理卡片的初始化邏輯）
        // realCardObject.GetComponent<CardDisplay>().Initialize(selectedCard, false);

        yield return new WaitForSeconds(3f);
        ClearPlayCardContainer(playerPlayCardContainer);
        ClearPlayCardContainer(enemyPlayCardContainer);
        turnManager.OnTimeUp();

    }

    public int CompareCards(CardType playerCard, CardType enemyCard)
    {
        //平局區域
        if (playerCard == enemyCard)
            return 0; 

        if (playerCard == CardType.loveCard && (enemyCard != CardType.fingerGunCard))
        {
            return 0; 
        } 

        if (playerCard == CardType.lieCard && 
        (
            enemyCard == CardType.stoneCard || 
            enemyCard == CardType.paperCard ||
            enemyCard == CardType.scissorsCard
        ))
        {
            return 0; 
        } 

        if (playerCard == CardType.scissorsCard && 
        (
            enemyCard == CardType.actuallyScissorsCard
        ))
        {
            return 0; 
        }

        if (playerCard == CardType.actuallyScissorsCard && 
        (
            enemyCard == CardType.scissorsCard
        ))
        {
            return 0; 
        }

        //玩家勝利
        if ((playerCard == CardType.stoneCard && (enemyCard == CardType.scissorsCard)) ||
            (playerCard == CardType.scissorsCard && (enemyCard == CardType.paperCard)) ||
            (playerCard == CardType.paperCard && (enemyCard == CardType.stoneCard)))
        {
            return 1; 
        }

        if (playerCard == CardType.fingerGunCard)
        {
            return 1; 
        }
        
        if(playerCard == CardType.middleFingerCard && (enemyCard != CardType.fingerGunCard))
        {
            return 1; 
        }

        if(playerCard == CardType.actuallyScissorsCard && 
        (
            enemyCard != CardType.fingerGunCard ||
            enemyCard != CardType.middleFingerCard ||
            enemyCard != CardType.paperCard || 
            enemyCard != CardType.scissorsCard 
        ))
        {
            return 1; 
        }

        //玩家失敗
        if(playerCard == CardType.actuallyScissorsCard && 
        (
            enemyCard == CardType.fingerGunCard
        ))
        {
            return -1;
        }

        return -1;
    }

    // 清空出牌區的方法
    public void ClearPlayCardContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject); // 刪除子物件
        }
    }

    // 下一回合邏輯（可自行實現具體內容）
    public void StartNextTurn()
    {
        Debug.Log("準備開始下一回合！");
        // 例如初始化計時器、發牌等邏輯
        // gameManager.ResetTimer();
        // gameManager.DealNewCards();
    }


    #endregion



    #region 勝負判定
    public void PlayerWinRound()
    {
        // 處理玩家勝利邏輯，如增加分數
        playerScore++;
        Debug.Log($"玩家得分：{playerScore}");
        string enemyline = GetRandomEnemyLine(playerWinLines);
        enemyLineText.text = enemyline;
        enemyLineDialog.SetActive(true);
        StartCoroutine(WaitAndCloseLineDialog());
    }    

    public void EnemyWinRound()
    {
        // 處理敵人勝利邏輯，如增加分數
        enemyScore++;
        Debug.Log($"敵人得分：{enemyScore}");
        string enemyline = GetRandomEnemyLine(enemyWinLines);
        enemyLineText.text = enemyline;
        enemyLineDialog.SetActive(true);
        StartCoroutine(WaitAndCloseLineDialog());

    }

    public void BothWinRound()
    {
        // 處理平局邏輯
        string enemyline = GetRandomEnemyLine(drawLines);
        enemyLineText.text = enemyline;
        enemyLineDialog.SetActive(true);
        StartCoroutine(WaitAndCloseLineDialog());

    }

    IEnumerator WaitAndCloseLineDialog()
    {
        yield return new WaitForSeconds(5f);
        enemyLineDialog.SetActive(false);
    }

    public void OpenScorePanel()
    {
        scorePanel.SetActive(true);
    }

    public void CloseScorePanel()
    {
        scorePanel.SetActive(false);
    }

    public void UpdateStarColor(int turnIndex, Color color)
    {
        if (turnIndex < stars.Count && turnIndex >= 0)
        {
            stars[turnIndex].color = color;
        }
    }

    // 隨機選擇敵人一句台詞
    private string GetRandomEnemyLine(List<string> lines)
    {
        int index = UnityEngine.Random.Range(0, lines.Count);
        return lines[index];
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
