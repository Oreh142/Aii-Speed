using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour
{
    private Menu _menu;
    private PlayfabLogin _playfabLogin;
    [SerializeField] private Text _statusText;
    public bool isLogged;
    private DateTime lastCheckTime;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoginAsGuest()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        _statusText = GameObject.Find("StatusText").GetComponent<Text>();
        _statusText.text = "Вход произведён успешно!";
        PlayerPrefsSafe.SetBool("isOutdatedVersion", false);
        _playfabLogin = GameObject.Find("playfabLogin").GetComponent<PlayfabLogin>();
        _playfabLogin.SaveLastEmail();
        GetTitleData();
    }

    private void OnError(PlayFabError error)
    {
        if (GameObject.Find("playfabLogin"))
        {
            _playfabLogin = GameObject.Find("playfabLogin").GetComponent<PlayfabLogin>();
            _playfabLogin.StopLoading();
            _statusText = GameObject.Find("StatusText").GetComponent<Text>();
            if (_statusText)
            {
                _statusText.text = (error.ErrorMessage);
                if (_statusText.text == "Cannot resolve destination host" ^ _statusText.text == "Failed to receive data")
                {
                    isLogged = false;
                    TimeSpan ts = DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("lastLoginTime"));
                    if (ts.TotalDays <= 10)
                    {
                        if (!PlayerPrefsSafe.GetBool("isOutdatedVersion"))
                        {
                            SceneManager.LoadScene(1);
                        }
                        else
                        {
                            _playfabLogin.CreateWarningPanel(true, 0);
                        }
                    }
                    else
                    {
                        _playfabLogin.CreateWarningPanel(false, 0);
                    }
                }
            }
        }
        else if (GameObject.Find("MenuManager") && (error.ErrorMessage == "DataUpdateRateExceeded" ^ error.ErrorMessage == "The client has exceeded the maximum API request rate and is being throttled"))
        {
            if (isLogged)
            {
                isLogged = false;
                _menu.StartOnlineLoading();
            }
        }
        else if (error.ErrorMessage == "PlayFabException: Must be logged in to call this method" && GameObject.Find("MenuManager"))
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            isLogged = false;
            Debug.Log(error.ErrorMessage);
        }
    }

    public void SaveData(string dataName)
    {
        if (GameObject.Find("MenuManager"))
        {
            CheckDate();
            _menu = GameObject.Find("MenuManager").GetComponent<Menu>();
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                {dataName, JsonUtility.ToJson(_menu.ReturnClass())}
                }
            };
            PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        }
    }

    public void SaveBackups()
    {
        if (isLogged)
        {
            for (int i = 0; i < 4; i++)
            {
                string s = PlayerPrefs.GetString("backup" + i);
                if (s == "")
                {
                    s = DateTime.Now.ToString();
                    PlayerPrefs.SetString("backup" + i, s);
                }
            }
            TimeSpan ts = DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("backup0"));
            if ((int)ts.TotalDays / 1 >= 1)
            {
                Debug.Log(ts.TotalDays + " day");
                PlayerPrefs.SetString("backup0", DateTime.Now.ToString());
                SaveData("YesterdayData");
            }
            ts = DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("backup1"));
            if ((int)ts.TotalDays / 2 >= 1)
            {
                Debug.Log(ts.TotalDays + " yesterday");
                PlayerPrefs.SetString("backup1", DateTime.Now.ToString());
                SaveData("BeforeYesterdayData");
            }
            ts = DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("backup2"));
            if ((int)ts.TotalDays / 10 >= 1)
            {
                Debug.Log(ts.TotalDays + " 10daysAgo");
                PlayerPrefs.SetString("backup2", DateTime.Now.ToString());
                SaveData("10DaysAgoData");
            }
            ts = DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("backup3"));
            if ((int)ts.TotalDays / 30 >= 1)
            {
                Debug.Log(ts.TotalDays + " 30daysAgo");
                PlayerPrefs.SetString("backup3", DateTime.Now.ToString());
                SaveData("30DaysAgoData");
            }
        }
    }

    public void CheckDate()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "CheckDate"
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteSuccess, OnError);
    }

    private void OnExecuteSuccess(ExecuteCloudScriptResult result)
    {
        lastCheckTime = Convert.ToDateTime(result.FunctionResult);
        TimeSpan ts = DateTime.Now - lastCheckTime;
        if (ts.TotalSeconds > 300 ^ ts.TotalSeconds < -300 ^ Application.internetReachability == NetworkReachability.NotReachable)
        {
            isLogged = false;
        }
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        if (!isLogged)
        {
            isLogged = true;
        }
        _menu.ServerAnswer();
        Debug.Log("Data send!");
    }

    private void GetData(string typeData)
    {
        if (typeData == "all")
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
        }
        else if (typeData == "time")
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnTimeDataRecieved, OnError);
        }
    }

    private void OnDataRecieved(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Data"))
        {
            LoadData(JsonUtility.FromJson<Data>(result.Data["Data"].Value));
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
        StartCoroutine(SaveDataCoroutine());
    }

    private void OnTimeDataRecieved(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Data"))
        {
            LoadLastSaveTime(JsonUtility.FromJson<Data>(result.Data["Data"].Value));
        }
        else
        {
            ClearData();
        }
    }

    private IEnumerator SaveDataCoroutine()
    {
        yield return new WaitForSeconds(13);
        StartCoroutine(SaveDataCoroutine());
        SaveData("Data");
    }

    public void Register(string email, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
        isLogged = true;
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        ClearData();
        PlayerPrefs.SetString("playerRegisterDate", DateTime.Now.ToString());
        PlayerPrefsSafe.SetInt("numberOfNewYear", (FormatNumsHelper.DayOfTheYear() / 365) + 1);
        Debug.Log(PlayerPrefsSafe.GetInt("numberOfNewYear"));
        _statusText = GameObject.Find("StatusText").GetComponent<Text>();
        _statusText.text = "Регистрация прошла успешно!";
        Login(PlayerPrefs.GetString("playerEmail"), PlayerPrefs.GetString("playerPassword"));
    }

    public void Login(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void ResetPassword(string email)
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = "B26D9"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    private void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        _statusText = GameObject.Find("StatusText").GetComponent<Text>();
        _statusText.text = "Письмо отправлено на почту!";
        _playfabLogin.StopLoading();
    }

    public void GetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnTitleDataRecieved, OnError);
    }

    private void OnTitleDataRecieved(GetTitleDataResult result)
    {
        _playfabLogin = GameObject.Find("playfabLogin").GetComponent<PlayfabLogin>();
        CheckDate();
        int currentVersion = int.Parse(result.Data["Version"]);
        int projectVersion = 15;
        if (result.Data == null | result.Data.ContainsKey("Version") == false)
        {
            return;
        }
        if (projectVersion >= currentVersion && currentVersion != 228666)
        {
            isLogged = true;
            GetData("time");
        }
        else if (currentVersion == 228666)
        {
            _playfabLogin.CreateWarningPanel(true, 1);
        }
        else
        {
            _playfabLogin.CreateWarningPanel(true, 0);
        }
    }

    public void ClearData()
    {
        PlayerPrefsSafe.SetFloat("money", 0);
        PlayerPrefsSafe.SetFloat("record", 0);
        PlayerPrefsSafe.SetInt("idPlayerPlane", 0);
        for (int i = 1; i < 28; i++)
        {
            PlayerPrefsSafe.SetBool("isPlaneBuyed" + i, false);
        }
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefsSafe.SetInt("dayOpened" + i, 0);
            PlayerPrefsSafe.SetInt("recievedReward" + i, 0);
        }
        PlayerPrefs.SetString("lastOpening", "01.01.2000 00:00:00");
        PlayerPrefs.SetString("lastShopReloading", "01.01.2000 00:00:00");
        PlayerPrefsSafe.SetFloat("totalTimePlayer", 0);
        PlayerPrefsSafe.SetFloat("totalScorePlayer", 0);
        PlayerPrefsSafe.SetFloat("totalSpeedPlayer", 0);
        PlayerPrefsSafe.SetFloat("totalGamesPlayer", 0);
        PlayerPrefsSafe.SetFloat("totalDistancePlayer", 0);
        PlayerPrefsSafe.SetFloat("topDistancePlayer", 0);
        PlayerPrefsSafe.SetFloat("totalMoneyPlayer", 0);
        PlayerPrefsSafe.SetFloat("tasksTotalMoney", 0);
        PlayerPrefsSafe.SetFloat("containerTotalMoney", 0);
        for (int i = 0; i < 28; i++)
        {
            PlayerPrefsSafe.SetFloat("totalTime" + i, 0);
            PlayerPrefsSafe.SetFloat("totalScore" + i, 0);
            PlayerPrefsSafe.SetFloat("topScore" + i, 0);
            PlayerPrefsSafe.SetFloat("totalSpeed" + i, 0);
            PlayerPrefsSafe.SetFloat("totalGames" + i, 0);
            PlayerPrefsSafe.SetFloat("totalDistance" + i, 0);
            PlayerPrefsSafe.SetFloat("topDistance" + i, 0);
            PlayerPrefsSafe.SetFloat("totalMoney" + i, 0);
        }
        PlayerPrefs.SetString("lastDayTasks", "01.01.2000 00:00:00");
        PlayerPrefsSafe.SetBool("isDayTasksReloaded", false);
        PlayerPrefsSafe.SetBool("isDayTasksCompleted", false);
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefsSafe.SetInt("dayTaskId" + i, 0);
            PlayerPrefsSafe.SetFloat("dayTask" + i, 0);
            PlayerPrefsSafe.SetFloat("dayReward" + i, 0);
            PlayerPrefsSafe.SetFloat("completedDayTask" + i, 0);
            PlayerPrefsSafe.SetBool("isDayTaskCompleted" + i, false);
        }
        PlayerPrefsSafe.SetBool("isDayTaskCompleted-1", false);
        PlayerPrefs.SetString("lastWeekTasks", "01.01.2000 00:00:00");
        PlayerPrefsSafe.SetBool("isWeekTasksReloaded", false);
        PlayerPrefsSafe.SetBool("isWeekTaskCompleted-2", false);
        PlayerPrefsSafe.SetBool("isAllTasksCompleted", false);
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefsSafe.SetInt("weekTaskId" + i, 0);
            PlayerPrefsSafe.SetFloat("weekTask" + i, 0);
            PlayerPrefsSafe.SetFloat("weekReward" + i, 0);
            PlayerPrefsSafe.SetFloat("completedWeekTask" + i, 0);
            PlayerPrefsSafe.SetBool("isWeekTaskCompleted" + i, false);
        }
        PlayerPrefs.SetString("ArsenAnimeshnik", "yes");
        PlayerPrefs.SetString("lastLoginTime", DateTime.Now.ToString());
        PlayerPrefs.SetString("playerRegisterDate", "01.01.2000 00:00:00");
        PlayerPrefs.SetString("lastSaveTime", "01.01.2000 00:00:00");
        PlayerPrefs.SetString("backup0", "01.01.2000 00:00:00");
        PlayerPrefs.SetString("backup1", "01.01.2000 00:00:00");
        PlayerPrefs.SetString("backup2", "01.01.2000 00:00:00");
        PlayerPrefs.SetString("backup3", "01.01.2000 00:00:00");
        PlayerPrefsSafe.SetBool("isRecordingStatistics", true);
        PlayerPrefsSafe.SetInt("numberOfNewYear", 0);
        PlayerPrefs.SetString("lastDayTrials", "01.01.2000 00:00:00");
        PlayerPrefsSafe.SetBool("isDayTrialReloaded", false);
        PlayerPrefsSafe.SetInt("trialId", 0);
        PlayerPrefsSafe.SetInt("trialCoupons", 0);
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefsSafe.SetInt("dayTrialId" + i, 0);
            PlayerPrefsSafe.SetFloat("dayTrial" + i, 0);
            PlayerPrefsSafe.SetFloat("dayTrialReward" + i, 0);
            PlayerPrefsSafe.SetFloat("completedDayTrial" + i, 0);
            PlayerPrefsSafe.SetBool("isDayTrialCompleted" + i, false);
            PlayerPrefsSafe.SetInt("dayTrialPlaneId" + i, 0);
        }
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefsSafe.SetInt("generalTrialId" + i, 0);
            PlayerPrefsSafe.SetFloat("generalTrialTask" + i, 0);
            PlayerPrefsSafe.SetFloat("generalTrialReward" + i, 0);
            PlayerPrefsSafe.SetInt("generalTrialPlaneId" + i, 0);
            PlayerPrefsSafe.SetFloat("completedGeneralTrial" + i, 0);
            PlayerPrefsSafe.SetBool("isGeneralTrialCompleted" + i, false);
        }
        PlayerPrefsSafe.SetInt("commonRadnomShopPlaneId", 0);
        PlayerPrefsSafe.SetInt("rareRadnomShopPlaneId", 0);
        GetData("all");
    }

    private void LoadLastSaveTime(Data data)
    {
        PlayerPrefs.SetString("lastSaveTime", data.lastSaveTime);
        if (PlayerPrefs.HasKey("lastOfflineSave") & PlayerPrefs.GetString("lastEmailLogin") == PlayerPrefs.GetString("playerEmail"))
        {
            TimeSpan ts = Convert.ToDateTime(PlayerPrefs.GetString("lastOfflineSave")) - DateTime.Now;
            if (ts.TotalMinutes > 1)
            {
                _playfabLogin.StartLoading();
                ClearData();
            }
            else
            {
                ts = Convert.ToDateTime(PlayerPrefs.GetString("lastOfflineSave")) - Convert.ToDateTime(PlayerPrefs.GetString("lastSaveTime"));
                if (ts.TotalMinutes >= 1 && !_playfabLogin.LoginInAnotherAccount())
                {
                    _playfabLogin.StopLoading();
                    _playfabLogin.CreateChangeSavePanel();
                }
                else
                {
                    _playfabLogin.StartLoading();
                    ClearData();
                }
            }
        }
        else
        {
            _playfabLogin.StartLoading();
            ClearData();
        }
    }

    private void LoadData(Data data)
    {
        PlayerPrefsSafe.SetFloat("money", data.money);
        PlayerPrefsSafe.SetFloat("record", data.record);
        PlayerPrefsSafe.SetInt("idPlayerPlane", data.idPlayerPlane);
        PlayerPrefsSafe.SetBoolArray("isPlaneBuyed", data.isPlaneBuyed);
        for (int i = 0; i < data.dayOpened.Length; i++)
        {
            PlayerPrefsSafe.SetInt("dayOpened" + i, data.dayOpened[i]);
            PlayerPrefsSafe.SetInt("recievedReward" + i, data.recievedReward[i]);
        }
        PlayerPrefs.SetString("lastOpening", data.lastOpening);
        PlayerPrefs.SetString("lastShopReloading", data.lastShopReloading);
        PlayerPrefsSafe.SetFloat("totalTimePlayer", data.totalTimePlayer);
        PlayerPrefsSafe.SetFloat("totalScorePlayer", data.totalScorePlayer);
        PlayerPrefsSafe.SetFloat("totalSpeedPlayer", data.totalSpeedPlayer);
        PlayerPrefsSafe.SetFloat("totalGamesPlayer", data.totalGamesPlayer);
        PlayerPrefsSafe.SetFloat("totalDistancePlayer", data.totalDistancePlayer);
        PlayerPrefsSafe.SetFloat("topDistancePlayer", data.topDistancePlayer);
        PlayerPrefsSafe.SetFloat("totalMoneyPlayer", data.totalMoneyPlayer);
        PlayerPrefsSafe.SetFloat("tasksTotalMoney", data.tasksTotalMoney);
        PlayerPrefsSafe.SetFloat("containerTotalMoney", data.containerTotalMoney);
        for (int i = 0; i < data.isPlaneBuyed.Length; i++)
        {
            PlayerPrefsSafe.SetFloat("totalTime" + i, data.totalTime[i]);
            PlayerPrefsSafe.SetFloat("totalScore" + i, data.totalScore[i]);
            PlayerPrefsSafe.SetFloat("topScore" + i, data.topScore[i]);
            PlayerPrefsSafe.SetFloat("totalSpeed" + i, data.totalSpeed[i]);
            PlayerPrefsSafe.SetFloat("totalGames" + i, data.totalGames[i]);
            PlayerPrefsSafe.SetFloat("totalDistance" + i, data.totalDistance[i]);
            PlayerPrefsSafe.SetFloat("topDistance" + i, data.topDistance[i]);
            PlayerPrefsSafe.SetFloat("totalMoney" + i, data.totalMoney[i]);
        }
        PlayerPrefs.SetString("lastDayTasks", data.lastDayTasks);
        PlayerPrefsSafe.SetBool("isDayTasksReloaded", data.isDayTasksReloaded);
        PlayerPrefsSafe.SetBool("isDayTasksCompleted", data.isDayTasksCompleted);
        for (int i = 0; i < data.dayTaskId.Length; i++)
        {
            PlayerPrefsSafe.SetInt("dayTaskId" + i, data.dayTaskId[i]);
            PlayerPrefsSafe.SetFloat("dayTask" + i, data.dayTask[i]);
            PlayerPrefsSafe.SetFloat("dayReward" + i, data.dayReward[i]);
            PlayerPrefsSafe.SetFloat("completedDayTask" + i, data.completedDayTask[i]);
            PlayerPrefsSafe.SetBool("isDayTaskCompleted" + i, data.isDayTaskCompleted[i]);
        }
        PlayerPrefsSafe.SetBool("isDayTaskCompleted-1", data.isAllDayTasksCompleted);
        PlayerPrefs.SetString("lastWeekTasks", data.lastWeekTasks);
        PlayerPrefsSafe.SetBool("isWeekTasksReloaded", data.isWeekTasksReloaded);
        PlayerPrefsSafe.SetBool("isWeekTaskCompleted-2", data.isWeekTasksCompleted);
        PlayerPrefsSafe.SetBool("isAllTasksCompleted", data.isAllTasksCompleted);
        for (int i = 0; i < data.weekTaskId.Length; i++)
        {
            PlayerPrefsSafe.SetInt("weekTaskId" + i, data.weekTaskId[i]);
            PlayerPrefsSafe.SetFloat("weekTask" + i, data.weekTask[i]);
            PlayerPrefsSafe.SetFloat("weekReward" + i, data.weekReward[i]);
            PlayerPrefsSafe.SetFloat("completedWeekTask" + i, data.completedWeekTask[i]);
            PlayerPrefsSafe.SetBool("isWeekTaskCompleted" + i, data.isWeekTaskCompleted[i]);
        }
        PlayerPrefs.SetString("ArsenAnimeshnik", data.arsenAnimeshnik);
        PlayerPrefsSafe.SetInt("antiCheatTriggers", data.antiCheatTriggers);
        PlayerPrefs.SetString("lastLoginTime", DateTime.Now.ToString());
        PlayerPrefs.SetString("lastEmailLogin", PlayerPrefs.GetString("playerEmail"));
        PlayerPrefs.SetString("playerRegisterDate", data.playerRegisterDate);
        PlayerPrefs.SetString("lastSaveTime", DateTime.Now.ToString());
        PlayerPrefs.SetString("backup0", data.backupYesterday);
        PlayerPrefs.SetString("backup1", data.backupBeforeYesterday);
        PlayerPrefs.SetString("backup2", data.backup10DaysAgo);
        PlayerPrefs.SetString("backup3", data.backup30DaysAgo);
        PlayerPrefsSafe.SetBool("isRecordingStatistics", data.isRecordingStatistics);
        PlayerPrefsSafe.SetInt("numberOfNewYear", data.numberOfNewYear);
        PlayerPrefs.SetString("lastDayTrials", data.lastDayTrials);
        PlayerPrefsSafe.SetBool("isDayTrialReloaded", data.isDayTrialsReloaded);
        PlayerPrefsSafe.SetInt("trialId", data.trialId);
        PlayerPrefsSafe.SetInt("trialCoupons", data.trialCoupons);
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefsSafe.SetInt("dayTrialId" + i, data.dayTrialId[i]);
            PlayerPrefsSafe.SetFloat("dayTrial" + i, data.dayTrial[i]);
            PlayerPrefsSafe.SetFloat("dayTrialReward" + i, data.dayTrialReward[i]);
            PlayerPrefsSafe.SetFloat("completedDayTrial" + i, data.completedDayTrial[i]);
            PlayerPrefsSafe.SetBool("isDayTrialCompleted" + i, data.isDayTrialCompleted[i]);
            PlayerPrefsSafe.SetInt("dayTrialPlaneId" + i, data.dayTrialPlaneId[i]);
        }
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefsSafe.SetInt("generalTrialId" + i, data.generalTrialId[i]);
            PlayerPrefsSafe.SetFloat("generalTrialTask" + i, data.generalTrialTask[i]);
            PlayerPrefsSafe.SetFloat("generalTrialReward" + i, data.generalTrialReward[i]);
            PlayerPrefsSafe.SetInt("generalTrialPlaneId" + i, data.generalTrialPlaneId[i]);
            PlayerPrefsSafe.SetFloat("completedGeneralTrial" + i, data.completedGeneralTrial[i]);
            PlayerPrefsSafe.SetBool("isGeneralTrialCompleted" + i, data.isGeneralTrialCompleted[i]);
        }
        PlayerPrefsSafe.SetInt("commonRadnomShopPlaneId", data.commonRandomShopPlaneId);
        PlayerPrefsSafe.SetInt("rareRadnomShopPlaneId", data.rareRandomShopPlaneId);
        SceneManager.LoadScene(1);
    }
}

