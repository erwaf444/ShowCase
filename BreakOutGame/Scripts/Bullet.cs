using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;




public class Bullet : MonoBehaviour
{
    int coins = 0;

    public TextMeshProUGUI coinsText;

    public GameObject youWinPanel;

    public GameObject flag;
    public AudioSource destroyBrickSound;

    int brickCount;
    
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Brick")) {
            HandleBrickCollision(collision.gameObject);
        } else if (!collision.gameObject.CompareTag("Brick"))
        {
            
        }
    }

    private void HandleBrickCollision(GameObject brick) {
        Destroy(brick);
        Destroy(gameObject);
        destroyBrickSound.Play();
        coins += 10;
        coinsText.text = coins.ToString("00000");
        brickCount--;
        if (brickCount <= 0) {
            flag.SetActive(true);
        }
    }
}
