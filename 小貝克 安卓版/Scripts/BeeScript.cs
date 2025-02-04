using UnityEngine;

public class BeeScript : MonoBehaviour
{
    public GameObject bee;
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public float speed = 2f;
    public float waveAmplitude = 1f;
    public float waveFrequency = 1f;

    private Vector3[] points;
    private int currentTargetIndex = 0;
    private Vector3 targetPosition;
    private float elapsedTime = 0f;
    private bool isFacingRight = false;
    public bool isMoving = true;

    void Start()
    {
        points = new Vector3[] { pointA.position, pointB.position, pointC.position };
        targetPosition = points[currentTargetIndex];
        UpdateFacing();  // 初始化朝向
    }

    void Update()
    {
        if (isMoving)
        {
            MoveBeeInSinePattern();
        }
    }

    void MoveBeeInSinePattern()
    {
        elapsedTime += Time.deltaTime * waveFrequency;
        Vector3 direction = (targetPosition - bee.transform.position).normalized;
        Vector3 waveOffset = Vector3.Cross(direction, Vector3.up) * Mathf.Sin(elapsedTime) * waveAmplitude;

        bee.transform.position = Vector3.MoveTowards(bee.transform.position, targetPosition + waveOffset, speed * Time.deltaTime);

        if (Vector3.Distance(bee.transform.position, targetPosition) < 0.1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % points.Length;
            targetPosition = points[currentTargetIndex];

            // 只在從 C 點返回 A 點時翻轉蜜蜂
            if (currentTargetIndex == 0)
            {
                FlipBee();
            }

            UpdateFacing();  // 更新朝向
        }
    }

    void FlipBee()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = bee.transform.localScale;
        currentScale.x *= -1;
        bee.transform.localScale = currentScale;
    }

    void UpdateFacing()
    {
        // 根據目標點更新朝向
        float directionToTarget = targetPosition.x - bee.transform.position.x;
        
        if ((directionToTarget > 0 && !isFacingRight) || (directionToTarget < 0 && isFacingRight))
        {
            FlipBee();
        }
    }
}