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

    // bool IsSignedIn = false;

    public bool SignInEnded = false;
    public UILogin uiLogin;
    // public NotesManager noteManager;
    // public LoadingManager loadingManager;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignedIn;
        await CheckSignInStatus();
    }

    private async Task CheckSignInStatus()
    {
        Debug.Log($"Session Token Exists: {AuthenticationService.Instance.SessionTokenExists}");

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
            SceneManager.LoadScene("MainMenu");

            // loadingManager.LoadToMainMenu(1);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Sign-in with session token failed: {ex.Message}");
        }
    }
    
    private void Start()
    {
        // if (SceneManager.GetActiveScene().name == "LoginPage")
        // {
        //     if (PlayerAccountService.Instance.IsSignedIn)
        //     {
        //         Debug.Log("Player is already signed in.");
        //         // Load the MainMenu scene
        //         SceneManager.LoadScene("MainMenu");
        //     }
        //     else
        //     {
        //         Debug.Log("Player is not signed in.");
        //     }
        // }

        // if (SceneManager.GetActiveScene().name == "LoginPage")
        // {
        //     if (uiLogin.IsLoggedIn())
        //     {
        //         if (PlayerAccountService.Instance.IsSignedIn)
        //         {
        //             Debug.Log("Player is already signed in.");
        //             SceneManager.LoadScene("MainMenu");
        //         }
              
        //     }
        // }

        // if (SceneManager.GetActiveScene().name == "MainMenu")
        // {
        //     if (!PlayerAccountService.Instance.IsSignedIn)
        //     {
        //         Debug.Log("Player is not signed in.");
        //         // Load the LoginPage scene
        //         SceneManager.LoadScene("LoginPage");
        //     }
           
        // }        
       
    }


    private async void SignedIn()
    {
        // if (IsSignedIn == true)
        // {
        //     SceneManager.LoadScene("MainMenu");
        //     return;
        // }
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);
            
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void SignOut()
    {
        PlayerAccountService.Instance.SignOut();
        Debug.Log("Player signed out.");

    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
        
    }



    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {   
            // if (PlayerAccountService.Instance.IsSignedIn)
            // {
            //     Debug.Log("Player is already signing in.");
            //     // IsSignedIn = true;
            //     SceneManager.LoadScene("MainMenu");
            //     return;
            // }
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            Debug.Log($"PlayerInfo ID: {playerInfo.Id}");

            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            playerProfile.playerInfo = playerInfo;
            playerProfile.Name = name;
            // if (noteManager != null)
            // {
            //     await noteManager.LoadNotesFromCloud();
            // }
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
}


[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}