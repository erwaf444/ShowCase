using UnityEngine;

public class CameraScript : MonoBehaviour
{
     [SerializeField] private Transform target; // 玩家 Transform
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // 固定 Z 軸位置
    [SerializeField] private float smoothTime = 0.0f; // 平滑跟隨時間

    private Vector3 velocity = Vector3.zero;

    // void LateUpdate()
    // {
    //     if (target == null) return;

    //     // 計算目標位置，鎖定 Z 軸
    //     Vector3 targetPosition = new Vector3(target.position.x, target.position.y, offset.z);

    //     // 平滑移動攝影機到目標位置
    //     transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    // }

    // void LateUpdate()
    // {
    //     if (target == null) return;

    //     // 計算目標位置，加上偏移量
    //     Vector3 targetPosition = new Vector3(target.position.x, target.position.y, offset.z);

    //     // 使用 Lerp 做輕微平滑插值
    //     transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 20f);
    // }

    void LateUpdate()
    {
        if (target == null) return;

        // 立即將攝影機位置設為玩家位置，加上偏移量
        transform.position = new Vector3(target.position.x, target.position.y, offset.z);
    }
    
}
