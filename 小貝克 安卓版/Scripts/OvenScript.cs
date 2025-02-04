using TMPro;
using UnityEngine;
using System.Collections.Generic; 
using System.Collections;
using UnityEngine.UI;



public class OvenScript : MonoBehaviour
{
    public Image[] foodImages = new Image[9];  // 用於顯示9個蛋糕圖片的陣列
    public TextMeshProUGUI[] ovenFoodNames = new TextMeshProUGUI[9];  // 用於顯示9個蛋糕名稱的陣列
    public TextMeshProUGUI[] foodCountTexts = new TextMeshProUGUI[9]; // 用於顯示蛋糕數量的陣列
    public Image[] foodNameDialog = new Image[9];
    public Image[] countDialog = new Image[9];  

    //食物袋
    public GameObject[] foodBags = new GameObject[9];  
    public GameObject[] foodBagsDialog = new GameObject[9];  
    public GameObject[] foodBagsFoodImage = new GameObject[9];  
    
    private MakeCake makeCake;
    private MakeDonut makeDonut;
    private MakeBread makeBread;
    private MakeCupCake makeCupCake;
    private MakeStyleBread makeStyleBread;
    public GameObject outSpaceNotice;
    private int currentFoodBagIndex = 0;
    public FoodManager foodManagerScript;
    public TextMeshProUGUI completeNoticeText;
    private Vector3[] initialPositions;
    public bool isMaking = false;




    void Start()
    {
        GameObject foodManagerObject = GameObject.Find("FoodManager");
        if (foodManagerObject != null)
        {
            foodManagerScript = foodManagerObject.GetComponent<FoodManager>();
        }
        GameObject makeCakeObject = GameObject.Find("MakeCakeScript");
        if (makeCakeObject != null)
        {
            makeCake = makeCakeObject.GetComponent<MakeCake>();
        }
        GameObject makeDonutObject = GameObject.Find("MakeDonutScript");
        if (makeDonutObject != null)
        {
            makeDonut = makeDonutObject.GetComponent<MakeDonut>();
        }
        GameObject makeBreadObject = GameObject.Find("MakeBreadScript");
        if (makeBreadObject != null)
        {
            makeBread = makeBreadObject.GetComponent<MakeBread>();
        }
        GameObject makeCupCakeObject = GameObject.Find("MakeCupCakeScript");
        if (makeCupCakeObject != null)
        {
            makeCupCake = makeCupCakeObject.GetComponent<MakeCupCake>();
        }
        GameObject makeStyleBreadObject = GameObject.Find("MakeStyleBreadScript");
        if (makeStyleBreadObject != null)
        {
            makeStyleBread = makeStyleBreadObject.GetComponent<MakeStyleBread>();
        }
      
        outSpaceNotice.SetActive(false);

        // 初始化食物袋
        for (int i = 0; i < foodBags.Length; i++)
        {
            foodBags[i].SetActive(false);
            foodBagsDialog[i].SetActive(false);
            foodBagsFoodImage[i].SetActive(false);

        }

        // 初始化每個蛋糕圖片和數量文字
        for (int i = 0; i < foodImages.Length; i++)
        {
            foodImages[i].enabled = false; // 初始時隱藏所有圖片
            foodNameDialog[i].enabled = false;
            countDialog[i].enabled = false;
            ovenFoodNames[i].text = "";    // 初始時不顯示名稱
            foodCountTexts[i].text = "";   // 初始時不顯示數量

            int index = i;
            foodImages[i].GetComponent<Button>().onClick.AddListener(() => OnCakeImageClicked(index));
        }

        initialPositions = new Vector3[foodBags.Length];
        for (int i = 0; i < foodBags.Length; i++)
        {
            initialPositions[i] = foodBags[i].transform.position; // 存储初始位置
        }
    }

    void OnCakeImageClicked(int index)
    {
        packageFood(index);
    }

    void Update()
    {
        
    }

    public void UpdateOvenPanel(Sprite foodSprite,  string foodName, int foodCount, int cakeId, FoodType foodType)
    {
        bool cakeFound = false;

        for (int i =0; i < foodImages.Length; i++)
        {
            if (ovenFoodNames[i].text == foodName)
            {
                // 更新該蛋糕的數量
                foodCountTexts[i].text = foodCount + "x";
                cakeFound = true;
                return;
            }
        }

        // 找到第一個空的空間來放蛋糕
        for (int i = 0; i < foodImages.Length; i++)
        {
            if (!foodImages[i].enabled)  // 檢查是否有空的蛋糕空間
            {
                foodImages[i].sprite = foodSprite;  // 設定蛋糕圖片
                foodImages[i].enabled = true;       // 顯示蛋糕圖片
                foodNameDialog[i].enabled = true;
                countDialog[i].enabled = true;
                ovenFoodNames[i].text = foodName;   // 設定蛋糕名稱
                foodCountTexts[i].text = foodCount + "x";  // 設定蛋糕數量
                
                foodImages[i].GetComponent<CakeData>().cakeId = cakeId; 
                foodImages[i].GetComponent<CakeData>().foodType = foodType;
                
                break;  // 一旦找到空位並更新後就跳出迴圈
            }
        }
    }

