using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

[System.Serializable]
public class Avatar
{
    public Sprite sprite;
}

public class NewBieSet : MonoBehaviour
{
    //選擇頭像屬性
    public Image selectedAvatar;
    [SerializeField] public Avatar[] avatars;
    private const string SelectedAvatarKey  = "SelectedAvatarIndex";
    public int avatarIndex;
    public GameObject avatarPanel;
    public GameObject[] avatarPanelUIs;
    public GameObject usernameAndEmailObj;
    public GameObject mainUI;
    public UsernameAndEmailSet usernameAndEmailSet;
    public bool isRegister = false;




    private async void Start()
    {
        isRegister = await usernameAndEmailSet.LoadRegisterStatus();
        if (isRegister)
        {
            avatarPanel.SetActive(false);
            usernameAndEmailObj.SetActive(false);
            mainUI.SetActive(true);
        }
        usernameAndEmailObj.SetActive(false);
        await UnityServices.InitializeAsync();
        // await LoadAvatarFromCloud();
    }


    //Avatar方法
    public async void SaveAvatar(int avatarIndex)
    {
        foreach (GameObject ui in avatarPanelUIs)
        {
            ui.SetActive(false);
        }
        await SaveAvatarToCloud(avatarIndex);
        avatarPanel.SetActive(false);
        usernameAndEmailObj.SetActive(true);
    }   

    public void SelectAvatar(int index)
    {
        if (index >= 0 && index < avatars.Length)
        {
            selectedAvatar.sprite = avatars[index].sprite;
            avatarIndex = index;

            // 自动调整选中的头像比例
            RectTransform rt = selectedAvatar.GetComponent<RectTransform>();
            // 获取 Sprite 的原始尺寸
            float spriteWidth = avatars[index].sprite.rect.width;
            float spriteHeight = avatars[index].sprite.rect.height;

            // 计算比例并调整 RectTransform 的大小
            float aspectRatio = spriteWidth / spriteHeight;
            rt.sizeDelta = new Vector2(rt.sizeDelta.y * aspectRatio, rt.sizeDelta.y); // 以高度为基准
        }
    }


    public async Task SaveAvatarToCloud(int avatarIndex)
    {
        try
        {
            var data = new Dictionary<string, object> { { SelectedAvatarKey, avatarIndex } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Avatar saved to cloud successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save avatar: {e.Message}");
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



  
}
