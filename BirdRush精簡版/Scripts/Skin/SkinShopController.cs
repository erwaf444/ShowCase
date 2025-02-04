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

 
public class SkinShopController : MonoBehaviour
{
    [SerializeField] private Image selectedSkin;
    [SerializeField] private TextMeshProUGUI birdNameText;
    [SerializeField] private TextMeshProUGUI birdScoreText;
    public TextMeshProUGUI scoreText;
    public int score;
    [SerializeField] private SkinManager skinManager;
    




    
    void Awake()
    {
        selectedSkin.enabled = true;
    }
   

    async void Start()
    {
        scoreText.gameObject.SetActive(true);
        score = await LoadScore();
        scoreText.text = score.ToString();
    }

    async void Update()
    {
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;
        birdNameText.text = skinManager.GetSelectedSkin().birdName;
        birdScoreText.text = skinManager.GetSelectedSkin().birdScore.ToString();
    }
 
    public async void ShowSkin()
    {
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;

        selectedSkin.enabled = true;
        

        StartCoroutine(ShowSkinDelay());
    }

 
    private IEnumerator ShowSkinDelay() {
        yield return new WaitForSeconds(3);
        selectedSkin.enabled = false;


    }




    public async void ShowCoins()
    {
        score = await LoadScore();
        scoreText.text = score.ToString("00000");
      
        scoreText.gameObject.SetActive(true);
        StartCoroutine(HideCoinsAfterDelay(1f));
    }

    private IEnumerator HideCoinsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        scoreText.gameObject.SetActive(false);
      
    }

    // public void LoadMenu() => SceneManager.LoadScene("MainMenuScene");

    public async void SaveScore()
    {   
        var data = new Dictionary<string, object> { {"score", score} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("save score: " + score);
            Debug.Log("scoredata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving scoredata: {ex.Message}");
        }
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
  
}