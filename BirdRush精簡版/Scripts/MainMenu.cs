using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;

public class MainMenu : MonoBehaviour
{
    public GameObject ButtonContainer;  

    private bool isActive = true;  
    public int score;
    public TextMeshProUGUI scoreText;

    public int lastScore;
    public TextMeshProUGUI lastScoreText;

    async void Start()
    {
        isActive = ButtonContainer.activeSelf;  
        score = await LoadScore();
        scoreText.text = score.ToString();

        lastScore = await LoadLastScore();
        lastScoreText.text = lastScore.ToString();

        Debug.Log("Score: " + score);
        Debug.Log("Last Score: " + lastScore);
    }

    void Update()
    {
        // 檢測是否有觸摸
        if (Input.touchCount > 0)
        {
            // 獲取第一個觸摸點
            Touch touch = Input.GetTouch(0);

            // 檢查觸摸開始時是否在 ButtonContainer 範圍內
            if (touch.phase == TouchPhase.Began)
            {
                if (ButtonContainer.activeSelf && !IsPointerOverUIObject(ButtonContainer, touch.position))
                {
                    // 如果觸摸點不在範圍內，禁用 ButtonContainer
                    ButtonContainer.SetActive(false);
                    isActive = false;
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            // 檢查點擊是否在 ButtonContainer 範圍內
            if (ButtonContainer.activeSelf && !IsPointerOverUIObject(ButtonContainer, Input.mousePosition))
            {
                // 如果點擊不在範圍內，禁用 ButtonContainer
                ButtonContainer.SetActive(false);
                isActive = false;
            }
        }
    }
    

    public void ToggleButtonContainer()
    {
        isActive = !isActive;  
        ButtonContainer.SetActive(isActive); 
    }

    // 檢查觸摸點是否在 UI 物體上
    private bool IsPointerOverUIObject(GameObject target, Vector2 touchPosition)
    {
        RectTransform rectTransform = target.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, touchPosition, Camera.main);
    }

    public async Task<int> LoadScore()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "score" });
        if (data.ContainsKey("score"))
        {
            score = int.Parse(data["score"]);
            Debug.Log("Loaded score: " + score);
            return score;
        }
        else
        {
            Debug.Log("No score data found");
            return 0;
        }
    }

    public async Task<int> LoadLastScore()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "lastScore" });
        if (data.ContainsKey("lastScore"))
        {
            lastScore = int.Parse(data["lastScore"]);
            Debug.Log("Loaded lastScore: " + lastScore);
            return lastScore;
        }
        else
        {
            Debug.Log("No score data found");
            return 0;
        }
    }
}
