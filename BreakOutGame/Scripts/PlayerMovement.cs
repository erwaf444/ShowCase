using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.CloudSave;



//平台長寬初始是0.07 、 0.06
//每次增加是0.01 、 0.008


public class PlayerMovement : MonoBehaviour
{
    public float speed = 50;

    public float widthValue;
    public float widthForUpgradeSystem;
    public float heightForUpgradeSystem;
    private Vector3 originalScale;
    public GameObject extraBall;
    
    public BouncyBall bouncyBall;

    public float audoAddCoinsGapTime = 1f;

    public GameObject flag;

    public AudioSource getPotionSound;
 
    

    // public float maxX = 7.5f;

    // public float movementHorizontal;
    Rigidbody2D rb;

    // public GameObject selectedSkin;
    public GameObject Player;
    // private Sprite playerSprite;
    public PaddleAutoMinusCloudSave paddleAutoMinusCloudSave;
 
    public PaddleSkinManager paddleSkinManager;
    void Awake()
    {
        Player.GetComponent<SpriteRenderer>().sprite = paddleSkinManager.GetSelectedPaddleSkin().sprite;
    }
    

    // Start is called before the first frame update
    async void Start()
    {
        // Debug.Log("Start 方法开始执行");

        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        // extraBall = GameObject.Find("BouncyBall2");
        if (extraBall != null)
        {
            extraBall.SetActive(false); // 确保额外的球在开始时处于禁用状态
        }
        else
        {
            Debug.LogError("找不到名为'bouncyball2'的游戏对象！");
        }
       

        await UnityServices.InitializeAsync();

        
        // Load paddle width data
        float loadedWidth = await LoadPaddleWidth();

        // Apply loaded width to game object if necessary
        if (loadedWidth > 0)
        {
            transform.localScale = new Vector3(loadedWidth, transform.localScale.y, transform.localScale.z);
        }
        // Debug.Log("LoadPaddleWidth 方法执行完毕");
        float loadedHeight = await LoadPaddleHeight();
        if (loadedHeight > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, loadedHeight, transform.localScale.z);
        }
        // Debug.Log("LoadPaddleHeight 方法执行完毕");



        // Debug.Log("Prepared for AutoMinus");
        await AutoMinusPaddleWidth();
        // Debug.Log("准备自动减小速度");

