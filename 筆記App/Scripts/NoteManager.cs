using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;


public class NotesManager : MonoBehaviour
{
    public static NotesManager Instance { get; set; }
    public List<NoteData> notes = new List<NoteData>();
    public event Action OnNotesUpdated;
    public GameObject notePanelPrefab;
    public Transform noteContainer;
    public NotePanel notePanel;
    public AddNote addNote;
    public GameObject notePanelUI;
    public NoteData currentEditingNote; 
    public TMP_InputField titleInput;
    public TMP_InputField contentInput;
    public Image noteBoxImage;
    public GameObject burgurMenuPanel;
    public string noteId;
    public GameObject deleteButton;
    public TagScriptWtihBindNote tagScript;

    private void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }
    }

    private async void Start()
    {
        notePanelUI.SetActive(false);
        // 初始化Unity服務
        await InitializeUnityServices();
        // 加載筆記
        // await LoadNotesFromCloud();
        TestNotification();
    }

    async void OnEnable()
    {
        await LoadNotesFromCloud(); 
    }
    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
        }
    }

    // 添加新筆記
    public async Task AddNote(string title, string content, Image noteBoxImage, List<string> tags)
    {
        string colorIdentifier = noteBoxImage.name; 
        // string tagIdentifier = tagText;
        Debug.Log("colorIdentifier: " + colorIdentifier);
        NoteData newNote = new NoteData(title, content)
        {
            id = Guid.NewGuid().ToString(),
            createdTime = DateTime.Now.ToString(),
            noteBoxImage = colorIdentifier,  // 設置外觀顏色
        };

        // 將傳入的所有標籤添加到新筆記
        if (tags != null && tags.Count > 0)
        {
            newNote.tags.AddRange(tags);
        }
        
        notes.Add(newNote);
        await SaveNotesToCloud();
        OnNotesUpdated?.Invoke();
    }

    // 刪除筆記
    public async Task DeleteNote(string noteId)
    {
        Debug.Log("Attempting to delete note with ID: " + noteId);
        if (string.IsNullOrEmpty(noteId))
        {
            Debug.LogError("noteId is invalid or empty!");
            return;
        }
        notes.RemoveAll(note => note.id == noteId);
        Debug.Log("Notes count after removal: " + notes.Count); 
        await SaveNotesToCloud();
        OnNotesUpdated?.Invoke();
    }

    public async void DeleteNoteFunc()
    {
        await DeleteNote(noteId);
        notePanelUI.SetActive(false);
    }

    // 從雲端加載筆記
    public async Task LoadNotesFromCloud()
    {
        try
        {
            if (CloudSaveService.Instance == null)
            {
                Debug.LogError("CloudSaveService.Instance is null!");
                return;
            }
            // Debug.Log("Attempting to load notes from cloud");
            var data = await CloudSaveService.Instance.Data.LoadAsync();
            
            if (data.TryGetValue("userNotes", out var notesJson))
            {
                notes = JsonUtility.FromJson<NotesList>(notesJson).notes;
                // Debug.Log($"Loaded {notes.Count} notes from cloud");

                foreach (Transform child in noteContainer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var note in notes)
                {
                    GameObject newNotePanel = Instantiate(notePanelPrefab, noteContainer);
                    NotePanel notePanel = newNotePanel.GetComponent<NotePanel>();
                    if (notePanel != null)
                    {
                        notePanel.SetNoteData(note);
                        // Debug.Log($"Trying to load noteBoxSprite from path: {note.noteBoxImage}");
                        // 加載對應顏色的Sprite
                        Sprite noteBoxSprite = Resources.Load<Sprite>(note.noteBoxImage); // 確保顏色Sprite存在於Resources文件夾中
                        if (noteBoxSprite != null)
                        {
                            // Debug.Log("noteBoxSprite: " + noteBoxSprite);
                            notePanel.SetNoteBoxImage(noteBoxSprite); // 假設有這個方法來設置圖片
                        } else
                        {
                            Debug.LogError($"Failed to load noteBoxSprite for path: {note.noteBoxImage}");
                        }

                        // 添加按钮并设置点击事件
                        Button detailButton = newNotePanel.GetComponentInChildren<Button>();
                        if (detailButton != null)
                        {
                            detailButton.onClick.AddListener(() => ShowNoteDetails(note));
                            detailButton.onClick.AddListener(() => tagScript.OnNoteSelected(note));
                            // detailButton.onClick.AddListener(() =>
                            // {
                            //     tagScript.OnNoteSelected(note);  // 设置当前笔记数据
                            //     tagScript.DisplayTags();         // 确保显示标签的时机在数据更新后
                            // });
                            // Debug.Log("tag select: " + tagScript.currentNoteData.title);
                        }
                    }
                }
            
            }
            OnNotesUpdated?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load notes: {e.Message}");
        }
    }

    public void ShowNoteDetails(NoteData note)
    {
        Debug.Log("這個noteId是" + note.id);
        Debug.Log($"Showing note: Title = {note.title}, Content = {note.content}");
        // 设置面板的文本内容
        noteId = note.id;
        Debug.Log("noteId: " + noteId);
        addNote.titleInput.text = note.title; // 假设 Note 类有一个 title 属性
        addNote.contentInput.text = note.content; // 假设 Note 类有一个 content 属性
        addNote.dateText.text = "創建時間:" + note.createdTime.ToString();
        currentEditingNote = note; // 设置当前正在编辑的笔记
        Debug.Log($"Title Text: {addNote.titleInput.text}, Content Text: {addNote.contentInput.text}");

        // 显示面板
        notePanelUI.SetActive(true);
        deleteButton.SetActive(true);
    }

    public void CreateNewNotePnael()
    {
        titleInput.text = "";
        contentInput.text = "";
        addNote.dateText.text = "";
        notePanelUI.SetActive(true);
        deleteButton.SetActive(false);
        currentEditingNote = null;
    }

    public async Task UpdateNote(string title, string content, List<string> tags)
    {
        if (currentEditingNote != null)
        {
            // 找到正在编辑的笔记在列表中的索引
            int noteIndex = notes.FindIndex(n => n.id == currentEditingNote.id);
            if (noteIndex != -1)
            {
                // 更新笔记内容
                notes[noteIndex].title = title;
                notes[noteIndex].content = content;
                notes[noteIndex].noteBoxImage = currentEditingNote.noteBoxImage; // 保持原有的颜色
                
                // Clear current tags and add the updated tags
                notes[noteIndex].tags.Clear();
                notes[noteIndex].tags.AddRange(tags);

                Debug.Log($"Updating note at index {noteIndex}: Title = {title}, Content = {content}");
                
                // 保存到云端
                await SaveNotesToCloud();
                
                // 重新加载显示
                await LoadNotesFromCloud();
            }
            else
            {
                Debug.LogError($"Failed to find note with ID: {currentEditingNote.id}");
            }
        }
        else
        {
            Debug.LogError("No note is currently being edited");
        }
    }

    // 保存筆記到雲端
    public async Task SaveNotesToCloud()
    {
        try
        {
            var notesData = new NotesList { notes = notes };
            string jsonData = JsonUtility.ToJson(notesData);
            
            var data = new Dictionary<string, object> { { "userNotes", jsonData } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            
            Debug.Log($"Saved {notes.Count} notes to cloud");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save notes: {e.Message}");
        }
    }

    // 獲取所有筆記
    public List<NoteData> GetAllNotes()
    {
        return new List<NoteData>(notes);
    }

   

    public async void OnSetReminder(DateTime selectedTime)
    {
        if (currentEditingNote != null)
        {
            await SetReminder(currentEditingNote.id, selectedTime);
            Debug.Log($"Reminder set for note {currentEditingNote.title} at {selectedTime}");
        }
    }
    public async Task SetReminder(string noteId, DateTime reminderTime)
    {
        NoteData note = notes.Find(n => n.id == noteId);
        if (note != null)
        {
            note.reminderTime = reminderTime.ToString("o"); // 轉換為 ISO 8601 格式
            await SaveNotesToCloud(); // 保存更新到雲端

            // 設置本地通知
            Notifications.Instance.ScheduleNotification(note.title, note.content, reminderTime);
            Debug.Log($"Reminder set for note: {note.title} at {reminderTime}");

        }
    }
    public void DisplayReminder(NoteData note)
    {
        if (!string.IsNullOrEmpty(note.reminderTime))
        {
            DateTime reminderTime = DateTime.Parse(note.reminderTime);
            Debug.Log($"Reminder set for {note.title}: {reminderTime}");
            // 在面板中顯示提醒時間
        }
    }

    private void TestNotification()
    {
        DateTime reminderTime = DateTime.Now.AddSeconds(10); // 10秒後觸發
        Notifications.Instance.ScheduleNotification("Test Reminder", "This is a test notification.", reminderTime);
    }

}

// 更新NoteData類
[System.Serializable]
public class NoteData
{ 
    public string id;
    public string title;
    public string content;
    public string createdTime;
    public string noteBoxImage; 
    public List<string> tags; 
    public string reminderTime;

    public NoteData(string title, string content)
    {
        this.title = title;
        this.content = content;
        this.tags = new List<string>();
        this.reminderTime = null;
    }
}

// 用於序列化的包裝類
[System.Serializable]
public class NotesList
{
    public List<NoteData> notes = new List<NoteData>();
}