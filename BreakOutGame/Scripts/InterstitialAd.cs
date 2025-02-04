using UnityEngine;
using UnityEngine.Advertisements;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;
using System;
using UnityEngine.Purchasing;
using UnityEngine.UI;
 
public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;
    bool adsRemoved = false;
 
    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;
    }
  
    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }
  
    // Show the loaded content in the Ad Unit:
    public async Task ShowAd() 
    {
        bool IsRemoved = await LoadAdsRemovedFromCloud();
        if (!IsRemoved)
        {
            // Note that if the ad content wasn't previously loaded, this method will fail
            Debug.Log("Showing Ad: " + _adUnitId);
            Advertisement.Show(_adUnitId, this);
        } else
        {
            Debug.Log("no Ads , cause it is removed");
        }
     
    }
 
    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
    }
 
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }
 
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }
 
    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) { }

    // Method to remove all ads
    public async void RemoveAllAds()
    {
        await SetAdsRemoved(true);
    }

    public async Task SetAdsRemoved(bool removed)
    {
        adsRemoved = removed;
        var data = new Dictionary<string, object> { { "interstitialAdsRemoved", adsRemoved } };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving data: {ex.Message}");
        }
    }

    public async Task<bool> LoadAdsRemovedFromCloud()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "interstitialAdsRemoved" });

        if (data.TryGetValue("interstitialAdsRemoved", out string adsRemovedValue))
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
            Debug.Log("No adsRemoved data found in CloudSave.");
        }

        return false;
    }



}