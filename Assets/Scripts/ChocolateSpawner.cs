using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI; // ボタン制御用
using System.Threading.Tasks;

public class ChocolateSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject chocolatePrefab;
    [SerializeField] private Button dropButton; // Inspectorでボタンを紐付け
    [SerializeField] private float coolTime = 1.0f; // 1秒

    private bool canDrop = true;

    public async void RequestSpawnChocolate()
    {
        if (!canDrop) return;

        canDrop = false;
        if (dropButton != null) dropButton.interactable = false; // ボタンを押せなくする

        // サーバーに生成を依頼
        SpawnChocolateRpc();

        // クールタイム待機
        await Task.Delay((int)(coolTime * 1000));

        canDrop = true;
        if (dropButton != null) dropButton.interactable = true; // 再び押せるようにする
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnChocolateRpc()
    {
        float randomX = Random.Range(-3f, 3f);
        Vector3 spawnPos = new Vector3(randomX, 6f, 0);
        GameObject go = Instantiate(chocolatePrefab, spawnPos, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
    }
}