using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PetObjectScript : MonoBehaviour,  IPointerClickHandler
{
    public Animator PetAnimator;  // 參照到該物件的 Animator
    public PetScript petScripte;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 這個方法會在玩家點擊該物件時觸發
    public void OnPointerClick(PointerEventData eventData)
    {
        if (petScripte.isSleeping || petScripte.pet.GetComponent<Image>().sprite == petScripte.asleepSprite) return;
        Debug.Log("Object clicked!");
        
        // 觸發動畫
        if (PetAnimator != null)
        {
            PetAnimator.SetTrigger("isClick");  // 使用動畫觸發器來開始動畫
        }
        StartCoroutine(PlayNextAnimationAfterDelay("isClick", "isBlink"));
    }

    private IEnumerator PlayNextAnimationAfterDelay(string currentAnim, string nextAnim)
    {
        // 等待 "isPlay" 動畫播放完畢
        AnimatorStateInfo stateInfo = PetAnimator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;  // 動畫的長度

        // 等待動畫完成
        yield return new WaitForSeconds(animationLength);

        // 播放 "isBlink" 動畫
        PetAnimator.SetTrigger(nextAnim);
    }

   
}
