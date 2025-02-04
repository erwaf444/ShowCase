using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public LoadingManager loadingManager;
    public Animator mainMenuSettingPanelAnimator;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset SimplifiedFontAsset2; //這個asset是在第一個不能使用的時候 
    public TMP_FontAsset TraditionalFontAsset;
    public LocaleSelector localeSelector;
    public TextMeshProUGUI[] texts;
    public TextMeshProUGUI[] texts2; //這個Text是在第一個不能使用的時候 
    public TextMeshProUGUI selectEngButtonText;
    public TextMeshProUGUI selectChiTButtonText;
    public TextMeshProUGUI selectChiSButtonText;
    public TextMeshProUGUI ResolutionText;

    public TextMeshProUGUI shopNameText;

    void Start()
    {
        GameObject loadingManagerObject = GameObject.Find("LoadingManager");
        if (loadingManagerObject != null)
        {
            loadingManager = loadingManagerObject.GetComponent<LoadingManager>();
        }
        if (SteamManager.Initialized)
        {
            // ResetDayStat("Days");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (localeSelector.localID == 0)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = TraditionalFontAsset;
                text.SetAllDirty();
            }
            selectEngButtonText.text = "English";
            selectEngButtonText.font = TraditionalFontAsset;
            selectChiTButtonText.text = "Chinese (Traditional)";
            selectChiTButtonText.font = TraditionalFontAsset;
            selectChiSButtonText.text = "Chinese (Simplified)";
            selectChiSButtonText.font = TraditionalFontAsset;
            ResolutionText.font = TraditionalFontAsset;
            ResolutionText.text = "Resolution";
            shopNameText.text = "Little Baker";
            shopNameText.font = TraditionalFontAsset;
            
        } else if (localeSelector.localID == 1)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = TraditionalFontAsset;
                text.SetAllDirty();
            }
            selectEngButtonText.text = "英文";
            selectEngButtonText.font = TraditionalFontAsset;
            selectChiTButtonText.text = "中文（繁體）";
            selectChiTButtonText.font = TraditionalFontAsset;
            selectChiSButtonText.text = "中文（簡體）";
            selectChiSButtonText.font = TraditionalFontAsset;
            ResolutionText.font = TraditionalFontAsset;
            ResolutionText.text = "分辨率";
            shopNameText.text = "小貝克";
            shopNameText.font = TraditionalFontAsset;
        } else if (localeSelector.localID == 2)
        {
            foreach (TextMeshProUGUI text in texts)
            {
                text.font = SimplifiedFontAsset;
                text.SetAllDirty();
            }
            selectEngButtonText.text = "英文";
            selectEngButtonText.font = SimplifiedFontAsset2;
            selectChiTButtonText.text = "中文（繁体）";
            selectChiTButtonText.font = SimplifiedFontAsset2;
            selectChiSButtonText.text = "中文（简体）";
            selectChiSButtonText.font = SimplifiedFontAsset2;
            ResolutionText.font = SimplifiedFontAsset2;
            ResolutionText.text = "分辨率";
            shopNameText.text = "小贝克";
            shopNameText.font = SimplifiedFontAsset2;
        }
    }

    public void StartGame()
    {
        loadingManager.LoadToGame(2);
    }

    // public void ExitGame()
    // {
    //     Application.Quit();
    // }

    public void SettingButton()
    {
        mainMenuSettingPanelAnimator.SetTrigger("MainMenuSettingPanelDown");
        AudioManager.instance.PlaySFX("TurnPage");

    }

    public void SettingPanelExit()
    {
        mainMenuSettingPanelAnimator.SetTrigger("MainMenuSettingPanelUp");
        AudioManager.instance.PlaySFX("TurnPage");
    }

    public void ResetDayStat(string statKey)
    {
      
    }

}

