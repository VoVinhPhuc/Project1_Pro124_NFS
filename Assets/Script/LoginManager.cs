using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public Button loginButton;

    private void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        panelSignUp.SetActive(false); // Ẩn Panel SignUp khi mới vào game
        forgetPasswordPanel.SetActive(false);

        passwordInput.onSubmit.AddListener(delegate { Login(); });
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (emailInput.isFocused)
            {
                passwordInput.Select();
            }
        }

        if (emailInput.isFocused) Debug.Log("Email input is focused");
        if (passwordInput.isFocused) Debug.Log("Password input is focused");

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("Enter pressed");

            if (passwordInput.isFocused)
            {
                Debug.Log("Resetting focus and simulating Login button click");
                EventSystem.current.SetSelectedGameObject(null); // Reset UI focus
                loginButton.onClick.Invoke();
            }
        }
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
                PlayerPrefs.SetString("email", email);  // ✅ THÊM DÒNG NÀY
                PlayerPrefs.Save();

                Debug.Log("Đăng nhập thành công! Email: " + loggedInEmail);
                SceneManager.LoadScene("Menu");
                return;
            }
        }
        
        messageText.text = "Username or Password is not correct!";
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
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting..."); // Chỉ hiện khi test trong Editor
    }
}
