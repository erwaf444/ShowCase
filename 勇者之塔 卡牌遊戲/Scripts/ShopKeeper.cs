using UnityEngine;
using UnityEngine.EventSystems;

public class ShopKeeper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject shopKeeperTalk;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 當滑鼠進入 UI 元素時觸發
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hover");
        if (shopKeeperTalk != null)
        {
            shopKeeperTalk.SetActive(true); // 顯示 UI 元素
        }
    }

    // 當滑鼠離開 UI 元素時觸發
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("unhover");
        if (shopKeeperTalk != null)
        {
            shopKeeperTalk.SetActive(false); // 隱藏 UI 元素
        }
    }   
}
