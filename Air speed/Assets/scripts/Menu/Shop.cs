using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private string[] _planesNames;
    private const float _dayTime = 86400;
    [SerializeField] private GameObject _acceptBuyingPlaneWindow;
    [SerializeField] private ContainerLoot _containerLoot;
    [SerializeField] private Menu _menu;
    [SerializeField] private MoneyContainer[] _moneyContainer;
    [SerializeField] private Text _timeToUpdateText;
    [SerializeField] private Text _couponsText;
    private PlayfabManager _playfabManager;
    [SerializeField] private List<GameObject> _randomShopPlanesList;

    private void Start()
    {
        _planesNames = _containerLoot.planesNames;
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        CheckTimer();
    }

    public void OnAcceptBuyingButtonClicked() //Узнаём, какой айди у самолёта, что нужен игроку, затем проверяем, хватает ли ему денег. После присваеваем булу нужного айди тру и сохраняем игру
    {
        if (_playfabManager.isLogged)
        {
            int id = 0;
            Text[] texts = _acceptBuyingPlaneWindow.GetComponentsInChildren<Text>();
            for (int i = 0; i < _planesNames.Length; i++)
            {
                if (texts[1].text == _planesNames[i])
                {
                    id = i;
                }
            }
            int cost = int.Parse(texts[2].text);
            if (_menu.Buyplane(cost, id) == 1)
            {
                _playfabManager.SaveData("Data");
                _menu.OnShopEscapeButtonClick();
            }
            OnEscapeAcceptBuyingWindowButtonClicked();
        }
    }

    public void OnEscapeAcceptBuyingWindowButtonClicked() //Закрытие окошка с подтверждением покупки самолёта
    {
        _acceptBuyingPlaneWindow.SetActive(false);
    }

    private void UpdateCouponsText()
    {
        _couponsText.text = PlayerPrefsSafe.GetInt("trialCoupons").ToString();
    }

    private void CheckTimer()
    {
        if (PlayerPrefs.HasKey("lastShopReloading"))
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastShopReloading"));
            float timer = Convert.ToSingle(ts.TotalSeconds);
            if (timer >= _dayTime)
            {
                for (int i = 0; i < _moneyContainer.Length; i++)
                {
                    _moneyContainer[i].ReloadContainer();
                }
                DateTime lastShopReloading = FormatNumsHelper.ChangeTime(DateTime.Now, 0, 0, 0, 0, 0);
                PlayerPrefs.SetString("lastShopReloading", lastShopReloading.ToString());
                foreach (GameObject obj in _randomShopPlanesList)
                {
                    RandomShopPlane _randomShopPlane = obj.GetComponent<RandomShopPlane>();
                    _randomShopPlane.ReloadPlane();
                }
            }
            else
            {
                for (int i = 0; i < _moneyContainer.Length; i++)
                {
                    _moneyContainer[i].CheckLimit();
                }
                if (PlayerPrefsSafe.GetInt("commonRadnomShopPlaneId") == 0)
                {
                    foreach (GameObject obj in _randomShopPlanesList)
                    {
                        RandomShopPlane randomShopPlane = obj.GetComponent<RandomShopPlane>();
                        randomShopPlane.ReloadPlane();
                    }
                }
            }
        }
        else
        {
            PlayerPrefs.SetString("lastShopReloading", DateTime.Now.ToString());
        }
    }

    private void Update()
    {
        if (_playfabManager.isLogged)
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastShopReloading"));
            _timeToUpdateText.text = "До обновления " + FormatNumsHelper.FormatTime(_dayTime - (float)ts.TotalSeconds);
            if (_dayTime - (float)ts.TotalSeconds <= 0)
            {
                CheckTimer();
            }
            UpdateCouponsText();
        }
    }
}