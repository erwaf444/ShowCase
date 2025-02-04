using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;


public class AddNote : MonoBehaviour
{
    public TMP_InputField titleInput;
    public TMP_InputField contentInput;
    public TextMeshProUGUI dateText;
    public Image selectedBoxSprite;
    public List<Image> boxSprites;
    public List<Image> littleArrow;
    public GameObject notePanelPrefab;
    public Transform noteContainer;
    public Button blueButton; // 藍色按鈕
    public Button greenButton; // 綠色按鈕
    public Button orangeButton; // 橙色按鈕
    public Button pinkButton; // 粉紅色按鈕
    public GameObject noticePanelWithTitle;
    public GameObject noticePanelWithBoxColor;
    public GameObject addCurrencyNoticePanel;
    public NotesManager notesManager;
    public GameObject addNotePanel;
    public TagScript2 tagScript;
    public TagScriptWtihBindNote tagScriptWtihBindNote;
    public PlayerCurrency.PlayerData playerData;
    public PetScript petScript;
    // public NotesManager notesManagerScript;


    async void Start()
    {
        await UnityServices.InitializeAsync();
        for (int i = 0; i < littleArrow.Count; i++)
        {
            littleArrow[i].gameObject.SetActive(false);
        }
        noticePanelWithTitle.SetActive(false);
        // addNotePanelObj.SetActive(false);
        pinkButton.onClick.AddListener(() => SelectBoxColor(0));
        orangeButton.onClick.AddListener(() => SelectBoxColor(1));
        greenButton.onClick.AddListener(() => SelectBoxColor(2));
        blueButton.onClick.AddListener(() => SelectBoxColor(3));
    }

    public async void SaveNote()
    {
        string title = titleInput.text;
        string content = contentInput.text;

        // if (NotesManager.Instance.currentEditingNote != null)
        // {   
        //     // 如果有正在编辑的笔记，调用更新方法
        //     Debug.Log("Updating note");
        //     await NotesManager.Instance.UpdateNote(title, content);
        // }
        // else
        // {
            //創建新的筆記邏輯
            if (string.IsNullOrEmpty(titleInput.text) || string.IsNullOrEmpty(contentInput.text))
            {
                StartCoroutine(NoticePanelWithTitleRoutine());
                return;
            }            
            

            // 判断是否有正在编辑的笔记
            if (notesManager.currentEditingNote != null)
            {
                Debug.Log($"Current Editing Note: {notesManager.currentEditingNote.title }");
                Debug.Log($"Current Editing Note: {notesManager.currentEditingNote.content }");
                // 如果有正在编辑的笔记，调用更新方法
                Debug.Log("Updating existing note");
                await notesManager.UpdateNote(
                    title, 
                    content,
                    tagScriptWtihBindNote.currentNoteData.tags
                );
                addNotePanel.SetActive(false);
            }
            else
            {
                if (notesManager.noteBoxImage == null)
                {
                    StartCoroutine(NoticePanelWithBoxColorRoutine());
                    return;
                }
                // 创建新的笔记
                Debug.Log("Creating new note");
              

                await notesManager.AddNote(
                    title, 
                    content, 
                    notesManager.noteBoxImage, 
                    tagScript.currentNoteData.tags
                );

                await AddVirtualCurrency(1);
                StartCoroutine(AddCurrencyNotice());
            }

            // await NotesManager.Instance.AddNote(titleInput.text, contentInput.text, selectedBoxSprite);
            RefreshNotesDisplay();
            ClearInputs();

        // }
    }

    IEnumerator AddCurrencyNotice()
    {
        addCurrencyNoticePanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        addCurrencyNoticePanel.SetActive(false);
        addNotePanel.SetActive(false);
    }

    IEnumerator NoticePanelWithTitleRoutine()
    {
        noticePanelWithTitle.SetActive(true);
        yield return new WaitForSeconds(2f);
        noticePanelWithTitle.SetActive(false);
    }

    
    IEnumerator NoticePanelWithBoxColorRoutine()
    {
        noticePanelWithBoxColor.SetActive(true);
        yield return new WaitForSeconds(2f);
        noticePanelWithBoxColor.SetActive(false);
    }