    public void packageFood(int index)
    {
        // 先檢查是否所有食物袋都已經被使用
        if (currentFoodBagIndex >= foodBags.Length)
        {
            outSpaceNotice.SetActive(true);
            StartCoroutine(HideOutSpaceNotice(3));
            return; // 如果食物袋已滿，則返回
        }
        
        
        // 如果該蛋糕圖片沒有被啟用，直接返回
        if (!foodImages[index].enabled)
        {
            return;
        }
        FoodType foodType = foodImages[index].GetComponent<CakeData>().foodType;


        Sprite currentFoodSprite = foodImages[index].sprite;
        int cakeId = foodImages[index].GetComponent<CakeData>().cakeId; // 獲取蛋糕 ID
        
        // 取得當前蛋糕的數量
        string countText = foodCountTexts[index].text.Replace("x", "");
        int count = int.Parse(countText);

        // 減少蛋糕數量
        count--;

        // 更新字典中的食物數量
        string foodName = ovenFoodNames[index].text;

        // if (makeCake.cakeCompletionCounts.ContainsKey(foodName))
        // {
        //     makeCake.cakeCompletionCounts[foodName]--;
        // }

        // 根據食物類型減少對應的完成計數
        switch (foodType)
        {
            case FoodType.Cake:
                if (makeCake.cakeCompletionCounts.ContainsKey(foodName))
                {
                    makeCake.cakeCompletionCounts[foodName]--;
                }
                break;
            case FoodType.Donut:
                if (makeDonut.donutCompletionCounts.ContainsKey(foodName))
                {
                    makeDonut.donutCompletionCounts[foodName]--;
                }
                break;
            case FoodType.Bread:
                if (makeBread.breadCompletionCounts.ContainsKey(foodName))
                {
                    makeBread.breadCompletionCounts[foodName]--;
                }
                break;
            case FoodType.CupCake:
                if (makeCupCake.cupCakeCompletionCounts.ContainsKey(foodName))
                {
                    makeCupCake.cupCakeCompletionCounts[foodName]--;
                }
                break;
            case FoodType.StyleBread:
                if (makeStyleBread.styleBreadCompletionCounts.ContainsKey(foodName))
                {
                    makeStyleBread.styleBreadCompletionCounts[foodName]--;
                }
                break;
            // 添加其他食物類型的處理
        }
        
    

        //減少completedFoods數量
        foodManagerScript.MinusToCompletedFoods(1);
        completeNoticeText.text = foodManagerScript.completedFoods + "x";

        if (count > 0)
        {
            foodCountTexts[index].text = count + "x";
        }
        else
        {
            // 如果蛋糕數量為0，隱藏蛋糕圖片和相關信息
            foodImages[index].enabled = false;
            foodNameDialog[index].enabled = false;
            countDialog[index].enabled = false;
            ovenFoodNames[index].text = "";
            foodCountTexts[index].text = "";

            

            // 將後面的蛋糕信息往前移動，填補空缺
            for (int i = index; i < foodImages.Length - 1; i++)
            {
                foodImages[i].sprite = foodImages[i + 1].sprite;
                foodImages[i].enabled = foodImages[i + 1].enabled;
                foodNameDialog[i].enabled = foodNameDialog[i + 1].enabled;
                countDialog[i].enabled = countDialog[i + 1].enabled;
                ovenFoodNames[i].text = ovenFoodNames[i + 1].text;
                foodCountTexts[i].text = foodCountTexts[i + 1].text;
            }

            // 清空最後一個位置的信息
            int lastIndex = foodImages.Length - 1;
            foodImages[lastIndex].enabled = false;
            foodNameDialog[lastIndex].enabled = false;
            countDialog[lastIndex].enabled = false;
            ovenFoodNames[lastIndex].text = "";
            foodCountTexts[lastIndex].text = "";
        }       

        ShowFoodBag(currentFoodSprite, cakeId, foodType);
       
    }
 
