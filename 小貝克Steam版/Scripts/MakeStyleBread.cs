using System.Collections;
using System.Collections.Generic; 
using TMPro;
using UnityEngine;

public class MakeStyleBread : MonoBehaviour
{
    public TextMeshProUGUI makeText;
    public Animator OvenAnim;
    private OvenAnim ovenAnimScript;
    public GameObject CompleteNotice;
    public TextMeshProUGUI CompleteNoticeText;
    public GameObject SmokeParticle;
    public GameObject RedLight;

    public int completedStyleBreads = 0; // 用來追蹤已完成的蛋糕數量
    public bool isAnimationPlaying = false;

    public Dictionary<string, int> styleBreadCompletionCounts = new Dictionary<string, int>();


    // 定義蛋糕的結構體，包含名稱和對應的圖片
    [System.Serializable]
    public struct StyleBread
    {
        public string name;
        public Sprite sprite;
        public int id;
        // public FoodType type;
    }

    public StyleBread[] styleBreads;  // 用於存放多種蛋糕的數組

    private int selectedStyleBreadIndex = 0;  // 當前選擇的蛋糕索引
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

        foreach (StyleBread styleBread in styleBreads)
        {
            styleBreadCompletionCounts[styleBread.name] = 0; // 初始化每個蛋糕類型的完成數量為0
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
        CompleteStyleBread(foodType);
    }
 
    // 蛋糕製作完成，更新蛋糕數量和通知
    private void CompleteStyleBread(FoodType foodType)
    {
        string styleBreadName = styleBreads[selectedStyleBreadIndex].name;
        int styleBreadId = styleBreads[selectedStyleBreadIndex].id;
        styleBreadCompletionCounts[styleBreadName]++;
        Debug.Log("做了一個蛋糕");
        // 計算所有完成的蛋糕總數
        foodManagerScript.AddToCompletedFoods(1); // 計算所有完成的蛋糕總數
        CompleteNoticeText.text = foodManagerScript.completedFoods + "x";
        CompleteNotice.SetActive(true); // 顯示完成通知
        ovenScript.UpdateOvenPanel(styleBreads[selectedStyleBreadIndex].sprite, styleBreads[selectedStyleBreadIndex].name, styleBreadCompletionCounts[styleBreadName], styleBreadId, foodType);
        ovenScript.isMaking = false;
        

    }

 


    IEnumerator OvenUsingText()
    {
        makeText.text = "Wait a sec!";
        yield return new WaitForSeconds(1.5f);
        makeText.text = "";
    }


    //製作造型麵包方法區域
    public void MakeFishStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 0;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeMatchaFishStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 1;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeCrossStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 2;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeRabbitStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 3;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeElephantStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 4;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeBearStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 5;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakePandaStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 6;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeCatStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 7;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeOctopusStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 8;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeChickenStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 9;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeHappyCatStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 10;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeHappyDogStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 11;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeFishTwoStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 12;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakePigStyleBread()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.StyleBread);
            selectedStyleBreadIndex = 13;
            StartCoroutine(MakeTextRoutine(3));
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }



  
}

