using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    private Lobby _lobby;
    private LobbyManager _manager;

    // ボタンに情報をセットする
    public void SetInfo(Lobby lobby, LobbyManager manager)
    {
        _lobby = lobby;
        _manager = manager;
        roomNameText.text = lobby.Name;
    }

    // ボタンが押された時（OnClickに登録）
    public void OnClick()
    {
        _manager.JoinRoom(_lobby);
    }
}