using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    // ホストとして起動（受ける側）
    public void OnHostButtonClicked()
    {
        // 本来はここでLobbyの作成処理を行いますが、
        SceneManager.LoadScene("RoomScene"); // 次の部屋設定シーンへ
    }

    // クライアントとして起動（落とす側）
    public void OnClientButtonClicked()
    {
        // 部屋一覧シーンへ遷移
        SceneManager.LoadScene("RoomListScene");
    }
}