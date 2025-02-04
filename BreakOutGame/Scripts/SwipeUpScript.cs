using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeUpScript : MonoBehaviour
{

    public GameObject SwipeUpTip;
    // Start is called before the first frame update
    void Start()
    {
        SwipeUpHide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwipeUpHide()
    {
        SwipeUpTip.SetActive(true);
        StartCoroutine(SwipeUpHideDelay());
    }

    IEnumerator SwipeUpHideDelay()
    {
        yield return new WaitForSeconds(5);
        SwipeUpTip.SetActive(false);
    }
}
