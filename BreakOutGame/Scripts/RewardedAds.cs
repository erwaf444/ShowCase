using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Collections.Generic;
using System;

 
public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms
    
    public int coins;

    public GameObject afterRewardAdsPanel;

    void Awake()
    {   
        // Get the Ad Unit ID for the current platform:
        #if UNITY_IOS
                _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
                _adUnitId = _androidAdUnitId;
        #endif

        // Disable the button until the ad is ready to show:
        _showAdButton.interactable = false;

        afterRewardAdsPanel.SetActive(false);
    }

    public void Start()
    {
        LoadAd();
    }
 
    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }
 
    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
 
        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }
  
    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }
 
    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            Get1000Coins();
        }
        LoadAd();
        _showAdButton.interactable = true;
    }
 
    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
 
    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }



    // 廣告獎勵不同類型

    public async void Get1000Coins()
    {
        coins = await LoadCoins();
        // ShowAd();
        coins += 1000;
        Debug.Log("Get 1000 coins");
        SaveCoins();
        afterRewardAdsPanel.SetActive(true);

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
}