    private void OnEnable()
    {
        if (notesManager != null)
        {
            notesManager.OnNotesUpdated += RefreshNotesDisplay;
            RefreshNotesDisplay();
        }
        else
        {
            Debug.LogError("notesManager is null.");
        }
    }

    private void OnDisable()
    {
        if (notesManager != null)
        {
            notesManager.OnNotesUpdated -= RefreshNotesDisplay;
        }
    }

    private async void RefreshNotesDisplay()
    {
        // 清除現有的筆記面板
        foreach (Transform child in noteContainer)
        {
            Destroy(child.gameObject);
        }

        // 从云端加载最新的笔记
        await notesManager.LoadNotesFromCloud(); 

        // 重新創建所有筆記面板
        // var notes = NotesManager.Instance.GetAllNotes();
        // foreach (var note in notes)
        // {
        //     GameObject newNotePanel = Instantiate(notePanelPrefab, noteContainer);
        //     NotePanel notePanel = newNotePanel.GetComponent<NotePanel>();
        //     if (notePanel != null)
        //     {
        //         notePanel.SetNoteData(note);
        //     }
        // }
    }

    private void ClearInputs()
    {
        titleInput.text = "";
        contentInput.text = "";
    }

    public void SelectBoxColor(int index)
    {
        if (index >= 0 && index < boxSprites.Count)
        {
            selectedBoxSprite = boxSprites[index];
            for (int i = 0; i < littleArrow.Count; i++)
            {
                littleArrow[i].gameObject.SetActive(i == index); // 只让当前选中的arrow活动
            }
            Debug.Log("Selected box color: " + selectedBoxSprite.name);
            notesManager.noteBoxImage = selectedBoxSprite;
        }
        else
        {
            Debug.LogWarning("顏色索引超出範圍！");
        }
    }

    public void CloseAddNotePanel()
    {
        for (int i = 0; i < littleArrow.Count; i++)
        {
            littleArrow[i].gameObject.SetActive(false);
        }
        addNotePanel.SetActive(false);
        noticePanelWithTitle.SetActive(false);
        noticePanelWithBoxColor.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            addNotePanel.SetActive(false);
        }
    }
   
    #region UnityCloud方法
    // 增加虛擬貨幣
    public async Task AddVirtualCurrency(int amount)
    {
        try 
        {
            // 從 Unity Cloud 獲取現有的虛擬貨幣數量
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync
            (
                new HashSet<string> { "virtualCurrency" }
            );

            int currentCurrency = data.ContainsKey("virtualCurrency") 
            ? Convert.ToInt32(data["virtualCurrency"]) 
            : 0;

            Debug.Log(currentCurrency);
            // 計算新的虛擬貨幣數量
            int newCurrency = currentCurrency + amount;
            
            Debug.Log(amount);
            Debug.Log(newCurrency);

            // 保存到 Unity Cloud Save
            var saveData = new Dictionary<string, object>
            {
                { "virtualCurrency", newCurrency }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);

            Debug.Log($"虛擬貨幣已更新：{currentCurrency} -> {newCurrency}");
            petScript.virtualCurrencyText.text = "x" + newCurrency.ToString();
            petScript.virtualCurrency = newCurrency;
        }
        catch (CloudSaveException e)
        {
            // 特定的Cloud Save异常处理
            Debug.LogError($"Cloud Save异常：{e.Message}");
            Debug.LogError($"错误码：{e.ErrorCode}");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存虛擬貨幣失敗：{e.Message}");
        }
    }

    public async Task<int> GetVirtualCurrency()
    {
        try 
        {
            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(
                new HashSet<string> { "virtualCurrency" }
            );

            if (data.ContainsKey("virtualCurrency"))
            {
                return Convert.ToInt32(data["virtualCurrency"]);
            }

            return 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"獲取虛擬貨幣失敗：{e.Message}");
            return 0;
        }
    }

    public async Task UpdatePlayerVirtualCurrency()
    {
        try
        {
            // 獲取虛擬貨幣
            int currency = await GetVirtualCurrency();
            
            // 賦值到 playerData
            playerData.virtualCurrency = currency;

            Debug.Log($"玩家虛擬貨幣已更新：{playerData.virtualCurrency}");
        }
        catch (Exception e)
        {
            Debug.LogError($"更新玩家虛擬貨幣失敗：{e.Message}");
        }
    }


    #endregion
}