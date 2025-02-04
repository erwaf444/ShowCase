using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate = false;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject forwardButton;
    public BookFIirstPageScript bookFIirstPageScript;
    public List<Button> RecipeButtons;
    public GameObject middle;
    public GameObject halfMiddle;
    public bool isOnFirstPage = true;
    public GameObject[] firstPageAnimation;
    public DogScript dogScript;
    public BeeScript beeScript;
    public BookLastPage bookLastPage;
    public GameObject[] lastPageButtons;

    private void Start()
    {
        InitialState();
        
    }

    void Update()
    {
        if (index == 0)
        {
            RecipeButtons[0].interactable = true;
            RecipeButtons[1].interactable = true;
            RecipeButtons[2].interactable = true;
            RecipeButtons[3].interactable = true;
            RecipeButtons[4].interactable = true;
            RecipeButtons[5].interactable = true;
            RecipeButtons[6].interactable = true;
            RecipeButtons[7].interactable = true;
            for (int i = 0; i < lastPageButtons.Length; i++)
            {
                lastPageButtons[i].SetActive(true);
            }
        }
        else
        {
            RecipeButtons[0].interactable = false;
            RecipeButtons[1].interactable = false;
            RecipeButtons[2].interactable = false;
            RecipeButtons[3].interactable = false;
            RecipeButtons[4].interactable = false;
            RecipeButtons[5].interactable = false;
            RecipeButtons[6].interactable = false;
            RecipeButtons[7].interactable = false;
            for (int i = 0; i < lastPageButtons.Length; i++)
            {
                lastPageButtons[i].SetActive(false);
            }
        }

        if (index == -1 && !isOnFirstPage)
        {
            isOnFirstPage = true; // 設置為已經在第一頁
            FadeInFirstPage(); // 呼叫淡入
        }
        else if (index != -1 && isOnFirstPage)
        {
            isOnFirstPage = false; // 設置為不在第一頁
            FadeOutFirstPage(); // 呼叫淡出
        }

        if(forwardButton.activeInHierarchy == false)
        {
            StartCoroutine(showHalfMiddleDelay());
            
        } else 
        {
            StartCoroutine(showMiddleDelay());
        }
        
    }

    IEnumerator showMiddleDelay()
    {
        yield return new WaitForSeconds(0.2f);
        halfMiddle.SetActive(false);
        middle.SetActive(true);
    }

    IEnumerator showHalfMiddleDelay()
    {
        yield return new WaitForSeconds(0.2f);
        halfMiddle.SetActive(true);
        middle.SetActive(false);
    }

    public void InitialState()
    {
        for (int i=0; i<pages.Count; i++)
        {
            pages[i].transform.rotation=Quaternion.identity;
        }
        pages[0].SetAsLastSibling();
        backButton.SetActive(false);

    }

    public void RotateForward()
    {
        if (rotate == true) { return; }
        index++;
        float angle = 180;
        ForwardButtonActions();
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));
    }

    public void ForwardButtonActions()
    {
        AudioManager.instance.PlaySFX("TurnPage");
        if (backButton.activeInHierarchy == false)
        {
            backButton.SetActive(true); //every time we turn the page forward, the back button should be activated
            bookFIirstPageScript.nextButton.interactable = false;
            bookFIirstPageScript.prevButton.interactable = false;
        }
        if (index == pages.Count - 1)
        {
            forwardButton.SetActive(false); //if the page is last then we turn off the forward button
            
        }
    }


    public void RotateBackward()
    {
        if (rotate == true) { return; }
        float angle = 0; //in order to rotate the page back, you need to set the rotation to 0 degrees around the y axis
        pages[index].SetAsLastSibling();
        BackButtonActions();
        StartCoroutine(Rotate(angle, false));
        
    }

    public void BackButtonActions()
    {
        AudioManager.instance.PlaySFX("TurnPage");
        if (forwardButton.activeInHierarchy == false)
        {
            forwardButton.SetActive(true); //every time we turn the page back, the forward button should be activated
        }
        if (index - 1 == -1)
        {
            backButton.SetActive(false); //if the page is first then we turn off the back button
            bookFIirstPageScript.nextButton.interactable = true;
            bookFIirstPageScript.prevButton.interactable = true;
            
        }
    }

    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            rotate = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value); //smoothly turn the page
            float angle1 = Quaternion.Angle(pages[index].rotation, targetRotation); //calculate the angle between the given angle of rotation and the current angle of rotation
            if (angle1 < 0.1f)
            {
                if (forward == false)
                {
                    index--;
                }
                rotate = false;
                break;

            }
            yield return null;

        }
    }

    public void FadeInFirstPage()
    {
        foreach (var obj in firstPageAnimation)
        {
            StartCoroutine(FadeTo(obj, 1f, 1f)); // 1f 代表完全可見，1秒內淡入
            dogScript.isMoving = true;
            beeScript.isMoving = true;
        }
    }

    public void FadeOutFirstPage()
    {
        foreach (var obj in firstPageAnimation)
        {
            StartCoroutine(FadeTo(obj, 0f, 1f)); // 0f 代表完全透明，1秒內淡出
            dogScript.isMoving = false;
            beeScript.isMoving = false;
        }
    }

    IEnumerator FadeTo(GameObject obj, float targetAlpha, float duration)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = obj.AddComponent<CanvasGroup>();
        
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha; // 確保最終值設置正確
    }

  
   
}
