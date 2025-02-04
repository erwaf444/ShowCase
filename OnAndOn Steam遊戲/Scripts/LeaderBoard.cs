using UnityEngine;
using Steamworks;
using TMPro;


public class LeaderBoard : MonoBehaviour
{
    private SteamLeaderboard_t leaderboardHandle;
    private SteamLeaderboardEntries_t leaderboardEntries;
    private CallResult<LeaderboardFindResult_t> m_callbackLeaderboardFound;
    private CallResult<LeaderboardScoresDownloaded_t> m_callbackLeaderboardScores;
    // public TextMeshProUGUI playerNameText;
    // public TextMeshProUGUI scoreText;
    public GameObject leaderBoard;
    public GameManager gameManager;
    public GameObject leaderboardEntryPrefab; // 排行榜項目預製物
    public Transform leaderboardContentParent; // 排行榜項目的父物件（有 Layout Group 的 Panel）


    

    void Start()
    {
        // 初始化回調
        m_callbackLeaderboardFound = CallResult<LeaderboardFindResult_t>.Create();
        m_callbackLeaderboardScores = CallResult<LeaderboardScoresDownloaded_t>.Create();

        // FetchLeaderboard();
    }

    void Update()
    {
        SteamAPI.RunCallbacks();

    }

    public void UploadScore(int time)
    {
        Debug.Log("上傳分數中");
        // 使用Steam排行榜API上傳分數
        SteamUserStats.UploadLeaderboardScore(
            leaderboardHandle, 
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, 
            time,
            null,
            0
        );
        Debug.Log("上傳的時間"+time);

      
        Debug.Log("上傳分數成功");


    }

    public void FetchLeaderboard()
    {
        // 找到排行榜
        SteamAPICall_t hSteamAPICall = SteamUserStats.FindLeaderboard("BestTime");
        
        // 設置回調方法處理結果
        m_callbackLeaderboardFound.Set(hSteamAPICall, OnLeaderboardFound);
    }

    // 回調處理方法
    private void OnLeaderboardFound(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_bLeaderboardFound == 0)
        {
            Debug.LogError("找不到排行榜");
            return;
        }

        leaderboardHandle = pCallback.m_hSteamLeaderboard;
        UploadScore(gameManager.totalSeconds); 
        // 獲取排行榜前N名
        SteamAPICall_t hDownloadCall = SteamUserStats.DownloadLeaderboardEntries(
            pCallback.m_hSteamLeaderboard, 
            ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 
            1, 
            1000
        );

        

        m_callbackLeaderboardScores.Set(hDownloadCall, DisplayLeaderboard);

        Debug.Log("獲取排行榜");
        Debug.Log($"下載排行榜的 API 呼叫: {hDownloadCall.m_SteamAPICall}");
        if (hDownloadCall == SteamAPICall_t.Invalid)
        {
            Debug.LogError("無法下載排行榜條目，SteamAPICall_t 無效");
            return;
        }
    }

    // private void DisplayBestRecordTime(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    // {
    //     Debug.Log("DisplayLeaderboard 回調觸發");
    //     if (bIOFailure)
    //     {
    //         Debug.LogError("下載排行榜失敗");
    //         return;
    //     }

    //     if(pCallback.m_cEntryCount > 0)
    //     {
    //         LeaderboardEntry_t entry;
    //         int[] details = new int[3];

    //         if (SteamUserStats.GetDownloadedLeaderboardEntry(
    //             pCallback.m_hSteamLeaderboardEntries,
    //             0, // 获取第一名
    //             out entry,
    //             details,
    //             details.Length
    //         ))
    //         {
    //             // 将第一名的时间设置到 gameManager 的 worldBestRecordTime
    //             worldBestRecordTime.text = entry.m_nScore.ToString();
    //         }
    //     }
    // }

    private void DisplayLeaderboard(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
    {
        Debug.Log("DisplayLeaderboard 回調觸發");
        if (bIOFailure)
        {
            Debug.LogError("下載排行榜失敗");
            return;
        }

        Debug.Log($"排行榜條目總數: {pCallback.m_cEntryCount}");
        if (pCallback.m_cEntryCount == 0)
        {
            Debug.Log("排行榜沒有條目，請先上傳分數");
            return;
        }

        // 清空過去的排行榜項目
        foreach (Transform child in leaderboardContentParent)
        {
            Destroy(child.gameObject);
        }

        

        for (int i = 0; i < pCallback.m_cEntryCount; i++)
        {
            LeaderboardEntry_t entry;
            int[] details = new int[3];


           // 獲取排行榜條目
            if (SteamUserStats.GetDownloadedLeaderboardEntry(
                pCallback.m_hSteamLeaderboardEntries,
                i, 
                out entry,
                details,
                details.Length
            ))
            {
                // 在UI上顯示排名、玩家名稱和分數
                string playerName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
                int score = entry.m_nScore;
                
                GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContentParent);
                newEntry.transform.SetParent(leaderboardContentParent, false);

                TextMeshProUGUI[] texts = newEntry.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length >= 2)
                {
                    texts[0].text = $"{i + 1}. {playerName}"; // 顯示排名和玩家名稱
                    texts[1].text = score.ToString(); // 顯示時間或分數
                }

                
                // playerNameText.text = playerName;
                // scoreText.text = score.ToString();

                Debug.Log($"排名 {i+1}: {playerName} - 分數: {score}");
            }
        }
    }

   


    


    public void OpenLeaderBoard()
    {
        leaderBoard.SetActive(true);
        FetchLeaderboard();
    }
}
