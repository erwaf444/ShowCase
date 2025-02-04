using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class Speaker : MonoBehaviour
{
    #region 嘲諷旁白區域
    public AudioClip idleNarrationClip;
    public AudioClip[] jokeNarrationClips; // 存放所有笑話音檔
    public string[] jokeTexts; // 存放所有笑話對應的文字

        
    #endregion

    public AudioClip narrationClip; // 旁白音檔
    public AudioClip secondNarrationClip; // 第二段旁白音檔
    public AudioClip secondTwoNarrationClip; // 第二段旁白音檔
    public AudioClip thirdNarrationClip; 
    public AudioClip forthNarrationClip; 
    public AudioClip fifthNarrationClip; 
    public AudioClip sixthNarrationClip; 

    [TextArea] public string narrationText; // 第一段旁白文字
    [TextArea] public string secondNarrationText; // 第二段旁白文字
    [TextArea] public string secondTwoNarrationText; // 第二段插句旁白文字
    [TextArea] public string thirdNarrationText; // 第三段旁白文字
    [TextArea] public string forthNarrationText; // 第四段旁白文字
    [TextArea] public string fifthNarrationText; // 第五段旁白文字
    [TextArea] public string sixthNarrationText; // 第六段旁白文字
    [TextArea] public string idleNarrationText; // 無操作時的旁白文字
    public TextMeshProUGUI narrationTextUI; // UI元件: Text
    private float lineDisplayTime = 3f; // 每行顯示的時間間隔


    public AudioSource audioSource;
    public Slider volumeSlider;  // 用來控制音量的UI Slider
    private float volume = 1f;    // 音量的初始值 (1 表示最大音量)
    public const string SpeakVolumeKey = "SpeakVolume";

    public float secondNarrationClipTriggerTime = 30f; 
    public float secondTwoNarrationClipTriggerTime = 60f; 
    public float thirdNarrationClipTriggerTime = 100f; 
    public float forthNarrationClipTriggerTime = 200f; 
    public float fifthNarrationClipTriggerTime = 290f; 
    public float sixthNarrationClipTriggerTime = 430f; 


    private float timeSinceLastMove = 0f; // 記錄上次移動後經過的時間
    public float idleTimeThreshold = 30f; // 玩家無移動時間超過此閾值後觸發旁白
    public bool isSpeaking = false;
    public Transform player; // 玩家物體

    private bool hasIdleNarrationPlayed = false;

    private float lastPlayerY;       // 上一幀玩家的 y 坐標
    public float fallThreshold = 15f; // 掉落高度的閾值，可以根據遊戲調整
    private Queue<float> yPositionHistory = new Queue<float>(); // 記錄過去幾秒玩家的 y 坐標
    private float recordInterval = 3f; // 要記錄的時間間隔（3 秒）
    private float timeSinceLastRecord = 0f;
    public GameManager gameManager;
    public LocaleSelector localeSelector;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public float canPlayIdleNarrationTime;
    public AudioManager audioManager;

    void Start()
    {
        if (SteamManager.Initialized)
        {
            LoadVolumeSettings();
        }
        if (player != null)
        {
            lastPlayerY = player.position.y; // 初始化玩家 y 坐標
        }
        // 添加 AudioSource 組件並設置音檔
        // audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = narrationClip;

        // 設置音量控制Slider
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume; 
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged); // 當滑動條值變更時，呼叫這個方法
            // volumeSlider.value = volume; // 初始化Slider的音量值
        }

        StartCoroutine(PlayNarrationAfterDelay(3f)); // 延遲3秒播放旁白
        StartCoroutine(PlaySecondNarrationAfterDelay(secondNarrationClipTriggerTime));
        // StartCoroutine(PlaySecondTwoNarrationAfterDelay(secondTwoNarrationClipTriggerTime));
        StartCoroutine(PlayThirdNarrationAfterDelay(thirdNarrationClipTriggerTime));
        StartCoroutine(PlayForthNarrationAfterDelay(forthNarrationClipTriggerTime));
        StartCoroutine(PlayFifthNarrationAfterDelay(fifthNarrationClipTriggerTime));
        // StartCoroutine(PlayBGM());
        StartCoroutine(PlaySixthNarrationAfterDelay(sixthNarrationClipTriggerTime));
       
    }

    //播放配樂
    private IEnumerator PlayBGM()
    {   
        yield return new WaitForSeconds(330f);
        audioManager.PlayMusic("BGM1");
    }

    // 當音量滑動條的值發生變化時，更新音量
    private void OnVolumeSliderChanged(float value)
    {
        volume = value;
        audioSource.volume = volume; // 設定 AudioSource 的音量為Slider的值
        SaveVolumeSettings();
    }


    void Update()
    {
        canPlayIdleNarrationTime += Time.deltaTime;
        // Debug.Log(canPlayIdleNarrationTime);
        // if(gameManager.panelOn == true)
        // {
        //     if(audioSource.isPlaying)
        //     {
        //         audioSource.Pause();
        //     }
        // } else
        // {
        //     if (!audioSource.isPlaying && isSpeaking)
        //     {
        //         audioSource.UnPause(); // 恢復音頻播放
        //     }
        // }
        // 檢查玩家的y坐標是否有變化，判斷玩家是否移動
        if (player != null)
        {
            // 這裡假設玩家的 y 坐標移動來判斷是否上移
            float playerY = player.position.y;

            // 檢查玩家是否有上移動作
            if (playerY > 0) // 假設玩家有上移動作
            {
                timeSinceLastMove = 0f; // 重置計時器
            }
            else
            {
                timeSinceLastMove += Time.deltaTime; // 計時器累加
            }

            // 如果時間超過閾值，觸發旁白
            if (timeSinceLastMove >= idleTimeThreshold && !isSpeaking && !hasIdleNarrationPlayed && canPlayIdleNarrationTime > 600f)
            {
                PlayIdleNarration();
                timeSinceLastMove = 0f; // 重置計時器
                hasIdleNarrationPlayed = true; // 標記為已播放
                ResetIdleNarration();
            }




            float currentPlayerY = player.position.y;

            // 每隔一段時間記錄玩家的 y 坐標
            timeSinceLastRecord += Time.deltaTime;
            if (timeSinceLastRecord >= 0.1f) // 每 0.1 秒記錄一次
            {
                yPositionHistory.Enqueue(currentPlayerY); // 加入最新的坐標
                timeSinceLastRecord = 0f; // 重置計時器

                // 確保只保存過去 3 秒的數據
                while (yPositionHistory.Count > (recordInterval / 0.1f))
                {
                    yPositionHistory.Dequeue(); // 移除最早的坐標
                }
            }

            // 獲取 3 秒前的 y 坐標（隊列中最舊的值）
            float lastPlayerY = yPositionHistory.Count > 0 ? yPositionHistory.Peek() : currentPlayerY;

            // 檢查掉落條件
            if (lastPlayerY - currentPlayerY > fallThreshold && !isSpeaking && canPlayIdleNarrationTime > 600f)
            {
                PlayRandomJokeNarration(); // 觸發笑話旁白
                Debug.Log("玩家掉落太多，觸發笑話旁白！");
            }

            // Debug.Log($"lastPlayerY: {lastPlayerY}, currentPlayerY: {currentPlayerY}, 差值: {lastPlayerY - currentPlayerY}");

            

            // float currentPlayerY = player.position.y;

            // // 檢查玩家是否掉落超過閾值
            // if (lastPlayerY - currentPlayerY > fallThreshold)
            // {
            //     PlayRandomJokeNarration(); // 觸發笑話旁白
            //     Debug.Log("玩家掉落太多，觸發笑話旁白！");
            // }
            // Debug.Log($"lastPlayerY: {lastPlayerY}, currentPlayerY: {currentPlayerY}, 差值: {lastPlayerY - currentPlayerY}");


            // lastPlayerY = currentPlayerY; // 更新玩家 y 坐標
        }

        // 檢查旁白是否播放結束
        if (isSpeaking && !audioSource.isPlaying)
        {
            isSpeaking = false; // 播放完畢，將 isSpeaking 設為 false
        }

        UpdateDialogue();

    }

    public void ResetIdleNarration()
    {
        StartCoroutine(ResetIdleNarrationTimer());
    }


    private IEnumerator ResetIdleNarrationTimer()
    {
        yield return new WaitForSeconds(30f);
        hasIdleNarrationPlayed = false;
    }

    private IEnumerator PlayNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(narrationClip, narrationText);
    }

    private IEnumerator PlaySecondNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(secondNarrationClip, secondNarrationText);
    }

    private IEnumerator PlaySecondTwoNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(secondTwoNarrationClip, secondTwoNarrationText);
    }

    private IEnumerator PlayThirdNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(thirdNarrationClip, thirdNarrationText);
    }

    private IEnumerator PlayForthNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(forthNarrationClip, forthNarrationText);
    }

    private IEnumerator PlayFifthNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(fifthNarrationClip, fifthNarrationText);
    }

    private IEnumerator PlaySixthNarrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayNarration(sixthNarrationClip, sixthNarrationText);
    }

    private void PlayIdleNarration()
    {
        PlayNarration(idleNarrationClip, idleNarrationText);
    }


    // 玩家出錯時呼叫此方法
    public void PlayRandomJokeNarration()
    {
        if (jokeNarrationClips.Length > 0)
        {
            int randomIndex = Random.Range(0, jokeNarrationClips.Length); // 隨機選擇一個索引
            AudioClip selectedClip = jokeNarrationClips[randomIndex]; // 選中的音檔
            // string selectedText = jokeTexts[randomIndex]; // 選中的文字
            audioSource.clip = selectedClip; 
            audioSource.Play();
            // StartCoroutine(DisplayTextLineByLine(selectedText));

            Debug.Log($"播放笑話旁白: {selectedClip.name}");
        }
        else
        {
            Debug.LogWarning("沒有設定笑話音檔！");
        }
    }

    private void PlayNarration(AudioClip clip, string text)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(DisplayTextLineByLine(text)); // 逐行顯示文字
            isSpeaking = true;
            Debug.Log("播放旁白: " + text);
        }
        else
        {
            Debug.LogError("未設定旁白音檔！");
        }
    }

    private IEnumerator DisplayTextLineByLine(string text)
    {
        string[] lines = text.Split('\n'); // 將文字按換行符分割成多行
        narrationTextUI.text = ""; // 清空當前文字

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // 忽略空行

            // 分割行文字和時間，例如 "這是一行文字|2"
            string[] parts = line.Split('|');
            string lineContent = parts[0]; // 行內容
            float displayTime = (parts.Length > 1 && float.TryParse(parts[1], out float time)) ? time : 3f; // 默認時間2秒

            narrationTextUI.text = lineContent; // 顯示文字
            yield return new WaitForSeconds(displayTime); // 等待指定時間
        }

        // 所有行顯示完畢
        narrationTextUI.text = ""; // 清空文字
    }

    private void UpdateNarrationText(string text)
    {
        if (narrationTextUI != null)
        {
            narrationTextUI.text = text; // 顯示文字
        }
        else
        {
            Debug.LogError("未設定UI文字元件！");
        }
    }

    private void ClearNarrationText()
    {
        if (narrationTextUI != null)
        {
            narrationTextUI.text = ""; // 清空文字
        }
    }

    public void SaveVolumeSettings()
    {
        // 保存音量值到 Steam Cloud
        // SteamUserStats.SetStat(SpeakVolumeKey, audioSource.volume);
        // SteamUserStats.SetStat(MusicMuteKey, musicSource.mute ? 1 : 0);
        // bool musicMutesuccess = SteamUserStats.SetStat(MusicMuteKey, musicSource.mute ? 1 : 0);
        // if (musicMutesuccess){
        //     Debug.Log($"Saving music mute state: {musicSource.mute}, Success: {musicMutesuccess}");
        // }

        // bool sfxMutesuccess = SteamUserStats.SetStat(SFXMuteKey, sfxSource.mute ? 1 : 0);
        // if (sfxMutesuccess){
        //     Debug.Log($"Saving SFX mute state: {sfxSource.mute}, Success: {sfxMutesuccess}");
        // }
        bool success =  SteamUserStats.SetStat(SpeakVolumeKey, audioSource.volume);
        if (success){
            Debug.Log("Saved speak volume: " + audioSource.volume);
        }
     


        // 確保數據被提交到 Steam Cloud
        SteamUserStats.StoreStats();
    }

    public void LoadVolumeSettings()
    {
        float speakVolumn = 0.2f; // 默認音量
        if (SteamUserStats.GetStat(SpeakVolumeKey, out float savedSpeakVolume))
        {
            speakVolumn = savedSpeakVolume;
        }
        else
        {
            Debug.LogWarning($"Failed to load SpeakVolumeKey: {SpeakVolumeKey}");
        }
        audioSource.volume = speakVolumn;
        Debug.Log($"Loaded SpeakVolumeKey: {SpeakVolumeKey}, Value: {savedSpeakVolume}");

    }

    void UpdateDialogue()
    {
        if (localeSelector.localID == 0)
        {
            narrationTextUI.font = SimplifiedFontAsset;
            narrationTextUI.SetAllDirty(); 

            narrationText = @"Oh! You finally started? Awesome!|3
            I was starting to think you’d press the 'Start Game' button one more time, |2.9
            then go grab a cup of coffee or something.|3.1
            Anyway, now that you're in, get ready for the challenge!|3
            You know, the time in this game won’t wait for you.|3
            It's like one of those situations where the bus never waits for you.";
            secondNarrationText = @"Time has begun to flow... Can you feel it?|2.8
            This is a trial, a race against time. Don’t ask who I am.|4
            I am the Guardian of Time, but you can call me... the Narrator.|5
            You know, this path is not easy. It will test your reflexes, patience, |5
            and that unwavering determination not to give up.|3.5
            Every second could be your last!";
            secondTwoNarrationText = @"What? You think this game was made carelessly? Hey, watch your words! |4.5
            We have our own unique style, you know! |3";
            thirdNarrationText = @"You know, when I was little, I was just as curious as you are. |4.5
            I remember one time, I snuck into my father’s clock shop, completely fascinated by the gears and pendulums. |6.5
            I reached out to touch the largest clock, and then—click—time stopped.|4.5
            I was terrified, thinking I’d done something unforgivable.|3.5
            But strangely, the world didn’t collapse. Instead, a voice whispered in my ear:|5
            You have opened the door of time. From now on, you shall be its guardian.’|5
            You might think that sounds like some grand mission, but for a child, it was an enormous burden.|5.5
            From that day forward, I could no longer play carefree like other kids.|4.5
            I had to watch time’s flow constantly, making sure it never went astray.|4
            But to be honest, I didn’t understand the meaning of time back then. |4
            I only knew—when I pressed a certain button, time would rush forward; |5
            when I let go, time would slow to a crawl. Can you imagine? |5
            A child with the power to control the rhythm of the entire world.|3
            But time never gives people many chances to make mistakes. |3
            I failed too—I once caused an entire day to repeat in chaos,|5
            and I watched flowers wither in an instant…  |3
            Eventually, I realized that time is an irreversible journey.|3
            It waits for no one, and it never turns back.|4
            And so, here I am, the voice whispering in your ear, |5
            reminding you, urging you, staying by your side.|3
            Because I don’t want you to get lost in time like I did when I was a child.|4
            This time, let’s move forward together—let time witness every ounce of your effort.|5";
            forthNarrationText = @"But you know, time itself doesn’t actually exist—it’s just something humans defined to make sense of the world. |6
            Think about it. The sun rises and sets, the seasons turn, and the stars drift across the sky. |6.5 
            These things happen, whether we measure them or not.|4
            Time is just the name we gave to these patterns, an attempt to tame something far greater than ourselves.|6
            Before clocks and calendars, people looked to nature.|3.5
            They measured life by the length of a shadow, the phases of the moon, or the growth of a tree.|5.5
            Back then, time was slower, softer... as if it were a companion rather than something to be chased. |7
            But now? Now we live in minutes and seconds, ticking away like an endless countdown. |6.5
            We chase time, afraid to lose it, afraid to waste it—when, in reality, time isn’t our enemy.|6.5
            We are the ones who built a cage around something infinite.|4
            So here’s a secret: time doesn’t care how fast you run or how perfectly you plan. |6
            It simply is. And when you stop fighting it—when you stop fearing it—that’s when you’ll understand the truth. |6.5
            Time isn’t about winning or losing. It’s about living. |3.5
            Now go. Prove to yourself that every tick, every tock, is worth it. |5";
            fifthNarrationText = @"Let me take you back to 1912, on a cold night, as the Titanic sailed toward its fate.|6
            This ship, at the time, was hailed as the 'unsinkable miracle,' with the most advanced technology and luxurious amenities.|8
            Everyone believed it was invincible. Then, Jack Phillips, a 25-year-old radio operator, sat at his telegraph machine, handling countless passenger messages.|10
            His work was busy, but everything seemed calm, until that fatal collision came suddenly.|6
            In the icy darkness, the ship trembled as Titanic struck an iceberg. |5.5
            In that moment, Jack's world was turned upside down.|4
            He knew the ship could no longer continue its journey, but without hesitation, he returned to his post.|5.5
            He heard the horrific sounds and saw the ship’s hull cracking—yet all he could do was send out the distress signal. |7
            'We have struck an iceberg!' This was the first message Jack sent out, marking the beginning of the Titanic’s tragedy.|7.5
            He understood that time had become his most ruthless enemy. Every second, the ship sank further, and distant help might be delayed because of these precious moments.|9.5
            However, Jack did not panic. With calm hands, he continued sending distress signals to every ship that could hear them.|7
            Again and again, he tapped on the telegraph machine, relaying Titanic’s location to distant waters, fighting for every second, fighting for every chance of survival.|11
            In that moment, Jack Phillips became the hero of time. Faced with towering waves and the cold sea, he raced against time, all for the hope of saving more lives.|11
            Yet, when the final message was sent, Jack knew time could not be reversed. At 2:17 AM, his last distress signal went out, and three minutes later, Titanic sank into the sea. |13
            Jack Phillips, along with the ship, disappeared into the ocean.|4
            But do you know? Because of his persistence, 705 passengers were rescued.|6
            That moment, though time was merciless, Jack’s bravery and sacrifice became an eternal moment in history.|7.5";
            sixthNarrationText = @"Hey! Wait a second, don’t start running just yet! |3
            I know you’re eager to finish the game, but my grand speech just ended—at least give me a round of applause, will you? |6.5
            …No? Fine, in that case, I’ll save my deeply philosophical quotes for next time! |6.5
            Now it’s your turn, hero! Show off your speed and prove this isn’t just some ‘meaningless sprinting,’ but an epic challenge that defies all logic! |9.5
            Go for it, my little rocket legs! Or next time, I might just nominate you for the ‘Slowest Runner Award’! |7.5";
            idleNarrationText = @"Hurry! Hurry! Are you a turtle? Oh, sorry, that’s not fair to turtles.|4.5";

        } else if (localeSelector.localID == 1)
        {
            narrationTextUI.font = TraditionalFontAsset;
            narrationTextUI.SetAllDirty(); 

            narrationText = @"哦！你終於開始了？太棒了！|3
            我還以為你會再按一次「開始遊戲」按鈕，|2.9
            然後去喝杯咖啡什麼的。|3.1
            無論如何，既然你已經進來了，就準備好迎接挑戰吧！|3
            你知道的，這遊戲裡的時間可不會等你。|3
            就像那種公車從來不等你的情況一樣。";
            secondNarrationText = @"時間已經開始流逝……你感覺到了嗎？|2.8
            這是一場考驗，一場與時間賽跑的試煉。別問我是誰。|4
            我是時間的守護者，但你可以叫我……旁白者。|5
            你知道，這條路並不容易，它將考驗你的反應、耐心，|5
            以及不屈不撓的決心。|3.5
            每一秒都可能是你的最後一秒！";
            secondTwoNarrationText = @"什麼？你認為這遊戲是隨便做的嗎？嘿，注意你的話語！|4.5
            我們有自己的獨特風格，你知道嗎！|3";
           
            thirdNarrationText = @"你知道嗎，當我還小的時候，我和你一樣充滿好奇心。|4.5
            我記得有一次，我偷偷進入父親的鐘錶店，對那些齒輪和擺錘著迷。|6.5
            我伸手去觸碰那個最大的鐘錶，然後——咔嚓——時間停止了。|4.5
            我嚇壞了，以為自己做了不可饒恕的事。|3.5
            但奇怪的是，世界並沒有崩塌。相反，一個聲音在我耳邊低語：|5
            你打開了時間之門，從今以後，你將成為它的守護者。|5
            你可能會認為這聽起來像是某種宏大的使命，但對於一個孩子來說，那是巨大的負擔。|5.5
            從那一天起，我再也無法像其他孩子一樣無憂無慮地玩耍。|4.5
            我必須時刻觀察時間的流逝，確保它不會偏離軌道。|4
            但說實話，那時候我並不理解時間的意義。|4
            我只知道——當我按下某個按鈕，時間就會向前奔跑；|5
            當我放開時，時間會慢得幾乎停滯。你能想像嗎？|5
            一個孩子，擁有控制整個世界節奏的力量。|3
            但時間從不會給人犯錯的機會。|3
            我也曾犯錯——我曾經讓整整一天在混亂中重複，|5
            我看著花朵瞬間枯萎… |3
            最後，我明白了時間是一條不可逆轉的旅程。|3
            它不等任何人，也永遠不會回頭。|4
            所以，現在我在這裡，這個聲音在你耳邊低語，|5
            提醒你，催促你，始終陪伴在你身邊。|3
            因為我不希望你像我小時候那樣迷失在時間中。|4
            這次，我們一起向前走——讓時間見證你每一分努力。|5";
            forthNarrationText = @"但你知道嗎，時間本身其實並不存在——它只不過是人類為了理解這個世界而定義的東西。|6
            想想看。太陽升起又落下，四季變換，星星在天空中漂浮。|6.5
            這些事情無論我們是否測量它們，都會發生。|4
            時間只是我們賦予這些模式的名稱，試圖馴服一個遠超過我們自身的存在。|6
            在鐘錶和日曆之前，人們仰望自然。|3.5
            他們用影子的長短、月亮的圓缺、或是樹木的生長來測量生命。|5.5
            那時候，時間是比較緩慢、柔和的... 好像它是一位伴侶，而非一個被追趕的對象。|7
            但是現在呢？現在我們生活在分鐘和秒鐘中，一刻不停地倒數。|6.5
            我們追逐時間，害怕浪費它，害怕失去它——然而，現實是，時間並不是我們的敵人。|6.5
            是我們自己為這無限的東西建造了一個牢籠。|4
            所以，這是一個祕密：時間並不在乎你跑得多快，或是你計劃得多完美。|6
            它只是存在。當你停止與它抗爭——當你停止害怕它——那時候，你會理解真相。|6.5
            時間不是關於贏或輸，它是關於生活。|3.5
            現在，去吧。向自己證明每一秒、每一滴時光都是值得的。|5";
            fifthNarrationText = @"讓我帶你回到 1912 年，在一個寒冷的夜晚，當泰坦尼克號駛向它的命運。|6
            當時，這艘船被譽為『不沉的奇蹟』，擁有最先進的科技和奢華的設施。|8
            每個人都相信它是無敵的。然後，25 歲的無線電操作員 Jack Phillips 坐在他的電報機前，處理著無數乘客的訊息。|10
            他的工作忙碌，但一切似乎很平靜，直到那場致命的碰撞突然來臨。|6
            在冰冷的黑暗中，泰坦尼克號顫抖著撞上了一座冰山。|5.5
            那一刻，Jack 的世界發生了天翻地覆的變化。|4
            他知道這艘船無法再繼續航行，但他毫不猶豫地回到了崗位。|5.5
            他聽到可怕的聲音，看到船身裂開——然而，他所能做的只有發送求救訊號。|7
            『我們撞上了冰山！』這是 Jack 發出的第一條訊息，標誌著泰坦尼克號悲劇的開始。|7.5
            他明白時間成了他最無情的敵人。每一秒鐘，船正在下沉，而遙遠的幫助可能因為這些珍貴的時光而被延誤。|9.5
            然而，Jack 並沒有驚慌。雙手平穩，他繼續將求救訊號發送給每一艘能聽到的船。|7
            一次又一次，他敲打著電報機，將泰坦尼克號的位置轉告遠處的海域，爭取每一秒鐘，爭取每一個生還的機會。|11
            那一刻，Jack Phillips 成為了時間的英雄。在高大的波浪和冰冷的海洋面前，他與時間賽跑，所有的一切都是為了挽救更多的生命。|11
            然而，當最後一條訊息發送出去時，Jack 知道時間無法倒流。凌晨 2:17，他的最後一條求救訊號發出，三分鐘後，泰坦尼克號沉入海中。|13
            Jack Phillips 和這艘船一起消失在海洋中。|4
            但是你知道嗎？正因為他的堅持，705 名乘客被成功救出。|6
            那一刻，雖然時間無情，但 Jack 的勇氣和犧牲成為了歷史中的永恆時刻。|7.5";
            sixthNarrationText = @"嘿！等一下，別急著跑啊！|3
            我知道你急著完成遊戲，但我的雄偉演講才剛結束—至少給我鼓個掌，好嗎？|6.5
            …不行嗎？好吧，那下次我就留著我那些深刻的哲理名言等著再說吧！|6.5
            現在輪到你了，英雄！展示你的速度，證明這不只是一場‘毫無意義的奔跑’，而是一次挑戰邏輯極限的史詩挑戰！|9.5
            去吧，我的小火箭腿！不然下次我可能就提名你為‘最慢跑者獎’啦！|7.5";
            idleNarrationText = @"快點！快點！你是烏龜嗎？哦，對不起，這樣說對烏龜不公平。|4.5";

        } else if (localeSelector.localID == 2)
        {
            narrationTextUI.font = SimplifiedFontAsset;
            narrationTextUI.SetAllDirty(); 

            narrationText = @"哦！你终于开始了？太棒了！|3
            我还以为你会再按一次“开始游戏”按钮，|2.9
            然后去喝杯咖啡什么的。|3.1
            无论如何，既然你已经进来了，就准备好迎接挑战吧！|3
            你知道的，这游戏里的时间可不会等你。|3
            就像那种公交车从来不等你的情况一样。";
            secondNarrationText = @"时间已经开始流逝……你感觉到了吗？|2.8
            这是一场考验，一场与时间赛跑的试炼。别问我是谁。|4
            我是时间的守护者，但你可以叫我……旁白者。|5
            你知道，这条路并不容易，它将考验你的反应、耐心，|5
            以及不屈不挠的决心。|3.5
            每一秒都可能是你的最后一秒！";
            secondTwoNarrationText = @"什么？你认为这个游戏是随便做的吗？嘿，注意你的话语！|4.5
            我们有我们自己独特的风格，你知道吗！|3";
            thirdNarrationText = @"你知道吗，当我还小的时候，我和你一样充满好奇心。|4.5
            我记得有一次，我偷偷进入父亲的钟表店，对那些齿轮和摆锤着迷。|6.5
            我伸手去触碰那个最大的钟表，然后——咔嚓——时间停止了。|4.5
            我吓坏了，以为自己做了不可饶恕的事。|3.5
            但奇怪的是，世界并没有崩塌。相反，一个声音在我耳边低语：|5
            你打开了时间之门，从今以后，你将成为它的守护者。|5
            你可能会认为这听起来像是某种宏大的使命，但对于一个孩子来说，那是巨大的负担。|5.5
            从那一天起，我再也无法像其他孩子一样无忧无虑地玩耍。|4.5
            我必须时刻观察时间的流逝，确保它不会偏离轨道。|4
            但说实话，那时候我并不理解时间的意义。|4
            我只知道——当我按下某个按钮，时间就会向前奔跑；|5
            当我放开时，时间会慢得几乎停滞。你能想象吗？|5
            一个孩子，拥有控制整个世界节奏的力量。|3
            但时间从不会给人犯错的机会。|3
            我也曾犯错——我曾经让整整一天在混乱中重复，|5
            我看着花朵瞬间枯萎… |3
            最后，我明白了时间是一条不可逆转的旅程。|3
            它不等任何人，也永远不回头。|4
            所以，现在我在这里，这个声音在你耳边低语，|5
            提醒你，催促你，始终陪伴在你身边。|3
            因为我不希望你像我小时候那样迷失在时间中。|4
            这次，我们一起向前走——让时间见证你每一分努力。|5";
            forthNarrationText = @"但你知道吗，时间本身其实并不存在——它只不过是人类为了理解这个世界而定义的东西。|6
            想想看。太阳升起又落下，四季更替，星星在天空中漂浮。|6.5
            这些事情无论我们是否测量它们，都会发生。|4
            时间只是我们赋予这些模式的名称，试图驯服一个远超出我们自身的存在。|6
            在钟表和日历之前，人们仰望自然。|3.5
            他们通过影子的长度、月亮的圆缺、或者树木的生长来衡量生命。|5.5
            那时候，时间比较慢，比较温和…就像它是一个伴侣，而不是一个被追赶的对象。|7
            但是现在呢？现在我们活在分钟和秒钟中，像是无休止的倒计时。|6.5
            我们追逐时间，害怕失去它，害怕浪费它——但实际上，时间并不是我们的敌人。|6.5
            是我们自己为这无限的东西建立了一个笼子。|4
            所以，这有个秘密：时间并不在乎你跑得多快，或你计划得多完美。|6
            它就是存在。当你停止与它对抗——当你不再害怕它——那时你会明白真相。|6.5
            时间不是关于赢或输，它是关于生活。|3.5";
            fifthNarrationText = @"让我带你回到 1912 年，在一个寒冷的夜晚，当泰坦尼克号驶向它的命运。|6
            当时，这艘船被誉为『不沉的奇迹』，拥有最先进的科技和奢华的设施。|8
            每个人都相信它是无敌的。然后，25 岁的无线电操作员 Jack Phillips 坐在他的电报机前，处理着无数乘客的讯息。|10
            他的工作忙碌，但一切似乎很平静，直到那场致命的碰撞突然来临。|6
            在冰冷的黑暗中，泰坦尼克号颤抖着撞上了一座冰山。|5.5
            那一刻，Jack 的世界发生了天翻地覆的变化。|4
            他知道这艘船无法再继续航行，但他毫不犹豫地回到了岗位。|5.5
            他听到可怕的声音，看到船身裂开——然而，他所能做的只有发送求救信号。|7
            『我们撞上了冰山！』这是 Jack 发出的第一条讯息，标志着泰坦尼克号悲剧的开始。|7.5
            他明白时间成了他最无情的敌人。每一秒钟，船正在下沉，而远处的帮助可能因为这些宝贵的时光而被延误。|9.5
            然而，Jack 并没有惊慌。双手平稳，他继续将求救信号发送给每一艘能听到的船。|7
            一次又一次，他敲打着电报机，将泰坦尼克号的位置转告远处的海域，争取每一秒钟，争取每一个生还的机会。|11
            那一刻，Jack Phillips 成为了时间的英雄。在高大的波浪和冰冷的海洋面前，他与时间赛跑，所有的一切都是为了挽救更多的生命。|11
            然而，当最后一条讯息发送出去时，Jack 知道时间无法倒流。凌晨 2:17，他的最后一条求救信号发出，三分钟后，泰坦尼克号沉入海中。|13
            Jack Phillips 和这艘船一起消失在海洋中。|4
            但是你知道吗？正因为他的坚持，705 名乘客被成功救出。|6
            那一刻，虽然时间无情，但 Jack 的勇气和牺牲成为了历史中的永恒时刻。|7.5";
            sixthNarrationText = @"嘿！等一下，别急着跑啊！|3
            我知道你急着完成游戏，但我的宏伟演讲才刚结束—至少给我鼓个掌，好吗？|6.5
            …不行吗？好吧，那下次我就留着我那些深刻的哲理名言等着再说吧！|6.5
            现在轮到你了，英雄！展示你的速度，证明这不仅仅是场‘毫无意义的奔跑’，而是一次挑战逻辑极限的史诗挑战！|9.5
            去吧，我的小火箭腿！不然下次我可能就提名你为‘最慢跑者奖’啦！|7.5";
            idleNarrationText = @"快点！快点！你是乌龟吗？哦，对不起，这样说对乌龟不公平。|4.5";

        }
    }
}
