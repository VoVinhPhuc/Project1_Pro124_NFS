using UnityEngine;
using UnityEngine.Video;

public class VideoAutoHide : MonoBehaviour
{
    public GameObject videoCanvasObject;     // Canvas chứa Video + RawImage
    public GameObject trafficLightPanel;     // Panel chứa đèn tín hiệu

    private VideoPlayer videoPlayer;

    void Start()
    {
        // Dừng toàn bộ gameplay
        Time.timeScale = 0f;

        // Đảm bảo traffic light bị ẩn khi bắt đầu
        if (trafficLightPanel != null)
            trafficLightPanel.SetActive(false);

        // Lấy VideoPlayer
        videoPlayer = videoCanvasObject.GetComponentInChildren<VideoPlayer>();

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Tắt canvas video
        videoCanvasObject.SetActive(false);

        // Hiện panel đèn giao thông
        if (trafficLightPanel != null)
            trafficLightPanel.SetActive(true);

        // Resume gameplay
        Time.timeScale = 1f;
    }
}
