using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayfabLogin : MonoBehaviour
{
    private PlayfabManager _playfabManager;
    [SerializeField] private InputField _emailInput;
    [SerializeField] private InputField _passwordInput;
    [SerializeField] private InputField _emailToResetPasswordInput;
    [SerializeField] private GameObject _loadingBg;
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _changePasswordPanel;
    [SerializeField] private GameObject _warningPanel;
    [SerializeField] private GameObject _changeSavePanel;
    [SerializeField] private Text _onlineSaveText, _offlineSaveText;
    private bool _isOutdatedVersion;
    private int _numOfError;

    private void Start()
    {
        _emailInput.text = PlayerPrefs.GetString("playerEmail");
        _passwordInput.text = PlayerPrefs.GetString("playerPassword");
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        AutoLogin();
    }

    public void AutoLogin()
    {
        if (PlayerPrefs.HasKey("playerEmail") & PlayerPrefs.HasKey("playerPassword") & !_playfabManager.isLogged)
        {
            if (PlayerPrefs.GetString("lastPlayerEmail") == "guest")
            {
                OnPlayAsGuestButtonClicked();
            }
            else
            {
                OnLoginButtonClicked();
            }
        }
    }

    public void CreateWarningPanel(bool isOutdatedVersion, int numOfError)
    {
        _isOutdatedVersion = isOutdatedVersion;
        _numOfError = numOfError;
        StopLoading();
        _warningPanel.SetActive(true);
        Text[] texts = _warningPanel.GetComponentsInChildren<Text>();
        if (isOutdatedVersion && numOfError == 0)
        {
            texts[0].text = "Вышла новая версия игры. Пожалуйста, обновите приложение.";
            texts[1].text = "Обновить";
            PlayerPrefsSafe.SetBool("isOutdatedVersion", isOutdatedVersion);
        }
        else if (isOutdatedVersion && numOfError == 1)
        {
            texts[0].text = "На сервере проводятся технические работы. Пожалуйста, подождите.";
            texts[1].text = "Ок";
            PlayerPrefsSafe.SetBool("technicalWorks", true);
        }
        else
        {
            texts[0].text = "Вы не были онлайн 10 дней или более. Чтобы зайти, вам нужно подключиться к своему аккаунту.";
            texts[1].text = "Подключиться";
        }
        _loginPanel.SetActive(false);
    }

    public void OnRegisterButtonClicked()
    {
        SetDataToAutoLogin();
        _playfabManager.Register(_emailInput.text, _passwordInput.text);
        StartLoading();
    }

    public void SetDataToAutoLogin()
    {
        PlayerPrefs.SetString("playerEmail", _emailInput.text);
        if (_emailInput.text != "guest")
            PlayerPrefs.SetString("playerPassword", _passwordInput.text);
        else
            PlayerPrefs.SetString("playerPassword", "");
    }

    public void OnLoginButtonClicked()
    {
        SetDataToAutoLogin();
        if (_emailInput.text == "guest")
            _playfabManager.LoginAsGuest();
        else
            _playfabManager.Login(_emailInput.text, _passwordInput.text);
        StartLoading();
    }

    public void OnWarningButtonClicked()
    {
        if (_isOutdatedVersion && _numOfError == 0)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.SnakeAppsStudio.WhistleofWings");
        }
        else if (_isOutdatedVersion && _numOfError == 1)
        {
            _playfabManager.isLogged = false;
            _playfabManager.Logout();
            SceneManager.LoadScene(1);
        }
        else
        {
            if (_emailInput.text == "guest")
                _playfabManager.LoginAsGuest();
            else
                _playfabManager.Login(_emailInput.text, _passwordInput.text);
            StartLoading();
        }
    }

    public void CreateResetPasswordPanel()
    {
        _changePasswordPanel.SetActive(true);
        _loginPanel.SetActive(false);
    }

    public void CreateChangeSavePanel()
    {
        StopLoading();
        _onlineSaveText.text = "Ваши данные были сохранены на сервере " + PlayerPrefs.GetString("lastSaveTime") + "\nХотите ли вы использовать это сохранение?";
        _offlineSaveText.text = "Ваши данные были сохранены локально " + PlayerPrefs.GetString("lastOfflineSave") + "\nХотите ли вы использовать это сохранение?";
        _changeSavePanel.SetActive(true);
        _loginPanel.SetActive(false);
    }

    public void OnOnlineSaveButtonClicked()
    {
        StartLoading();
        _changeSavePanel.SetActive(false);
        _playfabManager.ClearData();
    }

    public void OnOfflineSaveButtonClicked()
    {
        StartLoading();
        _changeSavePanel.SetActive(false);
        SceneManager.LoadScene(1);
    }

    public void OnResetPasswordButtonClicked()
    {
        _playfabManager.ResetPassword(_emailToResetPasswordInput.text);
        _changePasswordPanel.SetActive(false);
        StartLoading();
    }

    public void OnPlayAsGuestButtonClicked()
    {
        _emailInput.text = "guest";
        _passwordInput.text = "";
        _playfabManager.LoginAsGuest();
        StartLoading();
    }

    public void StartLoading()
    {
        _warningPanel.SetActive(false);
        _loadingBg.SetActive(true);
        _loginPanel.SetActive(false);
    }

    public void StopLoading()
    {
        _loadingBg.SetActive(false);
        _loginPanel.SetActive(true);
    }

    public void SaveLastEmail()
    {
        PlayerPrefs.SetString("lastPlayerEmail", _emailInput.text);
    }

    public bool LoginInAnotherAccount()
    {
        if (PlayerPrefs.GetString("playerEmail") == PlayerPrefs.GetString("lastPlayerEmail"))
            return false;
        else
            return true;
    }
}