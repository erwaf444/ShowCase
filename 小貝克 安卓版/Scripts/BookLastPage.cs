using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookLastPage : MonoBehaviour
{
    public Material cuteMaterial; 
    public Material contentMaterial;
    public Texture2D[] frontTextures; 
    public Texture2D[] contentTextures; 
    public Button[] buttons; 

    void Start()
    {
        // 为每个按钮添加点击事件
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 防止闭包问题
            buttons[i].onClick.AddListener(() => ChangeFrontTextureTo(frontTextures[index], contentTextures[index]));
        }
    }

    // 更改 front tex
    void ChangeFrontTextureTo(Texture2D newFrontTex, Texture2D newContentTex)
    {
        cuteMaterial.SetTexture("_FrontTex", newFrontTex);
        contentMaterial.SetTexture("_FrontTex", newContentTex);
    }

    public void ButtonClikc()
    {
        Debug.Log("click");
    }
}
