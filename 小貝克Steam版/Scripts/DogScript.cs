using UnityEngine;

public class DogScript : MonoBehaviour
{
    public GameObject dog;
    public Transform  pointA;
    public Transform  pointB;
    public float speed = 2f;  // 移動速度
    public Animator dogAnimator; // 動畫控制器
    private Vector3 targetPosition;  // 目標位置
    private bool movingToB = true;   // 追蹤是否朝向 B 點
    public bool isMoving = true; 

    void Start()
    {
        targetPosition = pointB.position;

        if (dogAnimator == null)
        {
            dogAnimator = dog.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveBetweenPoints();
        }
    }

    void MoveBetweenPoints()
    {
        // 移動狗的位置
        dog.transform.position = Vector3.MoveTowards(dog.transform.position, targetPosition, speed * Time.deltaTime);

        dogAnimator.SetBool("isWalking", true); 

        // 如果狗已經到達目標位置，切換到另一個點
        if (Vector3.Distance(dog.transform.position, targetPosition) < 0.1f)
        {
            dogAnimator.SetBool("isWalking", false);

            if (movingToB)
            {
                // 如果正在移動到 B 點，則目標改為 A 點
                targetPosition = pointA.position;
                FlipDog(true);
            }
            else
            {
                // 如果正在移動到 A 點，則目標改為 B 點
                targetPosition = pointB.position;
                FlipDog(false);
            }

            // 切換方向
            movingToB = !movingToB;
        }
    }

    void FlipDog(bool faceLeft)
    {
        Vector3 localScale = dog.transform.localScale;

        if (faceLeft)
        {
            // 面向左邊
            localScale.x = -Mathf.Abs(localScale.x);
        }
        else
        {
            // 面向右邊
            localScale.x = Mathf.Abs(localScale.x);
        }

        dog.transform.localScale = localScale;
    }
}