public class Data
{
    public float money;
    public float record;
    public int idPlayerPlane;
    public bool[] isPlaneBuyed;
    public int[] dayOpened;
    public int[] recievedReward;
    public string lastOpening;
    public string lastShopReloading;
    public float totalTimePlayer;
    public float totalScorePlayer;
    public float totalSpeedPlayer;
    public float totalGamesPlayer;
    public float totalDistancePlayer;
    public float topDistancePlayer;
    public float totalMoneyPlayer;
    public float tasksTotalMoney;
    public float containerTotalMoney;
    public float[] totalTime;
    public float[] totalScore;
    public float[] topScore;
    public float[] totalSpeed;
    public float[] totalGames;
    public float[] totalDistance;
    public float[] topDistance;
    public float[] totalMoney;
    public string lastDayTasks;
    public bool isDayTasksReloaded;
    public bool isDayTasksCompleted;
    public int[] dayTaskId;
    public float[] dayTask;
    public float[] dayReward;
    public float[] completedDayTask;
    public bool[] isDayTaskCompleted;
    public bool isAllDayTasksCompleted;
    public string lastWeekTasks;
    public bool isWeekTasksReloaded;
    public bool isWeekTasksCompleted;
    public int[] weekTaskId;
    public float[] weekTask;
    public float[] weekReward;
    public float[] completedWeekTask;
    public bool[] isWeekTaskCompleted;
    public bool isAllWeekTasksCompleted;
    public bool isAllTasksCompleted;
    public string arsenAnimeshnik;
    public int antiCheatTriggers; //тест античита
    public string lastLoginTime;
    public string playerRegisterDate;
    public string lastSaveTime;
    public string backupYesterday;
    public string backupBeforeYesterday;
    public string backup10DaysAgo;
    public string backup30DaysAgo;
    public bool isRecordingStatistics;
    public int numberOfNewYear;
    public string lastDayTrials;
    public bool isDayTrialsReloaded;
    public int trialId;
    public int trialCoupons;
    public int[] dayTrialId;
    public float[] dayTrial;
    public float[] dayTrialReward;
    public float[] completedDayTrial;
    public bool[] isDayTrialCompleted;
    public int[] dayTrialPlaneId;
    public bool isGeneralTrialsReloaded;
    public int[] generalTrialId;
    public float[] generalTrialTask;
    public float[] generalTrialReward;
    public float[] completedGeneralTrial;
    public bool[] isGeneralTrialCompleted;
    public int[] generalTrialPlaneId;
    public int commonRandomShopPlaneId;
    public int rareRandomShopPlaneId;

