using UnityEngine;
using TMPro;

public class NickNameManager : MonoBehaviour
{
    public TMP_InputField nickNameInput;
    public TMP_Text messageText;
    public GameObject nickNamePanel; // Panel nhập NickName

    private void Start()
    {
        // Kiểm tra nếu chưa đăng nhập thì không làm gì
        if (string.IsNullOrEmpty(LoginManager.loggedInEmail))
        {
            Debug.LogWarning("Không có email đăng nhập, không hiển thị Panel NickName.");
            return;
        }

        UserList userList = UserDataManager.LoadUsers();

        foreach (var user in userList.users)
        {
            if (user.email == LoginManager.loggedInEmail)
            {
                if (string.IsNullOrEmpty(user.nickName))
                {
                    // Nếu chưa có NickName, hiện Panel
                    Debug.Log("Người dùng chưa có NickName. Hiển thị Panel.");
                    nickNamePanel.SetActive(true);
                }
                else
                {
                    // Nếu đã có NickName, ẩn Panel
                    Debug.Log("Người dùng đã có NickName: " + user.nickName);
                    nickNamePanel.SetActive(false);
                }
                return;
            }
        }

        // Nếu không tìm thấy tài khoản, Panel vẫn bị ẩn
        Debug.LogWarning("Không tìm thấy email trong danh sách người dùng.");
        nickNamePanel.SetActive(false);

        nickNameInput.onSubmit.AddListener(delegate { SaveNickName(); });
    }

    public void SaveNickName()
    {
        string nickName = nickNameInput.text.Trim();

        if (string.IsNullOrEmpty(nickName))
        {
            messageText.text = "NickName can not be null!";
            return;
        }

        if (UserDataManager.IsNickNameTaken(nickName))
        {
            messageText.text = "NickName existed!";
            return;
        }

        // Lưu NickName vào UserData
        UserDataManager.UpdateNickName(LoginManager.loggedInEmail, nickName);

        PlayerPrefs.SetString("NickName", nickName);
        PlayerPrefs.Save(); // Đảm bảo giá trị được lưu ngay lập tức
        Debug.Log("NickName đã lưu: " + nickName);

        messageText.text = "NickName saved!";
        nickNamePanel.SetActive(false); // Ẩn panel sau khi lưu

        GameManager.Instance.SetPlayerInfo(nickName, LoginManager.loggedInEmail);
    }
}
