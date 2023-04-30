using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrialsInMenu : MonoBehaviour
{
    private bool isDayTrials;
    private int[] _currentDayTrials = new int[4];
    private int[] _currentGeneralTrials = new int[5];
    private const float _dayTime = 86400;
    private int _trialId;
    private int _coupons;
    [SerializeField] private Text _timeToUpdateText;
    [SerializeField] private Text _couponsText;
    [SerializeField] private GameObject _trialsPrefab;
    [SerializeField] private GameObject _trialsPanelButton;
    [SerializeField] private Transform _dayParent;
    [SerializeField] private Transform _generalParent;
    [SerializeField] private GameObject _dayTasksButton;
    [SerializeField] private GameObject _generalTasksButton;
    [SerializeField] private GameObject _scrollViewGeneral;
    [SerializeField] private GameObject _scrollViewDay;
    [SerializeField] private GameObject _reloadTasksButton;
    [SerializeField] private GameObject _reloadTrialsBg;
    [SerializeField] private Menu _menu;
    private PlayfabManager _playfabManager;
    private ContainerLoot _containerLoot;
    private List<int> _playerPlanes = new();

    private void Start()
    {
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        if (_playfabManager.isLogged)
        {
            _menu = GetComponent<Menu>();
            _containerLoot = GetComponent<ContainerLoot>();
            for (int i = 0; i < _menu.PlanesNumber(); i++)
            {
                if (_menu.CheckPlane(i) == 1)
                    _playerPlanes.Add(i);
            }
            if (_playerPlanes.Count >= 7)
            {
                _trialsPanelButton.SetActive(true);
                _coupons = PlayerPrefsSafe.GetInt("trialCoupons");
                _trialId = PlayerPrefsSafe.GetInt("trialId");
                OnDayTrialsButtonClicked();
                if (PlayerPrefs.HasKey("lastDayTrials") && isDayTrials)
                {
                    TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastDayTrials"));
                    if (ts.TotalSeconds > _dayTime)
                    {
                        ReloadDayTrials();
                    }
                    else
                    {
                        LoadDayTrials();
                    }
                }
                else
                {
                    ReloadDayTrials();
                }
                Debug.Log(_trialId + " " + DateTime.Now.Month);
                if (_trialId != DateTime.Now.Month)
                {
                    ReloadGeneralTrials();
                }
                else
                {
                    LoadGeneralTrials();
                }
                StartCoroutine(CheckTrialsCoroutine());
                OnGeneralTrialsButtonClicked();
                OnDayTrialsButtonClicked();
                _playfabManager.SaveData("Data");
            }
            else
            {
                _trialsPanelButton.SetActive(false);
            }
        }
    }

    public void LoadTrials()
    {
        LoadDayTrials();
        LoadGeneralTrials();
    }

    private IEnumerator CheckTrialsCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        CheckTrials();
        StartCoroutine(CheckTrialsCoroutine());
    }

    private void CheckTrials()
    {
        if (_playfabManager)
        {
            if (_playfabManager.isLogged)
            {
                if (isDayTrials)
                {
                    TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastDayTrials"));
                    _timeToUpdateText.text = "До обновления " + FormatNumsHelper.FormatTime(_dayTime - (float)ts.TotalSeconds);
                    if (_dayTime - (float)ts.TotalSeconds <= 0)
                    {
                        for (int i = 0; i < _dayParent.childCount; i++)
                        {
                            Destroy(_dayParent.transform.GetChild(i).gameObject);
                        }
                        ReloadDayTrials();
                        PlayerPrefsSafe.SetBool("isDayTrialsReloaded", false);
                    }
                }
                else if (!isDayTrials)
                {
                    _timeToUpdateText.text = "До конца " + FormatNumsHelper.FormatTime(TimeToTheEndOfTrial());
                    Debug.Log(_trialId + " " + DateTime.Now.Month);
                    if (_trialId != DateTime.Now.Month)
                    {
                        Debug.Log(_trialId + " " + DateTime.Now.Month);
                        for (int i = 0; i < _generalParent.childCount; i++)
                        {
                            Destroy(_generalParent.transform.GetChild(i).gameObject);
                        }
                        ReloadGeneralTrials();
                    }
                }
                if (isDayTrials)
                {
                    if (!PlayerPrefsSafe.GetBool("isDayTrialsReloaded"))
                    {
                        _reloadTasksButton.SetActive(true);
                        for (int i = 0; i < 4; i++)
                        {
                            if (PlayerPrefsSafe.GetBool("isDayTrialCompleted" + i))
                                _reloadTasksButton.SetActive(false);
                        }
                    }
                    else if (PlayerPrefsSafe.GetBool("isDayTrialsReloaded"))
                    {
                        _reloadTasksButton.SetActive(false);
                    }
                }
                else
                {
                    _reloadTasksButton.SetActive(false);
                }
                _couponsText.text = _coupons.ToString("0");
            }
        }
    }

    private void LoadDayTrials()
    {
        for (int i = 0; i < _dayParent.childCount; i++)
        {
            Destroy(_dayParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _currentDayTrials.Length; i++)
        {
            _currentDayTrials[i] = PlayerPrefsSafe.GetInt("dayTrialId" + i);
            float trialTask = PlayerPrefsSafe.GetFloat("dayTrial" + i);
            float trialReward = PlayerPrefsSafe.GetFloat("dayTrialReward" + i);
            float trialCompleted = PlayerPrefsSafe.GetFloat("completedDayTrial" + i);
            int planeId = PlayerPrefsSafe.GetInt("dayTrialPlaneId" + i);
            CreateTrials(_currentDayTrials[i], trialTask, trialCompleted, _dayParent, trialReward, true, planeId);
            CheckCompletedTrial(i, trialTask, trialCompleted, trialReward, true);
        }
    }

    private void ReloadDayTrials()
    {
        int[] trials = RandomTrial(true);
        int[] planesIds = RandomPlane(true);
        for (int i = 0; i < _currentDayTrials.Length; i++)
        {
            float trialTask = 0;
            float trialReward = 5;
            switch (trials[i])
            {
                case 0:
                    trialTask = (int)UnityEngine.Random.Range(_menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420, _menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * 1.05f);
                    break;

                case 1:
                    trialTask = (int)UnityEngine.Random.Range(_menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100), _menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100));
                    break;

                case 2:
                    trialTask = (int)UnityEngine.Random.Range(_menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100) / 50f, _menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100) / 50f * 1.05f);
                    break;

                case 3:
                    trialTask = UnityEngine.Random.Range(360, 481);
                    break;

                case 4:
                    trialTask = UnityEngine.Random.Range(10, 23);
                    break;
            }
            CreateTrials(trials[i], trialTask, 0, _dayParent, trialReward, true, planesIds[i]);
            PlayerPrefsSafe.SetInt("dayTrialId" + i, trials[i]);
            PlayerPrefsSafe.SetFloat("dayTrial" + i, trialTask);
            PlayerPrefsSafe.SetFloat("dayTrialReward" + i, trialReward);
            PlayerPrefsSafe.SetInt("dayTrialPlaneId" + i, planesIds[i]);
            PlayerPrefsSafe.SetFloat("completedDayTrial" + i, 0);
            PlayerPrefsSafe.SetBool("isDayTrialCompleted" + i, false);
        }
        PlayerPrefsSafe.SetBool("isDayTrialsReloaded", false);
        DateTime reloadDayTasksTime = FormatNumsHelper.ChangeTime(DateTime.Now, 0, 0, 0, 0, 0);
        PlayerPrefs.SetString("lastDayTrials", reloadDayTasksTime.ToString());
    }

    private void LoadGeneralTrials()
    {
        for (int i = 0; i < _generalParent.childCount; i++)
        {
            Destroy(_generalParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < _currentGeneralTrials.Length; i++)
        {
            _currentGeneralTrials[i] = PlayerPrefsSafe.GetInt("generalTrialId" + i);
            float trialTask = PlayerPrefsSafe.GetFloat("generalTrialTask" + i);
            float trialReward = PlayerPrefsSafe.GetFloat("generalTrialReward" + i);
            float trialCompleted = PlayerPrefsSafe.GetFloat("completedGeneralTrial" + i);
            int planeId = PlayerPrefsSafe.GetInt("generalTrialPlaneId" + i);
            CreateTrials(_currentGeneralTrials[i], trialTask, trialCompleted, _generalParent, trialReward, false, planeId);
            CheckCompletedTrial(i, trialTask, trialCompleted, trialReward, false);
        }
    }

    private void ReloadGeneralTrials()
    {
        _trialId = DateTime.Now.Month;
        PlayerPrefsSafe.SetInt("trialId", _trialId);
        _coupons = 140;
        PlayerPrefsSafe.SetInt("trialCoupons", _coupons);
        int[] trials = RandomTrial(false);
        int[] planesIds = RandomPlane(false);
        for (int i = 0; i < _currentGeneralTrials.Length; i++)
        {
            float trialTask = 0;
            float trialReward = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) * 20 / 5;
            switch (trials[i])
            {
                case 0:
                    trialTask = (int)UnityEngine.Random.Range(_menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), _menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * 1.05f * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) / 2;
                    break;

                case 1:
                    trialTask = (int)UnityEngine.Random.Range(_menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100) * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), _menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100) * 1.05f * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    break;

                case 2:
                    trialTask = (int)UnityEngine.Random.Range(_menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100) / 50f * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), _menu.GetPlanePrefab(i).GetComponent<Plane>().startSpeed / 3.6f * 420 * _menu.GetPlanePrefab(i).GetComponent<Plane>().maxSpeed / (_menu.GetPlanePrefab(i).GetComponent<Plane>().controllAbility * 100) / 50f * 1.05f * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    break;

                case 3:
                    trialTask = (int)UnityEngine.Random.Range(300 * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 401 * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    break;

                case 4:
                    trialTask = (int)UnityEngine.Random.Range(10 * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23 * (float)DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    break;
            }
            CreateTrials(trials[i], trialTask, 0, _generalParent, trialReward, false, planesIds[i]);
            PlayerPrefsSafe.SetInt("generalTrialId" + i, trials[i]);
            PlayerPrefsSafe.SetFloat("generalTrialTask" + i, trialTask);
            PlayerPrefsSafe.SetFloat("generalTrialReward" + i, trialReward);
            PlayerPrefsSafe.SetInt("generalTrialPlaneId" + i, planesIds[i]);
            PlayerPrefsSafe.SetFloat("completedGeneralTrial" + i, 0);
            PlayerPrefsSafe.SetBool("isGeneralTrialCompleted" + i, false);
        }
        PlayerPrefsSafe.SetBool("isGeneralTrialsReloaded", false);
    }

    private void CheckCompletedTrial(int taskId, float task, float completed, float reward, bool isdayReward)
    {
        if (completed >= task)
        {
            if (isdayReward)
            {
                if (!PlayerPrefsSafe.GetBool("isDayTrialCompleted" + taskId))
                {
                    PlayerPrefsSafe.SetBool("isDayTrialCompleted" + taskId, true);
                    _coupons += (int)reward;
                    PlayerPrefsSafe.SetInt("trialCoupons", _coupons);
                    _playfabManager.SaveData("Data");
                }
            }
            else
            {
                if (!PlayerPrefsSafe.GetBool("isGeneralTrialCompleted" + taskId))
                {
                    PlayerPrefsSafe.SetBool("isGeneralTrialCompleted" + taskId, true);
                    _coupons += (int)reward;
                    PlayerPrefsSafe.SetInt("trialCoupons", _coupons);
                    _playfabManager.SaveData("Data");
                }
            }
        }
    }

    private int[] RandomTrial(bool isDayTasks)
    {
        int[] newTasks;
        if (isDayTasks)
            newTasks = new int[4];
        else
            newTasks = new int[5];
        for (int i = 0; i < newTasks.Length; i++)
        {
            int id = UnityEngine.Random.Range(0, 5);
            newTasks[i] = id;
        }
        return newTasks;
    }

    private int[] RandomPlane(bool isDayTasks)
    {
        int[] newPlanes;
        if (isDayTasks)
            newPlanes = new int[4];
        else
            newPlanes = new int[5];
        for (int i = 0; i < newPlanes.Length; i++)
        {
            bool isready = false;
            while (!isready)
            {
                int expeptionsNumber = 0;
                int num = UnityEngine.Random.Range(0, _playerPlanes.Count);
                int id = _playerPlanes[num];
                for (int n = 0; n < newPlanes.Length; n++)
                {
                    if (id != newPlanes[n])
                    {
                        expeptionsNumber++;
                    }
                }
                if (expeptionsNumber == newPlanes.Length)
                {
                    isready = true;
                    newPlanes[i] = id;
                }
            }
        }
        return newPlanes;
    }

    private void CreateTrials(int id, float task, float completed, Transform parent, float reward, bool isDayTask, int planeId)
    {
        GameObject obj = Instantiate(_trialsPrefab, parent);
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
            texts[0].text = TasksTranslater.DecodeTrials(id, task, _containerLoot.planesNames[planeId]);
            texts[2].text = reward.ToString("0") + " купонов";
        }
        texts[1].text = completed.ToString("0") + "/" + task;
    }

    public void OnDayTrialsButtonClicked()
    {
        Image dayImage = _dayTasksButton.GetComponent<Image>();
        dayImage.color = Color.green;
        Image generalImage = _generalTasksButton.GetComponent<Image>();
        generalImage.color = Color.black;
        _scrollViewDay.SetActive(true);
        _scrollViewGeneral.SetActive(false);
        isDayTrials = true;
    }

    public void OnGeneralTrialsButtonClicked()
    {
        Image generalImage = _generalTasksButton.GetComponent<Image>();
        generalImage.color = Color.green;
        Image dayImage = _dayTasksButton.GetComponent<Image>();
        dayImage.color = Color.black;
        _scrollViewGeneral.SetActive(true);
        _scrollViewDay.SetActive(false);
        isDayTrials = false;
    }

    public void OnReloadTasksButtonClicked()
    {
        if (isDayTrials)
        {
            for (int i = 0; i < _dayParent.childCount; i++)
            {
                Destroy(_dayParent.transform.GetChild(i).gameObject);
            }
            ReloadDayTrials();
            PlayerPrefsSafe.SetBool("isDayTrialsReloaded", true);
            _playfabManager.SaveData("Data");
        }
        _reloadTrialsBg.SetActive(false);
    }

    private float TimeToTheEndOfTrial()
    {
        DateTime startTime = Convert.ToDateTime("01. " + DateTime.Now.Month + ".2023 00:00:00");
        TimeSpan ts = DateTime.Now - startTime;
        float secondsToTheEnd = (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) * 24 * 60 * 60);
        secondsToTheEnd -= (float)ts.TotalSeconds;
        return secondsToTheEnd;
    }
}