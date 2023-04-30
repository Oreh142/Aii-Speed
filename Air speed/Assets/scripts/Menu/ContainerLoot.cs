using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerLoot : MonoBehaviour
{
    public string[] planesNames;
    [SerializeField] private GameObject[] _framesPrefabs;
    [SerializeField] private GameObject _resultsPanel;
    [SerializeField] private GameObject _escapeButton;
    [SerializeField] private Transform _animParent;
    [SerializeField] private Transform _resultsParent;
    [SerializeField] private Text _moneyText;
    [SerializeField] private Menu _menu;
    [SerializeField] private PlaneStatistics _planesStatistics;
    [SerializeField] private AudioSource _audio;
    public Sprite[] rarityOfFrame;
    public int[] commonPlanes;
    public int[] rarePlanes;
    public int[] veryRarePlanes;
    public int[] legendaryPlanes;
    private int[] _planesId;
    private List<GameObject> _createPlanes = new();
    private GameObject _moneyObj;
    private List<int> _exceptions = new();
    private int _planeId;
    private float _moneyResult;
    public bool isScrolling;
    private PlayfabManager _playfabManager;

    private void Start()
    {
        _menu = GetComponent<Menu>();
        _planesStatistics = GetComponent<PlaneStatistics>();
    }

    private void CompleteContainerTask(float moneyResult, float price, bool isRecieved)
    {
        int taskId;
        for (int i = 0; i < 4; i++)
        {
            taskId = PlayerPrefsSafe.GetInt("dayTaskId" + i);
            CheckTask(taskId, true, i, moneyResult, price, isRecieved);
        }
        for (int i = 0; i < 4; i++)
        {
            taskId = PlayerPrefsSafe.GetInt("weekTaskId" + i);
            CheckTask(taskId, false, i, moneyResult, price, isRecieved);
        }
    }

    private void CheckTask(int id, bool isDayTask, int num, float moneyResult, float price, bool isRecieved)
    {
        if (id == 5)
        {
            if (isDayTask)
            {
                float completed = PlayerPrefsSafe.GetFloat("completedDayTask" + num);
                completed++;
                PlayerPrefsSafe.SetFloat("completedDayTask" + num, completed);
            }
            else
            {
                float completed = PlayerPrefsSafe.GetFloat("completedWeekTask" + num);
                completed++;
                PlayerPrefsSafe.SetFloat("completedWeekTask" + num, completed);
            }
        }
        if (id == 2 ^ id == 11)
        {
            if (isDayTask)
            {
                float completed = PlayerPrefsSafe.GetFloat("completedDayTask" + num);
                completed += moneyResult;
                PlayerPrefsSafe.SetFloat("completedDayTask" + num, completed);
            }
            else
            {
                float completed = PlayerPrefsSafe.GetFloat("completedWeekTask" + num);
                completed += moneyResult;
                PlayerPrefsSafe.SetFloat("completedWeekTask" + num, completed);
            }
        }
        if (id == 10)
        {
            if (!isRecieved)
            {
                float completed = PlayerPrefsSafe.GetFloat("completedWeekTask" + num);
                completed += price;
                PlayerPrefsSafe.SetFloat("completedWeekTask" + num, completed);
            }
        }
        if (price == 0 & id == -4)
        {
            float completed = PlayerPrefsSafe.GetFloat("completedDayTask" + num);
            completed++;
            PlayerPrefsSafe.SetFloat("completedDayTask" + num, completed);
        }
    }

    public void ScrollContainer(int price, float[] planeChances, float[] moneyChances, bool isRewcieved, int numberOfScrolls)
    {
        float money = PlayerPrefsSafe.GetFloat("money");
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        _playfabManager.CheckDate();
        if ((money >= price | isRewcieved) && _playfabManager.isLogged)
        {
            _audio.mute = true;
            if (_createPlanes.Count > 0)
            {
                foreach (var item in _createPlanes)
                {
                    Destroy(item);
                }
            }
            _createPlanes = new List<GameObject>();
            if (_moneyObj)
            {
                Destroy(_moneyObj);
            }
            if (!isRewcieved)
            {
                money -= price;
            }
            int result = Random.Range(0, 10001);
            float moneyResult;
            moneyResult = MoneyScroll(result, moneyChances, price);
            _moneyResult = moneyResult;
            float containerMoney = PlayerPrefsSafe.GetFloat("containerTotalMoney");
            containerMoney += moneyResult;
            PlayerPrefsSafe.SetFloat("containerTotalMoney", containerMoney);
            money += moneyResult;
            _moneyObj = (Instantiate(_framesPrefabs[0], _animParent));
            Text text = _moneyObj.GetComponentInChildren<Text>();
            text.text = moneyResult.ToString();
            if (PlayerPrefs.GetString("ArsenAnimeshnik") != "animeshnik")
            {
                planeChances[0] = 10000;
                planeChances[1] = 3000;
                planeChances[2] = 1000;
                PlayerPrefs.SetString("ArsenAnimeshnik", "animeshnik");
            }
            result = Random.Range(0, 10001);
            int[] results = PlanesScroll(result, planeChances, numberOfScrolls);
            _planesId = new int[results.Length];
            int compensation = 0;
            if (results.Length > 0 && results[0] != 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    _planesId[i] = results[i];
                    compensation += CheckCompensation(results[i]);
                    CreatePlaneObj(_planesId[i], i);
                    SavePlanes(money, results[i], compensation);
                    _planeId = _planesId[i];
                }
                StartCoroutine(ScrollingCoroutine(true));
            }
            else
            {
                StartCoroutine(ScrollingCoroutine(false));
            }
            SaveResults(money, compensation);
            CompleteContainerTask(moneyResult, price, isRewcieved);
            _exceptions = new();
            _resultsPanel.SetActive(true);
        }
    }

    private int CheckCompensation(int id)
    {
        int compensation = 0;
        foreach (var item in commonPlanes)
        {
            if (item == id)
            {
                compensation = 10000;
            }
        }
        foreach (var item in rarePlanes)
        {
            if (item == id)
            {
                compensation = 50000;
            }
        }
        foreach (var item in veryRarePlanes)
        {
            if (item == id)
            {
                compensation = 100000;
            }
        }
        foreach (var item in legendaryPlanes)
        {
            if (item == id)
            {
                compensation = 200000;
            }
        }
        if (_menu.CheckPlane(id) == 0)
        {
            compensation = 0;
        }
        return compensation;
    }

    public int CheckRarity(int id)
    {
        int rarity = 0;
        foreach (var item in rarePlanes)
        {
            if (item == id)
            {
                rarity = 1;
                break;
            }
        }
        foreach (var item in veryRarePlanes)
        {
            if (item == id)
            {
                rarity = 2;
                break;
            }
        }
        foreach (var item in legendaryPlanes)
        {
            if (item == id)
            {
                rarity = 3;
                break;
            }
        }
        return rarity;
    }

    private void CreatePlaneObj(int id, int i)
    {
        _createPlanes.Add(Instantiate(_framesPrefabs[1], _animParent));
        Image[] images = _createPlanes[i].GetComponentsInChildren<Image>();
        images[1].sprite = _planesStatistics.planesImages[id];
        images[0].sprite = rarityOfFrame[CheckRarity(id)];
        Text planeText = _createPlanes[i].GetComponentInChildren<Text>();
        planeText.text = planesNames[id].ToString();
    }

    private int MoneyScroll(int result, float[] chanses, int price)
    {
        float k = price / 2000 + 1;
        float money = Random.Range(270 * k, 301 * k);
        for (int i = 0; i < chanses.Length; i++)
        {
            if (result < chanses[i])
            {
                money += Random.Range(5 * i * k, 10 * i * k);
            }
        }
        return (int)money;
    }

    private int[] PlanesScroll(int result, float[] chance, int num)
    {
        int planeId = 0;
        float commonChance = chance[0];
        float rareChance = chance[1];
        float veryRareChance = chance[2];
        float legendaryChance = chance[3];
        if (num == 0)
        {
            while (result < commonChance)
            {
                num += 1;
                result = Random.Range(0, 10001);
                if (num >= 3)
                    break;
            }
        }
        int[] results = new int[num];
        for (int i = 0; i < num; i++)
        {
            planeId = PlaneScroll(chance);
            if (planeId != 0)
            {
                results[i] = planeId;
            }
        }
        return results;
    }

    private int PlaneScroll(float[] chance)
    {
        int result = 0;
        int planeId = 0;
        float commonChance = chance[0];
        float rareChance = chance[1];
        float veryRareChance = chance[2];
        float legendaryChance = chance[3];
        result = Random.Range(0, Mathf.RoundToInt(commonChance));
        if (result < legendaryChance)
        {
            result = Random.Range(0, legendaryPlanes.Length);
            for (int j = 0; j < legendaryPlanes.Length; j++)
            {
                if (j == result)
                {
                    planeId = legendaryPlanes[j];
                }
            }
        }
        else if (result <= veryRareChance)
        {
            result = Random.Range(0, veryRarePlanes.Length);
            for (int j = 0; j < veryRarePlanes.Length; j++)
            {
                if (j == result)
                {
                    planeId = veryRarePlanes[j];
                }
            }
        }
        else if (result <= rareChance)
        {
            result = Random.Range(0, rarePlanes.Length);
            for (int j = 0; j < rarePlanes.Length; j++)
            {
                if (j == result)
                {
                    planeId = rarePlanes[j];
                }
            }
        }
        else if (result <= commonChance)
        {
            result = Random.Range(0, commonPlanes.Length);
            for (int j = 0; j < commonPlanes.Length; j++)
            {
                if (j == result)
                {
                    planeId = commonPlanes[j];
                }
            }
        }
        else
        {
            planeId = 0;
        }
        return planeId;
    }

    private IEnumerator ScrollingCoroutine(bool isPlane)
    {
        isScrolling = true;
        if (isPlane)
        {
            yield return new WaitForSeconds(6);
        }
        else
        {
            yield return new WaitForSeconds(4.5f);
        }
        isScrolling = false;
        _escapeButton.SetActive(true);
    }

    public void CloseResultsPanel()
    {
        if (!isScrolling)
        {
            _audio.mute = false;
            _planeId = 0;
            _moneyResult = 0;
            _resultsPanel.SetActive(false);
            _escapeButton.SetActive(false);
        }
    }

    private void SavePlanes(float money, int id, float compensation)
    {
        if (id != 0)
        {
            int result = _menu.GetPlane(id);
            if (result == 0)
            {
                money += compensation;
            }
            else
            {
                PlayerPrefsSafe.SetInt("idPlayerPlane", id);
            }
        }
    }

    private void SaveResults(float money, float compensation)
    {
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        if (_playfabManager.isLogged)
        {
            float containerMoney = PlayerPrefsSafe.GetFloat("containerTotalMoney");
            containerMoney += compensation;
            PlayerPrefsSafe.SetFloat("containerTotalMoney", containerMoney);
            money += compensation;
            PlayerPrefsSafe.SetFloat("money", money);
            _playfabManager.SaveData("Data");
        }
        _moneyText.text = FormatNumsHelper.FormatNum(money);
    }

    public void ShowResults()
    {
        Animator moneyAnim = _moneyObj.GetComponent<Animator>();
        moneyAnim.enabled = false;
        Image[] img = _moneyObj.GetComponentsInChildren<Image>();
        for (int i = 0; i < 2; i++)
        {
            img[i].color = new Color(1, 1, 1, 1);
        }
        Text text = _moneyObj.GetComponentInChildren<Text>();
        text.color = new Color(0, 0, 0, 1);
        if (_createPlanes.Count > 0)
        {
            for (int i = 0; i < _createPlanes.Count; i++)
            {
                Animator planeAnim = _createPlanes[i].GetComponent<Animator>();
                planeAnim.enabled = false;
                Image[] planeImg = _createPlanes[i].GetComponentsInChildren<Image>();
                for (int j = 0; j < planeImg.Length; j++)
                {
                    planeImg[j].color = new Color(1, 1, 1, 1);
                    Debug.Log(j);
                }
                Text planeText = _createPlanes[i].GetComponentInChildren<Text>();
                planeText.color = new Color(0, 0, 0, 1);
            }
        }
        StopAllCoroutines();
        isScrolling = false;
        _escapeButton.SetActive(true);
    }

    public bool CheckMoneyForContainer(float price)
    {
        float money = PlayerPrefsSafe.GetFloat("money");
        if (money >= price)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}