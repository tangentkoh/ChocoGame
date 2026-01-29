using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;

public class ServiceInitializer : MonoBehaviour
{
    async void Start()
    {
        try
        {
            // Unity Servicesの初期化
            await UnityServices.InitializeAsync();

            // すでにサインインしていないか確認してサインイン
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"サインイン完了: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"初期化エラー: {e.Message}");
        }
    }
}