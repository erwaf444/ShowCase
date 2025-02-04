using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cat : MonoBehaviour
{
    public GameObject Bowl;
    public GameObject BowlWithFood;

    public GameObject Dialog;
    public TextMeshProUGUI DialogText;
    public LocaleSelector localeSelector;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public TMP_FontAsset TraditionalFontAsset2;//這是當第一個asset不行的時候使用
    public GameObject cat;
    public GameObject catWithEating;
    private string[] dialogs = {
        "喵~",
        "真是美好的一天。",
        "我餓了~",
        "這裡好舒服！",
        "我好想睡覺。"
    };
    private bool canFeed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        catWithEating.SetActive(false);
        Bowl.SetActive(true);
        BowlWithFood.SetActive(false);
        Dialog.SetActive(false);
        StartCoroutine(CheckAndShowDialogRoutine());
    }

    IEnumerator CheckAndShowDialogRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 每隔10秒檢測一次
            float randomValue = Random.Range(0f, 1f);
            Debug.Log(randomValue);
            if (randomValue > 0.01f) // 當隨機值大於0.5時顯示台詞
            {
                ShowRandomDialog();
                yield return new WaitForSeconds(2f); // 顯示2秒
                Dialog.SetActive(false); // 隱藏對話框
            }
        }
    }

    void ShowRandomDialog()
    {
        if (localeSelector.localID == 0)
        {
            DialogText.font = TraditionalFontAsset;
            DialogText.SetAllDirty();
            dialogs = new string[]
            {
                "Meow~",
                "What a beautiful day.",
                "I'm hungry~",
                "It's so comfortable here!",
                "I really want to sleep."
            };
        } else if (localeSelector.localID == 1)
        {
            DialogText.font = TraditionalFontAsset2;
            DialogText.SetAllDirty();
            dialogs = new string[]
            {
                "喵~",
                "真是美好的一天。",
                "我餓了~",
                "這裡好舒服！",
                "我好想睡覺。"
            };
        } else if (localeSelector.localID == 2)
        {
            DialogText.font = SimplifiedFontAsset;
            DialogText.SetAllDirty();
            dialogs = new string[]
            {
                "喵~",
                "真是美好的一天。",
                "我饿了~",
                "这里好舒服！",
                "我好想睡觉。"
            };
        }
        int randomIndex = Random.Range(0, dialogs.Length); // 隨機選擇一個台詞
        string selectedDialog = dialogs[randomIndex];

        
        DialogText.text = selectedDialog; // 將隨機選擇的台詞顯示在TextMeshProUGUI上
        Dialog.SetActive(true); // 確保Dialog物件顯示
        // 檢查是否是特定的台詞
        if (selectedDialog == "I'm hungry~" || selectedDialog == "我餓了~" || selectedDialog == "我饿了~")
        {
            canFeed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowBowlWithFood()
    {
        if (canFeed == true)
        {
            Bowl.SetActive(false);
            BowlWithFood.SetActive(true);
            catWithEating.SetActive(true);
            Dialog.SetActive(false);
            StartCoroutine(CatEatingRoutine()); // 1秒的淡入淡出時間
        } else
        {
            return;
        }
    }

    IEnumerator FadeOutIn(GameObject fadeOutObject, GameObject fadeInObject, float duration)
    {
        Debug.Log("FadeOutIn");
        SpriteRenderer fadeOutRenderer = fadeOutObject.GetComponent<SpriteRenderer>();
        SpriteRenderer fadeInRenderer = fadeInObject.GetComponent<SpriteRenderer>();

        if (fadeOutRenderer == null || fadeInRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on one of the objects!");
            yield break;
        }

        // 激活淡入物體
        fadeInObject.SetActive(true);
        
        float timer = 0f;
        Debug.Log("開始淡出淡入過程");

        Color fadeOutColor = fadeOutRenderer.color;
        Color fadeInColor = fadeInRenderer.color;

        // 同時進行淡出和淡入
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            Debug.Log($"timer: {timer}/{duration}");

            // 控制透明度
            fadeOutRenderer.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, 1f - progress); // 淡出
            fadeInRenderer.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, progress);           // 淡入

            yield return null;
        }
        Debug.Log("淡出淡入過程結束");

        // 保證最終的透明度為正確值
        fadeOutRenderer.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, 0f); // 完全透明
        fadeInRenderer.color = new Color(fadeInColor.r, fadeInColor.g, fadeInColor.b, 1f);    // 完全顯示
        Debug.Log("準備下一段");
        // 淡出後可以選擇隱藏物體（如果需要）
        // fadeOutObject.SetActive(false);
    }


    IEnumerator CatEatingRoutine()
    {
        CatEating();
        yield return new WaitForSeconds(2.0f);
        CatEatingDone();
        Bowl.SetActive(true);
        BowlWithFood.SetActive(false);
        canFeed = false;
    }

    public void CatEating()
    {
        StartCoroutine(FadeOutIn(cat, catWithEating, 1.0f)); // 1秒的淡入淡出時間
    }

    public void CatEatingDone()
    {
        StartCoroutine(FadeOutIn(catWithEating, cat, 1.0f)); // 1秒的淡入淡出時間
    }

}
