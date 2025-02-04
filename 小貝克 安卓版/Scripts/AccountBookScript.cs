using TMPro;
using UnityEngine;

public class AccountBookScript : MonoBehaviour
{
    //標題
    public TextMeshProUGUI[] TitleTexts;

    //金額數字
    public TextMeshProUGUI breadIncomeText;
    public TextMeshProUGUI otherIncomeText;
    public TextMeshProUGUI totalIncomeText;

    public TextMeshProUGUI rentOutlayText;
    public TextMeshProUGUI rentOutlayTitleText;
    public TextMeshProUGUI stuffOutlayText;
    public TextMeshProUGUI stuffOutlayTitleText;
    public TextMeshProUGUI equipmentOutlayText;
    public TextMeshProUGUI equipmentOutlayTitleText;
    public TextMeshProUGUI otherOutlayText;
    public TextMeshProUGUI otherOutlayTitleText;
    public TextMeshProUGUI totalOutlayText;

    private GameManager gameManagerScript;
    public TMP_FontAsset SimplifiedFontAsset; 
    public TMP_FontAsset SimplifiedFontAsset2;//這是在第一個asset不能使用的時候 這是簡體asset 
    public TMP_FontAsset TraditionalFontAsset;
    public TMP_FontAsset TraditionalFontAsset2;//這是在第一個asset不能使用的時候 這是繁體asset 
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
            rentOutlayTitleText.text = "Shop rent";
            rentOutlayTitleText.font = TraditionalFontAsset2;
            stuffOutlayTitleText.text = "Ingredients cost ";
            stuffOutlayTitleText.font = TraditionalFontAsset2;
            equipmentOutlayTitleText.text = "Equipment cost";
            equipmentOutlayTitleText.font = TraditionalFontAsset2;
            otherOutlayTitleText.text = "Other cost";
            otherOutlayTitleText.font = TraditionalFontAsset2;
        } else if (localeSelector.localID == 1)
        {
            foreach (TextMeshProUGUI titleText in TitleTexts)
            {
                titleText.font = TraditionalFontAsset;
            }
            rentOutlayTitleText.text = "店鋪租金";
            rentOutlayTitleText.font = TraditionalFontAsset2;
            stuffOutlayTitleText.text = "材料費用";
            stuffOutlayTitleText.font = TraditionalFontAsset2;
            equipmentOutlayTitleText.text = "設備維修和升級";
            equipmentOutlayTitleText.font = TraditionalFontAsset2;
            otherOutlayTitleText.text = "其他花費";
            otherOutlayTitleText.font = TraditionalFontAsset2;
        } else if (localeSelector.localID == 2)
        {
            foreach (TextMeshProUGUI titleText in TitleTexts)
            {
                titleText.font = SimplifiedFontAsset;
            }
            rentOutlayTitleText.text = "店铺租金";
            rentOutlayTitleText.font = SimplifiedFontAsset2;
            stuffOutlayTitleText.text = "材料费用";
            stuffOutlayTitleText.font = SimplifiedFontAsset2;
            equipmentOutlayTitleText.text = "设备维修和升级";
            equipmentOutlayTitleText.font = SimplifiedFontAsset2;
            otherOutlayTitleText.text = "其他花费";
            otherOutlayTitleText.font = SimplifiedFontAsset2;
        }
        breadIncomeText.text = gameManagerScript.money.ToString();
    }
}
