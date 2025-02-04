using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public LocaleSelector localeSelector;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    [SerializeField] TextMeshProUGUI tooltipText;
    [SerializeField] string message;
    [SerializeField] string englishMessage; 
    [SerializeField] string chineseSMessage; 
    [SerializeField] GameObject messageBG;
    public bool isToolTipEnabled = true;


    void Start()
    {
        messageBG.SetActive(false);
        tooltipText.text = "";
        
    }

    void Update()
    {
        if (localeSelector.localID == 0)
        {
            tooltipText.font = TraditionalFontAsset;
        } else if (localeSelector.localID == 1)
        {
            tooltipText.font = TraditionalFontAsset;
        } else if (localeSelector.localID == 2)
        {
            tooltipText.font = SimplifiedFontAsset;
        }
    }

    void OnMouseEnter() {
        if (!isToolTipEnabled)
            return;
        if (localeSelector.localID == 0)
        {
            tooltipText.text = englishMessage;
        }
        else if (localeSelector.localID == 1)
        {
            tooltipText.text = message;
        } 
        else if (localeSelector.localID == 2)
        {
            tooltipText.text = chineseSMessage;
        }
        messageBG.SetActive(true);
    }

    void OnMouseExit() {
        if (!isToolTipEnabled)
            return;
        tooltipText.text = "";
        messageBG.SetActive(false);
    }
}
