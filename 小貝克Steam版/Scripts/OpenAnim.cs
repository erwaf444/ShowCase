using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenAnim : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject Head1;
    public GameObject Head2;
    public TextMeshProUGUI Name1;
    public TextMeshProUGUI Name2;
    public GameObject SpeakSign1;
    public GameObject SpeakSign2;
    public TextMeshProUGUI Content;
    public bool OpenAnimIsEnd = false;
    public bool canStartOpenAnim = false;
    public GameObject[] OpenAnimObjects;
    public LocaleSelector localeSelector;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public GameObject[] Arrows;
    public float flickerInterval = 0.5f; // 閃爍間隔時間
    private bool isFlickering = true;

    private int dialogueStep = 0; // 對話步驟計數器
    private string[] dialogues = {
        "你好，很高興終於來到這裡了！", // 角色1
        "你好！你就是新來的師傅是吧？我是這裡的房東，歡迎！", // 角色2
        "是的，謝謝你的歡迎，很高興見到你。", // 角色1
        "坐了這麼久的車程來到這座小鎮，你一定累壞了吧？需要休息一下嗎？", // 角色2
        "是啊，這裡的風景真不錯，我覺得這會是個很棒的地方。", // 角色1
        "我相信你會喜歡的！這個地方雖然小，但總有讓人放鬆的魅力。", // 角色2
        "看來我得好好探索一下，謝謝你的熱情接待。", // 角色1
        "不用客氣！不過在你開始工作之前，我需要先帶你先了解一下環境。", // 角色2
        "好的，我們開始吧！", // 角色1
        "千萬記得，在開始做麵包以前，一定需要定期洗手，如果太久沒有洗手的話，可是會扣除清潔度點數的哦！", // 角色2
        "定期洗手，好的明白了。", // 角色1
        "然後，烤箱只能夠放入一個麵包，所以你需要等到烤好一個麵包以後，才能夠製作第二個。", // 角色2
        "非常明白！", // 角色1
        "接著，不要做錯麵包了，要是做錯的話，之後又沒有客人上門，丟掉麵包也會扣除點數的哦！", // 角色2
        "知道了房東！", // 角色1
        "不僅如此！如果讓顧客不耐煩走掉的話，也會扣除顧客滿意度的啊！", // 角色2
        "真的啊！？", // 角色1
        "是的！所以手腳必須要快知道嗎？", // 角色2
        "明白了。", // 角色1
        "不要忘了，如果貓咪餓的時候，記得要喂它啊！", // 角色2
        "我不會忘記的！", // 角色1
        "每天打烊的時候，可能都會又其他的收入或是支出，需要好好的注意一下。", // 角色2
        "我會好好注意的。", // 角色1
        "最後最後！也是最重要的，要是你那個月付不出房租的話，我就會請你走人了！", // 角色2
        "什麼！？", // 角色1
        "哈哈！別看這裡是一個小鎮，其實這個地理位置也是搶手的！很多人搶著要呢！", // 角色2
        "真的嗎，真是看不出來。", // 角色1
        "你剛剛說什麼？", // 角色2
        "蛤？沒事啊！我什麼也沒說。", // 角色1
        "好了好了！那麼沒事的話，這個店面就交給你呢，以後記得按時交房租就行了！", // 角色2
        "好的，房東慢走。", // 角色1
    };

    private string[] names = { "我", "房東", "我", "房東" };
 
    void Start()
    {
        canStartOpenAnim = false;
        localeSelector.localID = PlayerPrefs.GetInt("LocaleKey", 0);
        if (TimeSystem.day == 1 && TimeSystem.month == 1 && TimeSystem.year == 2025)
        {
            OpenAnimIsEnd = false;
            GameObject gameManagerObject = GameObject.Find("GameManager");
            if (gameManagerObject != null)
            {
                gameManager = gameManagerObject.GetComponent<GameManager>();
            } 
            // gameManager.canInteract = false;
            SpeakSign1.SetActive(false);
            SpeakSign2.SetActive(false);
            UpdateDialogue(); // 初始化第一步對話
        } else
        {
            for (int i = 0; i < OpenAnimObjects.Length; i++)
            {
                OpenAnimObjects[i].SetActive(false);
                OpenAnimIsEnd = true;
            }
        }
        StartCoroutine(FlickerArrows());
    }

    void Update()
    {
        if (canStartOpenAnim)
        {
            if (Input.GetKeyDown(KeyCode.Space)) // 按空格鍵推進對話
            {
                dialogueStep++;
                if (dialogueStep < dialogues.Length)
                {
                    UpdateDialogue();
                }
                else
                {
                    EndDialogue(); // 可選：處理對話結束的邏輯
                }
            }
        }
    }

    void UpdateDialogue()
    {
        if (localeSelector.localID == 0)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            dialogues = new string[]
            {
                "Hello, I'm so glad to finally be here!", // Character 1
                "Hello! You must be the new master, right? I'm the landlord here, welcome!", // Character 2
                "Yes, thank you for the welcome. Nice to meet you.", // Character 1
                "After such a long journey to this small town, you must be exhausted. Do you need a break?", // Character 2
                "Yeah, the scenery here is really nice. I think this will be a great place.", // Character 1
                "I’m sure you’ll love it! This place is small, but it has a relaxing charm.", // Character 2
                "Looks like I’ll have to explore a bit. Thanks for the warm welcome.", // Character 1
                "You’re welcome! But before you start working, I need to show you around.", // Character 2
                "Alright, let’s get started!", // Character 1
                "Just remember, before making bread, you must wash your hands regularly. If you go too long without washing, your cleanliness points will decrease!", // Character 2
                "Regular hand washing, got it.", // Character 1
                "Also, the oven can only hold one bread at a time, so you’ll need to wait until one is baked before making another.", // Character 2
                "Understood!", // Character 1
                "And don’t make the wrong bread! If you make a mistake and no customers come, throwing away bread will cost you points!", // Character 2
                "Got it, landlord!", // Character 1
                "And if a customer leaves out of impatience, your customer satisfaction will also drop!", // Character 2
                "Really?!", // Character 1
                "Yes! So, be quick on your feet, okay?", // Character 2
                "Understood.", // Character 1
                "Don't forget, if the cat is hungry, remember to feed it!", // Character 2
                "I won't forget it!", // Character 1
                "At the end of each day, you might have some extra income or expenses, so keep an eye on it.", // Character 2
                "I’ll keep that in mind.", // Character 1
                "Finally—and this is the most important—if you can’t pay the rent for that month, I’ll have to ask you to leave!", // Character 2
                "What!?", // Character 1
                "Haha! Don’t underestimate this small town. It’s actually in a prime location, and many people are lining up to rent here!", // Character 2
                "Really? I couldn’t tell.", // Character 1
                "What did you just say?", // Character 2
                "Huh? Nothing! I didn’t say anything.", // Character 1
                "Alright, alright! If there’s nothing else, I’ll leave the shop to you. Just remember to pay the rent on time!", // Character 2
                "Sure thing. Take care, landlord.", // Character 1
            };
        } else if (localeSelector.localID == 1)
        {
            Content.font = TraditionalFontAsset;
            Content.SetAllDirty(); 
            dialogues = new string[]
            {
                "你好，很高興終於來到這裡了！", // 角色1
                "你好！你就是新來的師傅是吧？我是這裡的房東，歡迎！", // 角色2
                "是的，謝謝你的歡迎，很高興見到你。", // 角色1
                "坐了這麼久的車程來到這座小鎮，你一定累壞了吧？需要休息一下嗎？", // 角色2
                "是啊，這裡的風景真不錯，我覺得這會是個很棒的地方。", // 角色1
                "我相信你會喜歡的！這個地方雖然小，但總有讓人放鬆的魅力。", // 角色2
                "看來我得好好探索一下，謝謝你的熱情接待。", // 角色1
                "不用客氣！不過在你開始工作之前，我需要先帶你先了解一下環境。", // 角色2
                "好的，我們開始吧！", // 角色1
                "千萬記得，在開始做麵包以前，一定需要定期洗手，如果太久沒有洗手的話，可是會扣除清潔度點數的哦！", // 角色2
                "定期洗手，好的明白了。", // 角色1
                "然後，烤箱只能夠放入一個麵包，所以你需要等到烤好一個麵包以後，才能夠製作第二個。", // 角色2
                "非常明白！", // 角色1
                "接著，不要做錯麵包了，要是做錯的話，之後又沒有客人上門，丟掉麵包也會扣除點數的哦！", // 角色2
                "知道了房東！", // 角色1
                "不僅如此！如果讓顧客不耐煩走掉的話，也會扣除顧客滿意度的啊！", // 角色2
                "真的啊！？", // 角色1
                "是的！所以手腳必須要快知道嗎？", // 角色2
                "明白了。", // 角色1
                "不要忘了，如果貓咪餓的時候，記得要喂它啊！", // 角色2
                "我不會忘記的！", // 角色1
                "每天打烊的時候，可能都會又其他的收入或是支出，需要好好的注意一下。", // 角色2
                "我會好好注意的。", // 角色1
                "最後最後！也是最重要的，要是你那個月付不出房租的話，我就會請你走人了！", // 角色2
                "什麼！？", // 角色1
                "哈哈！別看這裡是一個小鎮，其實這個地理位置也是搶手的！很多人搶著要呢！", // 角色2
                "真的嗎，真是看不出來。", // 角色1
                "你剛剛說什麼？", // 角色2
                "蛤？沒事啊！我什麼也沒說。", // 角色1
                "好了好了！那麼沒事的話，這個店面就交給你呢，以後記得按時交房租就行了！", // 角色2
                "好的，房東慢走。", // 角色1
            };
        } else if (localeSelector.localID == 2)
        {
            Content.font = SimplifiedFontAsset;
            Content.SetAllDirty(); 
            dialogues = new string[]
            {
                "你好，很高兴终于来到这里了！", // 角色1
                "你好！你就是新来的师傅是吧？我是这里的房东，欢迎！", // 角色2
                "是的，谢谢你的欢迎，很高兴见到你。", // 角色1
                "坐了这么久的车程来到这座小镇，你一定累坏了吧？需要休息一下吗？", // 角色2
                "是啊，这里的风景真不错，我觉得这会是个很棒的地方。", // 角色1
                "我相信你会喜欢的！这个地方虽然小，但总有让人放松的魅力。", // 角色2
                "看来我得好好探索一下，谢谢你的热情接待。", // 角色1
                "不用客气！不过在你开始工作之前，我需要先带你先了解一下环境。", // 角色2
                "好的，我们开始吧！", // 角色1
                "千万记得，在开始做面包以前，一定需要定期洗手，如果太久没有洗手的话，可是会扣除清洁度点数的哦！", // 角色2
                "定期洗手，好的明白了。", // 角色1
                "然后，烤箱只能够放入一个面包，所以你需要等到烤好一个面包以后，才能够制作第二个。", // 角色2
                "非常明白！", // 角色1
                "接着，不要做错面包了，要是做错的话，之后又没有客人上门，丢掉面包也会扣除点数的哦！", // 角色2
                "知道了房东！", // 角色1
                "不仅如此！如果让顾客不耐烦走掉的话，也会扣除顾客满意度的啊！", // 角色2
                "真的啊！？", // 角色1
                "是的！所以手脚必须要快知道吗？", // 角色2
                "明白了。", // 角色1
                "不要忘了，如果猫咪饿的时候，记得要喂它啊！", // 角色2
                "我不会忘记的！", // 角色1
                "每天打烊的时候，可能都会有其他的收入或是支出，需要好好注意一下。", // 角色2
                "我会好好注意的。", // 角色1
                "最后最后！也是最重要的，要是你那个付不出房租的话，我就会请你走人了！", // 角色2
                "什么！？", // 角色1
                "哈哈！别看这里是一个小镇，其实这个地理位置也是抢手的！很多人抢着要呢！", // 角色2
                "真的吗，真是看不出来。", // 角色1
                "你刚刚说什么？", // 角色2
                "蛤？没事啊！我什么也没说。", // 角色1
                "好了好了！那么没事的话，这个店面就交给你呢，以后记得按时交房租就行了！", // 角色2
                "好的，房东慢走。", // 角色1
            };
        }
        
        if (dialogueStep % 2 == 0)
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
        Content.text = dialogues[dialogueStep];
    }

    void EndDialogue()
    {
        SpeakSign1.SetActive(false);
        SpeakSign2.SetActive(false);
        OpenAnimIsEnd = true;
        for (int i = 0; i < OpenAnimObjects.Length; i++)
        {
            OpenAnimObjects[i].SetActive(false);
        }
        // 可選：結束對話後的邏輯，比如開始遊戲
        // gameManager.canInteract = true;
    }

    private IEnumerator FlickerArrows()
    {
        while (isFlickering)
        {
            foreach (GameObject arrow in Arrows)
            {
                if (arrow != null)
                {
                    arrow.SetActive(!arrow.activeSelf); // 切換啟用狀態
                }
            }
            yield return new WaitForSeconds(flickerInterval); // 等待間隔時間
        }
    }

    // 可選：停止閃爍
    public void StopFlickering()
    {
        isFlickering = false;
    }
}
