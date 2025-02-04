using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject settingPanel;
    public Animator SettingPanelAnimator;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OpenSettingPanel()
    {
        SettingPanelAnimator.ResetTrigger("Close");
        SettingPanelAnimator.SetTrigger("Open");
    }

    public void CloseSettingPanel()
    {
        SettingPanelAnimator.ResetTrigger("Open");
        SettingPanelAnimator.SetTrigger("Close");
    }
}
