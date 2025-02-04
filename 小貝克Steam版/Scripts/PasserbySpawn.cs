using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class MenuItem 
{ 
    public int id; // 或者使用 string 类型，如果你希望 ID 更具可读性
    //蛋糕id從0開始
    //甜甜圈id從100開始
    //麵包id從200開始
    //杯子蛋糕id從300開始
    //造型面包id從400開始
    public Sprite image; // 对应的图片
    public int price;
    public int experiencePoints;
}

//路人說話
[System.Serializable]
public class DialogueEntry
{
    public string[] greetings;
    public string[] requests;
    public string[] thanks;
}

public class PasserbySpawn : MonoBehaviour
{
    public Player player;
    public MakeTableTagSwitcher makeTableTagSwitcher;
    public GameObject passerbyPrefab;
    public Transform spawnPointA;
    public Transform spawnPointB;
    public int numberOfPasserbys;
    public float stopChance;
    public GameObject pointD;
    public GameObject pointE;
    public GameObject pointF;
    // public Sprite[] menuOrder;
    public MenuItem[] menuOrder;
    private bool allowPasserbyToStop = true; // 標誌是否允許新路人停下

    private List<Passerby> activePasserbys = new List<Passerby>();
    private Dictionary<Transform, Passerby> stoppedPasserbys = new Dictionary<Transform, Passerby>();
    private List<Transform> availableStopPoints = new List<Transform>();
    private GameManager gameManager;
    
    
    //路人說話屬性
    public DialogueEntry dialogues;
    public TextMeshProUGUI dialogueText;
    public float dialogueDuration = 3f;
    private Coroutine dialogueCoroutine; // 用于跟踪当前对话的协程
    public OpenAnim openAnimScript;
    public GameAnim gameAnimScript;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public TMP_FontAsset TraditionalFontAsset2;//這是在第一個asset不能使用的時候
    public LocaleSelector localeSelector;
    public TimeSystem timeSystem;


    void Start()
    {
        timeSystem = FindObjectOfType<TimeSystem>(); 
        GameObject openAnimObject = GameObject.Find("OpenAnimation");
        if (openAnimObject != null)
        {
            openAnimScript = openAnimObject.GetComponent<OpenAnim>();
        }
        GameObject gameAnimObject = GameObject.Find("GameAnimation");
        if (gameAnimObject != null)
        {
            gameAnimScript = gameAnimObject.GetComponent<GameAnim>();
        }
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
        pointD = GameObject.Find("PointD");
        pointE = GameObject.Find("PointE");
        pointF = GameObject.Find("PointF");

        availableStopPoints.Add(pointD.transform);
        availableStopPoints.Add(pointE.transform);
        availableStopPoints.Add(pointF.transform);
        Debug.Log("Spawn coroutine started"); 
        
        StartCoroutine(WaitForAnimationEnd());
    }


    IEnumerator WaitForAnimationEnd()
    {
        // 等待直到 OpenAnimIsEnd 为 true
        while (!openAnimScript.OpenAnimIsEnd || !gameAnimScript.gameAnimIsEnd)
        {
            yield return null; // 每帧检查一次
        }
        
        if (openAnimScript.OpenAnimIsEnd && gameAnimScript.gameAnimIsEnd)
        {
            yield return new WaitForSeconds(3f);
            // 当动画结束时，开始生成路人
            StartCoroutine(SpawnPasserbys());
        }
    }

    void Update()
    {
        if (localeSelector.localID == 0)
        {
            dialogues.greetings = new string[]
            {
                "Hello!",
                "Are there any more?",
                "Um....",
                "Let me see."
            };
            
            dialogues.requests = new string[]
            {
                "I want something delicious.",
                "I want this.",
                "This looks good.",
                "That's it."
            };
            
            dialogues.thanks = new string[]
            {
                "Great! Thank you!",
                "You're the best!",
                "Looks delicious!",
                "Thank you very much."
            };
            dialogueText.font = TraditionalFontAsset;
        } else if (localeSelector.localID == 1)
        {   
            dialogues.greetings = new string[]
            {
                "你好啊！",
                "請問還有嗎？",
                "嗯...。",
                "讓我看看。"
            };
            dialogues.requests = new string[]
            {
                "我想要一個好吃的。",
                "我想要這個。",
                "這看起來不錯。",
                "就這個吧。"
            };
            
            dialogues.thanks = new string[]
            {
                "太棒了！謝謝！",
                "你最棒了！",
                "看起來真好吃！",
                "感激不盡。"
            };
            dialogueText.font = TraditionalFontAsset2;
        } else if (localeSelector.localID == 2)
        {
            dialogues.greetings = new string[]
            {
                "你好啊！",
                "请问还有吗？",
                "嗯...。",
                "让我看看。"
            };
            
            dialogues.requests = new string[]
            {
                "我想要一个好吃的。",
                "我想要这个。",
                "这看起来不错。",
                "就这个吧。"
            };
            dialogues.thanks = new string[]
            {
                "太棒了！谢谢！",
                "你最棒了！",
                "看起来真好吃！",
                "感激不尽。"
            };
            dialogueText.font = SimplifiedFontAsset;
        }

      
        if (TimeSystem.AccumulateDay >= 1)
        {
            stopChance = 0.1f;
        } else if (TimeSystem.AccumulateDay >= 20)
        {
            stopChance = 0.2f;
        } else if (TimeSystem.AccumulateDay >= 50)
        {
            stopChance = 0.3f;
        } else if (TimeSystem.AccumulateDay >= 100)
        {
            stopChance = 0.4f;
        }

        // Debug.Log("stopChance" + stopChance);
    }
/// Each passerby is assigned waypoints and added to the active passerbys list.
/// Passerbys have a chance to stop at random intervals if stop points are available.
/// </summary>

