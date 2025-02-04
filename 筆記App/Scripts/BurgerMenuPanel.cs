using UnityEngine;
using UnityEngine.EventSystems;

public class BurgerMenuPanel : MonoBehaviour
{
    public GameObject burgerMenuPanel; //我的筆記的burgerMenu
    // public GameObject burgerMenuPanel2; //我的提醒的burgerMenu
    private bool isMenuOpen = false;

    void Update()
    {
        // 當菜單開啟時才檢查點擊
        if (isMenuOpen && Input.GetMouseButtonDown(0))
        {
            // 檢查是否點擊到UI元素
            if (!IsPointerOverUIElement())
            {
                CloseBurgerMenuPanel();
                // CloseBurgerMenuPanel2();
            }
        }
    }

    // 檢查是否點擊到UI元素
    private bool IsPointerOverUIElement()
    {
        // 獲取當前點擊位置
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // 創建射線檢測結果列表
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        
        // 執行射線檢測
        EventSystem.current.RaycastAll(eventData, results);

        // 遍歷所有射線檢測結果
        foreach (RaycastResult result in results)
        {
            // 檢查是否點擊到漢堡選單面板
            if (result.gameObject == burgerMenuPanel)
            {
                return true;
            }
        }

        return false;
    }

    public void OpenBurgerMenuPanel()
    {
        burgerMenuPanel.SetActive(true);
        isMenuOpen = true;
    }

    private void CloseBurgerMenuPanel()
    {
        burgerMenuPanel.SetActive(false);
        isMenuOpen = false;
    }

    // public void OpenBurgerMenuPanel2()
    // {
    //     burgerMenuPanel2.SetActive(true);
    //     isMenuOpen = true;
    // }

    // private void CloseBurgerMenuPanel2()
    // {
    //     burgerMenuPanel2.SetActive(false);
    //     isMenuOpen = false;
    // }
}