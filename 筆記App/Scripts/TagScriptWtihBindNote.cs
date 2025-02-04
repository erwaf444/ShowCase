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


public class TagScriptWtihBindNote : MonoBehaviour
{
    
    [SerializeField] private GameObject tagPrefab;
    [SerializeField] private Transform tagContainer;// 标签显示的父物体

    // [SerializeField] private GameObject showTagsPrefab;

    public TMP_InputField tagInput;
    private List<string> tags = new List<string>(); 
    public NoteData currentNoteData; // 当前正在编辑的笔记数据

    public GameObject addTagBindingToNotePanel;
    private NotesManager noteManager;
    public TextMeshProUGUI tagTitleText;


    async void Start()
    {
        tagTitleText.text = "";
        noteManager = FindObjectOfType<NotesManager>();
        // addTagBindingToNotePanel.SetActive(false);
        await UnityServices.InitializeAsync();
        await LoadTagsFromCloud();

    }

    async void OnEnable()
    {

    }
    void Update()
    {
        
    }

    public async void SaveTag()
    {
        string tagText = tagInput.text.Trim();

        // 检查标签是否为空或已存在
        if (!string.IsNullOrEmpty(tagText) && !tags.Contains(tagText))
        {
            tags.Add(tagText); // 保存标签到列表

          

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


    public async Task LoadTagsFromCloud()
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

            buttonDelete.SetActive(false);
            checkBoxFrame.SetActive(true);
 

            if (checkMark != null)
            {
                // 檢查標籤是否在 currentNoteData 的 tags 列表中
                if (currentNoteData != null && currentNoteData.tags.Contains(tag))
                {
                    checkMark.SetActive(true);  // 顯示勾選符號
                }
                else
                {
                    checkMark.SetActive(false);  // 隱藏勾選符號
                }
            }

            Button tagButton = newTag.GetComponentInChildren<Button>();  // 假設每個標籤上有一個按鈕
            if (tagButton != null)
            {
                string tagText = tag;
                tagButton.onClick.AddListener(() => OnTagClicked(checkMark, tagText));  // 綁定點擊事件
            }
         
        }
    }

    // public void ShowBool()
    // {
    //     Debug.Log(isBindingTagToNote);
    // }

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
 
  

    public void openAddNotePanelForBindingTagToNote()
    {
        // Debug.Log("Open Panel");
        addTagBindingToNotePanel.SetActive(true);
        DisplayTags();
        
    }


    // 设置当前编辑的笔记数据
    public void SetCurrentNoteData(NoteData noteData)
    {
        currentNoteData = noteData;
    }

    public void OnNoteSelected(NoteData selectedNote)
    {
        SetCurrentNoteData(selectedNote);
        // DisplayTags();
    }
}
