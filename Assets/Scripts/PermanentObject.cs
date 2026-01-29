using UnityEngine;

public class PermanentObject : MonoBehaviour
{
    private void Awake()
    {
        // 既に同じタグのオブジェクトがいる場合の重複回避（簡易版）
        var objs = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}