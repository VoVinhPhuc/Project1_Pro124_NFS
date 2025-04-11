using TMPro;
using UnityEngine;

public class NickNameDisplay : MonoBehaviour
{
    public TextMeshProUGUI nickNameText;
    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;

        // Lấy NickName từ PlayerPrefs
        string nickName = PlayerPrefs.GetString("NickName", "Player");
        nickNameText.text = nickName;
    }

    void LateUpdate()
    {
        // Luôn nhìn về phía camera
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position);
    }
}
