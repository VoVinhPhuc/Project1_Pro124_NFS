using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject panelPlay;
    public GameObject panelHost;
    public GameObject panelJoin;
    public GameObject panelPlaySelection; // Panel chọn Singleplayer hoặc Multiplayer
    public GameObject panelGameMode; // Panel GameMode (nếu chọn Singleplayer)

    public GameObject panelLoading;

    public TMP_Text generatedRoomIdText;
    public TMP_InputField joinRoomIdInput;
    public TMP_Text errorMessageText; // Hiển thị lỗi nếu nhập sai ID
    public Slider loadingSlider;

    private string selectedScene;

    private void Start()
    {
        panelPlaySelection.SetActive(false);
        panelGameMode.SetActive(false);
        panelPlay.SetActive(false);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);
        panelLoading.SetActive(false);
        errorMessageText.text = ""; // Ẩn lỗi khi bắt đầu
    }

    public void OpenPlaySelectionPanel()
    {
        panelPlaySelection.SetActive(true);
        panelGameMode.SetActive(false);
        panelPlay.SetActive(false);
    }

    public void OpenPlayPanel()
    {
        panelPlay.SetActive(true);
    }

    public void ChooseSinglePlayer()
    {
        panelGameMode.SetActive(true);
        panelPlay.SetActive(false);
    }

    public void ChooseMultiplayer()
    {
        panelGameMode.SetActive(false);
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
    public void StartSinglePlayer(string sceneName)
    {
        selectedScene = sceneName;
        panelGameMode.SetActive(false);
        panelLoading.SetActive(true);
        StartCoroutine(LoadSceneWithProgress());
    }

    private IEnumerator LoadSceneWithProgress()
    {
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        SceneManager.LoadScene(selectedScene);
    }
    public void BackToPlayPanel()
    {
        panelPlay.SetActive(true);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);
        errorMessageText.text = ""; // Ẩn lỗi khi quay lại
    }
}
