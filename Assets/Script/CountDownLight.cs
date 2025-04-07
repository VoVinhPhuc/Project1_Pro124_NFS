using UnityEngine;

public class CountDownLight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("HideCountdown", 4f); // Ẩn sau 5 giây
    }

    void HideCountdown()
    {
        gameObject.SetActive(false);
    }
}
