using UnityEngine;
using Unity.Notifications.Android;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System.Linq;

public class NoticeScript : MonoBehaviour
{
    [Serializable]
    public class Reminder
    {
        public string id;
        public string title;
        public string message;
        public DateTime reminderTime;
        public bool isRepeating;
        public int repeatInterval; // 重複間隔（分鐘）
    }

    public List<Reminder> reminders = new List<Reminder>();
    public TMP_InputField titleInput;
    public TMP_InputField contentInput;
    public TMP_InputField monthInput;
    public TMP_InputField dayInput;
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public Button addReminderButton;
    public GameObject successAddPanel;
    public GameObject successUpdatePanel;
    public GameObject backgroundMask; // 背景遮罩

    public TextMeshProUGUI selectedDayText;
    public Transform remindContainer;
    public GameObject remindPrefab;
    public GameObject reminderPanel;
    public GameObject reminderPanelDeleteBtn;
    private string currentEditingReminderId = null;
    public GameObject addNoticePanel;



    async void Start()
    {
        // 初始化通知渠道
        var channel = new AndroidNotificationChannel()
        {
            Id = "reminder_channel",
            Name = "Reminder Notifications",
            Importance = Importance.Default,
            Description = "Custom user reminders"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        successAddPanel.SetActive(false);
        backgroundMask.SetActive(false); // 初始时遮罩隐藏

        // 監聽月份、日期、小時、分鐘的變化
        monthInput.onValueChanged.AddListener(UpdateSelectedDayText);
        dayInput.onValueChanged.AddListener(UpdateSelectedDayText);
        hourInput.onValueChanged.AddListener(UpdateSelectedDayText);
        minuteInput.onValueChanged.AddListener(UpdateSelectedDayText);
        await LoadRemindersFromCloud();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            reminderPanel.SetActive(false);
        }
    }

    void UpdateSelectedDayText(string input)
    {
        // 確保月份、日期、小時和分鐘有效
        if (int.TryParse(monthInput.text, out int month) && month >= 1 && month <= 12 &&
            int.TryParse(dayInput.text, out int day) && day >= 1 && day <= DateTime.DaysInMonth(DateTime.Now.Year, month) &&
            int.TryParse(hourInput.text, out int hour) && hour >= 0 && hour <= 23 &&
            int.TryParse(minuteInput.text, out int minute) && minute >= 0 && minute <= 59)
        {
            // 更新顯示的日期和時間文本
            selectedDayText.text = $"提醒時間: {month}/{day} {hour:D2}:{minute:D2}";
        }
        else
        {
            // 顯示無效日期提示
            selectedDayText.text = "";
        }
    }

    // 添加新提醒
    public async void AddReminder(string title, string message, DateTime reminderTime, bool isRepeating = false, int repeatInterval = 0)
    {
        Reminder newReminder = new Reminder
        {
            id = Guid.NewGuid().ToString(),
            title = title,
            message = message,
            reminderTime = reminderTime,
            isRepeating = isRepeating,
            repeatInterval = repeatInterval
        };

        reminders.Add(newReminder);
        ScheduleReminder(newReminder);
        await SaveReminderToCloud(newReminder);
        StartCoroutine(SucessAddPanelRoutine());
        Debug.Log("提醒添加成功!");
        foreach (Transform child in remindContainer)
        {
            Destroy(child.gameObject);
        }
        await LoadRemindersFromCloud();
    }

    // 調度提醒通知
    private void ScheduleReminder(Reminder reminder)
    {
        var notification = new AndroidNotification
        {
            Title = reminder.title,
            Text = reminder.message,
            FireTime = reminder.reminderTime
        };

        // 發送通知
        int notificationId = AndroidNotificationCenter.SendNotification(notification, "reminder_channel");

        // 如果是重複提醒
        if (reminder.isRepeating)
        {
            // 安排下一次提醒
            reminder.reminderTime = reminder.reminderTime.AddMinutes(reminder.repeatInterval);
            ScheduleReminder(reminder);
        }
    }

    // 刪除特定提醒
    public void RemoveReminder(string title)
    {
        reminders.RemoveAll(r => r.title == title);
    }

    // 清除所有提醒
    public void ClearAllReminders()
    {
        reminders.Clear();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
    }

