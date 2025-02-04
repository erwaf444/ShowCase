using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;

public class DragDrop : MonoBehaviour
{
    private Vector3 offset;  // 用來儲存鼠標與物件之間的偏移
    private Camera mainCamera;  // 主攝影機
    private Vector3 originalPosition; 
    private Passerby currentPasserby;
    public int foodBagId;
    private Transform trashCan;
    public bool isBeingDragged = false;  // 用於標誌當前食物袋是否正在被拖動
    private GameManager gameManager;
    private Passerby passerby;
    private PasserbySpawn passerbySpawn;
    private Player player;
    public bool customerGetFood = false;
    private int? touchId = null;  // 当前拖动的触摸点 ID


  

    void Start()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
        GameObject passerbyObject = GameObject.Find("Passerby");
        if (passerbyObject != null)
        {
            passerby = passerbyObject.GetComponent<Passerby>();
        }
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<Player>();
        }
        // GameObject passerbySpawnObject = GameObject.Find("PassetbySpawn");
        // if (passerbySpawnObject != null)
        // {
        //     passerbySpawn = passerbySpawnObject.GetComponent<PasserbySpawn>();
        // }
        passerbySpawn = FindObjectOfType<PasserbySpawn>();
        if (passerbySpawn == null)
        {
            Debug.LogError("PasserbySpawn not found in the scene!");
        }
        if (passerbySpawn == null)
        {
            Debug.Log("passerbySpawn is null");
        }
        mainCamera = Camera.main;  // 獲取主攝影機
        originalPosition = transform.position;

        trashCan = GameObject.Find("TrashCan").transform;
    }

    void Update()
    {
        #if UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();  // 如果是移動設備，使用觸控輸入
        #else
            HandleMouseInput();  // 否則，使用滑鼠輸入
        #endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        else if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);  // 获取第一个触摸点

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch);
                    break;

                case TouchPhase.Moved:
                    OnTouchMoved(touch);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnTouchEnded(touch);
                    break;
            }
        }
    }

    void OnMouseDown()
    {
        // 計算鼠標與物件之間的偏移
        // Debug.Log("OnMouseDown called");
        isBeingDragged = true;  // 標誌開始拖動
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;
    }

    void OnTouchBegan(Touch touch)
    {
        Vector3 touchPosition = GetTouchWorldPosition(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            isBeingDragged = true;  // 标志开始拖动
            touchId = touch.fingerId;  // 记录当前触摸点 ID
            offset = transform.position - touchPosition;
        }
    }

    void OnMouseDrag()
    {
        // 更新物件位置，使其跟隨鼠標
        // Debug.Log("OnMouseDrag called");
        Vector3 mousePosition = GetMouseWorldPosition();
        transform.position = mousePosition + offset;
    }

    void OnTouchMoved(Touch touch)
    {
        if (isBeingDragged && touchId == touch.fingerId)
        {
            Vector3 touchPosition = GetTouchWorldPosition(touch.position);
            transform.position = touchPosition + offset;
        }
    }

    void OnMouseUp()
    {
        if (isBeingDragged)
        {
            // 检查是否拖到垃圾桶
            if (trashCan != null)  // 设定一个合适的距离
            {
                float distance = Vector3.Distance(transform.position, trashCan.position);
                if (distance < 2f)
                {
                    // 调用ResetFoodBag
                    OvenScript ovenScript = FindObjectOfType<OvenScript>();
                    if (ovenScript != null)
                    {
                        ovenScript.ResetFoodBag(foodBagId);  // 重置食物袋
                    }
                    Debug.Log("銷毀食物袋 ID: " + foodBagId);
                    AudioManager.instance.PlaySFX("ThrowTrash");
                    gameManager.MinusPlayerFoodFreshValue();
                    return;  // 退出，避免进行ID匹配检查
                }
            } else
            {
                Debug.Log("DragDrop OnMouseUp: trashCan is null");
            }

            List<Passerby> stoppedPasserbys = passerbySpawn.GetStoppedPasserbys();


            foreach (Passerby passerby in stoppedPasserbys)
            {
                if (passerby.currentDisplayedImage.imageObject != null)
                {
                    float distance = Vector3.Distance(transform.position, passerby.currentDisplayedImage.imageObject.transform.position);
                    if (distance < 2f) 
                    {
                        currentPasserby = passerby;
                        if (CheckFoodBagId())
                        {
                            MakePasserbyContinueWalking();
                            break;
                        }   
                    }
                }
            }

            if (currentPasserby == null)
            {
                ResetPosition();
            }
        }

        isBeingDragged = false;
    }

    void OnTouchEnded(Touch touch)
    {
        if (isBeingDragged && touchId == touch.fingerId)
        {
            isBeingDragged = false;
            touchId = null;

            // 检查是否拖到垃圾桶
            if (trashCan != null)
            {
                float distance = Vector3.Distance(transform.position, trashCan.position);
                if (distance < 2f)
                {
                    OvenScript ovenScript = FindObjectOfType<OvenScript>();
                    if (ovenScript != null)
                    {
                        ovenScript.ResetFoodBag(foodBagId);  // 重置食物袋
                    }
                    Debug.Log("销毁食物袋 ID: " + foodBagId);
                    AudioManager.instance.PlaySFX("ThrowTrash");
                    gameManager.MinusPlayerFoodFreshValue();
                    return;  // 退出，避免进行ID匹配检查
                }
            }

            List<Passerby> stoppedPasserbys = passerbySpawn.GetStoppedPasserbys();

            foreach (Passerby passerby in stoppedPasserbys)
            {
                if (passerby.currentDisplayedImage.imageObject != null)
                {
                    float distance = Vector3.Distance(transform.position, passerby.currentDisplayedImage.imageObject.transform.position);
                    if (distance < 2f)
                    {
                        currentPasserby = passerby;
                        if (CheckFoodBagId())
                        {
                            MakePasserbyContinueWalking();
                            break;
                        }
                    }
                }
            }

            if (currentPasserby == null)
            {
                ResetPosition();
            }
        }
    }

    public bool CheckFoodBagId()
    {
        if (isBeingDragged)
        {
            if (currentPasserby.currentDisplayedImage.id == foodBagId)
            {
                Debug.Log("ID Match! FoodBag ID: " + foodBagId + ", Passerby Image ID: " + currentPasserby.currentDisplayedImage.id);
                OvenScript ovenScript = FindObjectOfType<OvenScript>();
                if (ovenScript != null)
                {
                    // 呼叫重置食物袋的方法
                    ovenScript.ResetFoodBag(foodBagId);
                }

                //這裡是把食物交給客人，所以會增加金錢和處理顧客的離開
                AudioManager.instance.PlaySFX("TakeMoney");
                customerGetFood = true;
                player.AddExp(0.2f);
                int addMoney = currentPasserby.currentDisplayedImage.price;
                gameManager.money += addMoney;
                gameManager.moneyText.text = gameManager.money.ToString();
                gameManager.AddPlayerFoodFreshValue();
                SaveMoney();
                gameManager.makedBreads ++;
             
                return true; 
            }
            else
            {
                Debug.Log("ID Mismatch. FoodBag ID: " + foodBagId + ", Passerby Image ID: " + currentPasserby.currentDisplayedImage.id);
                ResetPosition();
                return false;
            }
        }   
        
        return false;
    }

    private void ResetPosition()
    {
        transform.position = originalPosition;
    }

    // 獲取世界坐標下的鼠標位置
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;  // 獲取屏幕坐標
        mouseScreenPosition.z = mainCamera.WorldToScreenPoint(transform.position).z;  // 保持 z 軸不變
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);  // 轉換為世界坐標
    }

    // 获取世界坐标下的触摸点位置
    private Vector3 GetTouchWorldPosition(Vector2 touchPosition)
    {
        Vector3 touchScreenPosition = new Vector3(touchPosition.x, touchPosition.y, mainCamera.WorldToScreenPoint(transform.position).z);
        return mainCamera.ScreenToWorldPoint(touchScreenPosition);
    }

    private void MakePasserbyContinueWalking()
    {
        if (currentPasserby != null && customerGetFood)
        {
            passerbySpawn.GetFoodAndResumePasserby(currentPasserby, currentPasserby.GetStopPoint());
            currentPasserby = null;
            gameManager.AddPlayerCustomerHappyValue();
            customerGetFood = false;
        }
    }

    // SteamApi
    public async void SaveMoney()
    {
        try
        {
            // 創建一個字典來儲存數據
            var data = new System.Collections.Generic.Dictionary<string, object>
            {
                { "PlayerMoney", gameManager.money }
            };

            // 將數據儲存到雲端
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);

            Debug.Log("Money saved to cloud: " + gameManager.money);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save money to cloud: " + e.Message);
        }
    }

    // 從雲端加載金錢
    public async Task LoadMoney()
    {
        try
        {
            // 從雲端加載數據
            var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "PlayerMoney" });

            // 檢查是否有儲存的金錢數據
            if (data.ContainsKey("PlayerMoney"))
            {
                gameManager.money = int.Parse(data["PlayerMoney"].ToString());
                Debug.Log("Money loaded from cloud: " + gameManager.money);
            }
            else
            {
                Debug.Log("No money data found in cloud.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load money from cloud: " + e.Message);
        }
    }

    


    
}
