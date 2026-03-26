using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    public Transform player;   // 玩家
    public float offsetX = 0f; // X轴偏移
    public float fixedY = 0f;  // 固定的Y位置

    void LateUpdate()
    {
        if (player == null) return;

        float targetX = player.position.x + offsetX;

        transform.position = new Vector3(
            targetX,
            fixedY,
            transform.position.z
        );

        if(transform.position.x > 5.9f)
        {
            transform.position = new Vector3(5.9f, 0, -10f);
        }
        if (transform.position.x < -5.9f)
        {
            transform.position = new Vector3(-5.9f, 0, -10f);
        }
    }
}