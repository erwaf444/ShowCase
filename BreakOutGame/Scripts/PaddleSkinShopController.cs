using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Threading.Tasks;


public class PaddleSkinShopController : MonoBehaviour
{
    [SerializeField] private Image selectedSkin;
    public TextMeshProUGUI coinsText;
    public int coins;
    [SerializeField] private PaddleSkinManager paddleSkinManager;


    // public BouncyBall bouncyBall;

    // public SkinShopItem skinShopItem;
    public Button ShowCoinsButton;
    public GameObject openBox;
    public GameObject closeBox;
    public GameObject openSkinBox;
    public GameObject closeSkinBox;


    
    void Awake()
    {
        coinsText.gameObject.SetActive(false);
        openBox.SetActive(false);
        selectedSkin.enabled = false;
        openSkinBox.SetActive(false);
    }
   

    async void Start()
    {
    }

    async void Update()
    {
        selectedSkin.sprite = paddleSkinManager.GetSelectedPaddleSkin().sprite;
    }

    public async void ShowSkin()
    {
        // selectedSkin.sprite = await skinManager.LoadSkin();
        selectedSkin.sprite = paddleSkinManager.GetSelectedPaddleSkin().sprite;

        selectedSkin.enabled = true;
        openSkinBox.SetActive(true);
        closeSkinBox.SetActive(false);

        StartCoroutine(ShowSkinDelay());
    }

 
    private IEnumerator ShowSkinDelay() {
        yield return new WaitForSeconds(3);
        selectedSkin.enabled = false;
        openSkinBox.SetActive(false);
        closeSkinBox.SetActive(true);

    }

    public async void ShowCoins()
    {
        coins = await LoadCoins();
        coinsText.text = coins.ToString("00000");
        openBox.SetActive(true);
        closeBox.SetActive(false);
        coinsText.gameObject.SetActive(true);
        StartCoroutine(HideCoinsAfterDelay(1f));
    } 

    private IEnumerator HideCoinsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        coinsText.gameObject.SetActive(false);
        openBox.SetActive(false);
        closeBox.SetActive(true);

    }

    // public void LoadMenu() => SceneManager.LoadScene("MainMenuScene");

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
