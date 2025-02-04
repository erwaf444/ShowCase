using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 

public class ToolTipName : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public TextMeshProUGUI toolTip;

    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // toolTip.text = name;
        ToolTipManager._instance.SetAndShowToolTip(message);
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        // toolTip.text = "";
        ToolTipManager._instance.HideToolTip();
    }
}
