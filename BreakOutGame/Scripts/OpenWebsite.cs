using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWebsite : MonoBehaviour
{
    // 打开网址的方法
    public void OpenWebsiteURL()
    {
        // 这里指定你想要打开的网址
        string url = "https://www.pivotquotient.com/";
        Application.OpenURL(url);
    }

    public void OpenSocialURL()
    {
        // 这里指定你想要打开的网址
        string url = "https://www.pivotquotient.com/";
        Application.OpenURL(url);
    }

    public void OpenAndGiveFiveStar()
    {
        // 这里指定你想要打开的网址
        string url = "https://play.google.com/store/apps/details?id=com.DefaultCompany.MyBreakout";
        Application.OpenURL(url);
    }
    
}
