using UnityEngine;
using UnityEngine.UI;

public class ScrollerImage : MonoBehaviour
{
    public float speed = 10f;
    Vector3 startPos;  
    float repeatWidth;
    void Start()
    {
        startPos = transform.position;
                // AdjustImageSize();

        repeatWidth = GetComponent<BoxCollider2D>().size.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left  * speed * Time.deltaTime);
        if(transform.position.x < startPos.x - repeatWidth)
        {
            transform.position = startPos;
        }
    }

}
