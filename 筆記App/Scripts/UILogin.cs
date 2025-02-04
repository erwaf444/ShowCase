using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class UILogin : MonoBehaviour
{
    [SerializeField] private Button loginButton;
    [SerializeField] private Button logoutButton;

    // [SerializeField] private TMP_Text userIdText;
    // [SerializeField] private TMP_Text userNameText;

    // [SerializeField] private Transform loginPanel, userPanel;

    [SerializeField] private LoginController loginController;

    private PlayerProfile playerProfile;

    private void OnEnable()
    {
        if (loginController != null)
        {
            loginButton.onClick.AddListener(LoginButtonPressed);
            logoutButton.onClick.AddListener(LogOutButtonPressed); 
            loginController.OnSignedIn += LoginController_OnSignedIn;
            loginController.OnAvatarUpdate += LoginController_OnAvatarUpdate;
        }
        else
        {
            Debug.LogWarning("loginController is not assigned!");
        }
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(LoginButtonPressed);
        logoutButton.onClick.RemoveListener(LoginButtonPressed);
        loginController.OnSignedIn -= LoginController_OnSignedIn;
        loginController.OnAvatarUpdate -= LoginController_OnAvatarUpdate;
    }

    public async void LoginButtonPressed()
    {
        await loginController.InitSignIn();
        SceneManager.LoadScene("MainScene");

    }

    public void LogOutButtonPressed()
    {
        loginController.SignOut();
        SceneManager.LoadScene("LoginScene");

    }

    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        Debug.Log("LoginController_OnSignedIn called");
        playerProfile = profile;
        // loginPanel.gameObject.SetActive(false);
        // userPanel.gameObject.SetActive(true);
        // SceneManager.LoadScene("MainMenu");
        Debug.Log($"Player ID: {playerProfile.playerInfo.Id}");
        Debug.Log($"Player Name: {profile.Name}");


        // userIdText.text = $"id_{playerProfile.playerInfo.Id}";
        // userNameText.text = profile.Name;
        Debug.Log("End");

    }
    private void LoginController_OnAvatarUpdate(PlayerProfile profile)
    {
        playerProfile = profile;
    }




    // Session Part Code
    // public void SetSession()
    // {
    //     // PlayerPrefs.SetString("UserID", userId);
    //     PlayerPrefs.SetInt("LoggedIn", 1);
    //     PlayerPrefs.Save();
    // }

    // public void ClearSession()
    // {
    //     // PlayerPrefs.DeleteKey("UserID");
    //     PlayerPrefs.DeleteKey("LoggedIn");
    //     PlayerPrefs.Save();
    // }

    // public bool IsLoggedIn()
    // {
    //     return PlayerPrefs.GetInt("LoggedIn", 0) == 1;
    // }

    // public string GetUserId()
    // {
    //     return PlayerPrefs.GetString("UserID", string.Empty);
    // }
   
}