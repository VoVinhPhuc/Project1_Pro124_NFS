using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    public static NetworkManagerUI Instance { get; private set; }
    public static string RoomID; // Lưu ID phòng

    public TMP_Text statusText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (NetworkManager.Singleton != null)
            {
                DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager chưa được thêm vào Scene!");
            return;
        }

        RoomID = PlayerPrefs.GetString("RoomID", ""); // Lấy ID từ PlayerPrefs
        if (string.IsNullOrEmpty(RoomID))
        {
            Debug.LogError("Room ID bị trống!");
            return;
        }

        statusText.text = "Hosting Room: " + RoomID;

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("✅ Host đã khởi động, chuyển sang Scene Room...");
            SceneManager.LoadScene("Room"); // Chuyển sang scene Room

            Debug.Log("🟢 Kiểm tra NetworkManager sau khi Load Scene...");
            Debug.Log($"🟢 NetworkManager.Singleton: {NetworkManager.Singleton}");
        }
        else
        {
            Debug.LogError("Lỗi khi khởi động Host!");
        }
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager chưa được thêm vào Scene!");
            return;
        }

        RoomID = PlayerPrefs.GetString("RoomID", "");
        if (string.IsNullOrEmpty(RoomID))
        {
            statusText.text = "ID phòng không hợp lệ!";
            return;
        }

        statusText.text = "Joining Room: " + RoomID;

        if (NetworkManager.Singleton.StartClient())
        {
            SceneManager.LoadScene("Room"); // Chuyển sang scene Room
        }
        else
        {
            Debug.LogError("Lỗi khi khởi động Client!");
        }
    }
}
