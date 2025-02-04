using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI; 
using System.Collections.Generic; 

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    public TextMeshProUGUI costTipText; // 提示文字
    public GameObject costTipDialog;    // 提示對話框

    // 定義物品和價格的字典
    private Dictionary<string, int> itemPrices = new Dictionary<string, int>()
    {
        { "雞塊麵包", 5 },
        { "麵包", 1 },
        { "小魚麵包", 2 },
        { "叉燒麵包", 2 },
        { "紙杯咖啡", 2 },
        { "小杯子咖啡", 1 },
        { "珍珠奶茶", 5 },
        { "能量飲料", 5 },
        { "啞鈴", 5 },
        { "橄欖球", 4 },
        { "網球", 3 },
        { "袜子", 1 }
    };

    void Start()
    {
    }

    // 實現 IPointerEnterHandler 介面，處理滑鼠進入事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 獲取當前物件的名稱
        string itemName = gameObject.name;

        // 檢查字典中是否有該物件的價格
        if (itemPrices.ContainsKey(itemName))
        {
            // 顯示提示對話框並設置提示文字
            costTipDialog.SetActive(true);
            costTipText.text = $"{itemName}, 需要{itemPrices[itemName]} 元";
        }
        else
        {
            Debug.LogWarning($"未找到 {itemName} 的價格！");
        }
    }

    // 實現 IPointerExitHandler 介面，處理滑鼠離開事件
    public void OnPointerExit(PointerEventData eventData)
    {
        costTipDialog.SetActive(false);
        costTipText.text = "";
    }
}