    // 範例使用方法
    public void ExampleReminderUsage()
    {
        // 單次提醒
        AddReminder("Meeting", "Team meeting in 30 minutes", DateTime.Now.AddMinutes(30));

        // 重複提醒（每小時）
        AddReminder("Water Reminder", "Time to drink water!", DateTime.Now.AddHours(1), true, 60);
    }

    public void ScheduleReminderAtSpecificTime(string title, string message, int hour, int minute)
    {
        DateTime reminderTime = DateTime.Today.AddHours(hour).AddMinutes(minute);
        
        // 如果選擇的時間已經過去，則設置為明天
        if (reminderTime <= DateTime.Now)
        {
            reminderTime = reminderTime.AddDays(1);
        }

        AddReminder(title, message, reminderTime);
    }

    public void AddUserReminder()
    {
        // 讀取用戶輸入的標題和內容
        string title = titleInput.text;
        string message = contentInput.text;

        // 假設你已經有了方法來選擇提醒的時間
        DateTime reminderTime = DateTime.Now.AddMinutes(30);  // 默認提醒時間為30分鐘後

        // 調用 AddReminder 方法，這裡你可以根據需求進一步獲取重複提醒的選項
        bool isRepeating = false;  // 設置是否為重複提醒
        int repeatInterval = 0;    // 設置重複間隔，這裡默認為不重複

        // 添加提醒
        AddReminder(title, message, reminderTime, isRepeating, repeatInterval);
    }



    public async void AddUserReminderWithTime()
    {
        // 讀取用戶輸入的標題、內容以及時間
        string title = titleInput.text;
        string message = contentInput.text;
        
        if (!int.TryParse(monthInput.text, out int month) || month < 1 || month > 12)
        {
            Debug.LogError("請輸入有效的月份（1到12之間）");
            return;
        }

        if (!int.TryParse(dayInput.text, out int day) || day < 1 || day > DateTime.DaysInMonth(DateTime.Now.Year, month))
        {
            Debug.LogError($"請輸入有效的日期（1到{DateTime.DaysInMonth(DateTime.Now.Year, month)} 之間）");
            return;
        }

        if (!int.TryParse(hourInput.text, out int hour) || hour < 0 || hour > 23)
        {
            // 顯示錯誤訊息或做其他處理
            Debug.LogError("請輸入有效的時間（小時必須在 0 到 23 之間）");
            return;
        }

        if (!int.TryParse(minuteInput.text, out int minute) || minute < 0 || minute > 59)
        {
            // 顯示錯誤訊息或做其他處理
            Debug.LogError("請輸入有效的時間（分鐘必須在 0 到 59 之間）");
            return;
        }

        int year = DateTime.Now.Year;
        DateTime reminderTime;
        
        try
        {
            reminderTime = new DateTime(year, month, day, hour, minute, 0);
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.LogError("請輸入有效的日期和時間");
            return;
        }
      
        

        // 添加提醒
        // AddReminder(title, message, reminderTime);

        if (!string.IsNullOrEmpty(currentEditingReminderId))
        {
            var existingReminder = reminders.Find(r => r.id == currentEditingReminderId);
            if (existingReminder != null)
            {
                existingReminder.title = title;
                existingReminder.message = message;
                existingReminder.reminderTime = reminderTime;

                // 更新雲端存儲
                await SaveReminderToCloud(existingReminder);
                foreach (Transform child in remindContainer)
                {
                    Destroy(child.gameObject);
                }
                await LoadRemindersFromCloud();
                Debug.Log("提醒更新成功！");
                StartCoroutine(SucessUpdatePanelRoutine());
            }
        }
        else
        {
            // 如果沒有編輯的提醒 ID，則添加新提醒
            AddReminder(title, message, reminderTime);
        }

        // 重置當前編輯的提醒 ID
        currentEditingReminderId = null;
    }

    IEnumerator SucessAddPanelRoutine()
    {
        backgroundMask.SetActive(true);
        successAddPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        successAddPanel.SetActive(false);
        backgroundMask.SetActive(false);
        reminderPanel.SetActive(false);

    }

    IEnumerator SucessUpdatePanelRoutine()
    {
        backgroundMask.SetActive(true);
        successUpdatePanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        successUpdatePanel.SetActive(false);
        backgroundMask.SetActive(false);
        reminderPanel.SetActive(false);

    }

   

