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

    private void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
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

        if (!email.EndsWith("@gmail.com"))
        {
            errorText.text = "Email phải có dạng ...@gmail.com!";
            return;
        }

        if (password != confirmPassword)
        {
            errorText.text = "2 mật khẩu không trùng khớp!";
            return;
        }

        UserList userList = UserDataManager.LoadUsers();
        foreach (var user in userList.users)
        {
            if (user.email == email)
            {
                errorText.text = "Email đã tồn tại!";
                return;
            }
        }

        // Nếu không trùng email, thêm người dùng mới
        userList.users.Add(new UserData { email = email, password = password });
        UserDataManager.SaveUsers(userList);

        // Hiển thị thông báo thành công và tự động đóng panel sau 1 giây
        errorText.text = "Đăng ký thành công!";
        StartCoroutine(ClosePanelAfterDelay(1f));
    }

    private IEnumerator ClosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        signUpPanel.SetActive(false);
    }
}
