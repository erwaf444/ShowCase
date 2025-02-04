using System.Collections;
using System.Collections.Generic; 
using TMPro;
using UnityEngine;
 
public class MakeCake : MonoBehaviour
{
    public TextMeshProUGUI makeText;
    // private bool isMaking = false;
    public Animator OvenAnim;
    private OvenAnim ovenAnimScript;
    public GameObject CompleteNotice;
    public TextMeshProUGUI CompleteNoticeText;
    public GameObject SmokeParticle;
    public GameObject RedLight;

    public int completedCakes = 0; // 用來追蹤已完成的蛋糕數量
    public bool isAnimationPlaying = false;

    public Dictionary<string, int> cakeCompletionCounts = new Dictionary<string, int>();


    // public Sprite cakeSprite;
    // public string cakeName; // 用來追蹤當前製作蛋糕的名稱

    // 定義蛋糕的結構體，包含名稱和對應的圖片
    [System.Serializable]
    public struct Cake
    {
        public string name;
        public Sprite sprite;
        public int id;
        // public FoodType type;
    }

    public Cake[] cakes;  // 用於存放多種蛋糕的數組

    private int selectedCakeIndex = 0;  // 當前選擇的蛋糕索引
    private OvenScript ovenScript;
    private FoodManager foodManagerScript;

    public GameManager gameManagerScript;
    public bool isMakingCake = false;




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

        foreach (Cake cake in cakes)
        {
            cakeCompletionCounts[cake.name] = 0; // 初始化每個蛋糕類型的完成數量為0
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
        CompleteCake(foodType);
    }
 
    // 蛋糕製作完成，更新蛋糕數量和通知
    private void CompleteCake(FoodType foodType)
    {
        Debug.Log(selectedCakeIndex);
        string cakeName = cakes[selectedCakeIndex].name;
        int cakeId = cakes[selectedCakeIndex].id;
        cakeCompletionCounts[cakeName]++;
        Debug.Log("做了一個蛋糕");
        // 計算所有完成的蛋糕總數
        foodManagerScript.AddToCompletedFoods(1); // 計算所有完成的蛋糕總數
        CompleteNoticeText.text = foodManagerScript.completedFoods + "x";
        CompleteNotice.SetActive(true); // 顯示完成通知
        ovenScript.UpdateOvenPanel(cakes[selectedCakeIndex].sprite, cakes[selectedCakeIndex].name, cakeCompletionCounts[cakeName], cakeId, foodType);
        Debug.Log(cakes[selectedCakeIndex].sprite);
        Debug.Log(cakes[selectedCakeIndex].name);
        Debug.Log(cakeCompletionCounts[cakeName]);
        Debug.Log(cakeId);
        Debug.Log(foodType);
        ovenScript.isMaking = false;
        AudioManager.instance.PlaySFX("Ding");
        

    }

 

    // public void SelectCake(int index)
    // {
    //     if (index >= 0 && index < cakes.Length)
    //     {
    //         selectedCakeIndex = index;
    //         Debug.Log("select call");
    //     }
    // }

    IEnumerator OvenUsingText()
    {
        makeText.text = "Wait a sec!";
        yield return new WaitForSeconds(1.5f);
        makeText.text = "";
    }


    //製作蛋糕方法區域
    public void MakeCakee()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 0;
            StartCoroutine(MakeTextRoutine(3)); // 3 seconds for this cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeChocolateCake()
    {
        if (!ovenScript.isMaking) 
        {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 1;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Chocolate Cake
            gameManagerScript.CalculatePlayerCleanValue();
        } 
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeMatchaCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 2;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Matcha Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeStrawberryCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 3;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeSuperLuxuryCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 4;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeChocolateLuxuryCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 5;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeTripleLevelCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 6;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeCrispyCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 7;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeChocolateInsideRollCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 8;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeMatchaRollCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 9;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeChocolateRollCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 10;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakePinkRollCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 11;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeRollCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 12;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }

    public void MakeTaroRollCake()
    {
        if (!ovenScript.isMaking) {
            ovenAnimScript.SetCurrentFoodType(FoodType.Cake);
            selectedCakeIndex = 13;
            StartCoroutine(MakeTextRoutine(5)); // 5 seconds for Strawberry Cake
            gameManagerScript.CalculatePlayerCleanValue();
        }
        else if (ovenScript.isMaking)
        {
            StartCoroutine(OvenUsingText());
        }
    }
}
