
using UnityEngine;

public class Coin : MonoBehaviour
{

    public ShopManager shopManager;
    int value;
    
    private void Start(){
        value = 10;
    }
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player")){
            shopManager.coins += value;
            Destroy(gameObject);
        }
    }
}
