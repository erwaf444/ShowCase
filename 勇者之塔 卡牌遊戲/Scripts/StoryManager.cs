using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class StoryManager : MonoBehaviour
{
    public GameObject mask;
    public Image[] FadeInImage;
    public TextMeshProUGUI[] FadeInText; // 用於漸入的文本陣列
    public float fadeDuration = 2.0f; // 漸入持續時間（秒）

    private bool isFadingIn = false;
    private float fadeTimer = 0f;

    public GameObject storyArea; //故事區域
    public GameObject itemArea; //物品區域
    public GameObject skillArea; //技能區域
    public GameObject saveArea; //儲存區域
    public GameObject settingArea; //設定區域
    private GameObject[] areas; // 用於存放所有區域
    private int currentAreaIndex = 0; // 當前區域索引
    public GameObject[] tags; 

    // Tags 對應的 UI 元素
    public RectTransform[] areaTags; // 每個區域對應一個 Tag (需要有 RectTransform)
    public float tagScaleDuration = 0.3f; // 放大縮小效果持續時間
    public Vector3 normalScale = new Vector3(0.95f, 0.8f, 1); // 正常大小
    public Vector3 enlargedScale =  new Vector3(0.95f, 0.8f, 1) * 1.2f; // 放大大小
    private bool isScalingTag = false;
    private float scaleTimer = 0f;

    #region 故事屬性
    //故事區域屬性
    [System.Serializable]
    public class Story
    {
        public int id;
        public string storyContent;
        public string towerContent;
    }

    [System.Serializable]
    public class StoryData
    {
        public Story[] stories;
    }
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI towerText;
    private StoryData storyData;
    public int currentLevel = 1; // 記錄當前塔層（1到101）
    #endregion

    #region 裝備屬性
    public enum ItemType { Material, Equipment }
    [System.Serializable]
    public class Recipe
    {
        public List<Item> requiredItems; // 需要的材料
        public Item resultItem; // 合成後的物品
    }
    [System.Serializable]
    public class Item
    {
        public string itemName; // 物品名稱
        public Sprite itemSprite;  // 物品圖片
        public string description;   // 物品描述（可選）
        public ItemType itemType;   // 物品類型（材料/裝備）

    }
    [Header("裝備屬性")]
    //右半頁
    public List<Image> pageIndicators; // 小圈圈的 Image 組
    public Color activeColor = Color.black; // 當前頁面的小圈圈顏色
    public Color inactiveColor = Color.white; // 非當前頁面的小圈圈顏色
    public GameObject itemPrefab; // 物品的預製體
    public Transform contentParent; // ScrollView Content 的父物件
    public Button nextPageButton; // 下一頁按鈕
    public Button previousPageButton; // 上一頁按鈕
    public int itemsPerPage = 16; // 每頁顯示的物品數量
    public List<Item> inventoryItems = new List<Item>(); // 所有物品的列表
    private int currentPage = 0; // 當前頁面

    //左半頁
    public Image itemSpriteImage;  // 改為 UI 圖像 (Image)
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemNameInDescription;
    public TextMeshProUGUI itemDescription;
    #endregion

    #region 商店
    public GameObject shopPanel;
    public Transform shopPanelTransform;
    public List<Item> shopItemPrefabs = new List<Item>(); // 存放所有可能的物品預製體
    public int numberOfItemsToGenerate = 5; // 要生成的物品數量
    #endregion


    #region 設定屬性 
    public GameObject audioPart;
    public GameObject videoPart;
    #endregion

    void Start()
    {
        LoadCurrentLevel();
        // 初始化區域陣列
        areas = new GameObject[] { storyArea, itemArea, skillArea, saveArea, settingArea };

        // 確保只有第一個區域啟用，其他區域關閉
        for (int i = 0; i < areas.Length; i++)
        {
            areas[i].SetActive(i == currentAreaIndex);
        }

        // 將所有 Image 的初始透明度設為 0
        foreach (var image in FadeInImage)
        {
            if (image != null)
            {
                SetAlpha(image, 0f);
            }
        }

        // 將所有 TextMeshProUGUI 的初始透明度設為 0
        foreach (var text in FadeInText)
        {
            if (text != null)
            {
                SetTextAlpha(text, 0f);
            }
        }

        StartFadeIn(); // 啟動漸入效果

        StartTagScaling(areaTags[0], enlargedScale);

        LoadStories();
        UpdateStoryText();


        #region 裝備Start方法
        // 初始化測試數據
        // for (int i = 0; i < 50; i++)
        // {
        //     inventoryItems.Add(new Item { 
        //         itemName = "Item " + (i + 1),
        //     });
        // }

        // 設置按鈕
        nextPageButton.onClick.AddListener(() => ChangePage(1));
        previousPageButton.onClick.AddListener(() => ChangePage(-1));

        // 更新顯示
        UpdateInventoryUI();
        UpdatePageIndicator(0);
        #endregion

        #region 商店Start方法
        GenerateRandomItems();
        #endregion
    }

    void Update()
    {
        if (isFadingIn)
        {
            FadeIn();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwitchArea();
        }

        // 處理 Tags 的放大縮小效果
        if (isScalingTag)
        {
            ScaleTagEffect();
        }
    }
       
    
    public void StartFadeIn()
    {
        isFadingIn = true;
        fadeTimer = 0f;
    }

    private void FadeIn()
    {
        fadeTimer += Time.deltaTime;
        float progress = fadeTimer / fadeDuration;
        float newAlpha = Mathf.Clamp01(progress); // 計算透明度

        foreach (var image in FadeInImage)
        {
            if (image != null)
            {
                SetAlpha(image, newAlpha);
            }
        }

        // 處理 TextMeshProUGUI 的透明度
        foreach (var text in FadeInText)
        {
            if (text != null)
            {
                SetTextAlpha(text, newAlpha);
            }
        }

        if (progress >= 1f)
        {
            isFadingIn = false; // 停止漸入
        }
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color newColor = image.color;
        newColor.a = alpha; // 設置透明度
        image.color = newColor;
    }

    // 設置文本的透明度
    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }


    private void SwitchArea()
    {
        // 關閉當前區域
        areas[currentAreaIndex].SetActive(false);

        // 更新到下一個區域
        currentAreaIndex = (currentAreaIndex + 1) % areas.Length;

        // 啟用新區域
        areas[currentAreaIndex].SetActive(true);

        // 處理標籤縮放
        for (int i = 0; i < areaTags.Length; i++)
        {
            if (areaTags[i] != null)
            {
                if (i == currentAreaIndex)
                {
                    // 放大選中的區域標籤
                    StartTagScaling(areaTags[i], enlargedScale);
                }
                else
                {
                    // 縮小未選中的區域標籤
                    StartTagScaling(areaTags[i], normalScale);
                }
            }
        }

        Debug.Log("切換到區域：" + areas[currentAreaIndex].name);
    }

    private void StartTagScaling(RectTransform tag, Vector3 targetScale)
    {
        isScalingTag = true;
        scaleTimer = 0f;

        // 設置目標縮放
        StartCoroutine(ScaleTag(tag, targetScale));
    }

    private void ScaleTagEffect()
    {
        scaleTimer += Time.deltaTime;

        if (scaleTimer >= tagScaleDuration)
        {
            isScalingTag = false; // 停止縮放
        }
    }

    private IEnumerator ScaleTag(RectTransform tag, Vector3 targetScale)
    {
        Vector3 initialScale = tag.localScale;
        float elapsed = 0f;

        while (elapsed < tagScaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / tagScaleDuration;
            tag.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }

        tag.localScale = targetScale;
    }

    #region 故事區域
    void LoadStories()
    {
        // 不需要直接操作路徑，只需指定檔名（不含副檔名）
        TextAsset jsonFile = Resources.Load<TextAsset>("StoryTexts");

        if (jsonFile != null)
        {
            string json = jsonFile.text;
            storyData = JsonUtility.FromJson<StoryData>(json);
        }
        else
        {
            Debug.LogError("StoryTexts.json not found in Resources folder!");
        }
    }


    void UpdateStoryText()
    {
        if (storyData != null && currentLevel <= storyData.stories.Length)
        {
            Story currentStory = storyData.stories[currentLevel - 1];
            storyText.text = currentStory.storyContent; // 顯示故事內容
            towerText.text = currentStory.towerContent; 
        }
        else
        {
            storyText.text = "No story available.";
            towerText.text ="No tower available.";
        }
    }

    // public void AdvanceLevel()
    // {
    //     if (currentLevel < storyData.stories.Length)
    //     {
    //         currentLevel++;
    //         UpdateStoryText();
    //     }
    //     else
    //     {
    //         Debug.Log("You have reached the top!");
    //     }
    // }

  
    #endregion

    #region 裝備區域
    void UpdateInventoryUI()
    {
        // 清空當前頁面的內容
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 計算當前頁面要顯示的物品範圍
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, inventoryItems.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentParent);
            // newItem.GetComponentInChildren<Text>().text = inventoryItems[i].itemName; // 假設物品有 Text
            
            // 設置圖片
            Image itemImage = newItem.GetComponentInChildren<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = inventoryItems[i].itemSprite;
            }

            // 設置名稱
            Text itemText = newItem.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = inventoryItems[i].itemName;
            }
            Button itemButton = newItem.GetComponent<Button>();
            if (itemButton != null)
            {
                // 使用閉包捕獲正確的索引
                int capturedIndex = i;
                itemButton.onClick.AddListener(() => OnItemClicked(capturedIndex));
            }
        }

        // 更新按鈕狀態
        previousPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = endIndex < inventoryItems.Count;
    }

    // 點選物品的處理方法
    void OnItemClicked(int itemIndex)
    {
        Debug.Log("選中物品: " + inventoryItems[itemIndex].itemName);

        itemSpriteImage.sprite = inventoryItems[itemIndex].itemSprite; // 設置為所選物品的圖片
        itemName.text = inventoryItems[itemIndex].itemName;
        itemNameInDescription.text = inventoryItems[itemIndex].itemName;
        itemDescription.text = inventoryItems[itemIndex].description;
        // Image itemImage = itemPrefab.GetComponentInChildren<Image>();
        // Debug.Log(itemImage);
        // if (itemImage != null)
        // {
        //     Debug.Log("itemImage.sprite: " + itemImage.sprite);
        //     itemSpriteImage.sprite = itemImage.sprite;
        //     Debug.Log("itemSpriteImage.sprite: " + itemSpriteImage.sprite);
        // }
        // else
        // {
        //     Debug.LogError("ItemPrefab 上找不到 Image 組件！");
        // }
    }

    void ChangePage(int direction)
    {
        currentPage += direction;
        UpdateInventoryUI();
        UpdatePageIndicator(currentPage);
    }

    // 更新頁面指示器
    public void UpdatePageIndicator(int pageIndex)
    {
        currentPage = pageIndex;

        for (int i = 0; i < pageIndicators.Count; i++)
        {
            if (i == currentPage)
            {
                pageIndicators[i].color = activeColor; // 當前頁面
            }
            else
            {
                pageIndicators[i].color = inactiveColor; // 非當前頁面
            }
        }
    }
    #endregion

    #region 商店
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        mask.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        mask.SetActive(false);
    }

    void GenerateRandomItems()
    {
        // 清空現有的物品
        // foreach (Transform child in shopPanelTransform)
        // {
        //     Destroy(child.gameObject);
        // }

        // 隨機生成物品
        for (int i = 0; i < numberOfItemsToGenerate; i++)
        {
            if (shopItemPrefabs.Count == 0) break; // 如果沒有物品可生成，則退出

            // 隨機選擇一個物品
            int randomIndex = Random.Range(0, shopItemPrefabs.Count);
            Item randomItem = shopItemPrefabs[randomIndex];

            // 實例化物品 UI
            GameObject itemUI = Instantiate(itemPrefab, shopPanelTransform);
            itemUI.name = randomItem.itemName;

            // 設置物品圖片
            Image itemImage = itemUI.GetComponentInChildren<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = randomItem.itemSprite;
            }

            // 設置物品名稱和描述
            Text itemText = itemUI.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = $"{randomItem.itemName}\n{randomItem.description}";
            }

            // 避免重複生成（可選）
            // shopItemPrefabs.RemoveAt(randomIndex);
        }
    }

    #endregion

    # region 技能區域
        
    #endregion

    # region 存檔區域
        
    #endregion

    # region 設定區域
    public void OpenAudioPart()
    {
        audioPart.SetActive(true);
        videoPart.SetActive(false);
    }    

    public void OpenVideoPart()
    {
        audioPart.SetActive(false);
        videoPart.SetActive(true);
    }   
    
    #endregion

    #region steamAPI
    
    public static void SaveCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("currentLevel", level);
        PlayerPrefs.Save(); // 確保資料存儲
        Debug.Log("Level saved: " + level);
    }

    public int LoadCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        Debug.Log("Loaded level: " + currentLevel);
        return currentLevel;
    }

    #endregion
    
}
