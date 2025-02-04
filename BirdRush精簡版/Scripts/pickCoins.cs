using UnityEngine;

public class pickCoins : MonoBehaviour
{
    int score;
    private Animator anim;

    void Start()
    {
        // 获取场景中的 Animator 对象
        GameObject animatorObject = GameObject.Find("CoinsImage");
        if (animatorObject != null)
        {
            anim = animatorObject.GetComponent<Animator>();
        }
        score = FindObjectOfType<GameManager>().score;
    }

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("player"))
        {
            Destroy(gameObject);
            SoundManager.PlaySound(SoundType.CoinPickup);
            // anim.SetTrigger("goAnim");

        }
    }

    // Update is called once per frame
    void Update()
    {
        // int currentScore;
        // currentScore = score;
        // if (currentScore != score)
        // {
        // }

    }
}
