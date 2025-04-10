using UnityEngine;

public class CountdownLED : MonoBehaviour
{
    void Start()
    {
        Invoke("HideCountdown", 5.5f); // Ẩn sau 5 giây
    }

    void HideCountdown()
    {
        gameObject.SetActive(false);
    }
}
