using System.Collections;
using UnityEngine;

public class PotionScript : MonoBehaviour
{
    public GameObject Player;
    public Pipes pipes;
    public GameManager gameManager;
    public Player player;
    public Spawner spawner;
    public float minSpawnRate = 0.5f; // 最小生成間隔
    public float maxSpawnRate = 2f; // 最大生成間隔

    void Start()
    {
        
    }


    void Update()
    {
        
    }
 
    public void AddShield(float duration)
    {
        if (!player.isShieldActive) // 如果盾牌未激活
        {
            StartCoroutine(ActivateShield(duration));
        }
    }

    private IEnumerator ActivateShield(float duration)
    {
        player.isShieldActive = true;

        // 启用视觉效果
        if (player.shieldVisual != null)
        {
            player.shieldVisual.SetActive(true);
            player.StartShieldBlink();
            player.shieldVisual.transform.SetParent(player.transform, false);
            player.shieldVisual.transform.localPosition = Vector3.zero;
            player.shieldVisual.transform.localScale = new Vector3(1.5f, 1.5f, -100f); // 根据需要调整X、Y、Z的比例

            Debug.Log("Shield visual enabled and set to follow player");

        }

        // 在持续时间内保持盾牌激活
        yield return new WaitForSeconds(duration);
        Debug.Log("Shield deactivated after duration");


        // 取消盾牌效果
        player.isShieldActive = false;

        // 禁用视觉效果
        if (player.shieldVisual != null)
        {
            player.shieldVisual.SetActive(false);
            player.StopShieldBlink();
            player.shieldVisual.transform.SetParent(null);
            Debug.Log("Shield visual disabled and detached from player");

        }
    }


    public void AddSpeed()
    {
        spawner.OnDisable();
        spawner.SpawnWithDelay(2f);
        pipes.speed += 5f;
        StartCoroutine(ResetAddSpeed());
    }

    public void MinusSpeed()
    {
        spawner.OnDisable();
        pipes.speed -= 2.5f;
        spawner.SpawnWithDelay(2f);
        StartCoroutine(ResetMinusSpeed());
    }

    public void DoubleScore()
    {
        gameManager.isDoubleScoreActive = true;
        StartCoroutine(ResetDoubleScore());
    }

    public void TurnSmallFunc()
    {
        TurnSmall(0.5f, 30f);
        Debug.Log("TurnSmall function start");
    }

    public void TurnOpacity()
    {
        TurnOpacity(30f);
    }

    private IEnumerator ResetAddSpeed()
    {
        yield return new WaitForSeconds(10f);
        pipes.speed = 5f;
        spawner.OnDisable();
        yield return new WaitForSeconds(2f);
        spawner.OnEnable();
    }

    private IEnumerator ResetMinusSpeed()
    {
        yield return new WaitForSeconds(10f);
        pipes.speed = 5f;
        spawner.OnDisable();
        yield return new WaitForSeconds(5f);
        spawner.OnEnable();
    }

    private IEnumerator ResetDoubleScore()
    {
        yield return new WaitForSeconds(30f);
        gameManager.isDoubleScoreActive = false;
    }

    public void TurnSmall(float scaleMultiplier, float duration)
    {
        StartCoroutine(TurnSmallRoutine(scaleMultiplier, duration));
    }

    private IEnumerator TurnSmallRoutine(float scaleMultiplier, float duration)
    {
        // 保存原始大小
        Vector3 originalScale = player.transform.localScale;

        // 缩小玩家
        player.transform.localScale = originalScale * scaleMultiplier;

        // 等待持续时间结束
        yield return new WaitForSeconds(duration);

        // 恢复原始大小
        player.transform.localScale = originalScale;
    }

    // public void TurnOpacity(float opacity, float duration)
    // {
    //     StartCoroutine(TurnOpacityRoutine(opacity, duration));
    // }

    // private IEnumerator TurnOpacityRoutine(float opacity, float duration)
    // {
    //     player.isOpacityActive = true;
    //     // 获取玩家的SpriteRenderer组件
    //     SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();

    //     // 保存原始颜色
    //     Color originalColor = spriteRenderer.color;

    //     // 设置新的透明度
    //     spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, opacity);

    //     // 等待持续时间结束
    //     yield return new WaitForSeconds(duration);

    //     // 恢复原始透明度
    //     spriteRenderer.color = originalColor;

    //     player.isOpacityActive = false;
    // }

    public void TurnOpacity(float duration)
    {
        StartCoroutine(TurnOpacityRoutine(duration));
    }

    private IEnumerator TurnOpacityRoutine(float duration)
    {
        player.isOpacityActive = true;

        // 获取玩家的SpriteRenderer组件
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();

        // 保存原始颜色
        Color originalColor = spriteRenderer.color;

        // 定义闪烁的频率
        float blinkSpeed = 0.5f; // 你可以根据需要调整闪烁速度
        float elapsedTime = 0f;  // 用于跟踪时间

        // 开始闪烁效果，持续指定的时间
        while (elapsedTime < duration)
        {
            // 闪烁透明度在0和1之间循环变化
            for (float t = 0; t <= 1; t += Time.deltaTime / blinkSpeed)
            {
                float alpha = Mathf.Lerp(0f, 1f, t);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
            for (float t = 0; t <= 1; t += Time.deltaTime / blinkSpeed)
            {
                float alpha = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            elapsedTime += Time.deltaTime;
        }

        // 恢复原始透明度
        spriteRenderer.color = originalColor;
        player.isOpacityActive = false;
    }


}