    private void ShowFoodBag(Sprite foodSprite, int cakeId, FoodType foodType)
    {
        bool foundResetBag = false;
        for (int i = 0; i < foodBags.Length; i++)
        {
            CakeData cakeData = foodBags[i].GetComponent<CakeData>();
            if (cakeData != null && cakeData.cakeId == -1) // 檢查已重置的食物袋
            {
                currentFoodBagIndex = i; // 使用已重置的食物袋
                foundResetBag = true;
                break; // 找到一個已重置的食物袋後停止尋找
            }
        }

        // Activate the new food bag and its related objects
        foodBags[currentFoodBagIndex].SetActive(true);
        foodBagsDialog[currentFoodBagIndex].SetActive(true);
        foodBagsFoodImage[currentFoodBagIndex].SetActive(true);

        // Set the food image in the food bag
        SpriteRenderer foodBagSpriteRenderer = foodBagsFoodImage[currentFoodBagIndex].GetComponent<SpriteRenderer>();
        if (foodBagSpriteRenderer != null)
        {
            foodBagSpriteRenderer.sprite = foodSprite;


            // Assuming the dialog is also a SpriteRenderer, we can calculate the bounds
            SpriteRenderer dialogSpriteRenderer = foodBagsDialog[currentFoodBagIndex].GetComponent<SpriteRenderer>();
            if (dialogSpriteRenderer != null)
            {
                Bounds dialogBounds = dialogSpriteRenderer.bounds;
                Bounds foodBounds = foodBagSpriteRenderer.sprite.bounds;

                // Calculate the scale factor to fit the food image inside the dialog
                float scaleFactor = Mathf.Min(
                    dialogBounds.size.x / foodBounds.size.x, 
                    dialogBounds.size.y / foodBounds.size.y
                ) * 0.7f;

                // Apply the scale to the food image
                foodBagsFoodImage[currentFoodBagIndex].transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            }
        }

        CakeData cakeDataNew = foodBags[currentFoodBagIndex].GetComponent<CakeData>();
        if (cakeDataNew != null)
        {
            cakeDataNew.cakeId = cakeId;
            cakeDataNew.foodType = foodType;  // Add this line to assign the foodType
        }

        //DragDrop腳本Id
        DragDrop dragDrop = foodBags[currentFoodBagIndex].GetComponent<DragDrop>();
        if (dragDrop != null)
        {
            dragDrop.foodBagId = cakeId;
        }

        currentFoodBagIndex++;
    }

    IEnumerator HideOutSpaceNotice(float delay)
    {
        yield return new WaitForSeconds(delay);
        outSpaceNotice.SetActive(false); // 隱藏通知
    }




    public void ResetFoodBag(int bagId)
    {
        // 找到對應的食物袋索引
        for (int i = 0; i < foodBags.Length; i++)
        {
            // 檢查食物袋的 ID
            CakeData cakeData = foodBags[i].GetComponent<CakeData>();
            DragDrop dragDrop = foodBags[i].GetComponent<DragDrop>();
            if (cakeData != null && cakeData.cakeId == bagId && dragDrop.isBeingDragged)
            {
                // 重置 ID 和圖片
                cakeData.cakeId = -1; // 或者您想要的重置值
                foodBagsFoodImage[i].GetComponent<SpriteRenderer>().sprite = null; // 重置圖片
                foodBags[i].SetActive(false); // 設置為不活動
                foodBags[i].transform.position = initialPositions[i];

                
                // 如果需要，可以重置拖放腳本的 ID
                // DragDrop dragDrop = foodBags[i].GetComponent<DragDrop>();
                // if (dragDrop != null)
                // {
                //     dragDrop.foodBagId = -1; // 重置拖放 ID
                // }
                break; // 一旦找到並重置就退出
            }
        }
    }

    //初始化食物袋和烤箱食物方法
    public void ResetFood()
    {
        // 初始化食物袋
        for (int i = 0; i < foodBags.Length; i++)
        {
            foodBags[i].SetActive(false);
            foodBagsDialog[i].SetActive(false);
            foodBagsFoodImage[i].SetActive(false);

        }

        // 初始化每個蛋糕圖片和數量文字
        for (int i = 0; i < foodImages.Length; i++)
        {
            foodImages[i].enabled = false; // 初始時隱藏所有圖片
            foodNameDialog[i].enabled = false;
            countDialog[i].enabled = false;
            ovenFoodNames[i].text = "";    // 初始時不顯示名稱
            foodCountTexts[i].text = "";   // 初始時不顯示數量

            int index = i;
            foodImages[i].GetComponent<Button>().onClick.AddListener(() => OnCakeImageClicked(index));
        }
    }

    
}
