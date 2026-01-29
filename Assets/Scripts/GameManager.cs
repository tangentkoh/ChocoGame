using Unity.Netcode;
using UnityEngine;
using TMPro;
using unityroom.Api;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject hostUIPanel;
    [SerializeField] private GameObject clientUIPanel;
    [SerializeField] private TextMeshProUGUI scoreText; // スコア表示用

    // ネットワーク変数：ホストが書き込み、全員が読み取れる
    // 重力の影響などで微妙にズレるのを防ぐため、サーバーが正解を持ちます
    private NetworkVariable<int> score = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        // UIの切り替え
        if (IsHost) {
            hostUIPanel.SetActive(true);
            clientUIPanel.SetActive(false);
        } else {
            hostUIPanel.SetActive(false);
            clientUIPanel.SetActive(true);
        }

        // スコアが変化した時にUIを更新するイベントを登録
        score.OnValueChanged += (oldValue, newValue) => {
            scoreText.text = newValue.ToString();
        };
        
        // 初期値の反映
        scoreText.text = score.Value.ToString();
    }

    // スコアを加算するメソッド（ホストのみ実行可能）
    public void AddScore()
    {
        if (!IsServer) return;
        score.Value++;
    }

    public void LeaveGame()
    {
        // 1. ネットワーク接続をシャットダウン
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

    // 2. ロビーからも退出（もしロビー機能を使っている場合）
    // LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId); 
    // ※今回は簡易的にシーン遷移だけで対応します

    // 3. タイトルシーンへ
        if (IsServer)
        {
            UnityroomApiClient.Instance.SendScore(1, score.Value, ScoreboardWriteMode.HighScoreDesc);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
}