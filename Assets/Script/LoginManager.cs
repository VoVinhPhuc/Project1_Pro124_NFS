using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    public Toggle passwordToggle;
    public GameObject panelSignUp; // Panel SignUp
    public TMP_Text signUpErrorText;
    public GameObject forgetPasswordPanel; // Panel Forget Password
    public TMP_Text forgetPasswordErrorText;
    public static string loggedInEmail;

    private void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        panelSignUp.SetActive(false); // Ẩn Panel SignUp khi mới vào game
        forgetPasswordPanel.SetActive(false);
    }

    public void TogglePasswordVisibility()
    {
        passwordInput.contentType = passwordToggle.isOn ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
    }

    public void Login()
    {
        UserList userList = UserDataManager.LoadUsers();
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        foreach (var user in userList.users)
        {
            if (user.email == email && user.password == password)
            {
                loggedInEmail = email;
                Debug.Log("Đăng nhập thành công! Email: " + loggedInEmail);
                SceneManager.LoadScene("Menu");
                return;
            }
        }

        messageText.text = "Email hoặc mật khẩu không đúng!";
    }

    public void OpenSignUpPanel()
    {
        panelSignUp.SetActive(true); // Hiển thị panel SignUp khi ấn nút
        signUpErrorText.text = "";
    }
    public void OpenForgetPasswordPanel()
    {
        forgetPasswordPanel.SetActive(true);
        forgetPasswordErrorText.text = ""; // Reset lỗi cũ
    }
}
