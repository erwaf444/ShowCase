using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private UnityEngine.Resolution[] resolutions;
    private List<UnityEngine.Resolution> filterResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;
    void Start()
    {
        resolutions = Screen.resolutions;
        filterResolutions = new List<UnityEngine.Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((resolutions[i].width == 1920 && resolutions[i].height == 1080) ||
                (resolutions[i].width == 1280 && resolutions[i].height == 720) ||
                (resolutions[i].width == 1920 && resolutions[i].height == 1200) ||
                (resolutions[i].width == 1024 && resolutions[i].height == 768) ||
                (resolutions[i].width == 1280 && resolutions[i].height == 1024) ||
                (resolutions[i].width == 1366 && resolutions[i].height == 768))
            {
                filterResolutions.Add(resolutions[i]);
            }
        }


        List<string> options = new List<string>();
        for (int i = 0; i < filterResolutions.Count; i++)
        {
            string resolutionOption = filterResolutions[i].width + " x " + filterResolutions[i].height + " " + filterResolutions[i].refreshRate + "Hz";
            options.Add(resolutionOption);
            if (filterResolutions[i].width == Screen.width && filterResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }


    public void setResolution(int resolutionIndex)
    {
        UnityEngine.Resolution resolution = filterResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    public void ToggleScreenMode()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            // 切換到全螢幕模式
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("切換到全螢幕模式");
        }
        else
        {
            // 切換到視窗模式
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1280, 720, false); // 設置寬度、高度、非全螢幕
            Debug.Log("切換到視窗模式");
        }
    }


    // 設置為視窗模式
    public void SetWindowMode()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1280, 720, false); // 寬 1280，高 720，false 表示非全螢幕
    }

    // 設置為全螢幕模式
    public void SetFullScreenMode()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    void Update()
    {
        
    }
}
