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

    [SerializeField] private TMP_Text userIdText;
    [SerializeField] private TMP_Text userNameText;

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
        SceneManager.LoadScene("MainMenu");

    }
 
    public async void LogOutButtonPressed()
    {
        await loginController.SignOut();

        SceneManager.LoadScene("LoginPage");
        AuthenticationService.Instance.ClearSessionToken();    
        Debug.Log("Session cleaned");

    }

    private void LoginController_OnSignedIn(PlayerProfile profile)
    {
        Debug.Log("LoginController_OnSignedIn called");
        playerProfile = profile;
     
        Debug.Log($"Player ID: {playerProfile.playerInfo.Id}");
        Debug.Log($"Player Name: {profile.Name}");


        userIdText.text = $"id_{playerProfile.playerInfo.Id}";
        userNameText.text = profile.Name;
        Debug.Log("End");

    }
    private void LoginController_OnAvatarUpdate(PlayerProfile profile)
    {
        playerProfile = profile;
    }




  
   
}