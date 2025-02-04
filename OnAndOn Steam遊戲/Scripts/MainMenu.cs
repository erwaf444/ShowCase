using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;
    public TextMeshProUGUI languageText;
    public LocaleSelector localeSelector;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public TextMeshProUGUI engText;
    public TextMeshProUGUI tchiText;
    public TextMeshProUGUI schiText;
    public TextMeshProUGUI languageTitleText;
    public TextMeshProUGUI settingTitleText;
    public TextMeshProUGUI pressArrowText;

    void Start()
    {
        // audioManager.PlayMusic("BGM1");
    }

    void Update()
    {
        if(localeSelector.localID == 0)
        {
            languageText.font = TraditionalFontAsset;
            languageText.text = "Language";
            engText.font = TraditionalFontAsset;
            engText.text = "English";
            tchiText.font = TraditionalFontAsset;
            tchiText.text = "T Chinese";
            schiText.font = TraditionalFontAsset;
            schiText.text = "S Chinese";
            languageTitleText.font = TraditionalFontAsset;
            languageTitleText.text = "language";
            settingTitleText.font = TraditionalFontAsset;
            settingTitleText.text = "setting";
            pressArrowText.font = TraditionalFontAsset;
            pressArrowText.text = "Press Arrow";
        } else if (localeSelector.localID == 1)
        {
            languageText.font = TraditionalFontAsset;
            languageText.text = "語言";
            engText.font = TraditionalFontAsset;
            engText.text = "英文";
            tchiText.font = TraditionalFontAsset;
            tchiText.text = "繁體中文";
            schiText.font = TraditionalFontAsset;
            schiText.text = "簡體中文";
            languageTitleText.font = TraditionalFontAsset;
            languageTitleText.text = "語言";
            settingTitleText.font = TraditionalFontAsset;
            settingTitleText.text = "設置";
            pressArrowText.font = TraditionalFontAsset;
            pressArrowText.text = "點擊箭頭";

        } else if (localeSelector.localID == 2)
        {
            languageText.font = SimplifiedFontAsset;
            languageText.text = "语言";
            engText.font = SimplifiedFontAsset;
            engText.text = "英文";
            tchiText.font = SimplifiedFontAsset;
            tchiText.text = "繁体中文";
            schiText.font = SimplifiedFontAsset;
            schiText.text = "简体中文";
            languageTitleText.font = SimplifiedFontAsset;
            languageTitleText.text = "语言";
            settingTitleText.font = SimplifiedFontAsset;
            settingTitleText.text = "设置";
            pressArrowText.font = SimplifiedFontAsset;
            pressArrowText.text = "点击箭头";
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
