//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager _instance;

    public Transform  toolTipPosition;
    public TextMeshProUGUI textComponent;
    [SerializeField] private GameObject ToolTipOn;
    [SerializeField] private GameObject ToolTipOff;
    [SerializeField] private Toggle toolTipToggle;


    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
        gameObject.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {   
        if (toolTipToggle == null)
        {
            Debug.LogError("toolTipToggle is not assigned in the Inspector!");
            return;
        }
        Cursor.visible = true;
        Debug.Log("gameObject.SetActive(false)");
        
    


        int toolTipState = PlayerPrefs.GetInt("ToolTipOn", 1);

        if (toolTipState == 1)
        {
            ToolTipOn.SetActive(true);
            ToolTipOff.SetActive(false);
            toolTipToggle.isOn = true;
        }
        else
        {
            ToolTipOn.SetActive(false);
            ToolTipOff.SetActive(true);
            toolTipToggle.isOn = false;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
       if (toolTipPosition != null)
        {
            transform.position = toolTipPosition.position; // 使用 Transform 的位置來設置工具提示位置
        }
    }

    public void SetAndShowToolTip(string message)
    {
        gameObject.SetActive(true);
        textComponent.text = message;
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }

    public void ToggleToolTip()
    {
        if (toolTipToggle.isOn)
        {
            ToolTipOn.SetActive(true);
            ToolTipOff.SetActive(false);
            PlayerPrefs.SetInt("ToolTipOn", 1);
            PlayerPrefs.Save();
        }
        else
        {
            ToolTipOn.SetActive(false);
            ToolTipOff.SetActive(true);
            PlayerPrefs.SetInt("ToolTipOn", 0);
            PlayerPrefs.Save();
        }
    }

    public void ToogleToolTipIconShow()
    {
        ToolTipOn.SetActive(true);
        ToolTipOff.SetActive(false);
        toolTipToggle.isOn = true;
        PlayerPrefs.SetInt("ToolTipOn", 1);
        PlayerPrefs.Save();
    }
    
    public void ToogleToolTipIconHide()
    {
        ToolTipOn.SetActive(false);
        ToolTipOff.SetActive(true);
        toolTipToggle.isOn = false;
        PlayerPrefs.SetInt("ToolTipOn", 0);
        PlayerPrefs.Save();
    }

   
   
}
