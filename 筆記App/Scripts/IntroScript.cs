using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;


public class IntroScript : MonoBehaviour
{
    public bool isRegister = false;
    public UsernameAndEmailSet usernameAndEmailSet;
    public GameObject avatarPanel;
    public GameObject usernameAndEmailObj;
    public GameObject mainUI;
    public GameObject IntroUi;

    async void Start()
    {
        isRegister = await usernameAndEmailSet.LoadRegisterStatus();
        if (isRegister)
        {
            avatarPanel.SetActive(false);
            usernameAndEmailObj.SetActive(false);
            IntroUi.SetActive(false);
            mainUI.SetActive(true);
        }
    }

    void Update()
    {
        
    }

    public void GetStart()
    {
        IntroUi.SetActive(false);
        avatarPanel.SetActive(true);
    }
}
