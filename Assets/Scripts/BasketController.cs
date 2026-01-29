using Unity.Netcode;
using UnityEngine;

public class BasketController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f; // 1回の移動量
    [SerializeField] private float xLimit = 5f;     // 左右の限界値

    public void Move(float direction)
    {
        if (!IsServer) return;

        // 新しい位置を計算
        float newX = transform.position.x + (direction * moveSpeed);
        
        // xLimitの範囲内に制限
        newX = Mathf.Clamp(newX, -xLimit, xLimit);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}