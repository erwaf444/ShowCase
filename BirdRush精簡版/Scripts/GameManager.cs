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

public class GameManager : MonoBehaviour
{
    public int score;
    public int lastScore;
    public int highscore;
    public TextMeshProUGUI scoreText;
    public GameObject gameover;
    public GameObject playAgainButton;
    public GameObject exitButton;
    public Button pauseButton;
    public GameObject pauseUI;
    public TextMeshProUGUI countdownText; 
    public bool isDoubleScoreActive = false;
    public Pipes pipes;
    public Player player;
    public ShopManager shopManager;

    //暫停菜單按鈕
    public GameObject restartButton;
    public GameObject shopButton;
    public GameObject audioButton;
    public GameObject questionButton;
    public GameObject arrowLeftButton;
    public GameObject exitButtonInPauseMenu;
    public GameObject exitButtonInPauseMenuInfoPanel;
    public GameObject exitButtonInGameOver;
    public GameObject restartButtonInGameOver;

    async void Awake()
    {
        Animator restartButtonAnimator = restartButton.GetComponent<Animator>();
        Animator shopButtonAnimator = shopButton.GetComponent<Animator>();
        Animator audioButtonAnimator = audioButton.GetComponent<Animator>();
        Animator questionButtonAnimator = questionButton.GetComponent<Animator>();
        Animator arrowLeftButtonAnimator = arrowLeftButton.GetComponent<Animator>();
        Animator exitButtonInPauseMenuAnimator = exitButtonInPauseMenu.GetComponent<Animator>();
        Animator exitButtonInPauseMenuInfoPanelAnimator = exitButtonInPauseMenuInfoPanel.GetComponent<Animator>();
        Animator exitButtonInGameOverAnimator = exitButtonInGameOver.GetComponent<Animator>();
        Animator restartButtonInGameOverAnimator = restartButtonInGameOver.GetComponent<Animator>();
        
        restartButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        shopButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        audioButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        questionButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        arrowLeftButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        exitButtonInPauseMenuAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        exitButtonInPauseMenuInfoPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        exitButtonInGameOverAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;  
        restartButtonInGameOverAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        
        await UnityServices.InitializeAsync();
        pipes.speed = 5f;
        score = 0;
    }
    async void Start()
    {
        player.gravityEnabled = false;
        // 開始倒數計時
        StartCoroutine(Countdown(4));
        SoundManager.PlayMusic(SoundType.GameMusic);
        // score = await LoadScore();
        scoreText.text = score.ToString();
        pauseUI.SetActive(false);
        gameover.SetActive(false);
        playAgainButton.SetActive(false);
        exitButton.SetActive(false);
        Pipes[] pipes = FindObjectsOfType<Pipes>();
        for(int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    private IEnumerator Countdown(int seconds)
    {
        // 停止遊戲
        Time.timeScale = 0;
        pauseButton.interactable = false;

        int count = seconds;
        while (count > 0)
        {   
            if (count == 4)
            {
                SoundManager.PlaySound(SoundType.GameStart, 0.5f);
                countdownText.text = "3";
            } else if (count == 3)
            {
                countdownText.text = "2";
            } else if (count == 2)
            {
                countdownText.text = "1";
            } else if (count == 1)
            {
                countdownText.text = "Go!";  // 在最後一秒顯示 "Go"
            }
  
   
            yield return new WaitForSecondsRealtime(1);  // 等待1秒（實際時間）
            count--;
        }

        countdownText.text = "";  // 清除倒數計時文字
        Time.timeScale = 1;  // 開始遊戲
        pauseButton.interactable = true;
        player.gravityEnabled = true;
    }
    
    void Update()
    {
        
    }



    public void AddScore()
    {
        if(isDoubleScoreActive)
        {
            score += 2;
        } else
        {
            score++;
        }
        scoreText.text = score.ToString();
   
    }

    public void PlayAgain()
    {
        gameover.SetActive(false);
        playAgainButton.SetActive(false);
        exitButton.SetActive(false);
        score = 0;
        scoreText.text = score.ToString();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pipes.speed = 5f;
        isDoubleScoreActive = false;
        // player.transform.localScale = new Vector3(1f, 1f, 1f);
    }


    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        // lastScore = score;
        // Debug.Log("Last Score: " + lastScore);
        // int highscore = await LoadScore();
        // if (lastScore > highscore)
        // {
        //     SaveScore();
        // }
        // SaveLastScore();
        Time.timeScale = 1;
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }
 
    public void Continue()
    {
        Time.timeScale = 1;
        pauseUI.SetActive(false);
    }

    public async void GameOver()
    {
        SoundManager.PlaySound(SoundType.GameOver); 
        Debug.Log("Game Over");
        gameover.SetActive(true);
        exitButton.SetActive(true);
        playAgainButton.SetActive(true);
        Time.timeScale = 0;
        lastScore = score;
        Debug.Log("Last Score: " + lastScore);
        highscore = await LoadScore();
        Debug.Log(highscore);
        if (lastScore > highscore)
        {
            Debug.Log("new highscore:" + lastScore);
            SaveScore();
        }
        SaveLastScore();
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


    public async void SaveScore()
    {   
        var data = new Dictionary<string, object> { {"score", lastScore} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("save score: " + lastScore);
            Debug.Log("scoredata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving scoredata: {ex.Message}");
        }
    }

    public async void SaveLastScore()
    {   
        var data = new Dictionary<string, object> { {"lastScore", lastScore} };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("save lastScore: " + lastScore);
            Debug.Log("scoredata saved successfully");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving scoredata: {ex.Message}");
        }
    }

    public async Task<int> LoadLastScore()
    {
        Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "lastScore" });
        if (data.ContainsKey("lastScore"))
        {
            score = int.Parse(data["lastScore"]);
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
