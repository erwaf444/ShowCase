using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;

using System.Collections.Generic;
using System;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BannerAds : MonoBehaviour
{
    // For the purpose of this example, these buttons are for functionality testing:
    [SerializeField] Button _loadBannerButton;
    [SerializeField] Button _showBannerButton;
    [SerializeField] Button _hideBannerButton;
 
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;
 
    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.
    
    public bool adsRemoved = false;

    void Start()
    {
        // Get the Ad Unit ID for the current platform:
        #if UNITY_IOS
                _adUnitId = _iOSAdUnitId;
        #elif UNITY_ANDROID
                _adUnitId = _androidAdUnitId;
        #endif

        // Disable the button until an ad is ready to show:
        _showBannerButton.interactable = false;
        _hideBannerButton.interactable = false;
 
        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);
 
        // Configure the Load Banner button to call the LoadBanner() method when clicked:
        _loadBannerButton.onClick.AddListener(LoadBanner);
        _loadBannerButton.interactable = true;

        LoadBanner();
        ShowBannerAdRun();
    }
 
    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
 
        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }
  
    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
 
        // Configure the Show Banner button to call the ShowBannerAd() method when clicked:
        _showBannerButton.onClick.AddListener(ShowBannerAd);
        // Configure the Hide Banner button to call the HideBannerAd() method when clicked:
        _hideBannerButton.onClick.AddListener(HideBannerAd);
 
        // Enable both buttons:
        _showBannerButton.interactable = true;
        _hideBannerButton.interactable = true;     
    }
 
    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }
 
    // Implement a method to call when the Show Banner button is clicked:
    public async void ShowBannerAd()
    {
        bool IsRemoved = await LoadAdsRemoved();
        if (!IsRemoved)
        {
            // Set up options to notify the SDK of show events:
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };
    
            // Show the loaded Banner Ad Unit:
            Advertisement.Banner.Show(_adUnitId, options);
        }
 
    }
 
    // Implement a method to call when the Hide Banner button is clicked:
    public void HideBannerAd()
    {
        // Hide the banner:
        Advertisement.Banner.Hide();
    }
 
    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }
 
    void OnDestroy()
    {
        // Clean up the listeners:
        _loadBannerButton.onClick.RemoveAllListeners();
        _showBannerButton.onClick.RemoveAllListeners();
        _hideBannerButton.onClick.RemoveAllListeners();
    }




    public async void SetRemoveAllAds()
    {
        Advertisement.Banner.Hide();

        // 設置 adsRemoved 為 true 並保存到雲端
        await SaveAdsRemoved(true);
    }

    // public async Task CheckRemoveAds()
    // {
    //     bool IsRemoved = await LoadAdsRemoved();
    //     if (IsRemoved)
    //     {
    //         Advertisement.Banner.Hide();
    //         Debug.Log("Ads removed.");

    //     } else
    //     {
    //         Debug.Log("Ads not removed or data load failed.");
    //     }
    // }

    public async Task SaveAdsRemoved(bool removed)
    {
        adsRemoved = removed;
        var data = new Dictionary<string, object> { {"adsRemoved", adsRemoved} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving data: {ex.Message}");
        }
    }

    public async Task<bool> LoadAdsRemoved()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "adsRemoved" });

        if (data.TryGetValue("adsRemoved", out string adsRemovedValue))
        {
            if (bool.TryParse(adsRemovedValue, out bool result))
            {
                adsRemoved = result;
                return adsRemoved;
            }
            else
            {
                Debug.LogError("Failed to parse adsRemoved value.");
            }
        }
        else
        {
            Debug.Log("No data found");
        }

        return false;
    }



    // Show BannerAds Part
    private IEnumerator ShowBannerAdEveryInterval()
    {
        while (true)
        {
            ShowBannerAd();
            yield return new WaitForSeconds(10f);
            HideBannerAd();
            yield return new WaitForSeconds(300f);
        }
    }

    void ShowBannerAdRun()
    {   
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            StartCoroutine(ShowBannerAdEveryInterval());
        } else if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            HideBannerAd();
        }
    }
}