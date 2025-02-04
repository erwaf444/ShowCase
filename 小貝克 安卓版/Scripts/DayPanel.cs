using UnityEngine;

public class DayPanel : MonoBehaviour
{
    public GameObject ContinueAnimObj;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) // 按 R 鍵重置動畫
        {
            ContinueAnimObj.SetActive(false);
        }
    }
}
