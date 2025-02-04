using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System;

public class SkinSelect : MonoBehaviour
{
    public SpriteRenderer sr;
    public List<Sprite> skins = new List<Sprite>();
    private int selectedSkin = 0;
    public GameObject playerSkin;

    async void Awake()
    {
        selectedSkin = PlayerPrefs.GetInt("selectedSkin", 0);
        await UnityServices.InitializeAsync();
    }

    public void NextOptions()
    {
        selectedSkin = selectedSkin + 1;
        if (selectedSkin == skins.Count)
        {
            selectedSkin = 0;
        }
        sr.sprite = skins[selectedSkin];
        PlayerPrefs.SetInt("selectedSkin", selectedSkin);
      

        // PrefabUtility.SaveAsPrefabAsset(playerSkin, "Assets/Sprites/Skin/Ball/Prefab/selectedSkin.prefab");
    }

    public void BackOptions()
    {
        selectedSkin = selectedSkin - 1;
        if (selectedSkin < 0)
        {
            selectedSkin = skins.Count - 1;
        }
        sr.sprite = skins[selectedSkin];
        PlayerPrefs.SetInt("selectedSkin", selectedSkin);

        // PrefabUtility.SaveAsPrefabAsset(playerSkin, "Assets/Sprites/Skin/Ball/Prefab/selectedSkin.prefab");
    }

 


    


 

}
