using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultUIManager : MonoBehaviour
{
    private string lastGameScene;

    private void Start()
    {
        // Lấy tên scene trước đó (nếu có)
        lastGameScene = PlayerPrefs.GetString("LastGameScene", "Map1"); // Mặc định là Map1
    }

    // Nút Back To Menu
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Nút Replay (chơi lại scene trước đó)
    public void ReplayLastGame()
    {
        SceneManager.LoadScene(lastGameScene);
    }

    // Nút Back To GameMode (quay lại menu nhưng mở sẵn panel gamemode)
    public void BackToGameMode()
    {
        PlayerPrefs.SetInt("OpenGameMode", 1); // Lưu trạng thái để mở panel
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
}
