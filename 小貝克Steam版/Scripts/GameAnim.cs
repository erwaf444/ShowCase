using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameAnim : MonoBehaviour
{
    public TimeSystem timeSystem;
    public GameManager gameManager;
    public GameObject Head1;
    public GameObject Head2;
    public Sprite[] Head1Sprites; // 角色1的頭像數組
    public Sprite[] Head2Sprites; // 角色2的頭像數組
    public TextMeshProUGUI Name1;
    public TextMeshProUGUI Name2;
    public GameObject SpeakSign1;
    public GameObject SpeakSign2;
    public TextMeshProUGUI Content;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public enum AnimationStatus
    {
        TwentyDays,
        FortyDays,
        SixtyDays,
        EightyDays,
        HundredDays,
        HundredTwentyDays,
        HundredFiftyDays
    }
    private Dictionary<AnimationStatus, bool> animationStatus = new Dictionary<AnimationStatus, bool>();

    public GameObject[] GameAnimObjects;
    public bool gameAnimIsEnd = false;
    public bool canStartGameAnim = false;
    public bool gameAnimObjectShow = false;
    public bool canRespondToRKey = true;
    public DayPanel dayPanel;
    public LocaleSelector localeSelector;



    void Start()
    {
        
        canStartGameAnim = false;
        gameAnimObjectShow = false;
        canRespondToRKey = true;
        // if (gameAnimIsEnd)
        // {
            for (int i = 0; i < GameAnimObjects.Length; i++)
            {
                GameAnimObjects[i].SetActive(false);
            }
        // } else 
        // {
        //     for (int i = 0; i < GameAnimObjects.Length; i++)
        //     {
        //         GameAnimObjects[i].SetActive(true);
        //     }
        // }
 
        animationStatus[AnimationStatus.TwentyDays] = false;
        animationStatus[AnimationStatus.FortyDays] = false;
        animationStatus[AnimationStatus.SixtyDays] = false;
        animationStatus[AnimationStatus.EightyDays] = false;
        animationStatus[AnimationStatus.HundredDays] = false;
        animationStatus[AnimationStatus.HundredTwentyDays] = false;
        timeSystem = FindObjectOfType<TimeSystem>(); 
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        } 
    }

    void SetAnimationStatusEnd(AnimationStatus animation, bool isEnd)
    {
        animationStatus[animation] = isEnd;
    }

    public bool IsAnimationStatusEnded(AnimationStatus animation)
    {
        return animationStatus.ContainsKey(animation) && animationStatus[animation];
    }

    void Update()
    {
        // Debug.Log(gameAnimIsEnd);

        if (TimeSystem.AccumulateDay == 20)
        {
            PlayerPressContinue();
            if (canStartGameAnim && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.TwentyDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    Time.timeScale = 1;
                    dayPanel.ContinueAnimObj.SetActive(false);
                    TwentyDaysDialogueStep++;
                    if (TwentyDaysDialogueStep < TwentyDaysDialogues.Length)
                    {
                        UpdateTwentyDaysDialogue();
                    }
                    else
                    {
                        EndTwentyDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
        if (TimeSystem.AccumulateDay == 40)
        {
            PlayerPressContinue();
            if (canStartGameAnim  && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.FortyDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    FortyDaysDialogueStep++;
                    if (FortyDaysDialogueStep < FortyDaysDialogues.Length)
                    {
                        UpdateFortyDaysDialogue();
                    }
                    else
                    {
                        EndFortyDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
        if (TimeSystem.AccumulateDay == 60)
        {
            PlayerPressContinue();
            if (canStartGameAnim  && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.SixtyDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    SixtyDaysDialogueStep++;
                    if (SixtyDaysDialogueStep < SixtyDaysDialogues.Length)
                    {
                        UpdateSixtyDaysDialogue();
                    }
                    else
                    {
                        EndSixtyDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
        if (TimeSystem.AccumulateDay == 80)
        {
            PlayerPressContinue();
            if (canStartGameAnim  && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.EightyDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    EightyDaysDialogueStep++;
                    if (EightyDaysDialogueStep < EightyDaysDialogues.Length)
                    {
                        UpdateEightyDaysDialogue();
                    }
                    else
                    {
                        EndEightyDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
        if (TimeSystem.AccumulateDay == 100)
        {
            PlayerPressContinue();
            if (canStartGameAnim  && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.HundredDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    HundredDaysDialogueStep++;
                    if (HundredDaysDialogueStep < HundredDaysDialogues.Length)
                    {
                        UpdateHundredDaysDialogue();
                    }
                    else
                    {
                        EndHundredDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
        if (TimeSystem.AccumulateDay == 120)
        {
            PlayerPressContinue();
            if (canStartGameAnim  && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.HundredTwentyDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    HundredTwentyDaysDialogueStep++;
                    if (HundredTwentyDaysDialogueStep < HundredTwentyDaysDialogues.Length)
                    {
                        UpdateHundredTwentyDaysDialogue();
                    }
                    else
                    {
                        EndHundredTwentyDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
        if (TimeSystem.AccumulateDay == 150)
        {
            PlayerPressContinue();
            if (canStartGameAnim  && gameAnimObjectShow)
            {
                gameManager.canInteract = false;
                for (int i = 0; i < GameAnimObjects.Length; i++)
                {
                    GameAnimObjects[i].SetActive(true);
                }
                SetHeads(AnimationStatus.HundredFiftyDays);
                if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
                {
                    HundredFiftyDaysDialogueStep++;
                    if (HundredFiftyDaysDialogueStep < HundredFiftyDaysDialogues.Length)
                    {
                        UpdateHundredFiftyDaysDialogue();
                    }
                    else
                    {
                        EndHundredFiftyDaysDialogue(); // 可選：處理對話結束的邏輯
                    }
                }
            }
        }
    }


    //20天動畫
    private int TwentyDaysDialogueStep = 0; // 對話步驟計數器
    private string[] TwentyDaysDialogues = {
        "今天又是美好的一天！", // 角色1
        "你好。", // 角色2
        "你好小朋友！你想要什麼麵包？我這裡可是應有盡有哦！", // 角色1
        "麵包嗎...。", // 角色2
        "怎麼了？", // 角色1
        "雖然我很想要吃麵包，但是我身上沒有錢。", // 角色2
        "是嗎？", // 角色1
        "嗯...。", // 角色2
        "這樣好了，這個麵包免費給你吃！", // 角色1
        "真的嗎！", // 角色2
        "真的啊！拿去吧！回去的路上小心哦。", // 角色1
        "太好了！謝謝老闆！", // 角色2
    };
    void UpdateTwentyDaysDialogue()
    {
        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            TwentyDaysDialogues = new string[]
            {
                "Today is a beautiful day!", // 英文对白
                "Hello.",
                "Hello kid! What kind of bread would you like? I have everything here!",
                "Bread...?",
                "What's wrong?",
                "I really want some bread, but I don’t have any money.",
                "Is that so?",
                "Hmm...",
                "Well then, this bread is free for you!",
                "Really?!",
                "Yes, take it! Be careful on your way back.",
                "Thank you so much, sir!"
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            TwentyDaysDialogues = new string[]
            {
                "今天又是美好的一天！", // 中文对白
                "你好。",
                "你好小朋友！你想要什麼麵包？我這裡可是應有盡有哦！",
                "麵包嗎...。",
                "怎麼了？",
                "雖然我很想要吃麵包，但是我身上沒有錢。",
                "是嗎？",
                "嗯...",
                "這樣好了，這個麵包免費給你吃！",
                "真的嗎！",
                "真的啊！拿去吧！回去的路上小心哦。",
                "太好了！謝謝老闆！"
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            TwentyDaysDialogues = new string[]
            {
                "今天又是美好的一天！", // 中文对白
                "你好。",
                "你好小朋友！你想要什么面包？我这里可是应有尽有哦！",
                "面包吗……。",
                "怎么了？",
                "虽然我很想要吃面包，但是我身上没有钱。",
                "是吗？",
                "嗯……",
                "这样好了，这个面包免费给你吃！",
                "真的吗！",
                "真的啊！拿去吧！回去的路上小心哦。",
                "太好了！谢谢老板！"
            };
        }


        if (TwentyDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }

        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Kid";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "房東";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "小朋友";
        }
        // Name1.text = "我";
        // Name2.text = "小朋友";
        Content.text = TwentyDaysDialogues[TwentyDaysDialogueStep];
    }

    void EndTwentyDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.TwentyDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }
    //40天動畫
    private int FortyDaysDialogueStep = 0; // 對話步驟計數器
    private string[] FortyDaysDialogues = {
        "今天也要早早起來開工！", // 角色1
        "咳咳咳！", // 角色2
        "咦？原來是房東啊！", // 角色1
        "早上好！最近生意如何啊！", // 角色2
        "還算過得去吧。", // 角色1
        "那就好！生意興隆就好！哈哈哈！", // 角色2
        "對了，房東你今天怎麼來了？我不是有按時交房租嗎？", // 角色1
        "我只是來看看你而已啊！看看這家麵包店最近的生意好不好！", // 角色2
        "這樣子呀，那麼，房東我做個麵包給你吃吧！", // 角色1
        "不用了！我待會就要走了！", // 角色2
        "是嗎...太好了。", // 角色1
        "嗯？你說什麼？", // 角色2
        "我是說！房東慢走！", // 角色1
        "那麼我先走了！拜拜！", // 角色2
    };
    void UpdateFortyDaysDialogue()
    {
        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            FortyDaysDialogues = new string[]
            {
                "Time to get up early and get to work again!", // Character 1
                "Cough cough cough!", // Character 2
                "Huh? So it's the landlord!", // Character 1
                "Good morning! How's business lately?", // Character 2
                "It's been okay, I guess.", // Character 1
                "That's good! As long as business is thriving! Hahaha!", // Character 2
                "By the way, why did you come today, landlord? I thought I paid the rent on time?", // Character 1
                "I just came to check on you! To see how this bakery is doing lately!", // Character 2
                "I see, then let me make you a bread!", // Character 1
                "No need! I'm leaving soon!", // Character 2
                "Oh really... that's great.", // Character 1
                "Hmm? What did you say?", // Character 2
                "I said! Safe travels, landlord!", // Character 1
                "Then I’ll take my leave! Bye bye!", // Character 2
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            FortyDaysDialogues = new string[]
            {
               "今天也要早早起來開工！", // 角色1
                "咳咳咳！", // 角色2
                "咦？原來是房東啊！", // 角色1
                "早上好！最近生意如何啊！", // 角色2
                "還算過得去吧。", // 角色1
                "那就好！生意興隆就好！哈哈哈！", // 角色2
                "對了，房東你今天怎麼來了？我不是有按時交房租嗎？", // 角色1
                "我只是來看看你而已啊！看看這家麵包店最近的生意好不好！", // 角色2
                "這樣子呀，那麼，房東我做個麵包給你吃吧！", // 角色1
                "不用了！我待會就要走了！", // 角色2
                "是嗎...太好了。", // 角色1
                "嗯？你說什麼？", // 角色2
                "我是說！房東慢走！", // 角色1
                "那麼我先走了！拜拜！", // 角色2
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            FortyDaysDialogues = new string[]
            {
                "今天也要早早起来开工！", // 角色1
                "咳咳咳！", // 角色2
                "咦？原来是房东啊！", // 角色1
                "早上好！最近生意如何啊！", // 角色2
                "还算过得去吧。", // 角色1
                "那就好！生意兴隆就好！哈哈哈！", // 角色2
                "对了，房东你今天怎么来了？我不是有按时交房租吗？", // 角色1
                "我只是来看看你而已啊！看看这家面包店最近的生意好不好！", // 角色2
                "这样子呀，那么，房东我做个面包给你吃吧！", // 角色1
                "不用了！我待会就要走了！", // 角色2
                "是吗...太好了。", // 角色1
                "嗯？你说什么？", // 角色2
                "我是说！房东慢走！", // 角色1
                "那么我先走了！拜拜！", // 角色2
            };
        }

        if (FortyDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }


        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Landlord";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "房東";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "房东";
        }

        // Name1.text = "我";
        // Name2.text = "房東";
        Content.text = FortyDaysDialogues[FortyDaysDialogueStep];
    }


    void EndFortyDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.FortyDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }
    //60天動畫
    private int SixtyDaysDialogueStep = 0; // 對話步驟計數器
    private string[] SixtyDaysDialogues = {
        "昨天好像睡得有點不好...。咦？", // 角色1
        "喵~", // 角色2
        "小貓，你怎麼在這裡？", // 角色1
        "喵~", // 角色2
        "我知道了，你是來找我們家橘貓的是嗎？", // 角色1
        "喵~", // 角色2
        "真是可愛！", // 角色1
        "喵~", // 角色2
        "你想要吃些東西嗎？", // 角色1
        "喵~", // 角色2
        "那麼，我去看看有什麼可以給你吃的吧！", // 角色1
        "喵~", // 角色2
        "我找找看...。", // 角色1
        "喵~", // 角色2
        "我找到了！咦？小貓怎麼不見了？", // 角色1
    };
    void UpdateSixtyDaysDialogue()
    {
        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            SixtyDaysDialogues = new string[]
            {
                "I didn't sleep very well yesterday... Huh?", // Character 1
                "Meow~", // Character 2
                "Little cat, what are you doing here?", // Character 1
                "Meow~", // Character 2
                "I see, you came looking for our orange cat, right?", // Character 1
                "Meow~", // Character 2
                "So cute!", // Character 1
                "Meow~", // Character 2
                "Do you want something to eat?", // Character 1
                "Meow~", // Character 2
                "Then let me see what I can find for you!", // Character 1
                "Meow~", // Character 2
                "Let me look around...", // Character 1
                "Meow~", // Character 2
                "I found it! Huh? Where did the little cat go?", // Character 1
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            SixtyDaysDialogues = new string[]
            {
                "昨天好像睡得有點不好...。咦？", // 角色1
                "喵~", // 角色2
                "小貓，你怎麼在這裡？", // 角色1
                "喵~", // 角色2
                "我知道了，你是來找我們家橘貓的是嗎？", // 角色1
                "喵~", // 角色2
                "真是可愛！", // 角色1
                "喵~", // 角色2
                "你想要吃些東西嗎？", // 角色1
                "喵~", // 角色2
                "那麼，我去看看有什麼可以給你吃的吧！", // 角色1
                "喵~", // 角色2
                "我找找看...。", // 角色1
                "喵~", // 角色2
                "我找到了！咦？小貓怎麼不見了？" // 角色1
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            SixtyDaysDialogues = new string[]
            {
                "昨天好像睡得有点不好...。咦？", // 角色1
                "喵~", // 角色2
                "小猫，你怎么在这里？", // 角色1
                "喵~", // 角色2
                "我知道了，你是来找我们家橘猫的是吗？", // 角色1
                "喵~", // 角色2
                "真是可爱！", // 角色1
                "喵~", // 角色2
                "你想要吃些东西吗？", // 角色1
                "喵~", // 角色2
                "那么，我去看看有什么可以给你吃的吧！", // 角色1
                "喵~", // 角色2
                "我找找看...。", // 角色1
                "喵~", // 角色2
                "我找到了！咦？小猫怎么不见了？", // 角色1
            };
        }


        if (SixtyDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }

        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Cat";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "小貓";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "小猫";
        }

        // Name1.text = "我";
        // Name2.text = "小貓";
        Content.text = SixtyDaysDialogues[SixtyDaysDialogueStep];
    }


    void EndSixtyDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.SixtyDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }

    //80天動畫
    private int EightyDaysDialogueStep = 0; // 對話步驟計數器
    private string[] EightyDaysDialogues = {
        "昨天好像夢到了麵包...？天啊。", // 角色1
        "你好...。", // 角色2
        "今天的第一個客人！早上好！", // 角色1
        "早上好...，我想要一個麵包。", // 角色2
        "沒問題，你想要什麼樣子的麵包呢？", // 角色1
        "這個...那個...。", // 角色2
        "沒關係，慢慢來。", // 角色1
        "我...，我想要...。", // 角色2
        "慢慢來沒關係。", // 角色1
        "我覺得我...。", // 角色2
        "...。", // 角色1
        "我還是選...。", // 角色2
        "......。", // 角色1
        "我要這個好了。", // 角色2
        "甜甜圈是嗎？真是好選擇！", // 角色1
        "謝謝老闆...。", // 角色2
    };
    void UpdateEightyDaysDialogue()
    {
        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            EightyDaysDialogues = new string[]
            {
                "I think I dreamed about bread yesterday...? Oh my.", // Character 1
                "Hello...", // Character 2
                "The first customer of the day! Good morning!", // Character 1
                "Good morning..., I would like a bread.", // Character 2
                "No problem, what kind of bread do you want?", // Character 1
                "This one... that one...", // Character 2
                "Take your time, no rush.", // Character 1
                "I... I want...", // Character 2
                "Take your time, it's okay.", // Character 1
                "I think I...", // Character 2
                "...", // Character 1
                "I'll just choose...", // Character 2
                "......", // Character 1
                "I'll go with this one.", // Character 2
                "A donut, right? Great choice!", // Character 1
                "Thank you, boss...", // Character 2
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            EightyDaysDialogues = new string[]
            {
                "昨天好像夢到了麵包...？天啊。", // 角色1
                "你好...。", // 角色2
                "今天的第一個客人！早上好！", // 角色1
                "早上好...，我想要一個麵包。", // 角色2
                "沒問題，你想要什麼樣子的麵包呢？", // 角色1
                "這個...那個...。", // 角色2
                "沒關係，慢慢來。", // 角色1
                "我...，我想要...。", // 角色2
                "慢慢來沒關係。", // 角色1
                "我覺得我...。", // 角色2
                "...。", // 角色1
                "我還是選...。", // 角色2
                "......。", // 角色1
                "我要這個好了。", // 角色2
                "甜甜圈是嗎？真是好選擇！", // 角色1
                "謝謝老闆...。", // 角色2
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            EightyDaysDialogues = new string[]
            {
                "昨天好像梦到了面包...？天啊。", // 角色1
                "你好...。", // 角色2
                "今天的第一个客人！早上好！", // 角色1
                "早上好...，我想要一个面包。", // 角色2
                "没问题，你想要什么样子的面包呢？", // 角色1
                "这个...那个...。", // 角色2
                "没关系，慢慢来。", // 角色1
                "我...，我想要...。", // 角色2
                "慢慢来没关系。", // 角色1
                "我觉得我...。", // 角色2
                "...。", // 角色1
                "我还是选...。", // 角色2
                "......。", // 角色1
                "我要这个好了。", // 角色2
                "甜甜圈是吗？真是好选择！", // 角色1
                "谢谢老板...。", // 角色2
            };
        }

        if (EightyDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }

        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Little Girl";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "小女孩";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "小女孩";
        }

        // Name1.text = "我";
        // Name2.text = "小女孩";
        Content.text = EightyDaysDialogues[EightyDaysDialogueStep];
    }


    void EndEightyDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.EightyDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }

    //100天動畫
    private int HundredDaysDialogueStep = 0; // 對話步驟計數器
    private string[] HundredDaysDialogues = {
        "好了好了！開工開工！", // 角色1
        "老闆！", // 角色2
        "哇，那麼早！早上好！", // 角色1
        "老闆你們這邊有賣什麼？", // 角色2
        "這裡是麵包店，所以我們有賣很多種類的麵包哦！", // 角色1
        "是哦！那麼我想要...。", // 角色2
        "嗯哼？", // 角色1
        "一份薯餅加薯條！", // 角色2
        "...，不好意思，這裡是麵包店。", // 角色1
        "蛤！這裡沒有薯餅和薯條哦！", // 角色2
        "沒有...。", // 角色1
        "我還以為這裡有呢！害我還特別走到這裡。", // 角色2
        "什麼東西...。", // 角色1
        "算了！我要去上課了！只好去學校附近的早餐店了！", // 角色2
        "好的，慢走...。", // 角色1
    };
    void UpdateHundredDaysDialogue()
    {

        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            HundredDaysDialogues = new string[]
            {
                "Alright, alright! Let's get to work!", // Character 1
                "Boss!", // Character 2
                "Wow, so early! Good morning!", // Character 1
                "What do you sell here, boss?", // Character 2
                "This is a bakery, so we sell a lot of different kinds of bread!", // Character 1
                "Oh! Then I want...", // Character 2
                "Hmm?", // Character 1
                "An order of hash browns and fries!", // Character 2
                "... Sorry, this is a bakery.", // Character 1
                "Huh! There's no hash browns and fries here!", // Character 2
                "No...", // Character 1
                "I thought you had them! I came all the way here for that.", // Character 2
                "What on earth...", // Character 1
                "Forget it! I'm going to class! I have to go to the breakfast shop near the school instead!", // Character 2
                "Okay, take care...", // Character 1
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            HundredDaysDialogues = new string[]
            {
                "好了好了！開工開工！", // 角色1
                "老闆！", // 角色2
                "哇，那麼早！早上好！", // 角色1
                "老闆你們這邊有賣什麼？", // 角色2
                "這裡是麵包店，所以我們有賣很多種類的麵包哦！", // 角色1
                "是哦！那麼我想要...。", // 角色2
                "嗯哼？", // 角色1
                "一份薯餅加薯條！", // 角色2
                "...，不好意思，這裡是麵包店。", // 角色1
                "蛤！這裡沒有薯餅和薯條哦！", // 角色2
                "沒有...。", // 角色1
                "我還以為這裡有呢！害我還特別走到這裡。", // 角色2
                "什麼東西...。", // 角色1
                "算了！我要去上課了！只好去學校附近的早餐店了！", // 角色2
                "好的，慢走...。", // 角色1
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            HundredDaysDialogues = new string[]
            {
                "好了好了！开工开工！", // 角色1
                "老板！", // 角色2
                "哇，那么早！早上好！", // 角色1
                "老板你们这边有卖什么？", // 角色2
                "这里是面包店，所以我们有卖很多种类的面包哦！", // 角色1
                "是哦！那么我想要...。", // 角色2
                "嗯哼？", // 角色1
                "一份薯饼加薯条！", // 角色2
                "...，不好意思，这里是面包店。", // 角色1
                "蛤！这里没有薯饼和薯条哦！", // 角色2
                "没有...。", // 角色1
                "我还以为这里有呢！害我还特别走到这里。", // 角色2
                "什么东西...。", // 角色1
                "算了！我要去上课了！只好去学校附近的早餐店了！", // 角色2
                "好的，慢走...。", // 角色1
            };
        }


        if (HundredDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }

        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Little Brat";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "小屁孩";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "小屁孩";
        }

        // Name1.text = "我";
        // Name2.text = "小屁孩";
        Content.text = HundredDaysDialogues[HundredDaysDialogueStep];
    }


    void EndHundredDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.HundredDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }

    #region 120天
    //120天動畫
    private int HundredTwentyDaysDialogueStep = 0; // 對話步驟計數器
    private string[] HundredTwentyDaysDialogues = {
        "第一百二十天，不知不覺，過了那麼久了！", // 角色1
        "嗯...。", // 角色2
        "咦？怎麼有隻熊？", // 角色1
        "什麼熊？有沒有禮貌！", // 角色2
        "天啊！還是一隻會說話的熊！", // 角色1
        "好吧，我是一隻會說話的熊沒有錯。", // 角色2
        "嗯哼？那麼請問你到這裡做什麼，這裡可沒有賣蜂蜜哦！", // 角色1
        "沒事，就看看而已，也不一定要買嘛，對不對！", // 角色2
        "說是沒有錯，但是如果你要買的話也是很好的哦。", // 角色1
        "好吧，既然你那麼說了，讓我看看我的口袋。", // 角色2
        "沒問題。", // 角色1
        "對了，我忘記我沒有口袋了。", // 角色2
        "...。", // 角色1
        "算了算了，我先走了！", // 角色2
        "好的，慢走...。", // 角色1
    };
    void UpdateHundredTwentyDaysDialogue()
    {

        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            HundredTwentyDaysDialogues = new string[]
            {
                "The one hundred and twentieth day, before you know it, so long has passed!", // Character 1
                "Hmm...", // Character 2
                "Huh? Why is there a bear?", // Character 1
                "What bear? How polite!", // Character 2
                "Oh my god! It's a talking bear!", // Character 1
                "Well, there's nothing wrong with me being a talking bear.", // Character 2
                "Huh? Then what are you doing here? Honey is not sold here!", // Character 1
                "It's okay, just take a look, you don't have to buy it, right!", // Character 2
                "It's not wrong to say it, but it's also good if you want to buy it.", // Character 1
                "Okay, now that you say that, let me see my pocket.", // Character 2
                "No problem.", // Character 1
                "By the way, I forgot I didn't have pockets.", // Character 2
                "....", // role 1
                "Forget it, let me go first!", // Character 2
                "Okay, walk slowly...", // Character 1
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            HundredTwentyDaysDialogues = new string[]
            {
                "第一百二十天，不知不覺，過了那麼久了！", // 角色1
                "嗯...。", // 角色2
                "咦？怎麼有隻熊？", // 角色1
                "什麼熊？有沒有禮貌！", // 角色2
                "天啊！還是一隻會說話的熊！", // 角色1
                "好吧，我是一隻會說話的熊沒有錯。", // 角色2
                "嗯哼？那麼請問你到這裡做什麼，這裡可沒有賣蜂蜜哦！", // 角色1
                "沒事，就看看而已，也不一定要買嘛，對不對！", // 角色2
                "說是沒有錯，但是如果你要買的話也是很好的哦。", // 角色1
                "好吧，既然你那麼說了，讓我看看我的口袋。", // 角色2
                "沒問題。", // 角色1
                "對了，我忘記我沒有口袋了。", // 角色2
                "...。", // 角色1
                "算了算了，我先走了！", // 角色2
                "好的，慢走...。", // 角色1
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            HundredTwentyDaysDialogues = new string[]
            {
                "第一百二十天，不知不觉，过了那么久了！", // 角色1
                "嗯...。", // 角色2
                "咦？怎么有只熊？", // 角色1
                "什么熊？有没有礼貌！", // 角色2
                "天啊！还是一只会说话的熊！", // 角色1
                "好吧，我是一只会说话的熊没有错。", // 角色2
                "嗯哼？那么请问你到这里做什么，这里可没有卖蜂蜜哦！", // 角色1
                "没事，就看看而已，也不一定要买嘛，对不对！", // 角色2
                "说是没有错，但是如果你要买的话也是很好的哦。", // 角色1
                "好吧，既然你那么说了，让我看看我的口袋。", // 角色2
                "没问题。", // 角色1
                "对了，我忘记我没有口袋了。", // 角色2
                "...。", // 角色1
                "算了算了，我先走了！", // 角色2
                "好的，慢走...。", // 角色1
            };
        }


        if (HundredTwentyDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }

        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Boring Bear";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "無聊熊";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "无聊熊";
        }


        Content.text = HundredTwentyDaysDialogues[HundredTwentyDaysDialogueStep];
    }


    void EndHundredTwentyDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.HundredTwentyDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }
    #endregion

    #region 150天
    //150天動畫
    private int HundredFiftyDaysDialogueStep = 0; // 對話步驟計數器
    private string[] HundredFiftyDaysDialogues = {
        "今天起來精神真是不錯！", // 角色1
        "嗯...。", // 角色2
        "咦？怎麼又是你！", // 角色1
        "我今天路過不行嗎？", // 角色2
        "是可以啊...，反正我也管不著，但是你今天看起來好像有點不太對勁。", // 角色1
        "嗚嗚嗚...。", // 角色2
        "怎麼了？怎麼開始哭起來了？", // 角色1
        "身為一隻熊，想要找到另外一半怎麼那麼困難。", // 角色2
        "呃呃...，因為你是一隻熊，應該去找你的同類，不是在人類世界中。", // 角色1
        "說的也是，我看我還是回到森林裡去好了。", // 角色2
        "太好了，明智的選擇。", // 角色1
        "起碼，那裡還有蜂蜜。", // 角色2
        "...。", // 角色1
        "我要走了。", // 角色2
        "好的，慢走...。", // 角色1
    };
    void UpdateHundredFiftyDaysDialogue()
    {

        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            HundredFiftyDaysDialogues = new string[]
            {
               "I feel really good today!", // Character 1
                "Hmm...", // Character 2
                "Eh? Why is it you again!", // Character 1
                "Can't I just stop by today?", // Character 2
                "Yes..., I can't care about it anyway, but you seem to be a little off today.", // Character 1
                "Woooooooo...", // Character 2
                "What's wrong? Why did you start crying?", // Character 1
                "As a bear, why is it so difficult to find your other half?", // Character 2
                "Uh uh..., because you are a bear, you should go find your own kind, not in the human world.", // Character 1
                "That's right. I think I'd better go back to the forest.", // Character 2
                "Great, smart choice.", // Character 1
                "At least, there's honey there.", // Character 2
                "....", // role 1
                "I'm leaving.", // Character 2
                "Okay, walk slowly...", // Character 1
            };
        }
        else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            HundredFiftyDaysDialogues = new string[]
            {
                "今天起來精神真是不錯！", // 角色1
                "嗯...。", // 角色2
                "咦？怎麼又是你！", // 角色1
                "我今天路過不行嗎？", // 角色2
                "是可以啊...，反正我也管不著，但是你今天看起來好像有點不太對勁。", // 角色1
                "嗚嗚嗚...。", // 角色2
                "怎麼了？怎麼開始哭起來了？", // 角色1
                "身為一隻熊，想要找到另外一半怎麼那麼困難。", // 角色2
                "呃呃...，因為你是一隻熊，應該去找你的同類，不是在人類世界中。", // 角色1
                "說的也是，我看我還是回到森林裡去好了。", // 角色2
                "太好了，明智的選擇。", // 角色1
                "起碼，那裡還有蜂蜜。", // 角色2
                "...。", // 角色1
                "我要走了。", // 角色2
                "好的，慢走...。", // 角色1
            };
        }
        else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            HundredFiftyDaysDialogues = new string[]
            {
                "今天起来精神真是不错！", // 角色1
                "嗯...。", // 角色2
                "咦？怎么又是你！", // 角色1
                "我今天路过不行吗？", // 角色2
                "是可以啊...，反正我也管不着，但是你今天看起来好像有点不太对劲。", // 角色1
                "呜呜呜...。", // 角色2
                "怎么了？怎么开始哭起来了？", // 角色1
                "身为一只熊，想要找到另外一半怎么那么困难。", // 角色2
                "呃呃...，因为你是一只熊，应该去找你的同类，不是在人类世界中。", // 角色1
                "说的也是，我看我还是回到森林里去好了。", // 角色2
                "太好了，明智的选择。", // 角色1
                "起码，那里还有蜂蜜。", // 角色2
                "...。", // 角色1
                "我要走了。", // 角色2
                "好的，慢走...。", // 角色1
            };
        }

        if (HundredFiftyDaysDialogueStep % 2 == 0)
        {
            SpeakSign1.SetActive(true);
            SpeakSign2.SetActive(false);
        }
        else
        {
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(true);
        }

        if (localeSelector.localID == 0)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "Me";
            Name2.text = "Boring Bear";
        } else if (localeSelector.localID == 1)
        {
            Name1.font = TraditionalFontAsset;
            Name2.font = TraditionalFontAsset;
            Name1.text = "我";
            Name2.text = "無聊熊";
        } else if (localeSelector.localID == 2)
        {
            Name1.font = SimplifiedFontAsset;
            Name2.font = SimplifiedFontAsset;
            Name1.text = "我";
            Name2.text = "无聊熊";
        }


        Content.text = HundredFiftyDaysDialogues[HundredFiftyDaysDialogueStep];
    }


    void EndHundredFiftyDaysDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        SetAnimationStatusEnd(AnimationStatus.HundredFiftyDays, true);
        for (int i = 0; i < GameAnimObjects.Length; i++)
        {
            GameAnimObjects[i].SetActive(false);
        }
        gameAnimIsEnd = true;
        gameAnimObjectShow = false;
        gameManager.canInteract = true;
    }
    #endregion

    void SetHeads(AnimationStatus status)
    {
        switch (status)
        {
            case AnimationStatus.TwentyDays:
                Head1.GetComponent<Image>().sprite= Head1Sprites[0]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[0]; //小朋友
                break;
            case AnimationStatus.FortyDays:
                Head1.GetComponent<Image>().sprite = Head1Sprites[1]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[1]; //房東
                break;
            case AnimationStatus.SixtyDays:
                Head1.GetComponent<Image>().sprite = Head1Sprites[2]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[2]; //小貓
                break;
            case AnimationStatus.EightyDays:
                Head1.GetComponent<Image>().sprite = Head1Sprites[3]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[3]; //小女孩
                break;
            case AnimationStatus.HundredDays:
                Head1.GetComponent<Image>().sprite = Head1Sprites[4]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[4]; //小女孩2
                break;
            case AnimationStatus.HundredTwentyDays:
                Head1.GetComponent<Image>().sprite = Head1Sprites[5]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[5]; //無聊熊
                break;
            case AnimationStatus.HundredFiftyDays:
                Head1.GetComponent<Image>().sprite = Head1Sprites[6]; //我
                Head2.GetComponent<Image>().sprite = Head2Sprites[6]; //傷心的無聊熊
                break;
        }
    }

    public void ResetGameAnimStatus()
    {
        gameAnimIsEnd = false;
    }

    public void PlayerPressContinue()
    {
        if (canRespondToRKey) // 按 R 鍵重置動畫
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1;
                ResetGameAnimStatus();
                canStartGameAnim = true;
                gameAnimObjectShow = true;
                canRespondToRKey = false;
            }
        }
    }
}
