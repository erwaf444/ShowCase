using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;

// 球的初始數字是0.05

public class BouncyBall : MonoBehaviour
{
    public float minY = -5.5f;
    public float maxVelocity = 7f;

    public float minVelocity = 5f;

    Rigidbody2D rb;


    [HideInInspector]
    public int coins;


    public int lives;

    public TextMeshProUGUI coinsText;
    public GameObject[] livesImage;

    public GameObject gameOverPanel;
    public GameObject youWinPanel;
    public GameObject flag;
    public AudioSource destroyBrickSound;
    public AudioSource youWinPanelSound;
    public AudioSource Count3Sec;

    public ShopManager shopManager;

    public PlayerMovement playerMovement;

    public int brickCount;

    [SerializeField] private TextMeshProUGUI startText;

    public GameObject gameStartPanel;
    public GameObject swipeHint;

    int lastCoins; 

    bool canReborn = false;

    // public Animator animator;

    public List<GameObject> bricks;
    public Animator FirstLevelAnim;
    public GameObject FirstLevelAnimObject;
    public GameObject ShowSettingButton;
    public VolumeSetting volumeSetting;
    // public TextMeshProUGUI MainMenuText;

    public float BallSizeValue = 0.1f;

    public float BallSizeValueForShopManager = 0.001f;
    private int unlockedLevel = 0;


    // public GameObject selectedSkin;
    public GameObject Player;
    // private Sprite playerSprite;

    public GameObject coolDownAnimObject;




    public PaddleAutoMinusCloudSave paddleAutoMinusCloudSave;

    public SkinManager skinManager;

    public GameObject coolDownTimeTip;
    public TextMeshProUGUI coolDownTimeText;

    //UsePotionAnimPart
    public GameObject usePotionAnimObject;
    public GameObject usePotionAnimObjectWithCoins;
    public GameObject UsePotionAnimObjectWithHeart;
    public GameObject UsePotionAnimObjectWithPaddleBig;
    public GameObject UsePotionAnimObjectWithPaddleSmall;
    public GameObject UsePotionAnimObjectWithAddBall;




    public InterstitialAd interstitialAd;
    public Button skipButton;
    private bool isAnimationPlaying = false;

    public GameObject notEnoughCoins;
    bool isSkip = false;


 
    async void Awake()
    {
        // playerSprite = selectedSkin.GetComponent<SpriteRenderer>().sprite;
        // Player.GetComponent<SpriteRenderer>().sprite = playerSprite;
        Player.GetComponent<SpriteRenderer>().sprite = skinManager.GetSelectedSkin().sprite;
        // Player.GetComponent<SpriteRenderer>().sprite = await skinManager.LoadSkin();
        coolDownAnimObject.SetActive(false);
        interstitialAd.LoadAd();

        bool IsRemoved = await interstitialAd.LoadAdsRemovedFromCloud();

        if (IsRemoved && SceneManager.GetActiveScene().name != "Level1")
        {
            gameStartPanel.SetActive(true);
        }
      
     

    }


    private void SkipAnimation()
    {
        if (isAnimationPlaying)
        {
            StopCoroutine(PlayOpeningAnimation()); // 停止动画播放的协程
            EndAnimation();
            isSkip = true;
        }
    }


    private void EndAnimation()
    {
        isAnimationPlaying = false;
        skipButton.gameObject.SetActive(false); // 隐藏Skip按钮
        FirstLevelAnim.enabled = false;
        FirstLevelAnimObject.SetActive(false);
        Count3Sec.Play();
        gameStartPanel.SetActive(true);
        swipeHint.SetActive(true);
        startText.gameObject.SetActive(true);
        if (ShowSettingButton != null)
        {
            Button button = ShowSettingButton.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }
        Time.timeScale = 1;

        StartCoroutine(HideStartTextAfterDelay());    

    }
    

