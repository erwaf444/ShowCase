using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExtraBall : MonoBehaviour
{
    public float minY = -5.5f;
    public float maxVelocity = 7f;

    public float minVelocity = 5f;

    Rigidbody2D rb;


    [HideInInspector]
    public int coins;



    public TextMeshProUGUI coinsText;

    public GameObject youWinPanel;
    public GameObject flag;
    public AudioSource destroyBrickSound;

    public ShopManager shopManager;

    public PlayerMovement playerMovement;

    int brickCount;


    public BouncyBall bouncyBall;
    public AudioSource youWinPanelSound;

   
    

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        // brickCount = GameObject.FindGameObjectsWithTag("Brick").Length;
        brickCount = bouncyBall.brickCount;
        
        StartCoroutine(FirstSpawn());

        // StartCoroutine(HideStartTextAfterDelay());    

        // coins = StaticData.valueToKeep;
        
        // coins = bouncyBall.coins;
        // coinsText.text = bouncyBall.coins.ToString("00000");
   
        
  
    }

    private IEnumerator FirstSpawn() {
        yield return new WaitForSeconds(3);
        rb.velocity = Vector2.down*5f;
    }

    // private IEnumerator  HideStartTextAfterDelay() {
    //     // startText.text = "Get\nReady";
    //     startText.gameObject.SetActive(true);
    //     gameStartPanel.SetActive(true);
    //     yield return new WaitForSeconds(3);
    //     startText.gameObject.SetActive(false);
    //     gameStartPanel.SetActive(false);
    // }

  
    void Update()
    {
        
        if(rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }

        if(rb.velocity.magnitude < minVelocity)
        {
            rb.velocity = rb.velocity.normalized * minVelocity;
        }
    }

  


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Brick")) {
            HandleBrickCollision(collision.gameObject);
        } else if (collision.gameObject.CompareTag("Flag")) {
            HandleFlagCollision(collision.gameObject);
        }
    }

    private void HandleBrickCollision(GameObject brick) {
        Destroy(brick);
        destroyBrickSound.Play();
        bouncyBall.coins += 10;
        shopManager.coins = bouncyBall.coins;
        coinsText.text = bouncyBall.coins.ToString("00000");
        bouncyBall.brickCount--;
        if (bouncyBall.brickCount <= 0) {
            flag.SetActive(true);
        }
    }

    private void HandleFlagCollision(GameObject flag) {
        Destroy(flag);
        youWinPanelSound.Play();
        youWinPanel.SetActive(true);
        Time.timeScale = 0;
        bouncyBall.Pass();
        bouncyBall.SaveCoins();
    }

    
    // public void Pass()
    // {
    //     int currentLevel = SceneManager.GetActiveScene().buildIndex;

    //     if(currentLevel >= PlayerPrefs.GetInt("UnlockedLevel"))
    //     {
    //         PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
    //     }

    //     Debug.Log("Unlocked Level: " + PlayerPrefs.GetInt("UnlockedLevel"));
    // }
    

 



    

    // public void SaveSpeed()
    // {
    //     PlayerPrefs.SetFloat("speed",playerMovement.speed);
    //     Debug.Log("set speed: " + playerMovement.speed);
    // }

    // public void LoadSpeed()
    // {
    //     if (PlayerPrefs.HasKey("speed"))
    //     {
    //         playerMovement.speed = PlayerPrefs.GetFloat("speed");
    //         Debug.Log("get speed: " + playerMovement.speed);
    //     }
    //     else 
    //     {
    //         playerMovement.speed = 20;
    //     }
    // }
    
    // public void ResetSpeed()
    // {
    //     PlayerPrefs.DeleteKey("speed");
    // }

    
}
