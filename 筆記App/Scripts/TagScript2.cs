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



public class TagScript2 : MonoBehaviour
{
    [SerializeField] private GameObject tagPrefab;
    [SerializeField] private Transform tagContainer;// 标签显示的父物体

    // [SerializeField] private GameObject showTagsPrefab;
    [SerializeField] private Transform TagsContainer;

    public TMP_InputField tagInput;
    private List<string> tags = new List<string>(); 
    public NoteData currentNoteData; // 当前正在编辑的笔记数据

    // private bool isBindingTagToNote = false;  // 用來區分操作模式
    public GameObject addTagBindingToNotePanel;
    private NotesManager noteManager;
    public  TagScriptWtihBindNote tagScriptWtihBindNote;
    public AddNote addNote;
    public GameObject showTagPanel;
    public GameObject TagPanel;
    public TextMeshProUGUI tagTitleText;
    // public GameObject addTagPanel;


    async void Start()
    {
        tagTitleText.text = "";
        noteManager = FindObjectOfType<NotesManager>();
        showTagPanel.SetActive(false);
        // addTagPanel.SetActive(false);
        addTagBindingToNotePanel.SetActive(false);
        await UnityServices.InitializeAsync();
        await LoadTagsFromCloud();

    }

    async void OnEnable()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TagPanel.SetActive(false);
        }
    }

    public async void SaveTag()
    {
        string tagText = tagInput.text.Trim();

        // 检查标签是否为空或已存在
        if (!string.IsNullOrEmpty(tagText) && !tags.Contains(tagText))
        {
            tags.Add(tagText); // 保存标签到列表

            // 如果有当前笔记数据，则把标签添加到当前笔记的标签列表中
            // if (currentNoteData != null)
            // {
            //     currentNoteData.tags.Add(tagText);
            // }

            // 创建并显示标签
            GameObject newTag = Instantiate(tagPrefab, tagContainer);
            TMP_Text tagTextComponent = newTag.GetComponentInChildren<TMP_Text>();
            if (tagTextComponent != null)
            {
                tagTextComponent.text = tagText;
            }

            // 清空输入框
            tagInput.text = "";

            Debug.Log($"Tag saved: {tagText}");
            await SaveTagsToCloud();
            DisplayTags();
            await tagScriptWtihBindNote.LoadTagsFromCloud();
        }
        else
        {
            Debug.Log("Tag is empty or already exists.");
        }
    }

    private async Task SaveTagsToCloud()
    {
        try
        {
            // 将标签列表包装并序列化为 JSON
            var tagListWrapper = new TagListWrapper { tags = tags };
            string tagsJson = JsonUtility.ToJson(tagListWrapper);

            // 创建一个 Dictionary，将标签数据保存到 Cloud Save
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "userTags", tagsJson }
            };

            // 调用 Cloud Save 的保存方法
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Tags saved to Unity Cloud Save.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save tags to Unity Cloud Save: {e.Message}");
        }
    }


    private async Task LoadTagsFromCloud()
    {
        try
        {
            // 从 Cloud Save 读取标签数据
            var keys = new HashSet<string> { "userTags" };
            var data = await CloudSaveService.Instance.Data.LoadAsync(keys);
            
            if (data.TryGetValue("userTags", out var tagsObject) && tagsObject is string tagsJson)
            {
                // 将 JSON 字符串转换为 List<string>
                tags = JsonUtility.FromJson<TagListWrapper>(tagsJson).tags;

                // 在 UI 上显示标签
                DisplayTags();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load tags from Unity Cloud Save: {e.Message}");
        }
    }

    public void DisplayTags()
    {
        Debug.Log("currentNoteData: " + currentNoteData.title);

        // 清空现有标签
        foreach (Transform child in tagContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建并显示标签
        foreach (var tag in tags)
        {
            GameObject newTag = Instantiate(tagPrefab, tagContainer);
            TMP_Text tagTextComponent = newTag.GetComponentInChildren<TMP_Text>();
            if (tagTextComponent != null)
            {
                tagTextComponent.text = tag;
            }

            GameObject checkBoxFrame = newTag.transform.Find("CheckBoxFrame")?.gameObject;
            GameObject checkMark = newTag.transform.Find("CheckMark")?.gameObject;
            GameObject buttonDelete = newTag.transform.Find("Button_Delete")?.gameObject;


            checkBoxFrame.SetActive(false);
            checkMark.SetActive(false);
   

           
            // 如果不是绑定到笔记的操作，点击时显示所有拥有此标签的笔记
            Button tagButton1 = newTag.GetComponentInChildren<Button>();
            if (tagButton1 != null)
            {
                string tagText = tag;
                tagButton1.onClick.AddListener(() => ShowNotesWithTag(tagText));  // 点击时显示所有拥有该标签的笔记
            }

            Button deleteButton = newTag.transform.Find("Button_Delete")?.GetComponent<Button>();
            if (deleteButton != null)
            {
                string tagText = tag; // 保存到局部变量以防闭包问题
                deleteButton.onClick.RemoveAllListeners(); // 防止事件重复绑定
                deleteButton.onClick.AddListener(() => DeleteTag(tagText));
            } else
            {
                Debug.Log("deleteButton is null");
            }

        }
    }

   
    private void OnTagClicked(GameObject checkMark, string tagText)
    {
        if (currentNoteData != null)
        {
            if (currentNoteData.tags.Contains(tagText))
            {
                // 若 tagText 已經存在於 tags 中，移除它並隱藏勾選符號
                currentNoteData.tags.Remove(tagText);
                if (checkMark != null)
                {
                    checkMark.SetActive(false);  // 隱藏√符號
                }
                Debug.Log($"Tag '{tagText}' removed from the current note.");
            }
            else
            {
                // 若 tagText 不存在於 tags 中，新增它並顯示勾選符號
                currentNoteData.tags.Add(tagText);
                if (checkMark != null)
                {
                    checkMark.SetActive(true);  // 顯示√符號
                }
                Debug.Log($"Tag '{tagText}' added to the current note.");
            }
        }
        else
        {
            Debug.LogError("CurrentNoteData is null. Cannot bind or unbind tag.");
        }
      
    }

    // 点击标签时显示所有拥有此标签的笔记
    private void ShowNotesWithTag(string tag)
    {
         foreach (Transform child in TagsContainer)
        {
            Destroy(child.gameObject);
        }

        List<NoteData> filteredNotes = noteManager.notes.FindAll(note => note.tags.Contains(tag));
        
        // 如果没有找到包含该标签的笔记，提示用户
        if (filteredNotes.Count == 0)
        {
            Debug.Log($"No notes found with tag: {tag}");
            return;
        }

        foreach (var note in filteredNotes)
        {
            GameObject newNotePanel = Instantiate(noteManager.notePanelPrefab, TagsContainer);
            NotePanel notePanel = newNotePanel.GetComponent<NotePanel>();
            if (notePanel != null)
            {
                notePanel.SetNoteData(note);
                Sprite noteBoxSprite = Resources.Load<Sprite>(note.noteBoxImage); // 確保顏色Sprite存在於Resources文件夾中
                if (noteBoxSprite != null)
                {
                    // Debug.Log("noteBoxSprite: " + noteBoxSprite);
                    notePanel.SetNoteBoxImage(noteBoxSprite); // 假設有這個方法來設置圖片
                } else
                {
                    Debug.LogError($"Failed to load noteBoxSprite for path: {note.noteBoxImage}");
                }
                tagTitleText.text = tag + "標籤";
            }
        }

        showTagPanel.SetActive(true);
   
    }

    public async void DeleteTag(string tagText)
    {
        if (tags.Contains(tagText))
        {
            // 從標籤列表中移除
            tags.Remove(tagText);
            Debug.Log($"Tag '{tagText}' removed.");

            foreach (var note in noteManager.notes)
            {
                if (note.tags.Contains(tagText))
                {
                    // noteManager.currentEditingNote = note;
                    note.tags.Remove(tagText);
                    await noteManager.SaveNotesToCloud();
                    Debug.Log($"Tag '{tagText}' removed from note '{note.title}'.");
                    // await noteManager.UpdateNote(
                    //     note.title, 
                    //     note.content,
                    //     tagScriptWtihBindNote.currentNoteData.tags
                    // );
                }
            }
            // 更新 Cloud Save
            await SaveTagsToCloud();
            await tagScriptWtihBindNote.LoadTagsFromCloud();
            // 更新 UI
            DisplayTags();

            // await SaveTagsToCloud();
        }
        else
        {
            Debug.LogWarning($"Tag '{tagText}' not found.");
        }
    }


   
    public void openAddTagPanel()
    {
        // isBindingTagToNote = false;
        // Debug.Log("isBindingTagToNote: " + isBindingTagToNote);
        TagPanel.SetActive(true);
                // DisplayTags();

    }

    // 设置当前编辑的笔记数据
    public void SetCurrentNoteData(NoteData noteData)
    {
        currentNoteData = noteData;
    }

    public void OnNoteSelected(NoteData selectedNote)
    {
        SetCurrentNoteData(selectedNote);
        DisplayTags();
    }
}