    // Start is called before the first frame update
    async void Start()
    {   
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            // 只在 Level1 场景中激活 skipButton
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true); // 激活按钮
                skipButton.onClick.AddListener(SkipAnimation); // 添加点击事件
            }
        }
        else
        {
            // 在其他场景中禁用 skipButton
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false); // 禁用按钮
            }
        }
        
        Debug.Log("Start 方法开始执行");
        coolDownTimeTip.SetActive(false);
        StartCoroutine(LoadVolumeSetting());
        
        if(SceneManager.GetActiveScene().name == "Level1")
        {
            skipButton.gameObject.SetActive(true); 
            gameStartPanel.SetActive(false);
            swipeHint.SetActive(false);
            startText.gameObject.SetActive(false);
             // 暂停游戏时间
            Time.timeScale = 0;
            // 获取Button组件并禁用
            Button button = ShowSettingButton.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = false;
            }

            if (FirstLevelAnim != null )
            {
                FirstLevelAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
                StartCoroutine(PlayOpeningAnimation());   
            }


        }
        // audioManager = gameObject.GetComponent<AudioManager>();
        // isAnimationPlaying = false;
        rb = GetComponent<Rigidbody2D>();
        await UnityServices.InitializeAsync();
        

        // brickCount = FindObjectOfType<LevelGenerator>().transform.childCount;
        brickCount = GameObject.FindGameObjectsWithTag("Brick").Length;
        Debug.Log("Brick Count: " + brickCount);
        StartCoroutine(FirstSpawn());

        StartCoroutine(HideStartTextAfterDelay());    

        coins = StaticData.valueToKeep;
        // coins = PlayerPrefs.GetInt("coins");


        // shopManager.UpdateCoinsText();
        // shopManager.coinsText.text = coins.ToString("00000");
        // coins = shopManager.coins;
        
        int savedCoins = await LoadCoins();
        coins = savedCoins;
        coinsText.text = coins.ToString("00000");
        // ResetSpeed();
        await LoadSpeed();

        lastCoins = coins;
        lives = 2;

        // Load BallSize data
        float loadBallSize = await LoadBallSize();

        // Apply BallSize to game object if necessary
        if (loadBallSize > 0)
        {
            transform.localScale = new Vector3(loadBallSize, loadBallSize, transform.localScale.z);
            Debug.Log("BallSize loaded: " + loadBallSize);
        }
        Debug.Log("AutoMinusBallSize");
        await AutoMinusBallSize();
        await shopManager.LoadUpgrades();

    }

 

    async Task AutoMinusBallSize()
    {
        Debug.Log("Prepared for AutoMinus");
        await paddleAutoMinusCloudSave.CheckAndReduceBallSizeRoutine();

    }

    IEnumerator PlayOpeningAnimation()
    {
        if (FirstLevelAnim != null)
        {
            isAnimationPlaying = true;
            FirstLevelAnim.SetTrigger("GoAnim");

            yield return null;
            yield return new WaitForEndOfFrame();


            // 获取动画片段的时长
            AnimatorClipInfo[] clipInfo = FirstLevelAnim.GetCurrentAnimatorClipInfo(0);
            // AnimatorStateInfo animStateInfo = FirstLevelAnim.GetCurrentAnimatorStateInfo(0);
            // Debug.Log("time :" + animStateInfo.length);
            if (clipInfo.Length > 0)
            {
                float animationLength = clipInfo[0].clip.length;
                Debug.Log("Animation length: " + animationLength);
                yield return null;

                // 等待动画播放结束
                yield return new WaitForSecondsRealtime(animationLength);
            
            }
            
            // float animationLength = animStateInfo.length;

            

            // 禁用Animator组件，防止重复播放
            FirstLevelAnim.enabled = false;
            yield return new WaitForSecondsRealtime(3);
            FirstLevelAnimObject.SetActive(false);

            yield return new WaitForSecondsRealtime(3);
            
            Time.timeScale = 1;
            // if (!skipButton.gameObject.activeSelf)
            // {
            if (isSkip)
            {
                gameStartPanel.SetActive(false);
                swipeHint.SetActive(false);
                startText.gameObject.SetActive(false);
            } else if (isSkip == false)
            {
                Count3Sec.Play();
                gameStartPanel.SetActive(true);
                swipeHint.SetActive(true);
                startText.gameObject.SetActive(true);
            }
               
            // } 
            // else 
            // {
                // gameStartPanel.SetActive(false);
                // swipeHint.SetActive(false);
                // startText.gameObject.SetActive(false);
            // }

            skipButton.gameObject.SetActive(false); 

            if (ShowSettingButton != null)
            {
                Button button = ShowSettingButton.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = true;
                }
            }
        }
    }


    IEnumerator LoadVolumeSetting()
    {
        // 异步加载设置
        volumeSetting.Load();

        // 等待加载完成
        yield return null;
    }





    private IEnumerator FirstSpawn() {
        yield return new WaitForSeconds(3);
        rb.velocity = Vector2.down*5f;
    }

    private IEnumerator  HideStartTextAfterDelay() {
        canReborn = false;
        if (SceneManager.GetActiveScene().name != "Level1")
        {
            Count3Sec.Play();
            startText.gameObject.SetActive(true);
            gameStartPanel.SetActive(true);
            swipeHint.SetActive(true);
        } 
    
        yield return new WaitForSeconds(3);
        startText.gameObject.SetActive(false);
        gameStartPanel.SetActive(false);
        swipeHint.SetActive(false);


     
        canReborn = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < minY)
        {
            if(lives <= 0)
            {
                GameOver();
            }
            // else
            // {
            //     transform.position = Vector3.zero;
            //     rb.velocity = Vector2.down*5f;

            //     lives--;
            //     livesImage[lives].SetActive(false);
        
            // } 
            // if(lives >= 0)
            // {   
            //     SpawnButtonPressed();
         
            // }
        }

        if(rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }

        if(rb.velocity.magnitude < minVelocity)
        {
            rb.velocity = rb.velocity.normalized * minVelocity;
        }
        
        UpdateLivesUI();
        CheckCoinsNubmer();

        if (youWinPanel.activeSelf)
        {
            Count3Sec.Stop();
        }

    }

  


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Brick")) {
            HandleBrickCollision(collision.gameObject);
        } else if (collision.gameObject.CompareTag("Flag")) {
            HandleFlagCollision(collision.gameObject);
        }
    }

    public void HandleBrickCollision(GameObject brick) {
        if (bricks.Contains(brick))
        {
            Animator brickAnimator = brick.GetComponent<Animator>();
            if (brickAnimator != null)
            {
                brickAnimator.SetTrigger("GoAnim");
            }
        }
        StartCoroutine(DestroyBrickDelay(brick));
        destroyBrickSound.Play();
        coins += 10;
        shopManager.coins = coins;
        coinsText.text = coins.ToString("00000");
        brickCount--;
        if (brickCount <= 0) {
            flag.SetActive(true);
        }
    }

    IEnumerator DestroyBrickDelay(GameObject brick)
    {
        
        yield return new WaitForSeconds(0.2f); // 等待1秒钟

        Destroy(brick);
    }

   

    private async void HandleFlagCollision(GameObject flag) {
        Destroy(flag);
        youWinPanel.SetActive(true);
        youWinPanelSound.Play();
        SaveCoins();
        Debug.Log("Coins before saving: " + coins); // 添加调试信息
        Time.timeScale = 0;
        // Pass();
    }

    
    // public async void Pass()
    // {
    //     int currentLevel = SceneManager.GetActiveScene().buildIndex;
        
    //     if(currentLevel >= PlayerPrefs.GetInt("UnlockedLevel"))
    //     {
    //         PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
    //     }

    //     Debug.Log("Unlocked Level: " + PlayerPrefs.GetInt("UnlockedLevel"));
    // }

    public async void Pass()
    {   
        
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
        Debug.Log($"Current level: {currentLevel}");

        // if (SceneManager.GetActiveScene().buildIndex == 2)
        // {
        //     return;
        // }
        if(currentLevel >= unlockedLevel)
        {
            await SaveLevel(currentLevel + 1);
        
        }

        unlockedLevel = await LoadLevel();
        // if (currentLevel > unlockedLevel)
        // {
        //     await SaveLevel(currentLevel);
        //     unlockedLevel = currentLevel;
        // }

        Debug.Log("Unlocked Level: " + unlockedLevel);
    }
    
   


    public async Task<int> LoadLevel()
    {
        try
        {
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "UnlockedLevelValue" });
            
            // int highestUnlockedLevel = 0;
            if(data.ContainsKey("UnlockedLevelValue"))
            {
                return int.Parse(data["UnlockedLevelValue"]);
                // highestUnlockedLevel = int.Parse(data["UnlockedLevelValue"]);
                // return highestUnlockedLevel;

            }
            else
            {
                return 0; // Default value if no data is found
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading data: {ex.Message}");
            return 0;
        }
    }

    public async Task SaveLevel(int currentLevel)
    {   
        int highestUnlockedLevel = await LoadLevel();
        var data = new Dictionary<string, object> { {"UnlockedLevelValue", currentLevel} };
        
        if (currentLevel >= highestUnlockedLevel)
        {
            try
            {
                await CloudSaveService.Instance.Data.ForceSaveAsync(data);
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving data: {ex.Message}");
            }
        }
     
    }

 


    

 


    public void SpawnButtonPressed()
    {  
        if(transform.position.y < minY ){
            if(lives >= 0) {
                Debug.Log("BornBall");
                
                transform.position = Vector3.zero;
                rb.velocity = Vector2.down * 0; 
                StartCoroutine(DelayedDrop());
                
                lives--;
                livesImage[lives].SetActive(false);
                UpdateLivesUI();
            }
        } else if (canReborn && transform.position.y > minY){

            ReBornBouncyBall();
        }
    }

    IEnumerator DelayedDrop()
    {
        Count3Sec.Play();
        yield return new WaitForSeconds(3f); // 等待3秒
        rb.velocity = Vector2.down * 5f; // 執行下落動作
        Debug.Log("DropBall");
    }


    void UpdateLivesUI()
    {
        for (int i = 0; i < livesImage.Length; i++)
        {
            livesImage[i].SetActive(i < lives);
        }
    }

    // public void SaveCoins()
    // {
    //     PlayerPrefs.SetInt("coins", coins);
    //     PlayerPrefs.Save(); // 确保保存到磁盘
    //     Debug.Log("set coins: " + coins);
    //     coinsText.text = coins.ToString("00000"); // 更新显示文本
    // }

    public async void SaveCoins()
    {   
        var data = new Dictionary<string, object> { {"coins", coins} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("save coins: " + coins);
            Debug.Log("coinsdata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving coinsdata: {ex.Message}");
        }
    }

    

    // public void LoadCoins()
    // {
    //     if (PlayerPrefs.HasKey("coins"))
    //     {
    //         coins = PlayerPrefs.GetInt("coins");
    //         Debug.Log("get coins: " + coins);
    //     } else
    //     {
    //         coins = 0;
    //     }
    //     coinsText.text = coins.ToString("00000");

        
    // }

    public async Task<int> LoadCoins()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "coins" });
        if (data.ContainsKey("coins"))
        {
            coins = int.Parse(data["coins"]);
            Debug.Log("Loaded coins: " + coins);
            return coins;
        }
        else
        {
            Debug.Log("No coins data found");
            return 0;
        }
    }

    public async Task SaveSpeed()
    {   
        var data = new Dictionary<string, object> { {"speed", playerMovement.speed} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("speeddata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving speeddata: {ex.Message}");
        }
    }

    // public void SaveSpeed()
    // {
    //     PlayerPrefs.SetFloat("speed",playerMovement.speed);
    //     Debug.Log("set speed: " + playerMovement.speed);
    // }

    // public void LoadSpeed()
    // {
    //     if (PlayerPrefs.HasKey("speed"))
    //     {
    //         playerMovement.speed = PlayerPrefs.GetFloat("speed");
    //         Debug.Log("get speed: " + playerMovement.speed);
    //     }
    //     else 
    //     {
    //         playerMovement.speed = 20;
    //     }
    // }

    public async Task<float> LoadSpeed()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "speed" });
        if (data.ContainsKey("speed"))
        {
            playerMovement.speed = float.Parse(data["speed"]);
            Debug.Log("Loaded speed: " + playerMovement.speed);
            return playerMovement.speed;
        }
        else
        {
            Debug.Log("No speed data found");
            return 0;
        }
    }

    

 
    
    public void ResetSpeed()
    {
        PlayerPrefs.DeleteKey("speed");
    }


    void CheckCoinsNubmer()
    {
        if (coins != lastCoins)
        {
            UpdateCoinsTextSize();
            lastCoins = coins;
            
           

            StartCoroutine(DecreaseTextSizeAfterDelay());
            
        } else 
        {
            ;
        }
    }

    void UpdateCoinsTextSize()
    {
        if (coinsText != null)
        {
            coinsText.fontSize = 45;
            coinsText.fontSize = coinsText.fontSize + 10;
            if (coinsText.fontSize > 55)
            {
                coinsText.fontSize = 45;
            }
        } else 
        {
            Debug.Log("null");
        }
   
    }

    IEnumerator DecreaseTextSizeAfterDelay()
    {
        
        yield return new WaitForSeconds(1.0f); // 等待1秒钟

        coinsText.fontSize = 45;
    }

  
    public void ReBornBouncyBall()
    {
        StartCoroutine(ReBornBouncyBallDelay());      
    }

    IEnumerator ReBornBouncyBallDelay()
    {
        transform.position = Vector3.zero; // 或者设置到其他初始位置
        rb.velocity = Vector2.down * 0; 
        yield return new WaitForSeconds(3.0f);
        rb.velocity = Vector2.down * 5f; 
    }

    public void AddBallSize()
    {
        transform.localScale = new Vector3(transform.localScale.x + BallSizeValue, transform.localScale.y + BallSizeValue, transform.localScale.z);
    }

   

    

    public void AddBallSizeForShopManager()
    {
        transform.localScale = new Vector3(transform.localScale.x + BallSizeValueForShopManager, transform.localScale.y + BallSizeValueForShopManager, transform.localScale.z);
    }

    public async Task SaveBallSize()
    {   
        var data = new Dictionary<string, object> { {"ballSize", transform.localScale.x} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("ballSizedata saved successfully");
        }
        catch 
        {
            Debug.LogError($"Error saving ballSizedata");
        }
    }

    public async Task<float> LoadBallSize()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "ballSize" });
        if (data.ContainsKey("ballSize"))
        {
            // transform.localScale.x = float.Parse(data["paddleWidth"]);
            // Debug.Log("Loaded paddleWidth: " + transform.localScale.x);
            // return transform.localScale.x;
            if (float.TryParse(data["ballSize"], out float ballSize))
            {
                Debug.Log("Loaded ballSize: " + ballSize);
                return ballSize;
            }
            else
            {
                Debug.LogError("Failed to parse ballSize data: " + data["ballSize"]);
                return 0f;
            }
        }
        else
        {
            Debug.Log("No ballSize data found");
            return 0;
        }
    }

    public void ShowPotionCoolDownTime(string time)
    {
        coolDownTimeText.text = time;
        coolDownTimeTip.SetActive(true);
        Debug.Log("Starting HidePotionCoolDownTime coroutine.");
        StartCoroutine(HidePotionCoolDownTime());
    }

    IEnumerator HidePotionCoolDownTime()
    {
        Debug.Log("HidePotionCoolDownTime");
        yield return new WaitForSeconds(3f);
        coolDownTimeTip.SetActive(false);
        Debug.Log("End");

    }
   


    void GameOver()
    {   
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        } else 
        {
            Debug.Log("null");
        }
        Time.timeScale = 0;
        Destroy(gameObject);
    }

    
    public void NotEnoughCoins()
    {
        StartCoroutine(NotEnoughCoinsDelay());
    }
    IEnumerator NotEnoughCoinsDelay()
    {
        notEnoughCoins.SetActive(true);
        yield return new WaitForSeconds(3f);
        notEnoughCoins.SetActive(false);
    }

}
