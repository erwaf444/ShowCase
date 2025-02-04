using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 確保只有食物袋物件可以進入垃圾桶
        if (other.CompareTag("FoodBag")) // 假設食物袋的標籤是 "FoodBag"
        {
            DragDrop dragDrop = other.GetComponent<DragDrop>();
            if (dragDrop != null && dragDrop.isBeingDragged)
            {
                // 調用重置食物袋的方法
                OvenScript ovenScript = FindObjectOfType<OvenScript>();
                if (ovenScript != null)
                {
                    ovenScript.ResetFoodBag(dragDrop.foodBagId); // 傳遞食物袋 ID
                }
            }
        }
    }
}
