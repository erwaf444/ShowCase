using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMainMenu : MonoBehaviour
{
    public GameObject scrollBar;
    float scroll_pos = 0;
    float []pos;
    int position = 0;
    public Button[] buttons;
    public GameObject nextButton;
    public GameObject prevButton;

    void Start()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollBar.GetComponent<Scrollbar>().value;
        } else {
            for (int i = 0; i < pos.Length; i++)
            {
                if(scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    position = i;
                    
                }
            }
        }
        UpdateTextVisibility();


    }

    void UpdateTextVisibility()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(i == position);
        }

        // 當在最後一個位置時隱藏nextButton，否則顯示
        nextButton.SetActive(position < pos.Length - 1);

        // 當在第一個位置時隱藏prevButton，否則顯示
        prevButton.SetActive(position > 0);
    }


    public void next()
    {
        if (position < pos.Length - 1)
        {
            AudioManager.instance.PlaySFX("TurnPage");
            position += 1;
            scroll_pos = pos[position];
        }

        
    }

    public void previous()
    {
        if (position > 0)
        {
            AudioManager.instance.PlaySFX("TurnPage");
            position -= 1;
            scroll_pos = pos[position];
        }
    }
}
