using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FIrstLevelAnimText : MonoBehaviour
{
    public TextMeshProUGUI firstLevelAnimText;

    public GameObject Arrow1;
    public GameObject Arrow2;
    public GameObject Arrow3;



    public GameObject Arrow4;

    public GameObject Arrow5;
    public GameObject Arrow6;
    public GameObject Arrow7;
    public GameObject Arrow8;
    public GameObject Arrow9;
    public GameObject Arrow10;
    public GameObject Arrow11;

    public GameObject SettingUI;

    

    private bool arrowsShown = false;


    // Start is called before the first frame update
    void Start()
    {
        Arrow1.SetActive(false);
        Arrow2.SetActive(false);
        Arrow3.SetActive(false);
        Arrow4.SetActive(false);
        SettingUI.SetActive(false);
        Arrow5.SetActive(false);
        Arrow6.SetActive(false);
        Arrow7.SetActive(false);
        Arrow8.SetActive(false);
        Arrow9.SetActive(false);
        Arrow10.SetActive(false);
        Arrow11.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowText(string textToShow)
    {
        firstLevelAnimText.text = textToShow;
    }
    

    public void ShowArrows()
    {
        Arrow1.SetActive(true);
        Arrow2.SetActive(true);
        Arrow3.SetActive(true);
        arrowsShown = true;
    }

    public void HideArrows()
    {
        Arrow1.SetActive(false);
        Arrow2.SetActive(false);
        Arrow3.SetActive(false);
        arrowsShown = false;
    }

    public void ShowSecondArrows()
    {
        Arrow4.SetActive(true);
    }

    public void HideSecondArrows()
    {
        Arrow4.SetActive(false);
    }

    public void ShowSettingUI()
    {
        SettingUI.SetActive(true);
    }

    public void HideSettingUI()
    {
        SettingUI.SetActive(false);
    }

    public void ShowThirdArrows()
    {
        Arrow5.SetActive(true);
        
    }

    public void HideThirdArrows()
    {
        Arrow5.SetActive(false);
    }

    public void ShowFourthArrows()
    {
        Arrow6.SetActive(true);

    }

    public void HideFourthArrows()
    {
        Arrow6.SetActive(false);
    }


    public void ShowFifthArrows()
    {
        Arrow7.SetActive(true);
    }

    public void HideFifthArrows()
    {
        Arrow7.SetActive(false);
    }


    public void ShowSixthArrows()
    {
        Arrow8.SetActive(true);
    }

    public void HideSixthArrows()
    {
        Arrow8.SetActive(false);
    }

    public void ShowSeventhArrows()
    {
        Arrow9.SetActive(true);
    }

    public void HideSeventhArrows()
    {
        Arrow9.SetActive(false);
    }

    public void ShowEighthArrows()
    {
        Arrow10.SetActive(true);
    }

    public void HideEighthArrows()
    {
        Arrow10.SetActive(false);
    }

    public void ShowNinthArrows()
    {
        Arrow11.SetActive(true);
    }

    public void HideNinthArrows()
    {
        Arrow11.SetActive(false);
    }

   


}
