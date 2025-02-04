using UnityEngine;
using System.Collections.Generic;
using Steamworks;

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

    void OnMouseDown()
    {
        // 計算鼠標與物件之間的偏移
        // Debug.Log("OnMouseDown called");
        isBeingDragged = true;  // 標誌開始拖動
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;
    }

    void OnMouseDrag()
    {
        // 更新物件位置，使其跟隨鼠標
        // Debug.Log("OnMouseDrag called");
        Vector3 mousePosition = GetMouseWorldPosition();
        transform.position = mousePosition + offset;
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
                // SaveMoney();
                gameManager.makedBreads ++;
                if (gameManager.makedBreads == 1)
                {
                    gameManager.UnlockAchievement("FIRST_BREAD");
                }
                if (gameManager.makedBreads == 10)
                {
                    gameManager.UnlockAchievement("FIRST_10_BREAD");
                }

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
    public void SaveMoney()
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat("Player_Money", gameManager.money);
            bool success = SteamUserStats.StoreStats();
            
            if (success)
            {
                Debug.Log("Money saved to Steam Cloud: " + gameManager.money);
            }
            else
            {
                Debug.LogWarning("Failed to store stats to Steam Cloud.");
            }
        }
        else
        {
            Debug.LogWarning("Steam is not initialized. Unable to save money.");
        }
    }

    


    
}
