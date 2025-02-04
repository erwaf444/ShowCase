using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider slider;
    
    // MainMenu轉到關卡
    public void LoadLevel(string levelId)
    {
        StartCoroutine(LoadAsynchronously(levelId));
    }

    IEnumerator LoadAsynchronously(string levelId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelId);
        loadingPanel.SetActive(true);

        float targetProgress = 0f;
        


        while(operation.isDone == false)
        {
            // float progress = Mathf.Clamp01(operation.progress / .9f);
            // slider.value = progress;
            // yield return null;

            // Calculate the target progress
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smoothly transition the slider value to the target progress
            slider.value = Mathf.Lerp(slider.value, targetProgress, Time.deltaTime * 20f);

            // Optionally, if you want to ensure the slider reaches 1 when the operation is done
            if (operation.isDone)
            {
                slider.value = 1f;
            }

            yield return null;
        }

    }


    // LoginPage轉到MainMenu

    public void LoadToMainMenu(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronouslyToMainMenu(sceneIndex));
    }

    public void LoadToGameScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronouslyToMainMenu(sceneIndex));
    }

    IEnumerator LoadAsynchronouslyToMainMenu(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingPanel.SetActive(true);

        float targetProgress = 0f;
        


        while(operation.isDone == false)
        {
            // float progress = Mathf.Clamp01(operation.progress / .9f);
            // slider.value = progress;
            // yield return null;

            // Calculate the target progress
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smoothly transition the slider value to the target progress
            slider.value = Mathf.Lerp(slider.value, targetProgress, Time.deltaTime * 20f);

            // Optionally, if you want to ensure the slider reaches 1 when the operation is done
            if (operation.isDone)
            {
                slider.value = 1f;
            }

            yield return null;
        }

    }


    // 關卡轉到關卡

    public void LoadLevelToLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronouslyLevelToLevel(sceneIndex));
    }

    IEnumerator LoadAsynchronouslyLevelToLevel(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingPanel.SetActive(true);

        float targetProgress = 0f;
        


        while(operation.isDone == false)
        {
            // float progress = Mathf.Clamp01(operation.progress / .9f);
            // slider.value = progress;
            // yield return null;

            // Calculate the target progress
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Smoothly transition the slider value to the target progress
            slider.value = Mathf.Lerp(slider.value, targetProgress, Time.deltaTime * 20f);

            // Optionally, if you want to ensure the slider reaches 1 when the operation is done
            if (operation.isDone)
            {
                slider.value = 1f;
            }

            yield return null;
        }

    }

}