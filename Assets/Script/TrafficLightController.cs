using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public GameObject trafficLightPanel;
    private CanvasGroup canvasGroup;

    // Biến static để các xe kiểm tra có được chạy chưa
    public static bool canStartRace = false;

    void Start()
    {
        canStartRace = false; // reset khi bắt đầu scene
        canvasGroup = trafficLightPanel.GetComponent<CanvasGroup>();
    }

    // Gọi từ Animation Event ở frame cuối đèn tín hiệu
    public void OnLightFinish()
    {
        StartCoroutine(FadeOutAndStartRace());
    }

    IEnumerator FadeOutAndStartRace()
    {
        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        trafficLightPanel.SetActive(false);

        // Cho phép xe bắt đầu chạy
        canStartRace = true;
        GameManager.Instance.StartCountdownTimer();
    }
}
