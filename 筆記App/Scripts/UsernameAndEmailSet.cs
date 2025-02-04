using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
public class UsernameAndEmailSet : MonoBehaviour
{
    //Username和Email屬性
    public TMP_InputField usernameInputField;
    public TMP_InputField emailInputField;
    public Image selectedAvatar;
    [SerializeField] public Avatar[] avatars;
    private const string SelectedAvatarKey  = "SelectedAvatarIndex";
    private const string UsernameKey = "Username";
    private const string EmailKey = "Email";
    private const string RegisteredKey = "IsRegistered";
    public GameObject usernameAndEmailObj;
    public NewBieSet newBieSet;
    public GameObject mainUI;
    void Start()
    {
        mainUI.SetActive(false);
    }

    void Update()
    {
        
    }

    async void OnEnable()
    {
        await newBieSet.LoadAvatarFromCloud();
    }

    public async void SaveUsernameAndEmail()
    {
        await SaveUsernameAndEmailToCloud();
        usernameAndEmailObj.SetActive(false);
        await SaveRegister(true);
        mainUI.SetActive(true);
    }

    //Username和email方法
    //保存已註冊狀態到云端
    public async Task SaveRegister(bool isRegistered)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { RegisteredKey, isRegistered.ToString() }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Registration status saved to cloud successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save registration status: {e.Message}");
        }
    }

    public async Task<bool> LoadRegisterStatus()
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { RegisteredKey });

            if (savedData.TryGetValue(RegisteredKey, out string isRegisteredString))
            {
                // 将字符串转换为布尔值
                if (bool.TryParse(isRegisteredString, out bool isRegistered))
                {
                    Debug.Log("Registration status loaded from cloud.");
                    return isRegistered; // 返回用户是否已注册的状态
                }
                else
                {
                    Debug.LogWarning($"Could not parse registration status: {isRegisteredString}. Defaulting to false.");
                    return false; // 默认返回未注册状态
                }
            }
            else
            {
                Debug.LogWarning("No registration status found in cloud save.");
                return false; // 默认返回未注册状态
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load registration status: {e.Message}");
            return false; // 出现异常时返回未注册状态
        }
    }

    // 从云端加载头像索引并显示对应的头像
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
                    selectedAvatar.sprite = avatars[index].sprite;
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
            }

            if (savedData.TryGetValue(EmailKey, out string savedEmail))
            {
                emailInputField.text = savedEmail;
            }

            Debug.Log("Username and Email loaded from cloud.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load Username and Email: {e.Message}");
        }
    }
}
