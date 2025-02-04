using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Passerby : MonoBehaviour
{
    public GameObject passerbyPrefab;
    public GameObject pointA;
    public GameObject pointB;
    public Rigidbody2D rb;
    Transform currentPoint;
    public float speed;
    public bool shouldStop;
    public Transform stopPoint; 

    public GameObject imagePrefab; // UI Image prefab to display above the passerby
    [System.Serializable]
    public struct displayedImage
    {
        public Sprite sprite;
        public int id;
        public GameObject imageObject;
        public int price;
    }
    public displayedImage currentDisplayedImage; // 使用结构体来保存显示图片的信息


    private float blinkSpeed = 1f; // 控制閃爍速度
    private float minAlpha = 0.1f; // 最小透明度
    private float maxAlpha = 1f; // 最大透明度
    public TimeSystem timeSystemScript;



    

    void Start()
    {
        timeSystemScript = FindObjectOfType<TimeSystem>();
        if (timeSystemScript == null)
        {
            Debug.LogError("TimeSystem not found!");
        }
        rb = GetComponent<Rigidbody2D>();
        pointA = GameObject.Find("PointA");
        pointB = GameObject.Find("PointB");
        currentPoint = pointA.transform;
        imagePrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPoint == pointA.transform)
        {
            rb.velocity = new Vector2(speed, 0f);
        }  
      
        if(Vector2.Distance(transform.position, pointB.transform.position) < 0.5f && currentPoint == pointA.transform)
        {
            currentPoint = pointB.transform;
        }
 
        if (shouldStop && stopPoint != null)
        {
            if (Vector2.Distance(transform.position, stopPoint.position) < 0.5f)
            {
                rb.velocity = Vector2.zero;
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color color = spriteRenderer.color;
                    color.a = 0.75f;  // 恢复为完全不透明
                    spriteRenderer.color = color;
                    spriteRenderer.color = new Color(0.678f, 0.847f, 0.902f, color.a); //變成紅色
                }

                // 如果存在显示的图片，则开始闪烁
                if (currentDisplayedImage.imageObject != null)
                {
                    StartCoroutine(BlinkImage());
                }
             

            }
        }


        if (shouldStop)
        {   
            GetComponent<Collider2D>().enabled = false;  // 禁用碰撞体
        } else
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;  // 恢复为完全不透明
                spriteRenderer.color = color;
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // 设置为白色，完全不透明

            }
            GetComponent<Collider2D>().enabled = true;  // 启用碰撞体
        }

        // 确保显示的图片跟随路人移动
        if (currentDisplayedImage.imageObject != null)
        {
            currentDisplayedImage.imageObject.transform.position = transform.position + new Vector3(0f, 2.5f, 0); // 调整偏移量使图片位于路人上方
        }

        if (timeSystemScript.isDayEnded)
        {
            ResetPasserby();
        }

        
      
    }

    private IEnumerator BlinkImage()
    {
        // 检查 imageObject 是否存在
        if (currentDisplayedImage.imageObject == null)
        {
            yield break; // 如果为 null，结束协程
        }

        SpriteRenderer displayedImageRenderer = currentDisplayedImage.imageObject.GetComponent<SpriteRenderer>();
        
         // Check if the SpriteRenderer is null
        if (displayedImageRenderer == null)
        {
            yield break; // If it's null, exit the coroutine
        }

        if (displayedImageRenderer != null)
        {
            while (true)
            {
                // 閃爍效果
                yield return StartCoroutine(FadeSprite(displayedImageRenderer, maxAlpha, minAlpha));
                yield return StartCoroutine(FadeSprite(displayedImageRenderer, minAlpha, maxAlpha));

            }
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer spriteRenderer, float startAlpha, float endAlpha)
    {
        // 检查 spriteRenderer 是否为 null
        if (spriteRenderer == null)
        {
            yield break; // 如果为 null，结束协程
        }

        float elapsedTime = 0f;
        float duration = 1f / blinkSpeed;
        Color startColor = spriteRenderer.color;
        startColor.a = startAlpha;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);

        while (elapsedTime < duration)
        {
            if (spriteRenderer == null)
            {
                yield break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
      

         // Final assignment
        if (spriteRenderer != null)
        {
            spriteRenderer.color = endColor; // Only set the color if it's still valid
        }
    }

    

  
 
    public Transform GetStopPoint()
    {
        return stopPoint; // 返回当前停留点
    }

    public void SetStopPoint(Transform stopPoint)
    {
        this.stopPoint = stopPoint;
        shouldStop = true;
        Debug.Log("Stop point: " + stopPoint);
        Debug.Log("Should stop: " + shouldStop);
    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == pointA)
        {
            currentPoint = pointA.transform;
        }
        else if (other.gameObject == pointB)
        {
            Destroy(this.gameObject);
        }
    }

    public void ShowImage(Sprite sprite, int id, int price)
    {
        Debug.Log("ShowImage Start");
        if (currentDisplayedImage.imageObject != null)
        {
            Destroy(currentDisplayedImage.imageObject); // 移除现有的图片
        }

        Debug.Log("Sprite: " + sprite);

        currentDisplayedImage.imageObject = Instantiate(imagePrefab, transform.position + new Vector3(0f, 2.5f, 0), Quaternion.identity);

        SpriteRenderer spriteRenderer = currentDisplayedImage.imageObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            currentDisplayedImage.sprite = sprite; // 保存 sprite 到结构体
            currentDisplayedImage.id = id; // 保存 id 到结构体
            currentDisplayedImage.price = price;
            Debug.Log("id:" + currentDisplayedImage.id);
            Debug.Log("sprite: " + spriteRenderer.sprite);



            // 目標大小，假設我們希望所有圖片的高度或寬度都為 1 單位
            float targetSize = 1.0f;

            // 獲取 Sprite 的邊界大小
            float spriteHeight = sprite.bounds.size.y;
            float spriteWidth = sprite.bounds.size.x;

            // 計算縮放比例，讓寬度或高度符合 targetSize
            float scaleFactor = targetSize / Mathf.Max(spriteHeight, spriteWidth);

            // 調整 Sprite 大小，保持寬高比例一致
            currentDisplayedImage.imageObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1) * 1.5f;
        }
    }

    public void HideImage()
    {
        /// 4. 清空 currentDisplayedImage.imageObject 的引用，以免記憶體洩露
        /// </remarks>
        if (currentDisplayedImage.imageObject != null)
        {
            StopAllCoroutines(); 
            currentDisplayedImage.imageObject.SetActive(false);
            Destroy(currentDisplayedImage.imageObject);
            currentDisplayedImage.imageObject = null; // 清空引用
        }
       
    }

    IEnumerator DestroyCurrentDisplayedImage()
    {
        yield return new WaitForSeconds(5f);
        if (currentDisplayedImage.imageObject != null)
        {
            Destroy(currentDisplayedImage.imageObject);
        }
        currentDisplayedImage.imageObject = null; // 清空引用
    }

    public void ResetPasserby()
    {
        Destroy(this.gameObject);
        HideImage();
    }

   
}
