using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameManager.Card cardData; // 儲存卡片數據
    public Button cardButton; // Unity 中的按鈕
    public GameManager gameManager;
    public TurnManager turnManager;
    public GameObject arrowPrefab; // 箭頭的預設
    private GameObject arrowInstance; // 儲存實例化的箭頭
    private GameObject playCardChoicePrefab;
    private GameObject playCardChoicePrefabInstance;
    private GameObject cardPropertyChoicePrefab;
    private GameObject cardPropertyChoicePrefabInstance;
    private GameObject cardInfoPrefab;
    private GameObject cardInfoPrefabInstance;
    // private TextMeshProUGUI cardInfoText;
    private bool isPlayerCard; // 判斷是否是玩家的牌


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        playCardChoicePrefab = gameManager.playCardChoicePrefab;
        cardPropertyChoicePrefab = gameManager.cardPropertyChoicePrefab;
        cardInfoPrefab = gameManager.cardInfoPrefab;
        arrowPrefab = gameManager.arrowPrefab;

    }

    // 初始化卡片數據
    public void Initialize(GameManager.Card card, bool isPlayer)
    {
        cardData = card;
        isPlayerCard = isPlayer; // 設置是否為玩家的牌
        cardButton = GetComponent<Button>();
        // cardButton.onClick.AddListener(OnCardClicked);

    }

    // public void OnCardClicked()
    // {
    //     Debug.Log($"Card clicked: {cardData}");
    //     // 調用遊戲管理器處理出牌邏輯
    //     gameManager.PlayCard(cardData, this.gameObject);
    // }

    // 揭示卡片內容
    public void RevealCard(Dictionary<GameManager.CardType, GameObject> cardPrefabs, Transform parent)
    {
        if (cardPrefabs.TryGetValue(cardData.type, out GameObject actualPrefab))
        {
            // 刪除當前的 closeCardPrefab
            Destroy(gameObject);

            // 創建實際的卡片對象
            GameObject revealedCard = Instantiate(actualPrefab, parent);
            // revealedCard.name = $"{cardData.type} {cardData.id}";
        }
        else
        {
            Debug.LogWarning($"No prefab found for card type: {cardData.type}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPlayerCard) return; // 如果不是玩家的牌，直接返回
        if (gameManager.isPlayedCard) return;
        if (turnManager.currentTurn == 4 || turnManager.currentTurn == 7 ||turnManager.currentTurn == 10)
        {
            if (turnManager.hasDrawCardForRound == false)
            {
                return;
            }
        }

        // 当鼠标进入卡牌时触发的逻辑
        Debug.Log("Mouse entered card!");
        // 在这里触发其他逻辑，例如显示卡牌的详细信息或启动动画
        // 实例化箭头并设置位置
     
        // arrowInstance = Instantiate(arrowPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        // arrowInstance.transform.SetParent(transform); // 设置箭头的父物体为卡牌
        // arrowInstance.transform.localPosition = new Vector3(0, 90, 0); // 调整箭头相对位置
        // arrowInstance.transform.localRotation = Quaternion.Euler(0, 0, -90); // 旋转45度
        // arrowInstance.transform.localScale = new Vector3(0.44f, 0.42f, 1f);
        
        playCardChoicePrefabInstance = Instantiate(playCardChoicePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        playCardChoicePrefabInstance.transform.SetParent(transform);
        playCardChoicePrefabInstance.transform.localPosition = new Vector3(50, 45, 0);
        playCardChoicePrefabInstance.transform.localScale = new Vector3(0.6f, 0.5f, 1f);
        // 添加按鈕功能
        Button playButton = playCardChoicePrefabInstance.GetComponent<Button>();
        if (playButton != null)
        {
            playButton.onClick.AddListener(() => OnPlayCardChoiceClicked());
        }

        cardPropertyChoicePrefabInstance = Instantiate(cardPropertyChoicePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        cardPropertyChoicePrefabInstance.transform.SetParent(transform);
        cardPropertyChoicePrefabInstance.transform.localPosition = new Vector3(-50, 45, 0); 
        cardPropertyChoicePrefabInstance.transform.localScale = new Vector3(0.6f, 0.5f, 1f);
        
  
        // 添加按鈕功能
        Button propertyButton = cardPropertyChoicePrefabInstance.GetComponent<Button>();
        if (propertyButton != null)
        {
            propertyButton.onClick.AddListener(() => OnCardPropertyChoiceClicked());
        }
    }

    private void OnPlayCardChoiceClicked()
    {
        Debug.Log("Play card choice clicked!");

        // 在這裡處理出牌邏輯
        if (cardData != null)
        {
            gameManager.PlayCard(cardData, this.gameObject);
        }
    }

    private void OnCardPropertyChoiceClicked()
    {
        Debug.Log("Card property choice clicked!");
        // 在這裡處理卡牌屬性邏輯
        if (cardInfoPrefabInstance != null)
        {
            Destroy(cardInfoPrefabInstance);
            cardInfoPrefabInstance = null;
        } else
        {
            cardInfoPrefabInstance = Instantiate(cardInfoPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            cardInfoPrefabInstance.transform.SetParent(transform);
            cardInfoPrefabInstance.transform.localPosition = new Vector3(0, 110, 0);
            cardInfoPrefabInstance.transform.localScale = new Vector3(1f, 1f, 1f);
        
            // 設置描述文字
            TMP_Text descriptionText = cardInfoPrefabInstance.GetComponentInChildren<TMP_Text>();
            if (descriptionText != null)
            {
                descriptionText.text = GetCardDescription(cardData.type);

            }
        }
      
    }

    public string GetCardDescription(GameManager.CardType type)
    {
        switch (type)
        {
            case GameManager.CardType.stoneCard:
                return "這是一張石頭牌，可以用來砸碎對手的剪刀！";
            case GameManager.CardType.scissorsCard:
                return "這是一張剪刀牌，用來剪破對手的布。";
            case GameManager.CardType.paperCard:
                return "這是一張布牌，可以包住對手的石頭。";
            case GameManager.CardType.middleFingerCard:
                return "這是一張中指牌，會激怒你的對手！可以吃掉除了指槍牌以外的牌。";
            case GameManager.CardType.actuallyScissorsCard:
                return "表面上是布牌，但其實是剪刀牌，驚喜！可以吃掉除了指槍牌、中指牌、布牌以外的牌。";
            case GameManager.CardType.loveCard:
                return "這是一張愛心牌，傳遞愛與和平！抵禦每一個攻擊除了指槍牌。";
            case GameManager.CardType.fingerGunCard:
                return "這是一張指槍牌，無視每一個防守並且不受到攻擊，可以無條件攻擊對手一次。";
            case GameManager.CardType.lieCard:
                return "這是一張騙人牌，讓對手陷入困惑，抵禦石頭牌、布牌、剪刀牌。";
            default:
                return "未知卡牌類型。";
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPlayerCard) return; // 如果不是玩家的牌，直接返回

        // 当鼠标退出卡牌时触发的逻辑
        Debug.Log("Mouse exited card!");
        // 在这里处理鼠标离开时的逻辑
        // 销毁箭头
        // Destroy(arrowInstance);
        // arrowInstance = null;
        Destroy(playCardChoicePrefabInstance);
        playCardChoicePrefabInstance = null;
        Destroy(cardPropertyChoicePrefabInstance);
        cardPropertyChoicePrefabInstance = null;
        Destroy(cardInfoPrefabInstance);
        cardInfoPrefabInstance = null;
    }

    // 用來返回卡片資料的方法
    public GameManager.Card GetCard()
    {
        return cardData;
    }
}
