using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BookFIirstPageScript : MonoBehaviour
{
    public TextMeshProUGUI subTitleText;
    public TextMeshProUGUI valueText;
    public Button nextButton;
    public Button prevButton;
    private int currentIndex = 0;

    public int cleanValue;
    public int foodFreshValue;
    public int customerHappyValue;
    public LocaleSelector localeSelector;


    private string[] titlesEnglish = { "Clean", "Food", "Customer" };
    private string[] titlesTraditional = { "清潔度", "食材新鮮度", "顧客滿意度"}; 
    private string[] titlesSimplified = { "清洁度", "食材新鲜度", "顾客满意度" };
    
    void Start()
    {
        // 监听右箭头按钮的点击事件
        nextButton.onClick.AddListener(OnRightArrowClicked);
        prevButton.onClick.AddListener(OnLeftArrowClicked);
        // 初始化显示
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }



    void OnRightArrowClicked()
    {
        string[] titles = titlesEnglish;
        if (localeSelector.localID == 0)
        {
            titles = titlesEnglish;
        }
        else if (localeSelector.localID == 1)
        {
            titles = titlesTraditional;
        }
        else if (localeSelector.localID == 2)
        {
            titles = titlesSimplified;
        }
        // 当右箭头被点击时，改变索引，循环遍历 title 和 value
        currentIndex = (currentIndex + 1) % titles.Length;
        UpdateUI();
        Debug.Log("右箭头被点击");
    }

    void OnLeftArrowClicked()
    {
        string[] titles = titlesEnglish;
        if (localeSelector.localID == 0)
        {
            titles = titlesEnglish;
        }
        else if (localeSelector.localID == 1)
        {
            titles = titlesTraditional;
        }
        else if (localeSelector.localID == 2)
        {
            titles = titlesSimplified;
        }
        currentIndex = (currentIndex - 1 + titles.Length) % titles.Length;
        UpdateUI();
    }

    void UpdateUI()
    {
        string[] titles = titlesEnglish;
        if (localeSelector.localID == 0)
        {
            titles = titlesEnglish;
        }
        else if (localeSelector.localID == 1)
        {
            titles = titlesTraditional;
        }
        else if (localeSelector.localID == 2)
        {
            titles = titlesSimplified;
        }
        // 更新 title 和 value 的显示
        if (titles != null && currentIndex >= 0 && currentIndex < titles.Length)
        {
            subTitleText.text = titles[currentIndex];
        }
        switch (currentIndex)
        {
            case 0:
                valueText.text = cleanValue.ToString() + "%";
                break;
            case 1:
                valueText.text = foodFreshValue.ToString() + "%";
                break;
            case 2:
                valueText.text = customerHappyValue.ToString() + "%";
                break;
        }
    }
}
