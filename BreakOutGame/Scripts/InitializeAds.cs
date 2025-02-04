using UnityEngine;
using UnityEngine.Advertisements;
 
 using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;

using System.Collections.Generic;
using System;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class InitializeAds : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = false;
    private string _gameId;
 
    void Awake()
    {

        InitializeUnityAds();
    }
 
    public void InitializeUnityAds()
    {
        #if UNITY_IOS
                _gameId = _iOSGameId;
        #elif UNITY_ANDROID
                _gameId = _androidGameId;
        #elif UNITY_EDITOR
                _gameId = _androidGameId; //Only for testing the functionality in the Editor
        #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

  
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }
 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    

     
    
}
