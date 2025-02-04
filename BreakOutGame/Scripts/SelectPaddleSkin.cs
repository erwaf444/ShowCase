using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class SelectPaddleSkin : MonoBehaviour
{
    // public GameObject[] skins;
    // public int selectedSkin;

    // private void Awake()
    // {
    //     selectedSkin = PlayerPrefs.GetInt("selectedPaddleSkin", 0);
    //     foreach (GameObject skin in skins)
    //     {
    //         skin.SetActive(false);
    //     }
    //     skins[selectedSkin].SetActive(true);
    // }

    // public void ChangeNext()
    // {
    //     skins[selectedSkin].SetActive(false);
    //     selectedSkin++;
    //     if (selectedSkin == skins.Length)
    //     {
    //         selectedSkin = 0;
    //     }
    //     skins[selectedSkin].SetActive(true);
    //     PlayerPrefs.SetInt("selectedPaddleSkin", selectedSkin);
    // }

    // public void ChangePrevious()
    // {
    //     skins[selectedSkin].SetActive(false);
    //     selectedSkin--;
    //     if (selectedSkin == -1)
    //     {
    //         selectedSkin = skins.Length -1;
    //     }
    //     skins[selectedSkin].SetActive(true);
    //     PlayerPrefs.SetInt("selectedPaddleSkin", selectedSkin);
    // }

    public SpriteRenderer sr;
    public List<Sprite> skins = new List<Sprite>();
    private int selectedSkin = 0;
    public GameObject playerSkin;

    void Awake()
    {
        selectedSkin = PlayerPrefs.GetInt("selectedPaddleSkin", 0);
    }

    public void NextOptions()
    {
        selectedSkin = selectedSkin + 1;
        if (selectedSkin == skins.Count)
        {
            selectedSkin = 0;
        }
        sr.sprite = skins[selectedSkin];
        PlayerPrefs.SetInt("selectedPaddleSkin", selectedSkin);
      

        // PrefabUtility.SaveAsPrefabAsset(playerSkin, "Assets/Sprites/Skin/Paddle/Prefab/selectedPaddleSkin.prefab");
    }

    public void BackOptions()
    {
        selectedSkin = selectedSkin - 1;
        if (selectedSkin < 0)
        {
            selectedSkin = skins.Count - 1;
        }
        sr.sprite = skins[selectedSkin];
        PlayerPrefs.SetInt("selectedPaddleSkin", selectedSkin);

        // PrefabUtility.SaveAsPrefabAsset(playerSkin, "Assets/Sprites/Skin/Paddle/Prefab/selectedPaddleSkin.prefab");
    }
}
