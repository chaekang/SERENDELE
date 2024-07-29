using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    [Header("Log in")]
    [SerializeField] GameObject LoginPanel;
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passField;
    [SerializeField] Button loginBtn;
    [SerializeField] GameObject FailLog;
    [SerializeField] Button RegisterTxt;

    [Header("Register")]
    [SerializeField] GameObject RegisterPanel;
    [SerializeField] TMP_InputField RegisterEmailField;
    [SerializeField] Button RegisterEmailCheckBtn;
    [SerializeField] TMP_Text RegisterEmailCheckTxt;
    [SerializeField] TMP_InputField RegisterPassField;
    [SerializeField] TMP_InputField RegisterPassCheckField;
    [SerializeField] TMP_Text RegisterPassCheckTxt;
    [SerializeField] TMP_InputField NicknameField;
    [SerializeField] Button RegisterBtn;
    [SerializeField] GameObject RegisterToLogin;

    // 인증을 관리할 객체
    private FirebaseAuth auth;

    private bool showFailLogCoroutine;

    private FirestoreManager firestoreManager;

    void Awake()
    {
        // 비밀번호 안 보이게 처리
        passField.contentType = TMP_InputField.ContentType.Password;
        RegisterPassField.contentType = TMP_InputField.ContentType.Password;
        RegisterPassCheckField.contentType = TMP_InputField.ContentType.Password;
    }

    void Start()
    {
        firestoreManager = FindObjectOfType<FirestoreManager>();

        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError("Firebase: Could not resolve all Firebase dependencies: " + task.Result);
            }
        });

        // 비밀번호 입력하는 거 체크
        RegisterPassField.onValueChanged.AddListener(delegate { CheckPasswords(); });
        RegisterPassCheckField.onValueChanged.AddListener(delegate { CheckPasswords(); });

        RegisterEmailCheckTxt.text = string.Empty;
        RegisterPassCheckTxt.text = string.Empty;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HandleTabPress();
        }

        if (showFailLogCoroutine)
        {
            showFailLogCoroutine = false;
            StartCoroutine(ShowFailLog());
        }
    }

    // 탭키 누르면 칸 내려감
    void HandleTabPress()
    {
        if (emailField.isFocused)
        {
            EventSystem.current.SetSelectedGameObject(passField.gameObject);
        }
        else if (passField.isFocused)
        {
            EventSystem.current.SetSelectedGameObject(loginBtn.gameObject);
        }
    }

    public void Login()
    {
        // 이메일과 비밀번호로 로그인 시켜 줌
        auth.SignInWithEmailAndPasswordAsync(emailField.text, passField.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                FirebaseUser user = task.Result.User; // Get the FirebaseUser object from the task result
                string nickname = user.DisplayName;
                Debug.Log($"{user.Email} 로 로그인 하셨습니다. 닉네임: {nickname}");

                // Update UI
                NicknameField.text = nickname;

                // 기본데이터 입력
                firestoreManager.CheckAndSaveDefaultData(user.UserId);
            }
            else
            {
                Debug.Log("로그인에 실패하셨습니다.");
                showFailLogCoroutine = true;
            }
        });
    }

    private IEnumerator ShowFailLog()
    {
        FailLog.SetActive(true);
        // 상호작용 비활성화
        emailField.interactable = false;
        passField.interactable = false;
        loginBtn.interactable = false;
        RegisterTxt.interactable = false;

        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        FailLog.SetActive(false);
        // 상호작용 활성화
        emailField.interactable = true;
        passField.interactable = true;
        loginBtn.interactable = true;
        RegisterTxt.interactable = true;
    }

    public void GoToRegister()
    {
        LoginPanel.SetActive(false);
        emailField.text = string.Empty;
        passField.text = string.Empty;
        RegisterPanel.SetActive(true);
    }

    public void Register()
    {
        string email = RegisterEmailField.text;
        string password = RegisterPassField.text;
        string nickname = NicknameField.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("회원가입 실패: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;  // AuthResult에서 FirebaseUser 가져오기
            UpdateUserProfile(newUser, nickname);
        });
    }

    private void UpdateUserProfile(FirebaseUser user, string nickname)
    {
        UserProfile profile = new UserProfile { DisplayName = nickname };

        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("프로필 업데이트 실패: " + task.Exception);
                return;
            }

            Debug.Log("회원가입 및 프로필 업데이트 성공!");
            StartCoroutine(ShowRegisterToLogin());
        });
    }

    private IEnumerator ShowRegisterToLogin()
    {
        RegisterPanel.SetActive(false);
        RegisterToLogin.SetActive(true);

        yield return new WaitForSeconds(2);

        RegisterToLogin.SetActive(false);
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);

        RegisterEmailField.text = string.Empty;
        RegisterPassField.text = string.Empty;
        RegisterPassCheckField.text = string.Empty;
        NicknameField.text = string.Empty;
    }

    private void CheckPasswords()
    {
        if (RegisterPassField.text == RegisterPassCheckField.text)
        {
            RegisterPassCheckTxt.text = "비밀번호가 일치합니다";
            RegisterPassCheckTxt.color = Color.green;
        }
        else
        {
            RegisterPassCheckTxt.text = "비밀번호가 일치하지 않습니다";
            RegisterPassCheckTxt.color = Color.red;
        }
    }
}
