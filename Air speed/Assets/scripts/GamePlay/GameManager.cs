using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _scoreTable;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] private GameObject[] _badWeatherObjects;
    [SerializeField] private Opponents _opponents;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _sounds;
    [SerializeField] private string[] _planesNames;
    [SerializeField] private Sprite[] _planesSprites;
    [SerializeField] private GameObject _firePrefab;
    private float _record;
    private float _boost;
    private float _multiplier;
    private float _money;
    private float _startSpeed;
    private float _maxSpeed;
    private float _currentMinSpeed;
    public int idPlayerPlane;
    public float score;
    public float currentSpeed;
    public float controllAbility;
    private bool _isRainy;
    private bool _isTheEnd;
    private bool _IsCompletedMaxSpeed;
    private OpponentsSpawner _opponentsSpawner;
    private StatisticsRecorder _statisticsRecorder;
    [SerializeField] private Text scoresText, recordText, resultsText;

    private void Awake()
    {
        LoadGame();
        StartRain();
        _opponentsSpawner = GetComponent<OpponentsSpawner>();
        _statisticsRecorder = GetComponent<StatisticsRecorder>();
    }

    private void Start()
    {
        ChangeSound();
        CheckMusic();
        StartCoroutine(BoostingCoroutine());
        StartCoroutine(ScoresCoroutine());
    }

    private void CheckMusic()
    {
        if (_audio)
        {
            _audio.Play();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(2);
    }

    private void SaveGame()
    {
        PlayerPrefsSafe.SetFloat("money", _money);
        PlayerPrefsSafe.SetFloat("record", _record);
    }

    private void LoadGame() //Загрузка игры, десериализация данных
    {
        _startSpeed = PlayerPrefsSafe.GetFloat("startSpeed");
        currentSpeed = _startSpeed;
        _maxSpeed = PlayerPrefsSafe.GetFloat("maxSpeed");
        controllAbility = PlayerPrefsSafe.GetFloat("controllAbility");
        _boost = PlayerPrefsSafe.GetFloat("boost");
        _multiplier = PlayerPrefsSafe.GetFloat("multiplier");
        idPlayerPlane = PlayerPrefsSafe.GetInt("idPlayerPlane");
        _record = PlayerPrefsSafe.GetFloat("record");
        _money = PlayerPrefsSafe.GetFloat("money");
        _currentMinSpeed = _startSpeed - _startSpeed / 20;
        recordText.text = FormatNumsHelper.FormatNum(_record);
    }

    private void ChangeSound()
    {
        if (_audio)
        {
            if (_audio)
            {
                switch (idPlayerPlane)
                {
                    case 13:
                        _audio.clip = _sounds[1];
                        break;
                    case 19:
                        _audio.clip = _sounds[1];
                        break;
                    case 21:
                        _audio.clip = _sounds[2];
                        break;
                    case 27:
                        _audio.clip = _sounds[1];
                        break;
                }
            }
        }   
    }

    public void CreateFireInPlane(Transform parent, bool isPlayer)
    {
        float zEuler;
        float xPosition = parent.position.x;
        if (isPlayer)
        {
            zEuler = 140;
            xPosition += 0.8f;
        }
        else
        {
            zEuler = -140;
        }
        Instantiate(_firePrefab, new Vector3(xPosition, parent.position.y, -1), Quaternion.Euler(0, 0, zEuler), parent);
    }

    public bool IsTheEnd()
    {
        return _isTheEnd;
    }

    private void StartRain()
    {
        if (Random.Range(0, 10) == 9)
        {
            _isRainy = true;
            _badWeatherObjects[Random.Range(0, _badWeatherObjects.Length)].SetActive(true);
            _boost *=  0.95f;
            _maxSpeed *= 0.95f;
            controllAbility *= 0.95f;
        }
    }

    public bool GetIsRainy()
    {
        return _isRainy;
    }

    private IEnumerator BoostingCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (currentSpeed < _maxSpeed)
        {
            currentSpeed += _boost / 10;
            if (!_IsCompletedMaxSpeed)
            {
                _currentMinSpeed += _boost / 10;
            }
        }
        else
        {
            _IsCompletedMaxSpeed = true;
        }
        StartCoroutine(BoostingCoroutine());
    }

    public void ChangeSpeed(bool isUp)
    {
        if (isUp & currentSpeed > _startSpeed & currentSpeed > _currentMinSpeed)
        {
            currentSpeed -= _boost / 9.5f;
        }
        else if (!isUp & currentSpeed < _maxSpeed)
        {
            currentSpeed += _boost / 9.5f;
        }
    }

    public float ScoresToTasksRecorder()
    {
        float localScore = score;
        float scoreK = (localScore / 60000) + 1;
        localScore = FormatNumsHelper.FormatToUnitySpeed(currentSpeed) * _multiplier * scoreK;
        return localScore;
    }

    private IEnumerator ScoresCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        float scoreK = (score / 60000) + 1;
        score += FormatNumsHelper.FormatToUnitySpeed(currentSpeed) * _multiplier * scoreK;
        scoresText.text = FormatNumsHelper.FormatNum(score) + " | " + currentSpeed.ToString("0.0") + "Км/ч";
        if (score > _record)
        {
            _record = score;
            recordText.text = FormatNumsHelper.FormatNum(_record);
        }
        StartCoroutine(ScoresCoroutine());
    }

    public void ChangeSoundStatus(bool change)
    {
        if (_audio)
        {
            if (change)
            {
                _audio.Play();
            }
            else
            {
                _audio.Pause();
            }
        }
    }

    public void TheEndGame()
    {
        if (_audio)
        {
            _audio.mute = true;
        }
        _pauseButton.SetActive(false);
        scoresText.text = "";
        recordText.text = "";
        _scoreTable.SetActive(true);
        PlaneStatsInScoreTable();
        _isTheEnd = true;
        _money += score / 50;
        resultsText.text = "Очки: " + FormatNumsHelper.FormatNum(score) + "\n\nРасстояние: " + FormatNumsHelper.FormatNum(_statisticsRecorder.GetDistance()) + " м\n\nСкорость: " + currentSpeed.ToString("0.0") + " км/ч\n\nПолучено: " + FormatNumsHelper.FormatNum(score / 50) + " монет\n\nИтого: " + FormatNumsHelper.FormatNum(_money) + " монет";
        SaveGame();
        _statisticsRecorder.StopAllCoroutines();
        _statisticsRecorder.FinishGame(score, currentSpeed, idPlayerPlane);
        StopAllCoroutines();
    }

    private void PlaneStatsInScoreTable()
    {
        Text planeNameText = _scoreTable.GetComponentsInChildren<Text>()[4];
        planeNameText.text = _planesNames[idPlayerPlane];
        Image planeImg = _scoreTable.GetComponentsInChildren<Image>()[9];
        planeImg.sprite = _planesSprites[idPlayerPlane];
        Text planeLeftStatsText = _scoreTable.GetComponentsInChildren<Text>()[5];
        Text planeRigstStatsText = _scoreTable.GetComponentsInChildren<Text>()[6];
        planeLeftStatsText.text = "\n\n" + _startSpeed + "\n\n" + _maxSpeed;
        planeRigstStatsText.text = controllAbility + "\n\n" + _boost.ToString("0.0") + "\n\n" + _multiplier.ToString("0.0");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(1);
    }
}