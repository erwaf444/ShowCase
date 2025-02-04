using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleWithPowerUp : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject addPaddleWidthPotion;
    public GameObject minusPaddleWidthPotion;
    public GameObject addPaddleBigWidthPotion;
    public GameObject minusPaddleBigWidthPotion;
    public GameObject addBall;
    public GameObject AutoAddCoins10Sec;
    public GameObject Add100Coins;
    public GameObject AutoDestroy5Brick;
    public GameObject AutoDestroyHalfBrick;
    public BouncyBall bouncyBall;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(gameObject.name == "AddPaddleWidthPotion")
            {
                if (playerMovement != null)
                {
                    playerMovement.AddPaddleWidth(); // 调用PlayerMovement中的方法增加挡板宽度
                    
                }
            }
            else if(gameObject.name == "MinusPaddleWidthPotion")
            {
                if (playerMovement != null)
                {
                    playerMovement.MinusPaddleWidth(); // 调用PlayerMovement中的方法减少挡板宽度
                }
            }

            else if (gameObject.name == "AddPaddleBigWidthPotion")
            {
                if (playerMovement != null)
                {
                    playerMovement.AddPaddleBigWidth(); // 调用PlayerMovement中的方法增加挡板宽度
                }
            }

            else if (gameObject.name == "MinusPaddleBigWidthPotion")
            {
                if (playerMovement != null)
                {
                    playerMovement.MinusPaddleBigWidth(); // 调用PlayerMovement中的方法增加挡板宽度
                }
            }

            else if (gameObject.name == "AddBallPotion")
            {
                if (playerMovement != null)
                {
                    playerMovement.AddBall();
                }
            }

            else if (gameObject.name == "AutoAddCoins10Sec")
            {
                if (playerMovement != null)
                {
                    playerMovement.AutoAddCoins10Sec();
                }
            }

            else if (gameObject.name == "Add100Coins")
            {
                if (playerMovement != null)
                {
                    playerMovement.Add100Coins();
                }
            }

            else if (gameObject.name == "AutoDestroy5Brick")
            {
                if (playerMovement != null)
                {
                    playerMovement.AutoDestroy5Brick();
                }
            }

            else if (gameObject.name == "AutoDestroyHalfBrick")
            {
                if (playerMovement != null)
                {
                    playerMovement.AutoDestroyHalfBrick();
                }
            }

            else if (gameObject.name == "AddLives")
            {
                if (bouncyBall != null)
                {
                    bouncyBall.lives += 1;
                }
            }

          
            Destroy(gameObject);
        }
    }

    

    
}
