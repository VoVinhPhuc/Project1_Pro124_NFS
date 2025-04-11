using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject panelPlay;
    public GameObject panelHost;
    public GameObject panelJoin;
    public GameObject panelPlaySelection; // Panel chọn Singleplayer hoặc Multiplayer
    public GameObject panelGameMode; // Panel GameMode (nếu chọn Singleplayer)

    public GameObject panelLoading;

    public GameObject myNFSPanel;   // Panel hiển thị thông tin xe
    public GameObject newsPanel;    // Panel hiển thị tin tức
    public GameObject settingsPanel; // Panel cài đặt
    public GameObject audioSettingsPopup; // Popup điều chỉnh âm thanh

    public TMP_Text generatedRoomIdText;
    public TMP_InputField joinRoomIdInput;
    public TMP_Text errorMessageText; // Hiển thị lỗi nếu nhập sai ID
    public Slider loadingSlider;

    private string selectedScene;

    // UI Audio Settings
    public Slider musicVolumeSlider;
    public Toggle muteMusicToggle;

    private void Start()
    {

        int openGameMode = PlayerPrefs.GetInt("OpenGameMode", 0);
        if (openGameMode == 1)
        {
            Debug.Log("Mở panel GameMode từ Result Scene");
            panelPlaySelection.SetActive(true);
            panelGameMode.SetActive(true);
            PlayerPrefs.SetInt("OpenGameMode", 0); // Reset lại
            PlayerPrefs.Save();
        }

        panelPlaySelection.SetActive(false);
        panelGameMode.SetActive(false);

        panelPlay.SetActive(false);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);
        errorMessageText.text = ""; // Ẩn lỗi khi bắt đầu

        
        audioSettingsPopup.SetActive(false); // Ẩn popup âm thanh ban đầu
        // Load âm lượng từ PlayerPrefs
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        bool isMusicMuted = PlayerPrefs.GetInt("MuteMusic", 0) == 1;

        musicVolumeSlider.value = musicVolume;
        muteMusicToggle.isOn = isMusicMuted;

        ApplyAudioSettings();

    }
    
    public void OpenPlaySelectionPanel()
    {
        panelPlaySelection.SetActive(true);
        panelGameMode.SetActive(false);
        panelPlay.SetActive(false);
    }


    public void OpenPlayPanel()
    {
        panelPlaySelection.SetActive(false);
        panelGameMode.SetActive(false);
        panelPlay.SetActive(false);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);

        // Bật Loading panel
        panelLoading.SetActive(true);

        // Tiến hành tải scene "SampleScene"
        StartCoroutine(LoadSceneWithProgress("SampleScene"));
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
        panelPlaySelection.SetActive(false);
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
            errorMessageText.text = "ID is not correct!";
        }
    }



    public void StartSinglePlayer(string sceneName)
    {
        selectedScene = sceneName;

        panelGameMode.SetActive(false);
        panelLoading.SetActive(true);
        StartCoroutine(LoadSceneWithProgress(selectedScene));
    }

    private IEnumerator LoadSceneWithProgress(string sceneName)
    {
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void BackToPlayPanel()
    {
        panelPlay.SetActive(true);
        panelHost.SetActive(false);
        panelJoin.SetActive(false);
        errorMessageText.text = ""; // Ẩn lỗi khi quay lại
    }

    public void OpenMyNFS()
    {
        CloseAllPanels();
        myNFSPanel.SetActive(true);
        Debug.Log("Mở My NFS");
    }

    // Mở News (Tin tức game)
    public void OpenNews()
    {
        CloseAllPanels();
        newsPanel.SetActive(true);
        Debug.Log("Mở News");
    }

    // Mở Settings (Cài đặt)
    public void OpenSettings()
    {
        CloseAllPanels();
        audioSettingsPopup.SetActive(true); // Chỉ mở popup âm thanh
        Debug.Log("Mở popup Audio Settings");
    }
    public void CloseAudioSettings()
    {
        audioSettingsPopup.SetActive(false);
        Debug.Log("Đóng popup Audio Settings");
    }

    public void ApplyAudioSettings()
    {
        float musicVolume = muteMusicToggle.isOn ? 0f : musicVolumeSlider.value;

        AudioListener.volume = musicVolume;

        // Lưu cài đặt
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetInt("MuteMusic", muteMusicToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Đóng tất cả panel để tránh chồng lên nhau
    private void CloseAllPanels()
    {
        myNFSPanel.SetActive(false);
        newsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        audioSettingsPopup.SetActive(false);
    }
}