    public Data(float money, float record, int idPlayerPlane, bool[] isPlaneBuyed, int[] dayOpened, int[] recievedReward, string lastOpening, string lastShopReloading, float totalTimePlayer, float totalScorePlayer, float totalSpeedPlayer, float totalGamesPlayer, float totalDistancePlayer, float topDistancePlayer, float totalMoneyPlayer, float tasksTotalMoney, float containerTotalMoney, float[] totalTime, float[] totalScore, float[] topScore, float[] totalSpeed, float[] totalGames, float[] totalDistance, float[] topDistance, float[] totalMoney, string lastDayTasks, bool isDayTasksReloaded, bool isDayTasksCompleted, int[] dayTaskId, float[] dayTask, float[] dayReward, float[] completedDayTask, bool[] isDayTaskCompleted, bool isAllDayTasksCompleted, string lastWeekTasks, bool isWeekTasksReloaded, bool isWeekTasksCompleted, int[] weekTaskId, float[] weekTask, float[] weekReward, float[] completedWeekTask, bool[] isWeekTaskCompleted, bool isAllWeekTasksCompleted, bool isAllTasksCompleted, string arsenAnimeshnik, int antiCheatTriggers, string lastLoginTime, string playerRegisterDate, string lastSaveTime, string backupYesterday, string backupBeforeYesterday, string backup10DaysAgo, string backup30DaysAgo, bool isrecordingStatistics, int numberOfNewYear, string lastDayTrials, bool isDayTrialsReloaded, int trialId, int trialCoupons, int[] dayTrialId, float[] dayTrial, float[] dayTrialReward, float[] completedDayTrial, bool[] isDayTrialCompleted, int[] dayTrialPlaneId, int[] generalTrialId, float[] generalTrialTask, float[] generalTrialReward, float[] completedGeneralTrial, bool[] isGeneralTrialCompleted, int[] generalTrialPlaneId, int commonRandomShopPlaneId, int rareRandomShopPlaneId)
    {
        this.money = money;
        this.record = record;
        this.idPlayerPlane = idPlayerPlane;
        this.isPlaneBuyed = isPlaneBuyed;
        this.dayOpened = dayOpened;
        this.recievedReward = recievedReward;
        this.lastOpening = lastOpening;
        this.lastShopReloading = lastShopReloading;
        this.totalTimePlayer = totalTimePlayer;
        this.totalScorePlayer = totalScorePlayer;
        this.totalSpeedPlayer = totalSpeedPlayer;
        this.totalGamesPlayer = totalGamesPlayer;
        this.totalDistancePlayer = totalDistancePlayer;
        this.topDistancePlayer = topDistancePlayer;
        this.totalMoneyPlayer = totalMoneyPlayer;
        this.tasksTotalMoney = tasksTotalMoney;
        this.containerTotalMoney = containerTotalMoney;
        this.totalTime = totalTime;
        this.totalScore = totalScore;
        this.topScore = topScore;
        this.totalSpeed = totalSpeed;
        this.totalGames = totalGames;
        this.totalDistance = totalDistance;
        this.topDistance = topDistance;
        this.totalMoney = totalMoney;
        this.lastDayTasks = lastDayTasks;
        this.isDayTasksReloaded = isDayTasksReloaded;
        this.isDayTasksCompleted = isDayTasksCompleted;
        this.dayTaskId = dayTaskId;
        this.dayTask = dayTask;
        this.dayReward = dayReward;
        this.completedDayTask = completedDayTask;
        this.isDayTaskCompleted = isDayTaskCompleted;
        this.isAllDayTasksCompleted = isAllDayTasksCompleted;
        this.lastWeekTasks = lastWeekTasks;
        this.isWeekTasksReloaded = isWeekTasksReloaded;
        this.isWeekTasksCompleted = isWeekTasksCompleted;
        this.weekTaskId = weekTaskId;
        this.weekTask = weekTask;
        this.weekReward = weekReward;
        this.completedWeekTask = completedWeekTask;
        this.isWeekTaskCompleted = isWeekTaskCompleted;
        this.isAllWeekTasksCompleted = isAllWeekTasksCompleted;
        this.isAllTasksCompleted = isAllTasksCompleted;
        this.arsenAnimeshnik = arsenAnimeshnik;
        this.antiCheatTriggers = antiCheatTriggers;
        this.lastLoginTime = lastLoginTime;
        this.playerRegisterDate = playerRegisterDate;
        this.lastSaveTime = lastSaveTime;
        this.backupYesterday = backupYesterday;
        this.backupBeforeYesterday = backupBeforeYesterday;
        this.backup10DaysAgo = backup10DaysAgo;
        this.backup30DaysAgo = backup30DaysAgo;
        this.isRecordingStatistics = isrecordingStatistics;
        this.numberOfNewYear = numberOfNewYear;
        this.lastDayTrials = lastDayTrials;
        this.isDayTrialsReloaded = isDayTrialsReloaded;
        this.trialCoupons = trialCoupons;
        this.dayTrialId = dayTrialId;
        this.dayTrial = dayTrial;
        this.dayTrialReward = dayTrialReward;
        this.completedDayTrial = completedDayTrial;
        this.isDayTrialCompleted = isDayTrialCompleted;
        this.dayTrialPlaneId = dayTrialPlaneId;
        this.generalTrialId = generalTrialId;
        this.generalTrialTask = generalTrialTask;
        this.generalTrialReward = generalTrialReward;
        this.completedGeneralTrial = completedGeneralTrial;
        this.isGeneralTrialCompleted = isGeneralTrialCompleted;
        this.generalTrialPlaneId = generalTrialPlaneId;
        this.commonRandomShopPlaneId = commonRandomShopPlaneId;
        this.rareRandomShopPlaneId = rareRandomShopPlaneId;
    }
}