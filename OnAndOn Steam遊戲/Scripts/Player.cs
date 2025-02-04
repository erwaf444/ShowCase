using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxForce = 1000f;       // 最大蓄力力量
    public float chargeRate = 500f;      // 蓄力增長速度
    private float currentForce = 0f;     // 當前蓄力值
    private float doubleForce = 0f;     // 當前蓄力值
    private bool isCharging = false;     // 是否正在蓄力
    public Rigidbody2D rb;                // 玩家 Rigidbody
    public Image chargeBar;              // 用於顯示蓄力條的 UI Image
    public RectTransform chargeBarRect; // 蓄力條的 RectTransform
    public RectTransform frameRect;     // 框架的 RectTransform
    private bool isAttached = false;     // 是否吸附在障礙物上
    private Transform attachedSurface;   // 吸附的障礙物
    private bool canAttach = true;       // 是否可以吸附
    private HashSet<Transform> attachedObjects = new HashSet<Transform>();
    public GameObject playerArrow;             // 用來顯示箭頭的 GameObject
    public GameManager gameManager;
    public Transform leftOpenEye;
    public Transform rightOpenEye;
    public Transform leftCloseEye;
    public Transform rightCloseEye;
    public Vector3 leftOpenOffset = new Vector3(-0f, 0f, 0f); // 相對於玩家的偏移量（左上角）
    public Vector3 rightOpenOffset = new Vector3(-0f, 0f, 0f); // 相對於玩家的偏移量（左上角）
    public Vector3 leftCloseOffset = new Vector3(-0f, 0f, 0f); // 相對於玩家的偏移量（左上角）
    public Vector3 rightCloseOffset = new Vector3(-0f, 0f, 0f); // 相對於玩家的偏移量（左上角）

    public Vector3 playerLeftSideleftOpenOffset = new Vector3(-0.3f, 0.2f, 0f); // 相對於玩家的偏移量（左上角）
    public Vector3 playerLeftSiderightOpenOffset = new Vector3(-0f, 0.2f, 0f); // 相對於玩家的偏移量（左上角）
    public Vector3 playerLeftSideleftCloseOffset = new Vector3(-0.3f, 0.2f, 0f); // 相對於玩家的偏移量（左上角）
    public Vector3 playerLeftSiderightCloseOffset = new Vector3(-0f, 0.2f, 0f); // 相對於玩家的偏移量（左上角）
    public Animator blinkEyeAnimator;
    public bool isLeft = false;
    private bool canChainFire = false;      // 是否允許連續發射
    private float chainFireTimeWindow = 1f; // 連續發射的時間窗口（1秒）
    private float chainFireTimer = 0f;      // 計時器

    // 用於控制是否可以連續跳躍的邏輯
    private bool canDoubleJump = false;  // 是否可以雙重跳躍
    private bool isJumping = false;      // 玩家是否正在跳躍中
    public ParticleSystem dust;
    private int attachCount = 0;  // 記錄連續附著的次數
    public Transform narrationText;
    public AudioManager audioManager;
    public bool canMove = true;


    void Start()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        trail.enabled = false;
        trail.enabled = true;

        rb = GetComponent<Rigidbody2D>();  // 獲取 Rigidbody 組件
        chargeBar.fillAmount = 0f;
        playerArrow.SetActive(true);
        blinkEyeAnimator.SetTrigger("BlinkEye");
    }


    void Update()
    {
        UpdateArrowDirection();
        leftOpenEye.position = transform.position + leftOpenOffset;
        rightOpenEye.position = transform.position + rightOpenOffset;
        leftCloseEye.position = transform.position + leftCloseOffset;
        rightCloseEye.position = transform.position + rightCloseOffset;

        // 跟隨玩家位置的邏輯
        if (chargeBarRect != null && frameRect != null)
        {
            // 設定 UI 相對於玩家的偏移量
            Vector3 offset = new Vector3(-12f, 7f, 0f); // 根據需求調整偏移量 (X, Y, Z)

            // 更新 UI 元素的位置為玩家位置 + 偏移量
            chargeBarRect.position = transform.position + offset;
            frameRect.position = transform.position + offset;
        }

        Vector3 narrationOffset = new Vector3(0f, -7f, 0f); // 根據需求調整偏移量 (X, Y, Z)
        narrationText.position = transform.position + narrationOffset;

        // 根據速度反轉玩家面朝方向
        if(rb.velocity.x < 0)
        {   
           
            isLeft = true;
        } else if (rb.velocity.x > 0)
        {
           
            isLeft = false;
        }
        
        if (isLeft)
        {
            leftOpenEye.position = transform.position + playerLeftSideleftOpenOffset;
            rightOpenEye.position = transform.position + playerLeftSiderightOpenOffset;
            leftCloseEye.position = transform.position + playerLeftSideleftCloseOffset;
            rightCloseEye.position = transform.position + playerLeftSiderightCloseOffset;
            transform.localScale = new Vector3(-0.5f, 0.5f, 1f);  // 翻轉 X 軸
            playerArrow.transform.localScale = new Vector3(-1f, 1f, 1f);
        }  else 
        {
            leftOpenEye.position = transform.position + leftOpenOffset;
            rightOpenEye.position = transform.position + rightOpenOffset;
            leftCloseEye.position = transform.position + leftCloseOffset;
            rightCloseEye.position = transform.position + rightCloseOffset;
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);   // 正常的 X 軸
            playerArrow.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // 檢測按下 Space 鍵脫離障礙物
        if (isAttached && Input.GetKeyDown(KeyCode.Space))
        {
            Detach(); // 只脫離，不發射
            isCharging = false;
            currentForce = 0f; // 重置蓄力值
            canAttach = false;
        }
    
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttached)
            {
                // 未附著時的第一次點擊：開始蓄力
                isCharging = true;
                currentForce = 0f;
                doubleForce = 0f;
                attachCount = 0;
            }
            else 
            {
                // 已附著時的第一次點擊：準備發射
                isCharging = true;
                // currentForce = 10000f;
                // doubleForce = 2000f;
            }
            
        }

        // 當滑鼠左鍵按住時，蓄力增加
        if (isCharging)
        {
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Min(currentForce, maxForce); // 限制蓄力值不超過最大值
            currentForce = Mathf.Max(currentForce, doubleForce); // 限制蓄力值不超過最大值
            // Debug.Log($"蓄力中: {currentForce}"); // 顯示蓄力值
            chargeBar.fillAmount = currentForce / maxForce; // 根據蓄力值更新蓄力條的填充
        }

        // 當釋放滑鼠左鍵時，施加力並彈射玩家
        if (Input.GetMouseButtonUp(0) && isCharging || Input.GetMouseButtonUp(0) && isAttached && isCharging)
        {
            isCharging = false;
            LaunchPlayer();
        }

    

        // if (canChainFire)
        // {
        //     chainFireTimer += Time.deltaTime;

        //     // 超過時間窗口，停止連續發射
        //     if (chainFireTimer > chainFireTimeWindow)
        //     {
        //         canChainFire = false;
        //         chainFireTimer = 0f;
        //         Debug.Log("超過時間窗口，無法繼續連續發射");
        //     }
        // }

        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (canChainFire)
        //     {
        //         // 在時間窗口內發射
        //         Debug.Log("連續發射!");
        //         LaunchPlayer();
        //         chainFireTimer = 0f; // 重置計時器
        //     }
        //     else if (!isAttached)
        //     {
        //         // 第一次發射
        //         Debug.Log("第一次發射!");
        //         LaunchPlayer();
        //         canChainFire = true;
        //         chainFireTimer = 0f; // 開始計時
        //     }
        // }

        // 檢查是否在吸附狀態下按下跳躍按鍵 (連續跳躍)
        // if (isAttached && Input.GetMouseButtonDown(0) && !isJumping)
        // {
        //     JumpWhileAttached();
        // }
    
     
    }

    // 玩家在吸附狀態下的跳躍邏輯
    // void JumpWhileAttached()
    // {
    //     if (!isJumping && isAttached)  // 如果玩家不是正在跳躍
    //     {
    //         isJumping = true;  // 設置為跳躍狀態
    //         Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
    //         rb.AddForce(direction * 10f);
    //     }
    // }


    void LaunchPlayer()
    {
        if (canMove)
        {
            // 施加力量時，確保玩家不是在吸附狀態
            if (isAttached) 
            {
                // 如果玩家處於吸附狀態，會先脫離再發射
                Detach(); // 先脫離障礙物
            }
            // 取得鼠標位置相對於玩家的位置
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

            // 施加力量，根據鼠標方向
            rb.AddForce(direction * currentForce);
            if (gameManager.panelOn == false)
            {
                audioManager.PlaySFX("jump");
            }
            dust.Play();
            Debug.Log("玩家彈射出去!");
            StartCoroutine(ReduceChargeBar());
        }
    }

    IEnumerator ReduceChargeBar()
    {
        float duration = 0.5f; // 讓蓄力條回到 0 的時間
        float startFill = chargeBar.fillAmount; // 獲取當前蓄力條填充量
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newFill = Mathf.Lerp(startFill, 0f, elapsed / duration); // 平滑過渡
            chargeBar.fillAmount = newFill; // 更新蓄力條
            yield return null; // 等待下一幀
        }

        // 確保最終填充值為 0
        chargeBar.fillAmount = 0f;
        // currentForce = 0f; // 重置蓄力值
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canAttach)
        {
            // 碰到障礙物，停止玩家並吸附到障礙物
            if (collision.gameObject.CompareTag("Obstacle")) // 確保障礙物有 "Obstacle" 標籤
            {
                // 檢查是否已經附著過這個物體
                if (!attachedObjects.Contains(collision.transform))
                {
                    // 從 HashSet 中移除之前吸附過的物體
                    if (attachedSurface != null)
                    {
                        attachedObjects.Remove(attachedSurface);
                        Debug.Log($"移除之前吸附過的物體: {attachedSurface.name}");
                    }

                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                    isAttached = true;
                    isJumping = false;
                    attachedSurface = collision.transform;
                    // transform.parent = attachedSurface;

                    // 將此物體加入已附著物體的集合
                    attachedObjects.Add(collision.transform);

                    // 更新 attachCount 和 doubleForce
                    attachCount++;  // 增加附著次數

                    if(attachCount > 100)
                    {
                        gameManager.UnlockAchievement("Surpassed");
                    }

                    if (attachCount == 1)
                    {
                        doubleForce = 1000f;
                    }
                    else if (attachCount >= 2)
                    {
                        doubleForce += 100f;
                    }

                    Debug.Log("玩家已吸附到障礙物上!");
                }
            }

            if (collision.gameObject.CompareTag("Ground"))
            {
                // 重置附著次數和doubleForce
                attachCount = 0;
                doubleForce = 0f;
            }

            if (collision.gameObject.CompareTag("PastToBegin")) 
            {
                transform.position = gameManager.beginDoor.position;
            }
        }
        
    }

    void Detach()
    {
        // 分離邏輯，讓玩家恢復自由運動
        if (isAttached)
        {
            rb.isKinematic = false;
            transform.parent = null; // 解除父子關係
            isAttached = false;
            Debug.Log("玩家已脫離障礙物!");

       
        }

        StartCoroutine(AllowReattachAfterDelay());

    }

    IEnumerator AllowReattachAfterDelay()
    {
        // 等待一段時間才允許再次吸附
        yield return new WaitForSeconds(0.3f); // 假設延遲1秒
        canAttach = true;
        Debug.Log("可以再次吸附障礙物!");
    }

    // 更新箭頭的方向
    void UpdateArrowDirection()
    {
        // 取得滑鼠在世界空間中的位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 計算從玩家到滑鼠的方向
        Vector2 directionToMouse = mousePosition - (Vector2)transform.position;

        // 計算角度，這是玩家到滑鼠的方向
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // 設置箭頭的旋轉，使其指向滑鼠的方向
        playerArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 檢查碰撞的物件是否有 "TheEnd" 標籤
        if (collision.CompareTag("End") && !gameManager.isGameEnded)
        {
            gameManager.isGameEnded = true;
            gameManager.GameEnd();
            
        }
    }



}
