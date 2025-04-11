using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class SignUpManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Text errorText;
    public GameObject signUpPanel; // Panel SignUp để ẩn đi sau khi đăng ký
    public Toggle passwordToggle;
    public Button backButton;

    private void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;

        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseSignUpPanel);
        }

        confirmPasswordInput.onSubmit.AddListener(delegate { SignUp(); });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (emailInput.isFocused)
            {
                passwordInput.Select(); // Chuyển focus từ Email → Password
            }
            else if (passwordInput.isFocused)
            {
                confirmPasswordInput.Select(); // Chuyển focus từ Password → Confirm Password
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (confirmPasswordInput.isFocused)
            {
                SignUp(); // Khi đang ở ô Confirm Password và nhấn Enter, thực hiện đăng ký
            }
        }
    }
    public void TogglePasswordVisibility()
    {
        bool showPassword = passwordToggle.isOn;
        passwordInput.contentType = showPassword ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = showPassword ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        confirmPasswordInput.ForceLabelUpdate();
    }

    public void SignUp()
    {
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Xóa thông báo cũ trước khi kiểm tra
        errorText.text = "";

        if (!(email.EndsWith("@gmail.com") || email.EndsWith("@fpt.edu.vn")))
        {
            errorText.text = "Email must have @gmail.com or @fpt.edu.vn!";
            return;
        }

        if (password != confirmPassword)
        {
            errorText.text = "Passwords didnt match!";
            return;
        }

        UserList userList = UserDataManager.LoadUsers();
        foreach (var user in userList.users)
        {
            if (user.email == email)
            {
                errorText.text = "Email existed !";
                return;
            }
        }

        // Nếu không trùng email, thêm người dùng mới
        userList.users.Add(new UserData { email = email, password = password });
        UserDataManager.SaveUsers(userList);

        // Hiển thị thông báo thành công và tự động đóng panel sau 1 giây
        errorText.text = "SUCCESS!";
        StartCoroutine(ClosePanelAfterDelay(1f));
    }

    private IEnumerator ClosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        signUpPanel.SetActive(false);
    }
    public void CloseSignUpPanel()
    {
        signUpPanel.SetActive(false);
    }
}
