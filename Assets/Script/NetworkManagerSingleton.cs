using UnityEngine;
using Unity.Netcode;

public class NetworkManagerSingleton : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Không tìm thấy NetworkManager!");
            return;
        }

        // Đảm bảo NetworkManager không bị hủy khi chuyển Scene
        DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
    }
}
