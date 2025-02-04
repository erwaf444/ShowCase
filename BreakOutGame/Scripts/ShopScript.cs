using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using UnityEngine.Purchasing;
using UnityEngine.UI;

[Serializable]
public class ConsumableItems
{
    public string Name;
    public string Id;
    public string Description;
    public float Price;
}

[Serializable]
public class NonConsumableItems
{
    public string Name;
    public string Id;
    public string Description;
    public float Price;
}

[Serializable]
public class SubscriptionItems
{
    public string Name;
    public string Id;
    public string Description;
    public float Price;
    public int TimeDuration; //in Days
}

public class ShopScript : MonoBehaviour, IStoreListener
{
    IStoreController m_StoreController;
    public ConsumableItems cItem;
    public NonConsumableItems ncItem;
    public SubscriptionItems sItem;

    public BouncyBall bouncyBall;
    public int coins;
    public TextMeshProUGUI coinsText;
    public GameObject openBox;
    public GameObject closeBox;
    public BannerAds bannerAds;
    public InterstitialAd interstitialAd;   
    public GameObject VIPIcon;

    public TextMeshProUGUI ExpiredDayText;
    
    public void Awake()
    {
        VIPIcon.SetActive(false);
    }

    async void Start()
    {
        coinsText.gameObject.SetActive(false);
        openBox.SetActive(false);
        SetupBuilder();
    }

    private void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(cItem.Id, ProductType.Consumable);
        builder.AddProduct(ncItem.Id, ProductType.NonConsumable);
        builder.AddProduct(sItem.Id, ProductType.Subscription);
    
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Successfully initialized IAP");
        m_StoreController = controller;
        CheckNonConsumable(ncItem.Id);
        CheckSubscription(sItem.Id);
    }

    public void Consumable_Btn_Pressed()
    {
        m_StoreController.InitiatePurchase(cItem.Id);
    }

    public void NonConsumable_Btn_Pressed()
    {
        m_StoreController.InitiatePurchase(ncItem.Id);
    }

    public void Subscription_Btn_Pressed()
    {
        m_StoreController.InitiatePurchase(sItem.Id);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        Debug.Log("Purchase Complete " + product.definition.id);

        if (product.definition.id == cItem.Id)
        {
            PurchaseForAdd1000Coins();
        }
        else if (product.definition.id == ncItem.Id)
        {
            PurserchaseForRemoveAllAds();
        }
        else if (product.definition.id == sItem.Id)
        {
            ActivateKnightPass();
        }

        return PurchaseProcessingResult.Complete;

    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Initialization Failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP Initialization Failed: " + error + " - " + message);
    }

   
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("IAP Purchase Failed: " + product.definition.id + " - " + failureReason);
    }

    public async void ShowCoins()
    {
        coins = await LoadCoins();
        coinsText.text = coins.ToString("00000");
        coinsText.gameObject.SetActive(true);
        openBox.SetActive(true);
        closeBox.SetActive(false);
        StartCoroutine(ShowCoinsAfterDelay(3f));
    }

    IEnumerator ShowCoinsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        coinsText.gameObject.SetActive(false);
        openBox.SetActive(false);
        closeBox.SetActive(true);
    }

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


    public async void PurchaseForAdd1000Coins()
    {
        coins = await LoadCoins();
        coins = coins + 2000;
        Debug.Log("Coins: " + coins);
        SaveCoins();
        Debug.Log("Coins saved: " + coins);
    }

    public async void PurserchaseForRemoveAllAds()
    {
        bannerAds.SetRemoveAllAds();
        interstitialAd.RemoveAllAds();

    }

    public async void ActivateKnightPass()
    {
        bannerAds.SetRemoveAllAds();
        interstitialAd.RemoveAllAds();
        coins = await LoadCoins();
        coins = coins + 3000;
        await SaveKnightPass(true);
        SaveCoins();
        VIPIcon.SetActive(true);
        Debug.Log("Activate Knight Pass");
 

    }

    public async Task SaveKnightPass(bool knightPass)
    {   
        bool isKnightPass = knightPass;
        var data = new Dictionary<string, object> { {"knightPass", isKnightPass} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("save knightPass: " + isKnightPass);
            Debug.Log("knightPass saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving knightPass: {ex.Message}");
        }
    }

    public async Task<bool> LoadKnightPass()
    {
         try
        {
            // 从 CloudSave 中加载数据
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "knightPass" });

            // 检查数据字典中是否包含 "knightPass" 键
            if (data.TryGetValue("knightPass", out string knightPassValue))
            {
                // 将从 CloudSave 中加载的数据转换为布尔值
                bool isKnightPass = bool.Parse(knightPassValue);
                Debug.Log("Loaded knightPass: " + isKnightPass);
                return isKnightPass;
            }
            else
            {
                Debug.LogWarning("No data found for key: knightPass");
                return false; // 或者根据需要返回其他默认值
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading knightPass: {ex.Message}");
            return false; // 或者根据需要返回其他默认值
        }
      
        
    }

    void CheckNonConsumable(string id)
    {
        if (m_StoreController != null)
        {
            var product = m_StoreController.products.WithID(id);
            if (product != null)
            {
                if (product.hasReceipt)
                {
                    bannerAds.SetRemoveAllAds();
                    interstitialAd.RemoveAllAds();
                    Debug.Log("Remove All Ads");
                } 
                else 
                {
                    Debug.Log("Not purchased yet");
                }
            }
        }
    }

    void CheckSubscription(string id)
    {
        var subProduct = m_StoreController.products.WithID(id);
        if (subProduct != null)
        {
            try
            {
                if (subProduct.hasReceipt)
                {
                    var subManager = new SubscriptionManager(subProduct, null);
                    var info = subManager.getSubscriptionInfo();
                    Debug.Log(info.getExpireDate());

                    if (info.isSubscribed() == Result.True)
                    {
                        ActivateKnightPass();
                        VIPIcon.SetActive(true);
                        Debug.Log("Activate Knight Pass");


                        // For Test            
                        DateTime expireDate = info.getExpireDate(); 
                        string expireDateString = expireDate.ToString("yyyy-MM-dd HH:mm:ss");
                        ExpiredDayText.text = "Expiration Date: " + expireDateString;

                    }
                    else 
                    {
                        Debug.Log("Not purchased yet");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }




    

   
}
