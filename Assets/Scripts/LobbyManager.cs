using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [Header("Host Settings")]
    [SerializeField] private TMP_InputField roomNameInputField;

    [Header("Client Settings")]
    [SerializeField] private Transform itemParent;
    [SerializeField] private GameObject roomItemPrefab;

    // --- ホスト側の処理 ---
    public void OnCreateRoomButtonClicked()
    {
        string name = roomNameInputField.text;
        if (!string.IsNullOrEmpty(name)) CreateRoom(name);
    }

    private async void CreateRoom(string roomName)
    {
        try {
            var allocation = await RelayService.Instance.CreateAllocationAsync(2);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData
            );

            var options = new CreateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            };
            await LobbyService.Instance.CreateLobbyAsync(roomName, 2, options);
            NetworkManager.Singleton.StartHost();
            // NetworkManagerのSceneManagerを使って遷移
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        } catch (System.Exception e) { Debug.LogError(e); }
    }

    // --- クライアント側の処理 ---
    public async void ListRooms()
    {
        try {
            foreach (Transform child in itemParent) Destroy(child.gameObject);
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(new QueryLobbiesOptions());
            foreach (var lobby in response.Results) {
                GameObject item = Instantiate(roomItemPrefab, itemParent);
                item.GetComponent<RoomListItem>().SetInfo(lobby, this);
            }
        } catch (System.Exception e) { Debug.LogError(e); }
    }

    // ★これがエラーの原因！追加してください★
    public async void JoinRoom(Lobby lobby)
    {
        try {
            var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joinedLobby.Data["JoinCode"].Value;

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            // クライアントはStartClientすると、ホストがいるシーンに自動で同期して飛ばされます
        } catch (System.Exception e) { Debug.LogError(e); }
    }

    public void BackToTitle()
{
    // 通信中であれば切断する
    if (NetworkManager.Singleton != null)
    {
        NetworkManager.Singleton.Shutdown();
    }
    // タイトルシーンへ遷移
    UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
}
}