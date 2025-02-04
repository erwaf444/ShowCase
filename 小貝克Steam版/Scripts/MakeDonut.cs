using System.Collections;
using System.Collections.Generic; 
using TMPro;
using UnityEngine;

public class MakeDonut : MonoBehaviour
{
    public TextMeshProUGUI makeText;
    // private bool isMaking = false;
    public Animator OvenAnim;
    private OvenAnim ovenAnimScript;
    public GameObject CompleteNotice;
    public TextMeshProUGUI CompleteNoticeText;
    public GameObject SmokeParticle;
    public GameObject RedLight;

    public int completedDonuts = 0; // 用來追蹤已完成的蛋糕數量
    public bool isAnimationPlaying = false;

    public Dictionary<string, int> donutCompletionCounts = new Dictionary<string, int>();

    // 定義蛋糕的結構體，包含名稱和對應的圖片
    [System.Serializable]
    public struct Donut
    {
        public string name;
        public Sprite sprite;
        public int id;
        // public FoodType type;
    }

    public Donut[] donuts;  // 用於存放多種蛋糕的數組

    private int selectedDonutIndex = 0;  // 當前選擇的蛋糕索引
    private OvenScript ovenScript;
    private FoodManager foodManagerScript;
    private GameManager gameManagerScript;

    




    void Start()    
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        }
        GameObject foodManagerObject = GameObject.Find("FoodManager");
        if (foodManagerObject != null)
        {
            foodManagerScript = foodManagerObject.GetComponent<FoodManager>();
        }
        GameObject ovenScriptObject = GameObject.Find("OvenPanel");
        if (ovenScriptObject != null)
        {
            ovenScript = ovenScriptObject.GetComponent<OvenScript>();
        }
        GameObject ovenAnimScriptObject = GameObject.Find("Oven");
        if (ovenAnimScriptObject != null)
        {
            ovenAnimScript = ovenAnimScriptObject.GetComponent<OvenAnim>();
        }

        makeText.text = "";
        CompleteNotice.SetActive(false);
        SmokeParticle.SetActive(false);
        RedLight.SetActive(false);

        foreach (Donut donut in donuts)
        {
            donutCompletionCounts[donut.name] = 0; // 初始化每個蛋糕類型的完成數量為0
        }
    }
 
    void Update()
    {
        if (CompleteNoticeText.text == "0x")
        {
            CompleteNotice.SetActive(false);
        }
    }

    // Coroutine to show "製作中..." with an animation
    IEnumerator MakeTextRoutine(int time)
    {
        ovenScript.isMaking = true;
        float timer = 0f;
        string baseText = "Makeing";
        int dotCount = 0; // Start with 0 dots

        while (timer < time)
        {
            // Update the text with "製作中" followed by dotCount number of dots
            makeText.text = baseText + new string('.', dotCount);
            
            // Increase dot count and reset to 0 after reaching 3
            dotCount = (dotCount + 1) % 4;

            // Wait for 0.5 seconds before updating again
            yield return new WaitForSeconds(0.5f);

            // Update the timer
            timer += 0.5f;
        }

        makeText.text = "Baking!"; // Clear the text after completion
        yield return new WaitForSeconds(2f);
        makeText.text = "";
        yield return new WaitForSeconds(1f);
        TriggerOvenAnimation();

    }

    public void TriggerOvenAnimation()
    {
        OvenAnim.SetTrigger("OvenAnim");
        SmokeParticle.SetActive(true);
        RedLight.SetActive(true);
        isAnimationPlaying = true;
    }

    public void OvenAnimationComplete(FoodType foodType)
    {
        CompleteDonut(foodType);
    }

    // 蛋糕製作完成，更新蛋糕數量和通知
    private void CompleteDonut(FoodType foodType)
    {
        string donutName = donuts[selectedDonutIndex].name;
        int donutId = donuts[selectedDonutIndex].id;
        donutCompletionCounts[donutName]++;
        Debug.Log("做了一個甜甜圈");
        // 計算所有完成的蛋糕總數
        foodManagerScript.AddToCompletedFoods(1); // 計算所有完成的蛋糕總數
        CompleteNoticeText.text = foodManagerScript.completedFoods + "x";
        CompleteNotice.SetActive(true); // 顯示完成通知
        ovenScript.UpdateOvenPanel(donuts[selectedDonutIndex].sprite, donuts[selectedDonutIndex].name, donutCompletionCounts[donutName], donutId, foodType);
        ovenScript.isMaking = false;
 
    }

 

    IEnumerator OvenUsingText()
    {
        makeText.text = "Wait a sec!";
        yield return new WaitForSeconds(1.5f);
        makeText.text = "";
    }


    //製作甜甜圈區域
    public void MakeDonutt()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 0;
            StartCoroutine(MakeTextRoutine(3)); // 3 seconds for this cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 1;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 2;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryChocolateNormalDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 3;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryWhiteChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 4;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryYellowChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 5;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryPinkSugarChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 6;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryWhiteSugarChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 7;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryGreenSugarChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 8;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryBlueChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 9;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryPurpleChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 10;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryOrangeChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 11;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryPinkChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 12;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeLuxuryRedChocolateDonut()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Donut);
            selectedDonutIndex = 13;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

}
