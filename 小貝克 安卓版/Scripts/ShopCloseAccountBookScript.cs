using UnityEngine;
using TMPro;

public class ShopCloseAccountBookScript : MonoBehaviour
{
    public TextMeshProUGUI[] TitleTexts;

    public TextMeshProUGUI breadIncomeText;
    public TextMeshProUGUI otherIncomeText;
    public TextMeshProUGUI totalIncomeText;

    public TextMeshProUGUI rentOutlayText;
    public TextMeshProUGUI stuffOutlayText;
    public TextMeshProUGUI equipmentOutlayText;
    public TextMeshProUGUI otherOutlayText;
    public TextMeshProUGUI totalOutlayText;

    private GameManager gameManagerScript;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset TraditionalFontAsset;
    public LocaleSelector localeSelector;



    void Start()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            gameManagerScript = gameManagerObject.GetComponent<GameManager>();
        }
 
        breadIncomeText.text = "0";
        otherIncomeText.text = "0";
        totalIncomeText.text = "0";
        rentOutlayText.text = "";
        stuffOutlayText.text = "";
        equipmentOutlayText.text = "";
        otherOutlayText.text = "";
        totalOutlayText.text = "";
    }

    void Update()
    {
        if (localeSelector.localID == 0)
        {
            foreach (TextMeshProUGUI titleText in TitleTexts)
            {
                titleText.font = TraditionalFontAsset;
            }
        } else if (localeSelector.localID == 1)
        {
            foreach (TextMeshProUGUI titleText in TitleTexts)
            {
                titleText.font = TraditionalFontAsset;
            }
        } else if (localeSelector.localID == 2)
        {
            foreach (TextMeshProUGUI titleText in TitleTexts)
            {
                titleText.font = SimplifiedFontAsset;
            }
        }
    }
}
