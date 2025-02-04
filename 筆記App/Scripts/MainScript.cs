using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class MainScript : MonoBehaviour
{
    public Image userAvatar;
    [SerializeField] public Avatar[] avatars;
    private const string SelectedAvatarKey  = "SelectedAvatarIndex";
    private const string UsernameKey = "Username";
    private const string EmailKey = "Email";
    public TMP_InputField usernameInputField;
    public TMP_InputField emailInputField;
    public GameObject userInfoPanel;
    private string _savedUsername;
    private string _savedEmail;


    async void Start()
    {
        await LoadUsernameAndEmailFromCloud();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            userInfoPanel.SetActive(false);
        }
    }

    async void OnEnable()
    {
        await LoadAvatarFromCloud();
    }


    //cloud save
    public async Task LoadAvatarFromCloud()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { SelectedAvatarKey });

            if (savedData.TryGetValue(SelectedAvatarKey, out string savedIndex))
            {
                int index = Convert.ToInt32(savedIndex);
                if (index >= 0 && index < avatars.Length)
                {
                    userAvatar.sprite = avatars[index].sprite;
                    Debug.Log("Avatar loaded from cloud.");
                }
            }
            else
            {
                Debug.LogWarning("No avatar found in cloud save.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load avatar: {e.Message}");
        }
    }

    public void CloseUserInfoPanel()
    {
        usernameInputField.text = _savedUsername;
        emailInputField.text = _savedEmail;
        userInfoPanel.SetActive(false);
    }

    public async void SaveUsernameAndEmail()
    {
        await SaveUsernameAndEmailToCloud();
    }

    // 保存Username和Email到云端
    public async Task SaveUsernameAndEmailToCloud()
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { UsernameKey, usernameInputField.text },
                { EmailKey, emailInputField.text }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Username and Email saved to cloud successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save Username and Email: {e.Message}");
        }
    }

    // 从云端加载Username和Email
    public async Task LoadUsernameAndEmailFromCloud()
    {
        try
        {
            var keysToLoad = new HashSet<string> { UsernameKey, EmailKey };
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(keysToLoad);

            if (savedData.TryGetValue(UsernameKey, out string savedUsername))
            {
                usernameInputField.text = savedUsername;
                _savedUsername = savedUsername;
            }

            if (savedData.TryGetValue(EmailKey, out string savedEmail))
            {
                emailInputField.text = savedEmail;
                _savedEmail = savedEmail;
            }

            Debug.Log("Username and Email loaded from cloud.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load Username and Email: {e.Message}");
        }
    }
 
}
