using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int spriteIndex;
    private Vector3 direction;

    public float gravity = -9.8f;
    public float strength = 5f;


    public bool isShieldActive = false;
    public GameObject shieldVisual;

    public bool isOpacityActive = false;
    [SerializeField] private SkinManager skinManager;
    public bool gravityEnabled = false;
    private Coroutine shieldBlinkCoroutine; 

    void Awake()
    {
        // GetComponent<SpriteRenderer>().sprite = skinManager.GetSelectedSkin().sprite;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Skin selectedSkin = skinManager.GetSelectedSkin();
        spriteRenderer.sprite = selectedSkin.sprite;
        sprites = selectedSkin.animationSprites;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            direction = Vector3.up * strength;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                direction = Vector3.up * strength;
            }
        }

        if (gravityEnabled)
        {
            direction.y += gravity * Time.deltaTime;
        }
        transform.position += direction * Time.deltaTime;
        // Debug.Log("Player size: " + transform.localScale);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Obstacle")
        {
            if (isShieldActive)
            {
                isShieldActive = false;
                StopShieldBlink();
                shieldVisual.SetActive(false);
                Debug.Log("Shield deactivated!");
                return;
            } 
            if (isOpacityActive)
            {
                Debug.Log("Player is invincible due to invisibility!");
                return;
            }
            FindObjectOfType<GameManager>().GameOver();
        } else if (other.gameObject.tag == "Score")
        {
            FindObjectOfType<GameManager>().AddScore();
        }
    }
    void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length)
        {
            spriteIndex = 0;
        }

        spriteRenderer.sprite = sprites[spriteIndex];
    }

    // 开始闪烁效果
    public void StartShieldBlink()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);  // 显示护盾
            if (shieldBlinkCoroutine == null)
            {
                shieldBlinkCoroutine = StartCoroutine(BlinkShield());
            }
        }
    }

    // 停止闪烁效果
    public void StopShieldBlink()
    {
        if (shieldBlinkCoroutine != null)
        {
            StopCoroutine(shieldBlinkCoroutine);
            shieldBlinkCoroutine = null;
            SetShieldOpacity(1f);  // 恢复护盾透明度为1（完全可见）
        }
    }

    // 闪烁协程
    private IEnumerator BlinkShield()
    {
        SpriteRenderer shieldRenderer = shieldVisual.GetComponent<SpriteRenderer>();
        float blinkSpeed = 0.5f;  // 闪烁的速度
        while (true)
        {
            // 让护盾透明度在0和1之间来回变化
            for (float t = 0; t <= 1; t += Time.deltaTime / blinkSpeed)
            {
                SetShieldOpacity(Mathf.Lerp(0f, 1f, t));
                yield return null;
            }
            for (float t = 0; t <= 1; t += Time.deltaTime / blinkSpeed)
            {
                SetShieldOpacity(Mathf.Lerp(1f, 0f, t));
                yield return null;
            }
        }
    }

    // 设置护盾透明度
    private void SetShieldOpacity(float opacity)
    {
        if (shieldVisual != null)
        {
            SpriteRenderer shieldRenderer = shieldVisual.GetComponent<SpriteRenderer>();
            if (shieldRenderer != null)
            {
                Color color = shieldRenderer.color;
                color.a = opacity;
                shieldRenderer.color = color;
            }
        }
    }
    
}
