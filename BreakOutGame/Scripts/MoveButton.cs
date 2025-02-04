using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class MoveButton : MonoBehaviour
{
    public float speed;

    public GameObject showMoveButtonEnd;
    public GameObject hideMoveButtonEnd;


    // private bool isMovingShow = false;
    // private bool isMovingHide = false;
 

    public Button ShowSettingButton;


    public GameObject SettingButtonUI;

    public Button HideSettingButton;


    // Start is called before the first frame update
    void Start()
    {
        ShowSettingButton.gameObject.SetActive(true);
        SettingButtonUI.SetActive(false);
        
        
        ShowSettingButton.onClick.AddListener(ShowSettingUI);
        HideSettingButton.onClick.AddListener(HideSettingUI);
        

    }

    // Update is called once per frame
    void Update()
    {
     

         
        
        
        
    }


    public void ShowSettingUI()
    {
        // isMovingShow = true;
        // isMovingHide = false;
        ShowSettingButton.gameObject.SetActive(false);
        SettingButtonUI.SetActive(true);
        
    }

    public void HideSettingUI()
    {
        // isMovingHide = true;
        // isMovingShow = false;
        ShowSettingButton.gameObject.SetActive(true);
        SettingButtonUI.SetActive(false);
    }

}
