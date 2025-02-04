using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;
    public event Action<PlayerProfile> OnAvatarUpdate;

    private PlayerInfo playerInfo;

    private PlayerProfile playerProfile;
    public PlayerProfile PlayerProfile => playerProfile;


    public UILogin uiLogin;
    public LoadingManager loadingManager;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignedIn;
        await CheckSignInStatus();
    }

    private async Task CheckSignInStatus()
    {
        if (AuthenticationService.Instance.SessionTokenExists && SceneManager.GetActiveScene().name == "LoginPage")
        {
            await SignInWithSessionToken();
        }
    }

 
    private async Task SignInWithSessionToken()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign-in with session token is successful.");
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            Debug.Log($"PlayerInfo ID: {playerInfo.Id}");
            // SceneManager.LoadScene("MainMenu");

            loadingManager.LoadToMainMenu(1);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sign-in with session token failed: {ex.Message}");
        }
    }
    
    private void Start()
    {
        SessionClean();
    }


    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
            playerInfo = AuthenticationService.Instance.PlayerInfo;
            Debug.Log($"PlayerInfo ID: {playerInfo.Id}");
            
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async Task SignOut()
    {
        PlayerAccountService.Instance.SignOut();
        AuthenticationService.Instance.SignOut(true);

        Debug.Log("Player signed out.");
        await Task.Delay(2000); 
    }

    public async Task InitSignIn()
    {
        await SignOut(); // 清理旧的登录状态
        await PlayerAccountService.Instance.StartSignInAsync();
    }



    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {   
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            Debug.Log($"PlayerInfo ID: {playerInfo.Id}");

            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            playerProfile.playerInfo = playerInfo;
            playerProfile.Name = name;

            OnSignedIn?.Invoke(playerProfile);
            
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }

    public void SessionClean()
    {
        if (SceneManager.GetActiveScene().name == "LoginPage")
        {
            AuthenticationService.Instance.ClearSessionToken(); // 清理会话令牌

        }
    } 
}


[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}