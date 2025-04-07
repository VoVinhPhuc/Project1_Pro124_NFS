using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class NetworkUIManager : MonoBehaviour
{
    public GameObject panel;
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    public TMP_InputField ipAddressInput;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        serverButton.onClick.AddListener(StartServer);
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        HidePanel();
    }

    void StartClient()
    {
        string hostIP = ipAddressInput.text; // Lấy IP nhập từ UI
        if (string.IsNullOrEmpty(hostIP))
        {
            hostIP = "127.0.0.1"; // Mặc định nếu không nhập
        }

        NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>().SetConnectionData(
            hostIP, 7777); // Cổng mặc định của NGO

        NetworkManager.Singleton.StartClient();
        HidePanel();
    }

    void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        HidePanel();
    }
    void HidePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false); // Ẩn panel khi ấn nút
        }
    }

}
