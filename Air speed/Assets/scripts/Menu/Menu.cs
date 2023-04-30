using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private bool[] _isPlaneBuyed;
    [SerializeField] private float _money;
    [SerializeField] private float _record;
    private float _startSpeed;
    private float _maxSpeed;
    private float _controllAbility;
    private float _boost;
    private float _multiplier;
    private int _idPlayerPlane;
    private DateTime _lastServerAnswer;
    [SerializeField] private GameObject[] _planes;
    [SerializeField] private GameObject[] _planesPrefabs;
    [SerializeField] private GameObject _acceptBuyingWindow;
    [SerializeField] private GameObject _shopBg;
    [SerializeField] private GameObject _statisticsBg;
    [SerializeField] private GameObject _tasksBg;
    [SerializeField] private GameObject _trialsBg;
    [SerializeField] private GameObject _playerStatisticsBg;
    [SerializeField] private GameObject _aviatecaBg;
    [SerializeField] private GameObject _settingsBg;
    [SerializeField] private GameObject _loadingBg;
    [SerializeField] private GameObject _warningPanel;
    [SerializeField] private GameObject _offlineGameImg;
    [SerializeField] private GameObject _statsButtons;
    [SerializeField] private GameObject _onlineLoadingPanel;
    [SerializeField] private GameObject _changelogPanel;
    [SerializeField] private GameObject _newYearGiftWindow;
    [SerializeField] private Transform _spawnPoint;
    private GameObject _planeInMenu;
    private TasksInMenu _tasksInMenu;
    private TrialsInMenu _trialsInMenu;
    private PlayfabManager _playfabManager;
    private Aviateca _aviateca;
    private ContainerLoot _containerLoot;
    [SerializeField] private Text _acceptBuyingWindowText, _acceptBuyigWindowButtonText, _moneyText, _leftPropertyesText, _rightPropertiesText, _logoutButtonText, _planeNameText, _changeRecordingStatisticsText;
    [SerializeField] private TextMeshProUGUI _advicesText;

    private void Awake()
    {
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        LoadGame();
        _tasksInMenu = GetComponent<TasksInMenu>();
        _trialsInMenu = GetComponent<TrialsInMenu>();
        _aviateca = GetComponent<Aviateca>();
        _containerLoot = GetComponent<ContainerLoot>();
    }

    private void Start()
    {
        if (PlayerPrefsSafe.GetInt("changelog") != 10)
        {
            OnChangelogButtonCLick();
            PlayerPrefsSafe.SetInt("changelog", 10);
        }
        if (PlayerPrefsSafe.GetBool("technicalWorks") && _playfabManager.isLogged)
        {
            int reward = PlayerPrefsSafe.GetInt("recievedReward" + 3);
            PlayerPrefsSafe.SetInt("recievedReward" + 3, reward + 3);
            PlayerPrefsSafe.SetBool("technicalWorks", false);
            SaveGame();
        }
        SaveOnlineData();
    }

    private void Update()
    {
        TimeSpan ts = DateTime.Now - _lastServerAnswer;
        if (ts.TotalMinutes >= 1 && _playfabManager.isLogged)
        {
            _playfabManager.isLogged = false;
            ChangeAccount();
        }
        if (Input.GetKey(KeyCode.Escape) && !_onlineLoadingPanel.activeInHierarchy)
        {
            if (_statisticsBg.activeInHierarchy)
                OnStatisticsEscapeButtonClick();
            else if (_tasksBg.activeInHierarchy)
                OnTasksEscapeButtonClick();
            else if (_playerStatisticsBg.activeInHierarchy)
                OnPlayerStatisticsEscapeButtonClick();
            else if (_aviatecaBg.activeInHierarchy)
                OnAviatecaEscapeButtonClick();
            else if (_changelogPanel.activeInHierarchy)
                OnChangelogEscapeButtonClick();
            else if (_settingsBg.activeInHierarchy)
                OnSettingsEscapeButtonClick();
        }
    }

    public void ServerAnswer()
    {
        _lastServerAnswer = DateTime.Now;
    }

    public GameObject GetPlanePrefab(int id)
    {
        return _planesPrefabs[id];
    }

    private void CreateNewYearGiftWindow()
    {
        DestroyPlane();
        _newYearGiftWindow.SetActive(true);
    }

    public void OpenNewYearGift()
    {
        _newYearGiftWindow.SetActive(false);
        OnShopButtonCLick();
        PlayerPrefsSafe.SetInt("numberOfNewYear", FormatNumsHelper.DayOfTheYear() / 365 + 1);
        _containerLoot.ScrollContainer((FormatNumsHelper.DayOfTheYear() / 365 + 1) + 2022, new float[] { 10000, 10000, 5000, 2500 }, new float[] { 10000 }, true, 1);
        SaveOnlineData();
    }

    private void SaveOnlineData()
    {
        if (_playfabManager.isLogged)
        {
            _logoutButtonText.text = "Сменить аккаунт";
            _playfabManager.SaveData("Data");
            _lastServerAnswer = DateTime.Now;
        }
        else
        {
            _offlineGameImg.SetActive(true);
            _logoutButtonText.text = "Подключиться";
        }
    }

    private void CheckRecordStatistics()
    {
        Image img = _changeRecordingStatisticsText.GetComponentInParent<Image>();
        if (!PlayerPrefsSafe.GetBool("isRecordingStatistics"))
        {
            _changeRecordingStatisticsText.text = "Запись статистики: выкл.";
            img.color = Color.red;
        }
        else
        {
            _changeRecordingStatisticsText.text = "Запись статистики: вкл.";
            img.color = Color.green;
        }
    }

    private void HidePlayerPlane(bool isHide)
    {
        if (isHide)
        {
            _planeInMenu.SetActive(false);
        }
        else
        {
            _planeInMenu.SetActive(true);
        }
    }

    private void SaveGame()
    {
        PlayerPrefsSafe.SetBoolArray("isPlaneBuyed", _isPlaneBuyed);
        PlayerPrefsSafe.SetInt("idPlayerPlane", _idPlayerPlane);
        PlayerPrefsSafe.SetFloat("money", _money);
        PlayerPrefsSafe.SetFloat("record", _record);
        PlayerPrefsSafe.SetFloat("startSpeed", _startSpeed);
        PlayerPrefsSafe.SetFloat("maxSpeed", _maxSpeed);
        PlayerPrefsSafe.SetFloat("controllAbility", _controllAbility);
        PlayerPrefsSafe.SetFloat("boost", _boost);
        PlayerPrefsSafe.SetFloat("multiplier", _multiplier);
        PlayerPrefs.SetString("lastOfflineSave", DateTime.Now.ToString());
        _moneyText.text = FormatNumsHelper.FormatNum(_money);
    }

    public int GetPlane(int id)
    {
        if (!_isPlaneBuyed[id])
        {
            _isPlaneBuyed[id] = true;
            PlayerPrefsSafe.SetBoolArray("isPlaneBuyed", _isPlaneBuyed);
            _planes[id].SetActive(true);
            SaveOnlineData();
            _aviateca.UpdateAviateca();
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public int CheckPlane(int id)
    {
        if (_isPlaneBuyed[id])
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public int Buyplane(int cost, int id)
    {
        if (_money >= cost)
        {
            if (!_isPlaneBuyed[id])
            {
                _money -= cost;
                _isPlaneBuyed[id] = true;
                PlayerPrefsSafe.SetBoolArray("isPlaneBuyed", _isPlaneBuyed);
                _planes[id].SetActive(true);
                _idPlayerPlane = id;
                SaveGame();
                SaveOnlineData();
                _aviateca.UpdateAviateca();
                _moneyText.text = FormatNumsHelper.FormatNum(_money);
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 3;
        }
    }

    public int PlanesNumber()
    {
        return _planes.Length;
    }

    public void GetRewardForTasks(float reward)
    {
        _money += reward;
        float tasksTotalMoney = PlayerPrefsSafe.GetFloat("tasksTotalMoney");
        tasksTotalMoney += reward;
        PlayerPrefsSafe.SetFloat("tasksTotalMoney", tasksTotalMoney);
    }

    public int SendMoneyToReloadTasks(float price)
    {
        SaveOnlineData();
        if (_money >= price)
        {
            _money -= price;
            SaveGame();
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void StartOnlineLoading()
    {
        Debug.Log("StartOnlineLoading0");
        if (!_playfabManager.isLogged && !_onlineLoadingPanel.activeInHierarchy)
        {
            _onlineLoadingPanel.SetActive(true);
            if (_planeInMenu)
                _planeInMenu.SetActive(false);
            Debug.Log("StartOnlineLoading1");
            StartCoroutine(RestartConnectionCoroutine());
        }
    }

    IEnumerator RestartConnectionCoroutine()
    {
        if (!_playfabManager.isLogged && _onlineLoadingPanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(2);
            _playfabManager.SaveData("Data");
            StartCoroutine(RestartConnectionCoroutine());
        }
        else if (_playfabManager.isLogged)
        {
            StopCoroutine(RestartConnectionCoroutine());
            _onlineLoadingPanel.SetActive(false);
            if (_planeInMenu && !_acceptBuyingWindow.activeInHierarchy && !_shopBg.activeInHierarchy && !_statisticsBg.activeInHierarchy && !_tasksBg.activeInHierarchy && !_playerStatisticsBg.activeInHierarchy && !_aviatecaBg.activeInHierarchy && !_loadingBg.activeInHierarchy && !_settingsBg.activeInHierarchy && !_warningPanel.activeInHierarchy && !_changelogPanel.activeInHierarchy && !_newYearGiftWindow.activeInHierarchy && !_trialsBg.activeInHierarchy)
                CreatePlane(_idPlayerPlane);
        }
    }

    IEnumerator HidePlaneInMenuCoroutine(int num)
    {
        yield return new WaitForSeconds(0.05f);
        if (_planeInMenu && num <= 1)
        {
            num += 1;
            DestroyPlane();
            StartCoroutine(HidePlaneInMenuCoroutine(num));
        }
        else if (num > 1)
        {
            StopCoroutine(HidePlaneInMenuCoroutine(num));
        }
    }

    IEnumerator SeekPlaneInMenuCoroutine(int num)
    {
        StopCoroutine(HidePlaneInMenuCoroutine(21));
        yield return new WaitForSeconds(0.05f);
        if (num <= 1)
        {
            num += 1;
            CreatePlane(_idPlayerPlane);
            StartCoroutine(SeekPlaneInMenuCoroutine(num));
        }
        else if (num > 1)
        {
            StopCoroutine(SeekPlaneInMenuCoroutine(num));
        }
    }

    private void LoadGame()
    {
        _isPlaneBuyed = PlayerPrefsSafe.GetBoolArray("isPlaneBuyed", _isPlaneBuyed);
        _idPlayerPlane = PlayerPrefsSafe.GetInt("idPlayerPlane");
        _money = PlayerPrefsSafe.GetFloat("money");
        _record = PlayerPrefsSafe.GetFloat("record");
        _moneyText.text = FormatNumsHelper.FormatNum(_money);
        _playfabManager.SaveBackups();
        HidePlanes();
        StopCoroutine(HidePlaneInMenuCoroutine(21));
        CreatePlane(_idPlayerPlane);
        StartCoroutine(SeekPlaneInMenuCoroutine(0));
        if (PlayerPrefsSafe.GetFloat("totalGamesPlayer") <= 0)
        {
            _statsButtons.SetActive(false);
        }
    }

    private void ChangePropertyes(int id)
    {
        Plane plane = _planesPrefabs[_idPlayerPlane].GetComponent<Plane>();
        _startSpeed = plane.startSpeed;
        _maxSpeed = plane.maxSpeed;
        _controllAbility = plane.controllAbility;
        _boost = (_maxSpeed - _startSpeed) / (_controllAbility * 15);
        _boost += plane.planeBoost;
        _multiplier = _maxSpeed / (_controllAbility * 100);
        _multiplier += plane.planeMultiplier;
        Text name = _planes[id].GetComponentInChildren<Text>();
        if (plane.rarity == "common")
            _planeNameText.color = Color.gray;
        if (plane.rarity == "rare")
            _planeNameText.color = Color.green;
        if (plane.rarity == "veryRare")
            _planeNameText.color = new Color (0.8f, 0.2f, 0.8f, 1f);
        if (plane.rarity == "legendary")
            _planeNameText.color = Color.yellow;
        _planeNameText.text = name.text;
        _leftPropertyesText.text = "Нач. скорость: " + _startSpeed + " км/ч\n\nМакс. скорость: " + _maxSpeed + " км/ч";
        _rightPropertiesText.text = "Управляемость: " + _controllAbility + "\n\nУскорение: " + _boost.ToString("0.0") + "\n\nМножитель: " + _multiplier.ToString("0.0");
    }

    private void HidePlanes()
    {
        for (int i = 1; i < _isPlaneBuyed.Length; i++)
        {
            if (!_isPlaneBuyed[i])
            {
                _planes[i].SetActive(false);
            }
        }
    }

    private void CreatePlane(int id) //При смене самолёта создаётся его моделька справа, при выборе определённого самолёта меняется цвет текста с его названием
    {
        DestroyPlane();
        ChangePropertyes(id);
        if (_planeInMenu)
            DestroyPlane();
        _planeInMenu = Instantiate(_planesPrefabs[id], _spawnPoint.position, Quaternion.identity, _spawnPoint);
        Text text = _planes[id].GetComponentInChildren<Text>();
        text.color = Color.yellow;
        SaveGame();
    }

    private void DestroyPlane() //При смене самолёта прошлая моделька удаляется, а к тексту с его названием возвращается чёрный цвет
    {
        if (_planeInMenu)
        {
            Text text = _planes[_idPlayerPlane].GetComponentInChildren<Text>();
            text.color = new Color(0, 0, 0, 255);
            Destroy(_planeInMenu);
        }
    }

    //Далее идут реакции на нажатие кнопочки того или иного самолётика
    //Далее идёт проверка, есть ли у нас самолёт. Если его нет, то появляется окошко с предлоежнием купить какой-либо самолёт

    public void OnPlaneButtonClick(int id)
    {
        if (_isPlaneBuyed[id])
        {
            DestroyPlane();
            _idPlayerPlane = id;
            CreatePlane(_idPlayerPlane);
        }
    }

    public void OnPlayButtonClick() //Запуск основного геймплея
    {
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
        _loadingBg.SetActive(true);
        _advicesText.text = "Совет:\n" + Advices.GetAdvice();
        SaveGame();
        SaveOnlineData();
        SceneManager.LoadScene(2);
    }

    public void OnShopButtonCLick()
    {
        if (_playfabManager.isLogged)
        {
            SaveGame();
            SaveOnlineData();
            _shopBg.SetActive(true);
            DestroyPlane();
            StartCoroutine(HidePlaneInMenuCoroutine(0));
        }
        else
        {
            CreateWarningPanel();
        }
    }

    public void OnShopEscapeButtonClick()
    {
        _shopBg.SetActive(false);
        LoadGame();
        SaveOnlineData();
    }

    public void OnStatisticsButtonCLick()
    {
        SaveGame();
        _statisticsBg.SetActive(true);
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
    }

    public void OnStatisticsEscapeButtonClick()
    {
        _statisticsBg.SetActive(false);
        LoadGame();
    }

    public void OnPlayerStatisticsButtonCLick()
    {
        SaveGame();
        _playerStatisticsBg.SetActive(true);
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
    }

    public void OnPlayerStatisticsEscapeButtonClick()
    {
        _playerStatisticsBg.SetActive(false);
        LoadGame();
    }

    public void OnAviatecaButtonCLick()
    {
        SaveGame();
        _aviatecaBg.SetActive(true);
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
    }

    public void OnAviatecaEscapeButtonClick()
    {
        _aviatecaBg.SetActive(false);
        LoadGame();
    }

    public void OnSettingsButtonCLick()
    {
        SaveGame();
        _settingsBg.SetActive(true);
        CheckRecordStatistics();
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
    }

    public void OnSettingsEscapeButtonClick()
    {
        _settingsBg.SetActive(false);
        LoadGame();
    }

    public void OnChangelogButtonCLick()
    {
        SaveGame();
        _changelogPanel.SetActive(true);
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
    }

    public void OnChangelogEscapeButtonClick()
    {
        _settingsBg.SetActive(false);
        _changelogPanel.SetActive(false);
        if (PlayerPrefsSafe.GetInt("numberOfNewYear") == FormatNumsHelper.DayOfTheYear() / 365 + 1)
        {
            LoadGame();
        }
    }

    public void OnTasksButtonCLick()
    {
        if (_playfabManager.isLogged)
        {
            SaveGame();
            SaveOnlineData();
            _tasksBg.SetActive(true);
            DestroyPlane();
            StartCoroutine(HidePlaneInMenuCoroutine(0));
            _tasksInMenu.LoadTasks();
        }
        else
        {
            CreateWarningPanel();
        }
    }

    public void OnTasksEscapeButtonClick()
    {
        _tasksBg.SetActive(false);
        LoadGame();
        SaveOnlineData();
    }

    public void OnTrialsButtonCLick()
    {
        if (_playfabManager.isLogged)
        {
            SaveGame();
            SaveOnlineData();
            _trialsBg.SetActive(true);
            DestroyPlane();
            StartCoroutine(HidePlaneInMenuCoroutine(0));
            _trialsInMenu.LoadTrials();
        }
        else
        {
            CreateWarningPanel();
        }
    }

    public void OnTrialsEscapeButtonClick()
    {
        _trialsBg.SetActive(false);
        LoadGame();
        SaveOnlineData();
    }

    public void ChangeAccount()
    {
        SaveGame();
        SaveOnlineData();
        PlayerPrefs.SetString("lastOfflineSave", "01.01.2000 00:00:00");
        _playfabManager.Logout();
        SceneManager.LoadScene(0);
    }

    public void CreateWarningPanel()
    {
        _warningPanel.SetActive(true);
        SaveGame();
        DestroyPlane();
        StartCoroutine(HidePlaneInMenuCoroutine(0));
    }

    public void OnEscapeWarningPanel()
    {
        _warningPanel.SetActive(false);
        LoadGame();
    }

    public void ChangeRecordStatistics()
    {
        Image img = _changeRecordingStatisticsText.GetComponentInParent<Image>();
        if (PlayerPrefsSafe.GetBool("isRecordingStatistics"))
        {
            if (PlayerPrefsSafe.GetFloat("totalGamesPlayer") > 0)
            {
                PlayerPrefsSafe.SetBool("isRecordingStatistics", false);
                _changeRecordingStatisticsText.text = "Запись статистики: выкл.";
                img.color = Color.red;
            }
        }
        else
        {
            PlayerPrefsSafe.SetBool("isRecordingStatistics", true);
            _changeRecordingStatisticsText.text = "Запись статистики: вкл.";
            img.color = Color.green;
        }
        SaveGame();
        SaveOnlineData();
    }

    public Data ReturnClass()
    {
        int[] dayOpened = new int[4];
        int[] recievedReward = new int[4];
        float[] totalTimeArray = new float[_isPlaneBuyed.Length];
        float[] totalScoreArray = new float[_isPlaneBuyed.Length];
        float[] topScoreArray = new float[_isPlaneBuyed.Length];
        float[] totalSpeedArray = new float[_isPlaneBuyed.Length];
        float[] totalGamesArray = new float[_isPlaneBuyed.Length];
        float[] totalDistanceArray = new float[_isPlaneBuyed.Length];
        float[] topDistanceArray = new float[_isPlaneBuyed.Length];
        float[] totalMoneyArray = new float[_isPlaneBuyed.Length];
        for (int i = 0; i < dayOpened.Length; i++)
        {
            dayOpened[i] = PlayerPrefsSafe.GetInt("dayOpened" + i);
            recievedReward[i] = PlayerPrefsSafe.GetInt("recievedReward" + i);
        }
        int[] dayTaskId = new int[4];
        float[] dayTask = new float[dayTaskId.Length];
        float[] dayReward = new float[dayTaskId.Length];
        float[] completedDayTask = new float[dayTaskId.Length];
        bool[] isDayTaskCompleted = new bool[dayTaskId.Length];
        for (int i = 0; i < dayTaskId.Length; i++)
        {
            dayTaskId[i] = PlayerPrefsSafe.GetInt("dayTaskId" + i);
            dayTask[i] = PlayerPrefsSafe.GetFloat("dayTask" + i);
            dayReward[i] = PlayerPrefsSafe.GetFloat("dayReward" + i);
            completedDayTask[i] = PlayerPrefsSafe.GetFloat("completedDayTask" + i);
            isDayTaskCompleted[i] = PlayerPrefsSafe.GetBool("isDayTaskCompleted" + i);
        }
        int[] weekTaskId = new int[4];
        float[] weekTask = new float[weekTaskId.Length];
        float[] weekReward = new float[weekTaskId.Length];
        float[] completedWeekTask = new float[weekTaskId.Length];
        bool[] isWeekTaskCompleted = new bool[weekTaskId.Length];
        for (int i = 0; i < weekTaskId.Length; i++)
        {
            weekTaskId[i] = PlayerPrefsSafe.GetInt("weekTaskId" + i);
            weekTask[i] = PlayerPrefsSafe.GetFloat("weekTask" + i);
            weekReward[i] = PlayerPrefsSafe.GetFloat("weekReward" + i);
            completedWeekTask[i] = PlayerPrefsSafe.GetFloat("completedWeekTask" + i);
            isWeekTaskCompleted[i] = PlayerPrefsSafe.GetBool("isWeekTaskCompleted" + i);
        }
        int[] dayTrialId = new int[4];
        float[] dayTrial = new float[dayTrialId.Length];
        float[] dayTrialReward = new float[dayTrialId.Length];
        float[] completedDayTrial = new float[dayTrialId.Length];
        bool[] isDayTrialCompleted = new bool[dayTrialId.Length];
        int[] dayTrialPlaneId = new int[dayTrialId.Length];
        for (int i = 0; i < dayTrialId.Length; i++)
        {
            dayTrialId[i] = PlayerPrefsSafe.GetInt("dayTrialId" + i);
            dayTrial[i] = PlayerPrefsSafe.GetFloat("dayTrial" + i);
            dayTrialReward[i] = PlayerPrefsSafe.GetFloat("dayTrialReward" + i);
            completedDayTrial[i] = PlayerPrefsSafe.GetFloat("completedDayTrial" + i);
            isDayTrialCompleted[i] = PlayerPrefsSafe.GetBool("isDayTrialCompleted" + i);
            dayTrialPlaneId[i] = PlayerPrefsSafe.GetInt("dayTrialPlaneId" + i);
        }
        int[] generalTrialId = new int[5];
        float[] generalTrial = new float[generalTrialId.Length];
        float[] generalReward = new float[generalTrialId.Length];
        float[] completedGeneralTrial = new float[generalTrialId.Length];
        bool[] isGeneralTrialCompleted = new bool[generalTrialId.Length];
        int[] generalTrialPlaneId = new int[generalTrialId.Length];
        for (int i = 0; i < generalTrialId.Length; i++)
        {
            generalTrialId[i] = PlayerPrefsSafe.GetInt("generalTrialId" + i);
            generalTrial[i] = PlayerPrefsSafe.GetFloat("generalTrial" + i);
            generalReward[i] = PlayerPrefsSafe.GetFloat("generalReward" + i);
            completedGeneralTrial[i] = PlayerPrefsSafe.GetFloat("completedGeneralTrial" + i);
            isGeneralTrialCompleted[i] = PlayerPrefsSafe.GetBool("isWeekTrialCompleted" + i);
            generalTrialPlaneId[i] = PlayerPrefsSafe.GetInt("generalTrialPlaneId" + i);
        }
        return new Data(PlayerPrefsSafe.GetFloat("money"), PlayerPrefsSafe.GetFloat("record"), PlayerPrefsSafe.GetInt("idPlayerPlane"), PlayerPrefsSafe.GetBoolArray("isPlaneBuyed", _isPlaneBuyed), dayOpened, recievedReward, PlayerPrefs.GetString("lastOpening"), PlayerPrefs.GetString("lastShopReloading"), PlayerPrefsSafe.GetFloat("totalTimePlayer"), PlayerPrefsSafe.GetFloat("totalScorePlayer"), PlayerPrefsSafe.GetFloat("totalSpeedPlayer"), PlayerPrefsSafe.GetFloat("totalGamesPlayer"), PlayerPrefsSafe.GetFloat("totalDistancePlayer"), PlayerPrefsSafe.GetFloat("topDistancePlayer"), PlayerPrefsSafe.GetFloat("totalMoneyPlayer"), PlayerPrefsSafe.GetFloat("tasksTotalMoney"), PlayerPrefsSafe.GetFloat("containerTotalMoney"), PlayerPrefsSafe.GetFloatArray("totalTime", totalTimeArray), PlayerPrefsSafe.GetFloatArray("totalScore", totalScoreArray), PlayerPrefsSafe.GetFloatArray("topScore", topScoreArray), PlayerPrefsSafe.GetFloatArray("totalSpeed", totalSpeedArray), PlayerPrefsSafe.GetFloatArray("totalGames", totalGamesArray), PlayerPrefsSafe.GetFloatArray("totalDistance", totalDistanceArray), PlayerPrefsSafe.GetFloatArray("topDistance", topDistanceArray), PlayerPrefsSafe.GetFloatArray("totalMoney", totalMoneyArray), PlayerPrefs.GetString("lastDayTasks"), PlayerPrefsSafe.GetBool("isDayTasksReloaded"), PlayerPrefsSafe.GetBool("isDayTasksCompleted"), dayTaskId, dayTask, dayReward, completedDayTask, isDayTaskCompleted, PlayerPrefsSafe.GetBool("isDayTaskCompleted-1"), PlayerPrefs.GetString("lastWeekTasks"), PlayerPrefsSafe.GetBool("isWeekTasksReloaded"), PlayerPrefsSafe.GetBool("isWeekTaskCompleted-2"), weekTaskId, weekTask, weekReward, completedWeekTask, isWeekTaskCompleted, PlayerPrefsSafe.GetBool("isWeekTaskCompleted-2"), PlayerPrefsSafe.GetBool("isWeekTaskCompleted-3"), PlayerPrefs.GetString("ArsenAnimeshnik"), PlayerPrefsSafe.GetInt("antiCheatTriggers"), PlayerPrefs.GetString("lastLoginTime"), PlayerPrefs.GetString("playerRegisterDate"), DateTime.Now.ToString(), PlayerPrefs.GetString("backup0"), PlayerPrefs.GetString("backup1"), PlayerPrefs.GetString("backup2"), PlayerPrefs.GetString("backup3"), PlayerPrefsSafe.GetBool("isRecordingStatistics"), PlayerPrefsSafe.GetInt("numberOfNewYear"), PlayerPrefs.GetString("lastDayTrials"), PlayerPrefsSafe.GetBool("isDayTrialReloaded"), PlayerPrefsSafe.GetInt("trialId"), PlayerPrefsSafe.GetInt("trialCoupons"), dayTrialId, dayTrial, dayTrialReward, completedDayTrial, isDayTrialCompleted, dayTrialPlaneId, generalTrialId, generalTrial, generalReward, completedGeneralTrial, isGeneralTrialCompleted, generalTrialPlaneId, PlayerPrefsSafe.GetInt("commonRadnomShopPlaneId"), PlayerPrefsSafe.GetInt("rareRadnomShopPlaneId"));
    }
}