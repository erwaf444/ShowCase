using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeInfo : MonoBehaviour
{
    public Image CurrentTipImage;
    public Image TipContentImage;
    public Material[] TipImageMaterials;
    public Material[] TipMaterials;
    private Vector2 originalSize;
    private bool heightReduced = false;
    private bool isDefaultImage = true;
    private int lastTipMaterialIndex = -1;
    
    // 新增：存儲實例化的材質
    private Material tipContentMaterialInstance;
    private Material currentTipMaterialInstance;
    
    void Start()
    {
        originalSize = TipContentImage.rectTransform.sizeDelta;
        
        // 創建材質實例
        InitializeMaterialInstances();
        
        SetDefaultImage();
    }
    
    // 新增：初始化材質實例的方法
    private void InitializeMaterialInstances()
    {
        // 為兩個 Image 創建材質實例
        tipContentMaterialInstance = new Material(TipContentImage.material);
        currentTipMaterialInstance = new Material(CurrentTipImage.material);
        
        // 將實例化的材質賦值給 Image
        TipContentImage.material = tipContentMaterialInstance;
        CurrentTipImage.material = currentTipMaterialInstance;
    }

    public void ChangeTipMaterialRandomly()
    {
        if (TipMaterials.Length > 0)
        {
            Debug.Log("changeTipMaterialRandomly!");
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, TipMaterials.Length);
            } while (randomIndex == lastTipMaterialIndex);

            ChangeTipContentMaterials(randomIndex);
            lastTipMaterialIndex = randomIndex;
        }
    }

    public void ChangeTipContentMaterials(int index)
    {
        if (index >= 0 && index < TipMaterials.Length)
        {
            if (TipMaterials[index] != null && TipImageMaterials[index] != null)
            {
                // 使用實例化的材質進行修改
                tipContentMaterialInstance.SetTexture("_BackTex", TipMaterials[index].GetTexture("_BackTex"));
                currentTipMaterialInstance.SetTexture("_BackTex", TipImageMaterials[index].GetTexture("_BackTex"));
                Debug.Log("Material changed successfully to index: " + index);
            }
            else
            {
                Debug.LogWarning("Material at index " + index + " is null!");
            }
        }
        else
        {
            Debug.LogWarning("Invalid material index: " + index);
        }
    }

    private void SetDefaultImage()
    {
        if (!heightReduced)
        {
            RectTransform rectTransform = TipContentImage.rectTransform;
            Vector2 newSize = rectTransform.sizeDelta;
            newSize.x *= 0.8f;
            rectTransform.sizeDelta = newSize;
            heightReduced = true;
            Debug.Log("heightReduced: " + heightReduced);
        }
        isDefaultImage = true;
        Debug.Log("Default image set. isDefaultImage: " + isDefaultImage);
    }

    private void RestoreOriginalHeight()
    {
        if (heightReduced)
        {
            RectTransform rectTransform = TipContentImage.rectTransform;
            rectTransform.sizeDelta = originalSize;
            heightReduced = false;
            Debug.Log("heightReduced: " + heightReduced);
        }
    }
    
    // 新增：在物件被銷毀時清理材質實例
    private void OnDestroy()
    {
        if (tipContentMaterialInstance != null)
            Destroy(tipContentMaterialInstance);
            
        if (currentTipMaterialInstance != null)
            Destroy(currentTipMaterialInstance);
    }
}