    public async Task SaveReminderToCloud(Reminder reminder)
    {
        var remindData = new RemindData(reminder.id, reminder.title, reminder.message)
        {
            reminderTime = reminder.reminderTime.ToString("yyyy-MM-dd HH:mm:ss")
        };

        try
        {
            // string validKey = "reminder_" + SanitizeKey(reminder.title); // 清理特殊字符
            string validKey = "reminder_" + reminder.id; // 用唯一 ID 代替 title

            // 获取现有的键列表
            var allRemindersKey = "all_reminders";
            List<string> keys = null;
            try
            {
                var loadedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { allRemindersKey });
                if (loadedData.ContainsKey(allRemindersKey))
                {
                    keys = JsonUtility.FromJson<StringListWrapper>(loadedData[allRemindersKey]).keys;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"讀取 all_reminders 失敗: {e.Message}");
            }

            if (keys == null)
            {
                keys = new List<string>(); // 如果讀取失敗或資料不存在，初始化為新列表
            }

            // 如果当前键不存在，则添加
            if (!keys.Contains(validKey))
            {
                keys.Add(validKey);
            }

            var wrapper = new StringListWrapper { keys = keys };

            var saveData = new Dictionary<string, object>
            {
                { validKey, JsonUtility.ToJson(remindData) }, // 保存提醒数据
                { allRemindersKey, JsonUtility.ToJson(wrapper) } // 保存所有键列表
            };
            Debug.Log($"即将保存的数据键: {validKey}, 数据内容: {JsonUtility.ToJson(remindData)}");
            Debug.Log("allRemindersKey: " + string.Join(", ", keys));
            Debug.Log($"保存 all_reminders 数据: {JsonUtility.ToJson(wrapper)}");


            await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);
            Debug.Log("提醒数据已保存到 Cloud Save");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存数据到 Cloud Save 失败: {e.Message}");
        }
    }

    public async void DeleteReminder(string reminderId)
    {
        try 
        {
            var reminderToRemove = reminders.Find(r => r.id == reminderId);
            if (reminderToRemove != null)
            {
                reminders.Remove(reminderToRemove);
            } 
            else
            {
                Debug.LogWarning($"找不到指定 ID 的提醒: {reminderId}");
                return;
            }
           
            // 2. 從雲端存儲中刪除
            // string validKey =  "reminder_" + SanitizeKey(reminderToRemove.title);
            string validKey = "reminder_" + reminderId; // 用唯一 ID 代替 title

            var allRemindersKey = "all_reminders";

            // 2.1 加載保存的提醒鍵列表
            var loadedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { allRemindersKey });
            if (loadedData.ContainsKey(allRemindersKey))
            {
                var wrapper = JsonUtility.FromJson<StringListWrapper>(loadedData[allRemindersKey]);
                wrapper.keys.Remove(validKey);

                // 2.2 更新雲端存儲
                var saveData = new Dictionary<string, object>
                {
                    { allRemindersKey, JsonUtility.ToJson(wrapper) }
                };

                await CloudSaveService.Instance.Data.ForceDeleteAsync(validKey);
                await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);
                Debug.Log($"提醒已成功刪除: {reminderToRemove.title}");

                // 2.3 更新本地提醒列表
                foreach (Transform child in remindContainer)
                {
                    Destroy(child.gameObject);
                }
                await LoadRemindersFromCloud();
                // 2.4 關閉面板
                reminderPanel.SetActive(false);
                
            }
            else
            {
                Debug.LogWarning("沒有找到 all_reminders 鍵");
            }
        
        } 
        catch(Exception e)
        {
            Debug.LogError($"刪除提醒失敗: {e.Message}");
        }
    }

    private string SanitizeKey(string key)
    {
        // 移除键中的特殊字符，只保留字母数字字符
        return new string(key.Where(char.IsLetterOrDigit).ToArray());
    }

    public async Task LoadRemindersFromCloud()
    {
        Debug.Log("LoadRemindersFromCloud開始工作");
        try
        {
            var allRemindersKey = "all_reminders";

            // 加载保存的键列表
            var loadedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { allRemindersKey });
            if (!loadedData.ContainsKey(allRemindersKey))
            {
                Debug.Log("没有提醒数据可加载");
                return;
            }

            var wrapper = JsonUtility.FromJson<StringListWrapper>(loadedData[allRemindersKey]);
            if (wrapper == null || wrapper.keys == null)
            {
                Debug.LogError("解析 all_reminders 数据失败或为空");
                return;
            }

            var keys = wrapper.keys;

            // var keys = JsonUtility.FromJson<List<string>>(loadedData[allRemindersKey]);
        
            foreach (var key in keys)
            {
                // 加载单个提醒数据
                var reminderData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key });
                var remindData = JsonUtility.FromJson<RemindData>(reminderData[key]);

                // remindData.id = key;  // 這步很重要

                // 將提醒添加到本地 reminders 列表
                var newReminder = new Reminder
                {
                    id = remindData.id, // 使用 key 作為 ID
                    title = remindData.title,
                    message = remindData.content,
                    reminderTime = DateTime.Parse(remindData.reminderTime)
                };
                reminders.Add(newReminder);

                Debug.Log($"加载的提醒: {remindData.title}, 时间: {remindData.reminderTime}");

                // 实例化提醒预制体
                var remindInstance = Instantiate(remindPrefab, remindContainer);

                var titleText = remindInstance.transform.Find("Title").GetComponent<TextMeshProUGUI>();
                var contentText = remindInstance.transform.Find("Content").GetComponent<TextMeshProUGUI>();
                var timeText = remindInstance.transform.Find("Time").GetComponent<TextMeshProUGUI>();

                if (titleText != null) titleText.text = remindData.title;
                if (contentText != null) contentText.text = remindData.content;
                if (timeText != null) {
                    DateTime parsedTime;
                    
                    if (DateTime.TryParse(remindData.reminderTime, out parsedTime))
                    {
                        timeText.text = parsedTime.ToString("MM/dd HH:mm");
                    } else 
                    {
                        Debug.LogWarning($"無法解析提醒時間: {remindData.reminderTime}");
                        timeText.text = remindData.reminderTime; // 解析失敗時保留原字串
                    }
                }

                // 添加按钮并设置点击事件
                Button detailButton = remindInstance.GetComponentInChildren<Button>();
                if (detailButton != null)
                {
                    detailButton.onClick.AddListener(() => ShowRemindDetails(remindData));
                    // detailButton.onClick.AddListener(() => tagScript.OnNoteSelected(note));
                  
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载数据失败: {e.Message}");
        }
    }

    public void ShowRemindDetails(RemindData remindData)
    {
        // 设置面板的文本内容
        titleInput.text = remindData.title;
        contentInput.text = remindData.content;

        DateTime parsedTime;

        if (DateTime.TryParse(remindData.reminderTime, out parsedTime))
        {
            // 分配時間到各個 Input 欄位
            dayInput.text = parsedTime.Day.ToString("00");    // 日 (兩位數)
            monthInput.text = parsedTime.Month.ToString("00"); // 月 (兩位數)
            hourInput.text = parsedTime.Hour.ToString("00");   // 時 (兩位數)
            minuteInput.text = parsedTime.Minute.ToString("00"); // 分 (兩位數)
        }

        currentEditingReminderId = remindData.id;

        Button deleteButton = reminderPanelDeleteBtn.GetComponentInChildren<Button>();
        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners(); // 清空之前的事件
            deleteButton.onClick.AddListener(() => DeleteReminder(remindData.id));
        }

        // 显示面板
        reminderPanel.SetActive(true);
        reminderPanelDeleteBtn.SetActive(true);
        // deleteButton.SetActive(true);

    }

    public void OpenAddNoticePanel()
    {
        reminderPanel.SetActive(true);
        reminderPanelDeleteBtn.SetActive(false);
        titleInput.text = "";
        contentInput.text = "";
        dayInput.text = "";
        monthInput.text = "";
        hourInput.text = "";
        minuteInput.text = "";
    }

    public void CloseAddNoticePanel()
    {
        reminderPanel.SetActive(false);
        currentEditingReminderId = null;

    }
}

public class RemindData
{ 
    public string id;
    public string title;
    public string content;
    // public string createdTime;
    // public string noteBoxImage; 
    // public List<string> tags; 
    public string reminderTime;

    public RemindData(string id, string title, string content)
    {
        this.id = id;
        this.title = title;
        this.content = content;
        // this.tags = new List<string>();
        this.reminderTime = null;
    }
}

[Serializable]
public class StringListWrapper
{
    public List<string> keys;
}
