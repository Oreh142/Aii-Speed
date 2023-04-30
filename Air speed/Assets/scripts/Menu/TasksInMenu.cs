using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TasksInMenu : MonoBehaviour
{
    private bool isDayTasks;
    private bool isDayTasksCompleted;
    private bool[] _dayTaskIsCompleted = new bool[4];
    private bool[] _weekTaskIsCompleted = new bool[4];
    private int[] _currentDayTasks = new int[4];
    private int[] _currentWeekTasks = new int[4];
    private const float _dayTime = 86400;
    private const float _weekTime = 604800;
    [SerializeField] private Text _timeToUpdateText;
    [SerializeField] private GameObject _tasksPrefab;
    [SerializeField] private Transform _dayParent;
    [SerializeField] private Transform _weekParent;
    [SerializeField] private GameObject _dayTasksButton;
    [SerializeField] private GameObject _weekTasksButton;
    [SerializeField] private GameObject _scrollViewWeek;
    [SerializeField] private GameObject _scrollViewDay;
    [SerializeField] private GameObject _reloadTasksButton;
    [SerializeField] private GameObject _reloadTasksBg;
    [SerializeField] private Menu _menu;
    private PlayfabManager _playfabManager;

    private void Start()
    {
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        if (_playfabManager.isLogged)
        {
            _menu = GetComponent<Menu>();
            OnDayTasksButtonClicked();
            if (PlayerPrefs.HasKey("lastDayTasks") & isDayTasks)
            {
                TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastDayTasks"));
                if (ts.TotalSeconds > _dayTime)
                {
                    ReloadDayTasks();
                }
                else
                {
                    LoadDayTasks();
                }
            }
            else
            {
                ReloadDayTasks();
            }
            if (PlayerPrefs.HasKey("lastWeekTasks"))
            {
                TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastWeekTasks"));
                if (ts.TotalSeconds > _weekTime)
                {
                    ReloadWeekTasks();
                }
                else
                {
                    LoadWeekTasks();
                }
            }
            else
            {
                ReloadWeekTasks();
            }
            StartCoroutine(CheckTasksCoroutine());
            OnWeekTasksButtonClicked();
            OnDayTasksButtonClicked();
            _playfabManager.SaveData("Data");
        }
    }

    public void LoadTasks()
    {
        LoadDayTasks();
        LoadWeekTasks();
    }

    private IEnumerator CheckTasksCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        CheckTasks();
        StartCoroutine(CheckTasksCoroutine());
    }

    private void CheckTasks()
    {
        if (_playfabManager)
        {
            if (_playfabManager.isLogged)
            {
                if (isDayTasks)
                {
                    TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastDayTasks"));
                    _timeToUpdateText.text = "До обновления " + FormatNumsHelper.FormatTime(_dayTime - (float)ts.TotalSeconds);
                    if (_dayTime - (float)ts.TotalSeconds <= 0)
                    {
                        for (int i = 0; i < _dayParent.childCount; i++)
                        {
                            Destroy(_dayParent.transform.GetChild(i).gameObject);
                        }
                        ReloadDayTasks();
                        PlayerPrefsSafe.SetBool("isDayTasksReloaded", false);
                    }
                }
                else if (!isDayTasks)
                {
                    TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastWeekTasks"));
                    _timeToUpdateText.text = "До обновления " + FormatNumsHelper.FormatTime(_weekTime - (float)ts.TotalSeconds);
                    if (_weekTime - (float)ts.TotalSeconds <= 0)
                    {
                        for (int i = 0; i < _weekParent.childCount; i++)
                        {
                            Destroy(_weekParent.transform.GetChild(i).gameObject);
                        }
                        ReloadWeekTasks();
                        PlayerPrefsSafe.SetBool("isWeekTasksReloaded", false);
                    }
                }
                if (isDayTasks)
                {
                    if (!PlayerPrefsSafe.GetBool("isDayTasksCompleted") && !PlayerPrefsSafe.GetBool("isDayTasksReloaded"))
                    {
                        _reloadTasksButton.SetActive(true);
                        for (int i = 0; i < 4; i++)
                        {
                            if (PlayerPrefsSafe.GetBool("isDayTaskCompleted" + i))
                                _reloadTasksButton.SetActive(false);
                        }
                    }
                    else if (!PlayerPrefsSafe.GetBool("isDayTasksCompleted") || PlayerPrefsSafe.GetBool("isDayTasksReloaded"))
                    {
                        _reloadTasksButton.SetActive(false);
                    }
                }
                else
                {
                    if (!PlayerPrefsSafe.GetBool("isWeekTasksCompleted") && !PlayerPrefsSafe.GetBool("isWeekTasksReloaded"))
                    {
                        _reloadTasksButton.SetActive(true);
                        for (int i = 0; i < 4; i++)
                        {
                            if (PlayerPrefsSafe.GetBool("isWeekTaskCompleted" + i))
                                _reloadTasksButton.SetActive(false);
                        }
                    }
                    else if (PlayerPrefsSafe.GetBool("isWeekTasksCompleted") || PlayerPrefsSafe.GetBool("isWeekTasksReloaded"))
                    {
                        _reloadTasksButton.SetActive(false);
                    }
                }
            }
        }
    }

    private void LoadDayTasks()
    {
        int count = 0;
        for (int i = 0; i < _dayParent.childCount; i++)
        {
            Destroy(_dayParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _currentDayTasks.Length; i++)
        {
            _currentDayTasks[i] = PlayerPrefsSafe.GetInt("dayTaskId" + i);
            float task = PlayerPrefsSafe.GetFloat("dayTask" + i);
            float reward = PlayerPrefsSafe.GetFloat("dayReward" + i);
            float completed = PlayerPrefsSafe.GetFloat("completedDayTask" + i);
            CreateTasks(_currentDayTasks[i], task, completed, _dayParent, reward, true);
            CheckCompletedTask(i, task, completed, reward, true);
            if (completed >= task)
            {
                count += 1;
            }
        }
        CreateTasks(-1, 4, count, _dayParent, 5000, true);
        CheckCompletedTask(4, 4, count, 5000, true);
        if (count == 4 && !PlayerPrefsSafe.GetBool("isDayTasksCompleted"))
        {
            PlayerPrefsSafe.SetBool("isDayTasksCompleted", true);

        }
    }

    private void ReloadDayTasks()
    {
        int[] tasks = RandomTask(_currentDayTasks, true);
        for (int i = 0; i < _currentDayTasks.Length; i++)
        {
            float task = 0;
            float reward = 0;
            switch (tasks[i])
            {
                case 0:
                    task = UnityEngine.Random.Range(70000, 100001);
                    reward = task / 70;
                    break;

                case 1:
                    task = UnityEngine.Random.Range(80000, 160001);
                    reward = task / 80;
                    break;

                case 2:
                    task = UnityEngine.Random.Range(5000, 10001);
                    reward = task / 5;
                    break;

                case 3:
                    task = UnityEngine.Random.Range(15000, 25001);
                    reward = task / 15;
                    break;

                case 4:
                    task = UnityEngine.Random.Range(15000, 30001);
                    reward = task / 15;
                    break;

                case 5:
                    task = 5;
                    reward = 1000;
                    break;

                case 6:
                    task = UnityEngine.Random.Range(600, 1201);
                    reward = task / 0.6f;
                    break;

                case 7:
                    task = UnityEngine.Random.Range(10000, 20001);
                    reward = task / 10;
                    break;

                case 8:
                    task = UnityEngine.Random.Range(15000, 30001);
                    reward = task / 15;
                    break;

                case 9:
                    task = UnityEngine.Random.Range(15, 31);
                    reward = task * 65;
                    break;

                case 10:
                    task = UnityEngine.Random.Range(4500, 10000);
                    reward = task / 1.3f;
                    break;

                case 11:
                    task = UnityEngine.Random.Range(2000, 5001);
                    reward = task / 3.5f;
                    break;

                case 12:
                    task = UnityEngine.Random.Range(1300, 3001);
                    reward = task / 1.5f;
                    break;
            }
            if (tasks[i] == 10)
            {
                tasks[i] = -4;
                task = 1;
                reward = 750;
            }
            CreateTasks(tasks[i], task, 0, _dayParent, reward, true);
            PlayerPrefsSafe.SetInt("dayTaskId" + i, tasks[i]);
            PlayerPrefsSafe.SetFloat("dayTask" + i, task);
            PlayerPrefsSafe.SetFloat("dayReward" + i, reward);
            PlayerPrefsSafe.SetFloat("completedDayTask" + i, 0);
            PlayerPrefsSafe.SetBool("isDayTaskCompleted" + i, false);
        }
        PlayerPrefsSafe.SetBool("isDayTasksReloaded", false);
        PlayerPrefsSafe.SetBool("isDayTasksCompleted", false);
        PlayerPrefsSafe.SetBool("isDayTaskCompleted-1", false);
        CreateTasks(-1, 4, 0, _dayParent, 5000, true);
        DateTime reloadDayTasksTime = FormatNumsHelper.ChangeTime(DateTime.Now, 0, 0, 0, 0, 0);
        PlayerPrefs.SetString("lastDayTasks", reloadDayTasksTime.ToString());
    }

    private void LoadWeekTasks()
    {
        float completed;
        float count = 0;
        for (int i = 0; i < _weekParent.childCount; i++)
        {
            Destroy(_weekParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _currentWeekTasks.Length; i++)
        {
            _currentWeekTasks[i] = PlayerPrefsSafe.GetInt("weekTaskId" + i);
            float task = PlayerPrefsSafe.GetFloat("weekTask" + i);
            float reward = PlayerPrefsSafe.GetFloat("weekReward" + i);
            completed = PlayerPrefsSafe.GetFloat("completedWeekTask" + i);
            CreateTasks(_currentWeekTasks[i], task, completed, _weekParent, reward, false);
            CheckCompletedTask(i, task, completed, reward, false);
            if (completed >= task)
            {
                count += 1;
            }
        }
        CreateTasks(-2, 4, count, _weekParent, 15000, false);
        CheckCompletedTask(4, 4, count, 15000, false);
    }

    private void ReloadWeekTasks()
    {
        int[] tasks = RandomTask(_currentWeekTasks, false);
        for (int i = 0; i < _currentWeekTasks.Length; i++)
        {
            float task = 0;
            float reward = 0;
            switch (tasks[i])
            {
                case 0:
                    task = UnityEngine.Random.Range(420000, 840001);
                    reward = task / 84;
                    break;

                case 1:
                    task = UnityEngine.Random.Range(630000, 1200001);
                    reward = task / 126;
                    break;

                case 2:
                    task = UnityEngine.Random.Range(36000, 72001);
                    reward = task / 7.2f;
                    break;

                case 3:
                    task = UnityEngine.Random.Range(170000, 270001);
                    reward = task / 34;
                    break;

                case 4:
                    task = UnityEngine.Random.Range(200000, 300001);
                    reward = task / 40;
                    break;

                case 5:
                    task = UnityEngine.Random.Range(21, 36);
                    reward = task * 240;
                    break;

                case 6:
                    task = UnityEngine.Random.Range(5400, 12801);
                    reward = task / 0.72f;
                    break;

                case 7:
                    task = UnityEngine.Random.Range(225000, 270001);
                    reward = task / 45;
                    break;

                case 8:
                    task = UnityEngine.Random.Range(200000, 300001);
                    reward = task / 40;
                    break;

                case 9:
                    task = UnityEngine.Random.Range(105, 201);
                    reward = task * 48;
                    break;

                case 10:
                    task = UnityEngine.Random.Range(37500, 100001);
                    reward = task / 1.5f;
                    break;

                case 11:
                    task = UnityEngine.Random.Range(21000, 35001);
                    reward = task / 4.2f;
                    break;

                case 12:
                    task = UnityEngine.Random.Range(10000, 20000);
                    reward = task / 2;
                    break;
            }
            CreateTasks(tasks[i], task, 0, _weekParent, reward, false);
            PlayerPrefsSafe.SetInt("weekTaskId" + i, tasks[i]);
            PlayerPrefsSafe.SetFloat("weekTask" + i, task);
            PlayerPrefsSafe.SetFloat("weekReward" + i, reward);
            PlayerPrefsSafe.SetFloat("completedWeekTask" + i, 0);
            PlayerPrefsSafe.SetBool("isWeekTaskCompleted" + i, false);
        }
        for (int i = 0; i < 7; i++)
        {
            PlayerPrefsSafe.SetBool("isEveryDayTaskCompleted" + i, false);
        }
        PlayerPrefsSafe.SetBool("isWeekTasksReloaded", false);
        PlayerPrefsSafe.SetBool("isWeekTasksCompleted", false);
        PlayerPrefsSafe.SetBool("isWeekTaskCompleted-2", false);
        PlayerPrefsSafe.SetBool("isWeekTaskCompleted-3", false);
        CreateTasks(-2, 4, 0, _weekParent, 15000, false);
        DateTime lastWeekTasks = FormatNumsHelper.ChangeTime(DateTime.Now, FormatNumsHelper.DayOfTheWeek(), 0, 0, 0, 0);
        PlayerPrefs.SetString("lastWeekTasks", lastWeekTasks.ToString());
    }

    private void CheckCompletedTask(int taskId, float task, float completed, float reward, bool isdayReward)
    {
        if (completed >= task)
        {
            if (isdayReward && taskId <= 3)
            {
                if (!PlayerPrefsSafe.GetBool("isDayTaskCompleted" + taskId))
                {
                    PlayerPrefsSafe.SetBool("isDayTaskCompleted" + taskId, true);
                    int recievedReward = PlayerPrefsSafe.GetInt("recievedReward" + 1);
                    recievedReward += 1;
                    PlayerPrefsSafe.SetInt("recievedReward" + 1, recievedReward);
                    _menu.GetRewardForTasks(reward);
                    _playfabManager.SaveData("Data");
                }
            }
            else
            {
                if (!PlayerPrefsSafe.GetBool("isWeekTaskCompleted" + taskId))
                {
                    PlayerPrefsSafe.SetBool("isWeekTaskCompleted" + taskId, true);
                    int recievedReward = PlayerPrefsSafe.GetInt("recievedReward" + 2);
                    recievedReward += 1;
                    PlayerPrefsSafe.SetInt("recievedReward" + 2, recievedReward);
                    _menu.GetRewardForTasks(reward);
                    _playfabManager.SaveData("Data");
                }
            }
        }
        if (taskId == 4 && completed >= task)
        {
            if (isdayReward && !PlayerPrefsSafe.GetBool("isDayTaskCompleted-1"))
            {
                PlayerPrefsSafe.SetBool("isDayTaskCompleted-1", true);
                int recievedReward = PlayerPrefsSafe.GetInt("recievedReward" + 3);
                recievedReward += 2;
                PlayerPrefsSafe.SetInt("recievedReward" + 3, recievedReward);
                _menu.GetRewardForTasks(reward);
                _playfabManager.SaveData("Data");
            }
            else if (!isdayReward && !PlayerPrefsSafe.GetBool("isWeekTaskCompleted-2"))
            {
                PlayerPrefsSafe.SetBool("isWeekTaskCompleted-2", true);
                Debug.Log(PlayerPrefsSafe.GetBool("isWeekTaskCompleted-2"));
                int recievedReward = PlayerPrefsSafe.GetInt("recievedReward" + 3);
                recievedReward += 6;
                PlayerPrefsSafe.SetInt("recievedReward" + 3, recievedReward);
                _menu.GetRewardForTasks(reward);
                _playfabManager.SaveData("Data");
            }
        }
    }

private int[] RandomTask(int[] lastTasks, bool isDayTasks)
{
    int[] newTasks = new int[4];
    for (int i = 0; i < lastTasks.Length; i++)
    {
        bool isready = false;
        while (!isready)
        {
            int expeptionsNumber = 0;
            int id = UnityEngine.Random.Range(0, 13);
            for (int j = 0; j < lastTasks.Length; j++)
            {
                if (id != lastTasks[j])
                {
                    expeptionsNumber++;
                }
            }
            for (int n = 0; n < newTasks.Length; n++)
            {
                if (id != newTasks[n])
                {
                    expeptionsNumber++;
                }
            }
            if (expeptionsNumber == lastTasks.Length + newTasks.Length)
            {
                isready = true;
                newTasks[i] = id;
            }
        }
    }
    return newTasks;
}

private void CreateTasks(int id, float task, float completed, Transform parent, float reward, bool isDayTask)
{
    GameObject obj = Instantiate(_tasksPrefab, parent);
    Text[] texts = obj.GetComponentsInChildren<Text>();
    if (completed >= task)
    {
        completed = task;
        texts[0].text = "Выполнено!";
        texts[0].color = Color.green;
        texts[1].color = Color.green;
        texts[2].color = Color.green;
        texts[2].text = "Получена!";
    }
    else
    {
        texts[0].text = TasksTranslater.DecodeTasks(id, task);
        texts[2].text = reward.ToString("0") + " монет " + WhichHangarReward(id, isDayTask);
    }
    texts[1].text = completed.ToString("0") + "/" + task;
}

private string WhichHangarReward(int id, bool isDayTask)
{
    string message = "";
    if (id >= 0 | id == -4)
    {
        if (isDayTask)
        {
            message = "+ ангар ефрейтора";
        }
        else
        {
            message = "+ ангар младшего сержанта";
        }
    }
    else if (id == -1)
    {
        message = "+ 2 ангара сержанта";
    }
    else if (id == -2)
    {
        message = "+ 6 ангаров сержанта";
    }

    return message;
}

public void OnDayTasksButtonClicked()
{
    Image dayImage = _dayTasksButton.GetComponent<Image>();
    dayImage.color = Color.green;
    Image weekImage = _weekTasksButton.GetComponent<Image>();
    weekImage.color = Color.black;
    _scrollViewDay.SetActive(true);
    _scrollViewWeek.SetActive(false);
    isDayTasks = true;
}

public void OnWeekTasksButtonClicked()
{
    Image weekImage = _weekTasksButton.GetComponent<Image>();
    weekImage.color = Color.green;
    Image dayImage = _dayTasksButton.GetComponent<Image>();
    dayImage.color = Color.black;
    _scrollViewWeek.SetActive(true);
    _scrollViewDay.SetActive(false);
    isDayTasks = false;
}

public void CreateReloadTasksBg()
{
    _reloadTasksBg.SetActive(true);
    Text[] texts = _reloadTasksBg.GetComponentsInChildren<Text>();
    if (isDayTasks)
    {
        texts[0].text = "Вы хотите обновить ежедневные задания? Таймер обновления заданий не сбросится.";
        texts[1].text = "5000 монет";
    }
    else
    {
        texts[0].text = "Вы хотите обновить еженедельные задания? Таймер обновления заданий не сбросится.";
        texts[1].text = "25000 монет";
    }
}

public void EscapeReloadingBg()
{
    _reloadTasksBg.SetActive(false);
}

public void OnReloadTasksButtonClicked()
{
    if (isDayTasks && !PlayerPrefsSafe.GetBool("isDayTasksReloaded"))
    {
         if (_menu.SendMoneyToReloadTasks(5000) == 1)
         {
             for (int i = 0; i < _dayParent.childCount; i++)
             {
                 Destroy(_dayParent.transform.GetChild(i).gameObject);
             }
             ReloadDayTasks();
             PlayerPrefsSafe.SetBool("isDayTasksReloaded", true);
             _playfabManager.SaveData("Data");
            }
    }
    else if (!isDayTasks && !PlayerPrefsSafe.GetBool("isWeekTasksReloaded"))
    {
        if (_menu.SendMoneyToReloadTasks(25000) == 1)
            {
                for (int i = 0; i < _weekParent.childCount; i++)
                {
                    Destroy(_weekParent.transform.GetChild(i).gameObject);
                }
                ReloadWeekTasks();
                PlayerPrefsSafe.SetBool("isWeekTasksReloaded", true);
                _playfabManager.SaveData("Data");
            }
    }
    _reloadTasksBg.SetActive(false);
}
}