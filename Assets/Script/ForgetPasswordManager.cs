using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForgetPasswordManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_Text resultText;
    public GameObject forgetPasswordPanel;
    public Button getPasswordButton; // Nút lấy lại mật khẩu

    private void Start()
    {
        getPasswordButton.onClick.AddListener(CheckEmail);

        emailInput.onSubmit.AddListener(delegate { CheckEmail(); });
    }

    public void CheckEmail()
    {
        string email = emailInput.text.Trim();
        UserList userList = UserDataManager.LoadUsers();

        foreach (var user in userList.users)
        {
            if (user.email == email)
            {
                resultText.text = "Mật khẩu của bạn là: " + user.password;
                return;
            }
        }

        resultText.text = "Email không tồn tại!";
    }

    public void ClosePanel()
    {
        forgetPasswordPanel.SetActive(false);
    }
}
