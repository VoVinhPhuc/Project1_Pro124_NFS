using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject panelPlay;
    public GameObject panelHost;
    public GameObject panelJoin;

    public TMP_Text generatedRoomIdText;
    public TMP_InputField joinRoomIdInput;
    public TMP_Text errorMessageText; // Hiển thị lỗi nếu nhập sai ID

    private void Start()
    {
        panelPlay.SetActive(false);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);
        errorMessageText.text = ""; // Ẩn lỗi khi bắt đầu
    }

    public void OpenPlayPanel()
    {
        panelPlay.SetActive(true);
    }

    public void OpenHostPanel()
    {
        panelPlay.SetActive(false);
        panelHost.SetActive(true);

        // Tạo ID phòng ngẫu nhiên (5 chữ số)
        NetworkManagerUI.RoomID = Random.Range(10000, 99999).ToString();
        generatedRoomIdText.text = "Room ID: " + NetworkManagerUI.RoomID;

        PlayerPrefs.SetString("RoomID", NetworkManagerUI.RoomID);
        PlayerPrefs.Save();
    }

    public void EnterHostRoom()
    {
        if (NetworkManagerUI.Instance != null)
        {
            string nickName = PlayerPrefs.GetString("NickName", "Unknown");
            Debug.Log("Hosting Room - NickName: " + nickName);
            NetworkManagerUI.Instance.StartHost(); // Bắt đầu host
        }
        else
        {
            Debug.LogError("NetworkManagerUI chưa được gán vào Scene!");
        }
    }

    public void OpenJoinPanel()
    {
        panelPlay.SetActive(false);
        panelJoin.SetActive(true);
        errorMessageText.text = ""; // Reset lỗi khi mở panel Join
    }

    public void EnterJoinRoom()
    {
        string enteredRoomId = joinRoomIdInput.text;
        string storedRoomId = PlayerPrefs.GetString("RoomID", ""); // Lấy ID của Host đã lưu

        if (enteredRoomId == storedRoomId && !string.IsNullOrEmpty(enteredRoomId))
        {
            if (NetworkManagerUI.Instance != null)
            {
                NetworkManagerUI.Instance.StartClient(); // Bắt đầu client
            }
            else
            {
                Debug.LogError("NetworkManagerUI chưa được gán vào Scene!");
            }
        }
        else
        {
            errorMessageText.text = "ID phòng không hợp lệ!";
        }
    }

    public void BackToPlayPanel()
    {
        panelPlay.SetActive(true);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);
        errorMessageText.text = ""; // Ẩn lỗi khi quay lại
    }
}
