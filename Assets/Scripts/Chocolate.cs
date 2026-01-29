using Unity.Netcode;
using UnityEngine;

public class Chocolate : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 当たり判定の計算は「サーバー（ホスト）」のみで行うのが鉄則（不正防止・同期ズレ防止）
        if (!IsServer) return;

        // もし当たった相手が「籠（Basket）」だったら
        if (other.CompareTag("Basket"))
        {
            // スコアを加算（GameManagerを探して実行）
            Object.FindFirstObjectByType<GameManager>().AddScore();

            // ネットワーク上からチョコを消去
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject); // ローカルのオブジェクトも削除
        }
        
        // 画面外（地面など）に落ちた場合
        if (other.CompareTag("Ground"))
        {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}