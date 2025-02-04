using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GetValue : MonoBehaviour
{
    public BouncyBall bouncyBall;
    public ShopManager shopManager;
    public PotionShopManager potionShopManager;
    // public TextMeshProUGUI coinsText;
    

   
    // Start is called before the first frame update
    void Start()
    {

        // if (bouncyBall != null && coinsText != null)
        // {
        //     coinsText.text = bouncyBall.coins.ToString();
        // }
        // else
        // {
        //     Debug.LogWarning("bouncyBall 或 coinsText 为空，无法更新 coinsText 的显示。");
        // }
    }

    public void LoadSceneAndKeepValue()
    {
        bouncyBall.coins = shopManager.coins;
        bouncyBall.coins = potionShopManager.coins;
        int coinsDataToKeep = bouncyBall.coins;
        StaticData.valueToKeep = coinsDataToKeep;


        // SceneManager.LoadScene("Level1");
        SceneManager.LoadScene("Level2");
        // SceneManager.LoadScene("Level3");
        // SceneManager.LoadScene("Level4");
        // SceneManager.LoadScene("Level5");
        // SceneManager.LoadScene("Level6");
        // SceneManager.LoadScene("Level7");
        // SceneManager.LoadScene("Level8");
        // SceneManager.DropTable("Level9");
        // SceneManager.LoadScene("Level10");
        // SceneManager.LoadScene("Level11");
    }

  

  
}
