using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class VideoPlayerUI : MonoBehaviour
{
    public RawImage rawImage;       // UI RawImage để hiển thị video
    public VideoPlayer videoPlayer; // Component VideoPlayer
    public string videoFileName = "car red 14s.mp4"; // Tên file video trong StreamingAssets

    private void Start()
    {
        if (videoPlayer == null) videoPlayer = gameObject.AddComponent<VideoPlayer>();

        // Kiểm tra nếu RawImage chưa được gán
        if (rawImage == null)
        {
            Debug.LogError("RawImage chưa được gán trong Inspector!");
            return;
        }

        // Kiểm tra xem file video có tồn tại không
        string videoPath = Path.Combine(Application.streamingAssetsPath, videoFileName);
        if (!File.Exists(videoPath))
        {
            Debug.LogError("Không tìm thấy video tại: " + videoPath);
            return;
        }

        // Tạo Render Texture nếu chưa có
        if (videoPlayer.targetTexture == null)
        {
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 16);
            videoPlayer.targetTexture = renderTexture;
            rawImage.texture = renderTexture;
        }

        // Cấu hình VideoPlayer
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        videoPlayer.isLooping = true;

        // Chuẩn bị video và phát khi sẵn sàng
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (VideoPlayer vp) =>
        {
            Debug.Log("Video đã sẵn sàng, bắt đầu phát...");
            vp.Play();
        };
    }
}
