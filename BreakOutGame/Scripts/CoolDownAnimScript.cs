using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownAnimScript : MonoBehaviour
{
    public PotionShopManager potionShopManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopAnim()
    {
        potionShopManager.HidePotionCoolDownAnim();
    }

    public void StopUsePotionAnim()
    {   
        potionShopManager.HideUsePotionAnim();
    }

    public void StopUsePotionWithCoinsAnim()
    {
        potionShopManager.HideUsePotionWithCoinsAnim();
    }

    public void StopUsePotionWithHeartAnim()
    {
        potionShopManager.HideUsePotionWithHeartAnim();
    }

    public void StopUsePotionWithPaddleBigAnim()
    {
        potionShopManager.HideUsePotionWithPaddleBigAnim();
    }

    public void StopUsePotionWithPaddleSmallAnim()
    {
        potionShopManager.HideUsePotionWithPaddleSmallAnim();
    }

    public void StopUsePotionWithAddBallAnim()
    {
        potionShopManager.HideUsePotionWithAddBallAnim();
    }
}
