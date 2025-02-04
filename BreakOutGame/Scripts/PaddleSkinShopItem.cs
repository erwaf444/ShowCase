using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class PaddleSkinShopItem : MonoBehaviour
{
    [SerializeField] private PaddleSkinManager paddleSkinManager;
    [SerializeField] private int skinIndex;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI costText;
    private PaddleSkin paddleSkin;
    public BouncyBall bouncyBall;
    public PaddleSkinShopController paddleSkinShopController;
    public int coins;

 
    async void Start()
    {
        paddleSkin = paddleSkinManager.paddleSkins[skinIndex];

        GetComponent<Image>().sprite = paddleSkin.sprite;       
        await CheckSkinUnlock(skinIndex);

        // if (paddleSkinManager.IsUnlocked(skinIndex))
        // {
        //     buyButton.gameObject.SetActive(false);    
        // }
        // else
        // {
        //     buyButton.gameObject.SetActive(true);
        //     costText.text = paddleSkin.cost.ToString();
        // }
    }

    public async Task CheckSkinUnlock(int skinIndex)
    {
        bool IsUnlocked = await paddleSkinManager.LoadIsUnlock(skinIndex);

        if (IsUnlocked)
        {
            buyButton.gameObject.SetActive(false);
        } else 
        {
            buyButton.gameObject.SetActive(true);
            costText.text = paddleSkin.cost.ToString();
        }
    } 

    public async void OnSkinPressed()
    {
        bool IsUnlocked = await paddleSkinManager.LoadIsUnlock(skinIndex);
        if (IsUnlocked)
        {
            paddleSkinManager.SelectSkin(skinIndex);
        }
    }

    public async void OnBuyButtonPressed()
    {
        coins = await bouncyBall.LoadCoins();
        bool IsUnlocked = await paddleSkinManager.LoadIsUnlock(skinIndex);

        // Unlock the skin
        if (coins >= paddleSkin.cost && !IsUnlocked)
        { 
            coins = coins - paddleSkin.cost;
            Debug.Log("New coins after deduction: " + coins); 
            paddleSkinShopController.coins = paddleSkinShopController.coins - paddleSkin.cost;
            Debug.Log("New skin shop coins: " + paddleSkinShopController.coins); 
            paddleSkinShopController.coinsText.text = paddleSkinShopController.coins.ToString("00000");
            Debug.Log("Coins text updated to: " + paddleSkinShopController.coinsText.text);
            bouncyBall.coins = coins;
            bouncyBall.SaveCoins();
            paddleSkinManager.SaveUnlock(skinIndex);
            buyButton.gameObject.SetActive(false);
            paddleSkinManager.SelectSkin(skinIndex);
        }
        else
        {
        Debug.Log("Not enough coins :(");
        }
    }
}
