using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MakeTableTagSwitcher : MonoBehaviour
{
    // [System.Serializable]
    // public class TagImageGroup
    // {
    //     public string tagName;
    //     public List<GameObject> images;
    // }

    [System.Serializable]
    public class TagImageGroup
    {
        public string tagName;
        [System.Serializable]
        public class ImageWithExperience
        {
            public GameObject image;
            public int experiencePoints;
        }
        public List<ImageWithExperience> imagesWithExperience;
    }

    public List<TagImageGroup> tagImageGroups;
    public Button[] tagButtons;
    private int currentTagIndex = -1;
    public Player player;

    void Start()
    {
        for (int i = 0; i < tagButtons.Length; i++)
        {
            int index = i;
            tagButtons[i].onClick.AddListener(() => SwitchTag(index));
        }

        // 初始化时隐藏所有图片
        HideAllImages();

        // 显示第一个标签的图片
        if (tagImageGroups.Count > 0)
        {
            SwitchTag(0);
        }
    }

    void HideAllImages()
    {
        foreach (var group in tagImageGroups)
        {
            foreach (var imageWithExp  in group.imagesWithExperience)
            {
                imageWithExp.image.SetActive(false);
            }
        }
    }


    public void SwitchTag(int tagIndex)
    {
        if (tagIndex < 0 || tagIndex >= tagImageGroups.Count) return;

        // 如果切换到同一个标签，不做任何操作
        if (currentTagIndex == tagIndex) return;

        // 隐藏当前标签的所有图片
        if (currentTagIndex >= 0 && currentTagIndex < tagImageGroups.Count)
        {
            foreach (var imageWithExp  in tagImageGroups[currentTagIndex].imagesWithExperience)
            {
                imageWithExp.image.SetActive(false);
            }
        }

        currentTagIndex = tagIndex;

        // 显示新选择的标签的所有图片
        foreach (var imageWithExp in tagImageGroups[currentTagIndex].imagesWithExperience)
        {
            imageWithExp.image.SetActive(true);

            // 获取 Image 组件并检查经验点数
            Image imageComponent = imageWithExp.image.GetComponent<Image>();
            Button buttonComponent = imageWithExp.image.GetComponent<Button>();

            if (imageComponent != null)
            {
                if (player.experiencePoints < imageWithExp.experiencePoints)
                {
                    // 将颜色变为灰色表示无法制作
                    Color grayWithTransparency = new Color(0.7f, 0.7f, 0.7f, 0.5f); // RGB 为灰色，Alpha 为 0.5
                    imageComponent.color = grayWithTransparency;
                    // 禁用按钮的交互
                    buttonComponent.interactable = false;
                }
                else
                {
                    // 如果经验点数足够，恢复原始颜色
                    imageComponent.color = Color.white;
                    buttonComponent.interactable = true;
                }
            }
        }
    }
}