    IEnumerator SpawnPasserbys()
    {
        for (int i = 0; i < numberOfPasserbys; i++)
        {
            // Instantiate the prefab
            GameObject passerbyObj = Instantiate(passerbyPrefab, spawnPointA.position, Quaternion.identity);
            
            Passerby passerbyScript = passerbyObj.GetComponent<Passerby>();
            if (passerbyScript != null)
            {
                passerbyScript.pointA = spawnPointA.gameObject;
                passerbyScript.pointB = spawnPointB.gameObject;
            }

            activePasserbys.Add(passerbyScript);

            float randomValue  = Random.Range(0f, 1f);
            if (availableStopPoints.Count > 0 && randomValue  < stopChance)
            {
                Debug.Log(randomValue + " stopChance" + stopChance);
                // 随机停止
                StopPasserby(passerbyScript);
            }
            

            float randomInterval = Random.Range(2f, 5f);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    private void StopPasserby(Passerby passerby)
    {
        if (availableStopPoints.Count == 0) return;

        int randomIndex = Random.Range(0, availableStopPoints.Count);
        Transform selectedStopPoint = availableStopPoints[randomIndex];
        availableStopPoints.RemoveAt(randomIndex);

        // 选择符合条件的菜单项
        List<MenuItem> validMenuItems = new List<MenuItem>();

        foreach (MenuItem item in menuOrder)
        {
            if (player.experiencePoints >= item.experiencePoints)
            {
                validMenuItems.Add(item);
            }
        }


        // 确保有可用的菜单项
        if (validMenuItems.Count > 0)
        {
            MenuItem selectedImage = validMenuItems[Random.Range(0, validMenuItems.Count)]; // 从有效菜单项中随机选择
            passerby.SetStopPoint(selectedStopPoint);
            passerby.ShowImage(selectedImage.image, selectedImage.id, selectedImage.price);
        
            string greeting = dialogues.greetings[Random.Range(0, dialogues.greetings.Length)];
            string request = dialogues.requests[Random.Range(0, dialogues.requests.Length)];
            Say($"{greeting} {request}");
        }

        // MenuItem selectedImage = menuOrder[Random.Range(0, menuOrder.Length)];
        // passerby.SetStopPoint(selectedStopPoint);
        // passerby.ShowImage(selectedImage.image, selectedImage.id, selectedImage.price);
        
        
        Rigidbody2D rb = passerby.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        stoppedPasserbys[selectedStopPoint] = passerby;
        StartCoroutine(HandlePasserbyStop(passerby, selectedStopPoint));
    }

    IEnumerator HandlePasserbyStop(Passerby passerby, Transform stopPoint)
    {
        float stopDuration = 30f;
        yield return new WaitForSeconds(stopDuration);

        if (stoppedPasserbys.ContainsKey(stopPoint) && stoppedPasserbys[stopPoint] == passerby)
        {
            ResumePasserby(passerby, stopPoint);
        }
    }
 
    public void ResumePasserby(Passerby passerby, Transform stopPoint)
    {
        gameManager.MinusPlayerCustomerHappyValue();
        passerby.shouldStop = false;
        Rigidbody2D rb = passerby.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(passerby.speed, 0f);
        }
        passerby.HideImage();
        stoppedPasserbys.Remove(stopPoint);
        availableStopPoints.Add(stopPoint);
    }

    public void GetFoodAndResumePasserby(Passerby passerby, Transform stopPoint)
    {
        passerby.shouldStop = false;
        Rigidbody2D rb = passerby.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(passerby.speed, 0f);
        }
        passerby.HideImage();
        stoppedPasserbys.Remove(stopPoint);
        availableStopPoints.Add(stopPoint);

        string thanks = dialogues.thanks[Random.Range(0, dialogues.thanks.Length)];
        SayThanks(thanks);
    }

    public List<Passerby> GetStoppedPasserbys()
    {
        return new List<Passerby>(stoppedPasserbys.Values);
    }

    public bool IsStopPointAvailable()
    {
        return availableStopPoints.Count > 0;
    }


    //路人說話方法
    public void Say(string message)
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine); // 停止当前对话的协程
        }
        dialogueCoroutine = StartCoroutine(ShowDialogue(message)); // 开始新的对话协程
    }

    public void SayThanks(string message)
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine); // 停止当前对话的协程
        }
        dialogueCoroutine = StartCoroutine(ShowThanksDialogue(message)); // 开始新的对话协程

    }

    private IEnumerator ShowDialogue(string message)
    {
        // dialogueText.text = message;
        // dialogueText.gameObject.SetActive(true);
        // yield return new WaitForSeconds(dialogueDuration);
        // dialogueText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = ""; // 清空文本以便逐字显示

        // 根据消息长度计算播放音效的总时间
        float sfxDuration = 0.2f; // 每个字符播放的时间
        AudioManager.instance.PlaySFX("DingDong");

        foreach (char letter in message)
        {
            dialogueText.text += letter; // 逐个添加字符
            yield return new WaitForSeconds(sfxDuration); // 设置字符显示的延迟时间
        }

        yield return new WaitForSeconds(dialogueDuration); // 显示完整文本的时间
        dialogueText.gameObject.SetActive(false);
    }

    private IEnumerator ShowThanksDialogue(string message)
    {
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = ""; // 清空文本以便逐字显示

        // 根据消息长度计算播放音效的总时间
        float sfxDuration = 0.2f; // 每个字符播放的时间

        foreach (char letter in message)
        {
            dialogueText.text += letter; // 逐个添加字符
            yield return new WaitForSeconds(sfxDuration); // 设置字符显示的延迟时间
        }

        yield return new WaitForSeconds(dialogueDuration); // 显示完整文本的时间
        dialogueText.gameObject.SetActive(false);
    }

    
}

