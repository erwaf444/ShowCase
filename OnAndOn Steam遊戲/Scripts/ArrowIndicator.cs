using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public Transform player;
    public Transform theEnd;
    public Vector3 offset = new Vector3(-4f, 4f, 0f); // 相對於玩家的偏移量（左上角）
    public float distanceFromPlayer = 2.0f;
    public Transform arrowToEndFrame;
    void Start()
    {

    }

    void Update()
    {
        if (theEnd == null || player == null) return;
        transform.position = player.position + offset;
        arrowToEndFrame.position = player.position + offset;
        Vector3 direction = (theEnd.position - player.position).normalized;

        // transform.position = player.position + direction * distanceFromPlayer;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
