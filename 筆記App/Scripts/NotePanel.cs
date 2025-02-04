using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NotePanel : MonoBehaviour
{ 
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI dateText;
    private string noteId;
    public Image noteBoxImage; // 繪製顏色框的SpriteRenderer
    // public GameObject notePanel;

     
    void Start()
    {
        // notePanel.SetActive(false);
    }
    
    public void SetNoteBoxImage(Sprite image)
    {
        noteBoxImage.sprite = image; // 設置顏色框的Sprite
    }

    public void SetNoteData(NoteData data)
    {
        noteId = data.id;
        titleText.text = data.title;
        contentText.text = data.content;
        dateText.text = data.createdTime;
    }


    public async void DeleteNote()
    {
        await NotesManager.Instance.DeleteNote(noteId);
    }

    public void OpenNote()
    {
        // notePanel.SetActive(true);
    }
}