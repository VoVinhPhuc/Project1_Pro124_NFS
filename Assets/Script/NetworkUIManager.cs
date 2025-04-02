using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUIManager : MonoBehaviour
{
    public GameObject panel;
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

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