        await AutoMinusPaddleSpeed();   
        // Debug.Log("自动减小初始化完成");

        
    }

    
    async Task AutoMinusPaddleWidth()
    {
        await paddleAutoMinusCloudSave.CheckAndReduceWidthAndHeightRoutine();
    }

    async Task AutoMinusPaddleSpeed()
    {
        await paddleAutoMinusCloudSave.CheckAndReduceSpeedRoutine();
    }

    // Update is called once per frame
    void Update()
    {
        // movementHorizontal = Input.GetAxis("Horizontal");
        // if((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > -maxX))
        // {
        //     transform.position += Vector3.right * movementHorizontal * speed * Time.deltaTime;   
        // }

        if (Input.GetMouseButton(0))
        {
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(touchPos.x < 0)
            {
                rb.AddForce( Vector2.left * speed);

            } else 
            {
                rb.AddForce(Vector2.right * speed);
            }
        } else
        {
            rb.velocity = Vector2.zero;
        }
    }

    

    public void AddPaddleWidthForShopManager()
    {
        transform.localScale = new Vector3(transform.localScale.x + (widthForUpgradeSystem + 0.01f) , transform.localScale.y , transform.localScale.z);
        Debug.Log("Paddle width: " + transform.localScale.x);
    }

    public async void AutoMinusPaddleWidthForShopManager()
    {
        float loadedWidth = await LoadPaddleWidth();
        if (loadedWidth > 0.07f)
        {
            transform.localScale = new Vector3(transform.localScale.x - 0.01f, transform.localScale.y - 0.008f, transform.localScale.z);
        }
    }

    public void AddPaddleHeightForShopManager()
    {
        transform.localScale = new Vector3(transform.localScale.x , transform.localScale.y + (heightForUpgradeSystem + 0.008f), transform.localScale.z);
        Debug.Log("Paddle width: " + transform.localScale.x);
    }

    public async void AutoMinusPaddleHeightForShopManager()
    {
        float loadedHeiht = await LoadPaddleHeight();
        if (loadedHeiht > 0.06f)
        {
            transform.localScale = new Vector3(transform.localScale.x , transform.localScale.y - 0.008f, transform.localScale.z);
        }
    }


    public async Task SavePaddleWidth()
    {   
        var data = new Dictionary<string, object> { {"paddleWidth", transform.localScale.x} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("paddleWidthdata saved successfully");
        }
        catch 
        {
            Debug.LogError($"Error saving paddleWidthdata");
        }
    }

    public async Task<float> LoadPaddleHeight()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "paddleHeight" });
        if (data.ContainsKey("paddleHeight"))
        {
            // transform.localScale.x = float.Parse(data["paddleWidth"]);
            // Debug.Log("Loaded paddleWidth: " + transform.localScale.x);
            // return transform.localScale.x;
            if (float.TryParse(data["paddleHeight"], out float paddleHeight))
            {
                Debug.Log("Loaded paddleHeight: " + paddleHeight);
                return paddleHeight;
            }
            else
            {
                Debug.LogError("Failed to parse paddleHeight data: " + data["paddleHeight"]);
                return 0f;
            }
        }
        else
        {
            Debug.Log("No paddleHeight data found");
            return 0;
        }
    }

    public async Task SavePaddleHeight()
    {   
        var data = new Dictionary<string, object> { {"paddleHeight", transform.localScale.y} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("paddleHeightdata saved successfully");
        }
        catch 
        {
            Debug.LogError($"Error saving paddleHeightdata");
        }
    }

    public async Task<float> LoadPaddleWidth()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "paddleWidth" });
        if (data.ContainsKey("paddleWidth"))
        {
            // transform.localScale.x = float.Parse(data["paddleWidth"]);
            // Debug.Log("Loaded paddleWidth: " + transform.localScale.x);
            // return transform.localScale.x;
            if (float.TryParse(data["paddleWidth"], out float paddleWidth))
            {
                Debug.Log("Loaded paddleWidth: " + paddleWidth);
                return paddleWidth;
            }
            else
            {
                Debug.LogError("Failed to parse paddleWidth data: " + data["paddleWidth"]);
                return 0f;
            }
        }
        else
        {
            Debug.Log("No paddleWidth data found");
            return 0;
        }
    }


    public void AddPaddleWidth()
    {
        transform.localScale = new Vector3(transform.localScale.x + widthValue, transform.localScale.y, transform.localScale.z);
        StartCoroutine(ResetPaddleWidth()); // 如果需要在一段时间后恢复原来的宽度，可以调用协程
    }

    public void MinusPaddleWidth()
    {
        transform.localScale = new Vector3(transform.localScale.x - widthValue, transform.localScale.y, transform.localScale.z);
        StartCoroutine(ResetPaddleWidth()); // 如果需要在一段时间后恢复原来的宽度，可以调用协程
    }

    public void AddPaddleBigWidth()
    {
        transform.localScale = new Vector3(transform.localScale.x + widthValue*5 , transform.localScale.y, transform.localScale.z);
        StartCoroutine(ResetPaddleWidth()); // 如果需要在一段时间后恢复原来的宽度，可以调用协程
    }

    public void MinusPaddleBigWidth()
    {
        transform.localScale = new Vector3(transform.localScale.x - widthValue*2 , transform.localScale.y, transform.localScale.z);
        StartCoroutine(ResetPaddleWidth()); // 如果需要在一段时间后恢复原来的宽度，可以调用协程
    }

    private IEnumerator ResetPaddleWidth()
    {
        yield return new WaitForSeconds(30); // 例如，10秒后恢复原来的宽度
        transform.localScale = originalScale;
    }


    public void AddBall()
    {
        if (extraBall != null)
        {
            extraBall.SetActive(true);
            Debug.Log("Ball activated!");
            StartCoroutine(ResetBallQuantity());
        }
    }

    private IEnumerator ResetBallQuantity()
    {
        yield return new WaitForSeconds(60); 
        extraBall.SetActive(false);
    }


    

    public void AutoAddCoins10Sec()
    {
        
        Debug.Log("Auto Add coins");

        StartCoroutine(AutoAddCoinsTime());

    }

    IEnumerator AutoAddCoinsTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            yield return new WaitForSeconds(audoAddCoinsGapTime);

            // 每秒增加10个硬币
            bouncyBall.coins += 10;
            bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");


            Debug.Log("Coins increased! Current coins: " + bouncyBall.coins);
            elapsedTime += audoAddCoinsGapTime;
        }

        Debug.Log("Auto Add coins end after 10 seconds");
        
    }

    public void Add100Coins()
    {
        bouncyBall.coins += 100;
    }

    public void AutoDestroy5Brick()
    {
        Debug.Log("Brick count: " + bouncyBall.brickCount);
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");

        if (bricks.Length <= 5)
        {
            foreach (GameObject brick in bricks)
            {
                Animator brickAnimator = brick.GetComponent<Animator>();
                if (brickAnimator != null)
                {
                    brickAnimator.SetTrigger("GoAnim"); // 设置动画触发器
                }
                StartCoroutine(AutoDestroy5BrickDelay(brick));
                // Destroy(brick);
                getPotionSound.Play();
                bouncyBall.coins += 10;
                bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");
                bouncyBall.brickCount--;
                Debug.Log("Brick count: " + bouncyBall.brickCount);
                if (bouncyBall.brickCount <= 0) {
                    flag.SetActive(true);
                } 
            }
            // PlayerPrefs.SetInt("coins", bouncyBall.coins);
            // bouncyBall.SaveCoins();
            // StartCoroutine(CheckBrickCountAndActivateFlag());

            return;
        }

        List<int> selectedIndices = new List<int>();

        // 随机选择5个砖块
        for (int i = 0; i < 5; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, bricks.Length);
            }
            while (selectedIndices.Contains(randomIndex)); // 确保不重复选择同一个砖块

            selectedIndices.Add(randomIndex);
        }

        // 销毁选中的砖块
        foreach (int index in selectedIndices)
        {   
            GameObject brick = bricks[index];
            Animator brickAnimator = brick.GetComponent<Animator>();
            if (brickAnimator != null)
            {
                brickAnimator.SetTrigger("GoAnim"); // 设置动画触发器
            }
            StartCoroutine(AutoDestroy5BrickDelay(brick));
            // Destroy(bricks[index]);
            getPotionSound.Play();
            bouncyBall.coins += 10;
            bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");
            bouncyBall.brickCount--;
           
            Debug.Log("Brick count: " + bouncyBall.brickCount);
            if (bouncyBall.brickCount <= 0) {
                flag.SetActive(true);
            } 

        }  
        // bouncyBall.SaveCoins();
        // PlayerPrefs.SetInt("coins", bouncyBall.coins);
        // StartCoroutine(CheckBrickCountAndActivateFlag());
    }

    IEnumerator AutoDestroy5BrickDelay(GameObject brick)
    {
        
        yield return new WaitForSeconds(0.2f); // 等待1秒钟

        Destroy(brick);
    }

    public void AutoDestroyHalfBrick()
    {
        Debug.Log("Brick count: " + bouncyBall.brickCount);
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        int halfBricksCount = bricks.Length / 2;
        
        if (halfBricksCount < 1)
        {
            foreach (GameObject brick in bricks)
            {
                Animator brickAnimator = brick.GetComponent<Animator>();
                if (brickAnimator != null)
                {
                    brickAnimator.SetTrigger("GoAnim"); // 设置动画触发器
                }
                StartCoroutine(AutoDestroyHalfBrickDelay(brick));
                // Destroy(brick);
                getPotionSound.Play();
                bouncyBall.coins += 10;
                bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");
                bouncyBall.brickCount--;
                Debug.Log("Brick count: " + bouncyBall.brickCount);
                if (bouncyBall.brickCount <= 0) {
                    flag.SetActive(true);
                } 
            }
            // bouncyBall.SaveCoins();
            // PlayerPrefs.SetInt("coins", bouncyBall.coins);
            return;
        }

        if (bricks.Length <= halfBricksCount)
        {
            foreach (GameObject brick in bricks)
            {
                Animator brickAnimator = brick.GetComponent<Animator>();
                if (brickAnimator != null)
                {
                    brickAnimator.SetTrigger("GoAnim"); // 设置动画触发器
                }
                StartCoroutine(AutoDestroyHalfBrickDelay(brick));
                // Destroy(brick);
                getPotionSound.Play();
                bouncyBall.coins += 10;
                bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");
                bouncyBall.brickCount--;
                Debug.Log("Brick count: " + bouncyBall.brickCount);
                if (bouncyBall.brickCount <= 0) {
                    flag.SetActive(true);
                } 
            }
            // bouncyBall.SaveCoins();
            // PlayerPrefs.SetInt("coins", bouncyBall.coins);
            return;
        }

        List<int> selectedIndices = new List<int>();

        // 随机选择10个砖块
        for (int i = 0; i < halfBricksCount; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, bricks.Length);
            }
            while (selectedIndices.Contains(randomIndex)); // 确保不重复选择同一个砖块

            selectedIndices.Add(randomIndex);
        }

        // 销毁选中的砖块
        foreach (int index in selectedIndices)
        {   
            GameObject brick = bricks[index];
            Animator brickAnimator = brick.GetComponent<Animator>();
            if (brickAnimator != null)
            {
                brickAnimator.SetTrigger("GoAnim"); // 设置动画触发器
            }
            StartCoroutine(AutoDestroyHalfBrickDelay(brick));
            // Destroy(bricks[index]);
            getPotionSound.Play();
            bouncyBall.coins += 10;
            bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");
            bouncyBall.brickCount--;
            Debug.Log("Brick count: " + bouncyBall.brickCount);
            if (bouncyBall.brickCount <= 0) {
                flag.SetActive(true);
            } 
        }
        // bouncyBall.SaveCoins();
        // PlayerPrefs.SetInt("coins", bouncyBall.coins);


    }

    IEnumerator AutoDestroyHalfBrickDelay(GameObject brick)
    {
        
        yield return new WaitForSeconds(0.2f); // 等待1秒钟

        Destroy(brick);
    }

    public void AutoDestroyAllBrick()
    {
        Debug.Log("Brick count: " + bouncyBall.brickCount);
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");

    
        
        foreach (GameObject brick in bricks)
        {
            Animator brickAnimator = brick.GetComponent<Animator>();
            if (brickAnimator != null)
            {
                brickAnimator.SetTrigger("GoAnim"); // 设置动画触发器
            }
            StartCoroutine(AutoDestroyAllBrickDelay(brick));
            getPotionSound.Play();
            bouncyBall.coins += 10;
            bouncyBall.coinsText.text = bouncyBall.coins.ToString("00000");
            bouncyBall.brickCount--;

            Debug.Log("Brick count: " + bouncyBall.brickCount);
            if (bouncyBall.brickCount <= 0) {
                flag.SetActive(true);
            } 
        }
        // bouncyBall.SaveCoins();
        // PlayerPrefs.SetInt("coins", bouncyBall.coins);

        
    }

    IEnumerator AutoDestroyAllBrickDelay(GameObject brick)
    {
        
        yield return new WaitForSeconds(0.2f); // 等待1秒钟

        Destroy(brick);
    }

    // IEnumerator CheckBrickCountAndActivateFlag()
    // {
    //     yield return null;

    //     yield return new WaitForSeconds(0.5f); // 等待0.5秒钟，确保所有销毁操作完成

    //     if (bouncyBall.brickCount <= 0)
    //     {
    //         flag.SetActive(true);
    //     }
    // }

    public void AddLives()
    {
        
        if (bouncyBall.lives < 5)
        {
            bouncyBall.lives += 1;
        } 
    }

    
}
