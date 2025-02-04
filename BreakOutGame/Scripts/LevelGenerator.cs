using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.CloudSave;

public class LevelGenerator : MonoBehaviour
{
    public Vector2Int size;
    public Vector2 offset;
    public GameObject birckPrefab;
    public Gradient gradient;

    public BouncyBall bouncyBall;
    public ShopManager shopManager;
    public PotionShopManager potionShopManager;
    public InterstitialAd interstitialAd;
    public GameObject ContinueButton;
    public GameObject GameStartPanel;
    // public TextMeshProUGUI GameStartPanelText;
    public LoadingManager loadingManager;

    

    private async void Awake()
    {
        // for (int i = 0; i < size.x; i++)
        // {
        //     for (int j = 0; j < size.y; j++)
        //     {
        //         GameObject newBrick = Instantiate(birckPrefab, transform);
        //         newBrick.transform.position = transform.position + new Vector3(((float)(size.x-1)*.5f-i) * offset.x, j * offset.y, 0);
        //         newBrick.GetComponent<SpriteRenderer>().color = gradient.Evaluate((float)j / (size.y-1));
        //     }    
        // }

        bool IsRemoved = await interstitialAd.LoadAdsRemovedFromCloud();
        if (SceneManager.GetActiveScene().name == "Level1" && IsRemoved)
        {
            ContinueButton.SetActive(false);
            GameStartPanel.SetActive(false);
        } 
        else if (SceneManager.GetActiveScene().name != "Level1" && !IsRemoved)
        {
            ContinueButton.SetActive(true);
            GameStartPanel.SetActive(false);
            Time.timeScale = 0;
        }
        else if (SceneManager.GetActiveScene().name == "Level1" && !IsRemoved)
        {
            GameStartPanel.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name != "Level1" && IsRemoved)
        {
            ContinueButton.SetActive(false);
            GameStartPanel.SetActive(true);
            // Time.timeScale = 0;
        }

      
   
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void Restart()
    {
        bool IsRemoved = await interstitialAd.LoadAdsRemovedFromCloud();
        if (!IsRemoved)
        {
            await interstitialAd.ShowAd();
            ContinueButton.SetActive(true);
            Time.timeScale = 0;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // public void NextLevel()
    // {
    //     Time.timeScale = 1;
    //     int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    //     SceneManager.LoadScene(nextSceneIndex);
        
    // }

    public async void NextLevel()
    {
            bouncyBall.Pass();

            bool IsRemoved = await interstitialAd.LoadAdsRemovedFromCloud();
            if (!IsRemoved)
            {
                await interstitialAd.ShowAd();
                ContinueButton.SetActive(true);
                Time.timeScale = 0;
            }
            

            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            Debug.Log("Attempting to load next level: " + nextSceneIndex);
            bouncyBall.coins = shopManager.coins;
            bouncyBall.coins = potionShopManager.coins;
            int coinsDataToKeep = bouncyBall.coins;
            StaticData.valueToKeep = coinsDataToKeep;

            if (IsRemoved)
            {
                Time.timeScale = 1;
            }
            
            loadingManager.LoadLevelToLevel(nextSceneIndex);
            // SceneManager.LoadScene(nextSceneIndex);


         
    }

    public void ContinueGameAfterAds()
    {
        Time.timeScale = 1;
        ContinueButton.SetActive(false);
        GameStartPanel.SetActive(true);
    }

    private async Task LoadAds()
    {
        Debug.Log("Display Ads");
        interstitialAd.LoadAd();
        Debug.Log("Loaded Ad");
    }

    private async Task DisplayAds()
    {     
        Debug.Log("Showing Ad");
        interstitialAd.ShowAd();
        Debug.Log("Showed Ad");
    }

    //當玩家到達最後一關的時候
    public void BackToMainMenu()
    {
        loadingManager.LoadToMainMenu(1);
    }

}
