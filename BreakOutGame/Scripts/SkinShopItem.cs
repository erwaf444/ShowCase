using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class SkinShopItem : MonoBehaviour
{
  [SerializeField] private SkinManager skinManager;
  [SerializeField] private int skinIndex;
  [SerializeField] private Button buyButton;
  [SerializeField] private TextMeshProUGUI costText;
  private Skin skin;
  public BouncyBall bouncyBall;
  public SkinShopController skinShopController;
  public int coins;


  async void Start()
  {
    skin = skinManager.skins[skinIndex];

    GetComponent<Image>().sprite = skin.sprite;
    await CheckSkinUnlock(skinIndex);
    // if (skinManager.IsUnlocked(skinIndex))
    // {
    //     buyButton.gameObject.SetActive(false);    
    // }
    // else
    // {
    //     buyButton.gameObject.SetActive(true);
    //     costText.text = skin.cost.ToString();
    // }
  }
  
  public async Task CheckSkinUnlock(int skinIndex)
  {
      bool IsUnlocked = await skinManager.LoadIsUnlock(skinIndex);

      if (IsUnlocked)
      {
        buyButton.gameObject.SetActive(false);
      } else 
      {
        buyButton.gameObject.SetActive(true);
        costText.text = skin.cost.ToString();
      }
  }


  public async void OnSkinPressed()
  {
    bool IsUnlocked = await skinManager.LoadIsUnlock(skinIndex);
    if (IsUnlocked)
    {
        // skinManager.SaveSelectSkin(skinIndex);
        skinManager.SelectSkin(skinIndex);
    }
  }

  public async void OnBuyButtonPressed()
  {
        coins = await bouncyBall.LoadCoins();
        bool isUnlocked = await skinManager.LoadIsUnlock(skinIndex);

        // Unlock the skin
        if (coins >= skin.cost && !isUnlocked)
        { 
            coins = coins - skin.cost;
            Debug.Log("New coins after deduction: " + coins); 
            skinShopController.coins = skinShopController.coins - skin.cost;
            Debug.Log("New skin shop coins: " + skinShopController.coins); 
            skinShopController.coinsText.text = skinShopController.coins.ToString("00000");
             Debug.Log("Coins text updated to: " + skinShopController.coinsText.text);
            bouncyBall.coins = coins;
            bouncyBall.SaveCoins();
            skinManager.SaveUnlock(skinIndex);
            buyButton.gameObject.SetActive(false);
            // skinManager.SaveSelectSkin(skinIndex);
            skinManager.SelectSkin(skinIndex);
        }
        else
        {
        Debug.Log("Not enough coins :(");
        }
  }
 
  
}
