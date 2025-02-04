using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomEvent : MonoBehaviour
{
    public Animator RefrigeratorAnim;
    public GameObject RefrigeratorRedLight2;
    public GameObject WaterRedLight2;
    public GameObject WaterSmokeParticle;
    public GameObject clickArrow1;
    public GameObject clickArrow2;
    public bool ingerdientOut = false;
    public bool waterOut = false;
    public GameObject refrigerator;
    public GameObject water;
    public GameObject randomEventTextPanel;
    public TextMeshProUGUI randomEventText;
    private float blinkSpeed = 1.0f; // 閃爍速度
    public OpenAnim openAnim;
    public GameAnim gameAnim;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomEventTextPanel.SetActive(false);
        randomEventText.text = "";
        clickArrow1.SetActive(false);
        clickArrow2.SetActive(false);
        RefrigeratorRedLight2.SetActive(false);
        WaterRedLight2.SetActive(false);
        WaterSmokeParticle.SetActive(false);
        StartCoroutine(IngredientEventRoutine());
        StartCoroutine(WaterEventRoutine());
        // CheckForIngerdientRandomEvent();
        // CheckForWaterRandomEvent();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(ingerdientOut)
        {
            randomEventTextPanel.SetActive(true);
            randomEventText.color = Color.red;
            randomEventText.text = "食材用完！";
            BlinkText();
        }
        if(waterOut)
        {
            randomEventTextPanel.SetActive(true);
            randomEventText.color = Color.red;
            randomEventText.text = "水用完！";
            BlinkText();
        }


    }

    IEnumerator IngredientEventRoutine()
    {
        while (true)
        {
            CheckForIngerdientRandomEvent();
            yield return new WaitForSeconds(60f); // 每隔5秒執行一次
        }
    }

    void CheckForIngerdientRandomEvent()
    {
        Debug.Log(gameAnim.gameAnimIsEnd);

        if (gameAnim.gameAnimIsEnd)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < 0.05f) // 10%的機率觸發食材用完事件
            {
                TriggerIngredientDepletion();
            }
        }
    }

    void TriggerIngredientDepletion()
    {
        // 保存當前的 SFX 音量
        float originalSFXVolume = AudioManager.instance.sfxSource.volume;
        // 調整 SFX 音量
        AudioManager.instance.sfxSource.volume = originalSFXVolume * 0.25f; // 減少音量，例如調整為原來的一半
        // 延遲一點時間再播放音效，確保音量調整生效
        StartCoroutine(PlayAlertSoundWithDelay(originalSFXVolume));    
        Debug.Log("隨機事件：某些食材已經用完！");
        RefrigeratorRedLight2.SetActive(true);
        StartCoroutine(BlinkRegrigeratorRedLight());
        RefrigeratorAnim.SetTrigger("RefregeratorAnim");
        StartCoroutine(WaitForAnimationToEnd());
        ingerdientOut = true;
        clickArrow1.SetActive(true);
    }

    IEnumerator PlayAlertSoundWithDelay(float originalVolume)
    {
        // 延遲一小段時間，確保音量調整生效
        yield return new WaitForSeconds(0.1f);

        // 播放警告音效
        AudioManager.instance.PlaySFX("Alert");

        // 等待音效播放結束後再恢復音量
        yield return new WaitForSeconds(4f);

        // 恢復 SFX 音量到原始音量
        AudioManager.instance.sfxSource.volume = originalVolume;
    }

    IEnumerator BlinkRegrigeratorRedLight()
    {
        float blinkDuration = 4f; // 閃爍的持續時間 (5秒)
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            RefrigeratorRedLight2.SetActive(!RefrigeratorRedLight2.activeSelf); // 切換狀態 (開/關)
            yield return new WaitForSeconds(0.5f); // 每隔0.5秒閃爍一次
            elapsedTime += 0.5f; // 更新已經過去的時間
        }

        // 確保燈最後是關閉狀態
        RefrigeratorRedLight2.SetActive(false);
    }


    IEnumerator WaitForAnimationToEnd()
    {
        // 等待直到動畫播放完成
        while (RefrigeratorAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f && !RefrigeratorAnim.IsInTransition(0))
        {
            yield return null; // 每幀都檢查一次
        }
        
        ResetRefrigerator();
    }


    void ResetRefrigerator()
    {
        // 把冰箱變成原本的狀態
        Debug.Log("冰箱恢復到原本的狀態！");
        RefrigeratorAnim.SetTrigger("RefregeratorEmpty");
        RefrigeratorRedLight2.SetActive(false);
    }


    IEnumerator WaterEventRoutine()
    {
        while (true)
        {
            CheckForWaterRandomEvent();
            yield return new WaitForSeconds(60f); // 每隔5秒執行一次
        }
    }

    void CheckForWaterRandomEvent()
    {
        if (gameAnim.gameAnimIsEnd)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue < 0.15f) 
            {
                TiggerWaterDepletion();
            }
        }
    }

    void TiggerWaterDepletion()
    {
         // 保存當前的 SFX 音量
        float originalSFXVolume = AudioManager.instance.sfxSource.volume;
        // 調整 SFX 音量
        // 延遲一點時間再播放音效，確保音量調整生效
        StartCoroutine(PlayAlertSoundWithDelay(originalSFXVolume));    
        AudioManager.instance.sfxSource.volume = originalSFXVolume * 0.25f; // 減少音量，例如調整為原來的一半
        Debug.Log("隨機事件：水已經用完！");
        StartCoroutine(BlinkWaterRedLight());
        WaterRedLight2.SetActive(true);
        WaterSmokeParticle.SetActive(true);
        waterOut = true;
        clickArrow2.SetActive(true);
    }

    IEnumerator BlinkWaterRedLight()
    {
        float blinkDuration = 4f; // 閃爍的持續時間 (5秒)
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            WaterRedLight2.SetActive(!WaterRedLight2.activeSelf); // 切換狀態 (開/關)
            yield return new WaitForSeconds(0.5f); // 每隔0.5秒閃爍一次
            elapsedTime += 0.5f; // 更新已經過去的時間
        }

        // 確保燈最後是關閉狀態
        // WaterRedLight2.SetActive(false);
        ResetWater();
    }


    void ResetWater()
    {
        WaterRedLight2.SetActive(false);
        WaterSmokeParticle.SetActive(false);
    }

    public void AddIngredient()
    {
        ingerdientOut = false;
        StartCoroutine(AddIngredientTextRoutine());
    }

    public void AddWater()
    {
        waterOut = false;
        StartCoroutine(AddWaterTextRoutine());
    }

    IEnumerator AddIngredientTextRoutine()
    {
        clickArrow1.SetActive(false);
        SetTextAlpha(1.0f);
        randomEventText.color = Color.blue;
        randomEventText.text = "食材回補！";
        yield return new WaitForSeconds(1.5f);
        randomEventText.text = "";
        randomEventTextPanel.SetActive(false);
        RefrigeratorRedLight2.SetActive(false);
    }

    IEnumerator AddWaterTextRoutine()
    {
        clickArrow2.SetActive(false);
        SetTextAlpha(1.0f);
        randomEventText.color = Color.blue;
        randomEventText.text = "水回補！";
        yield return new WaitForSeconds(1.5f);
        randomEventText.text = "";
        randomEventTextPanel.SetActive(false);
        WaterRedLight2.SetActive(false);
        WaterSmokeParticle.SetActive(false);
    }

    private void BlinkText()
    {
        // 使用 PingPong 函數來改變透明度，達到閃爍效果
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);
        SetTextAlpha(alpha);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = randomEventText.color;
        color.a = alpha;
        randomEventText.color = color;
    }
    
}
