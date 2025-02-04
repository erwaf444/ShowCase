using UnityEngine;

public class DayPanel : MonoBehaviour
{
    public GameObject ContinueAnimObj;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // 按 R 鍵重置動畫
        {
            ContinueAnimObj.SetActive(false);
        }
    }
}
