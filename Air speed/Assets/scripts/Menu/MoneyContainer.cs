using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyContainer : Container
{
    [SerializeField] private float _rechargeTime;
    [SerializeField] private int _dayLimit;
    private int _dayOpened;
    private int _recievedReward;

    private void Start()
    {
        if (_rechargeTime > 0)
        {
            CheckTimer();
        }
        else
        {
            CheckRecievedReward();
        }
        _dayOpened = PlayerPrefsSafe.GetInt("dayOpened" + _id);
        _recievedReward = PlayerPrefsSafe.GetInt("recievedReward" + _id);
        CheckLimit();
    }

    public int GetDayLimit()
    {
        _dayOpened = PlayerPrefsSafe.GetInt("dayOpened" + _id);
        return _dayLimit - _dayOpened;
    }

    private void ChangePriceText(string message)
    {
        Text[] texts = GetComponentsInChildren<Text>();
        texts[2].text = message;
    }

    private void ChangeLimitText(string message)
    {
        Text[] texts = GetComponentsInChildren<Text>();
        texts[1].text = message;
    }

    public void StartContainer(bool isRecieved, int price)
    {
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        if (_recievedReward > 0 & _playfabManager.isLogged & isRecieved)
        {
            _recievedReward--;
            _containerLoot.ScrollContainer(price, _planeChance, _moneyChances, true, 0);
            PlayerPrefsSafe.SetInt("recievedReward" + _id, _recievedReward);
        }
        else if (_dayOpened < _dayLimit & _containerLoot.CheckMoneyForContainer(price) & _playfabManager.isLogged & !isRecieved)
        {
            if (price > 0)
            {
                _containerLoot.ScrollContainer(price, _planeChance, _moneyChances, false, 0);
                _dayOpened++;
                PlayerPrefsSafe.SetInt("dayOpened" + _id, _dayOpened);
            }
            else if (CheckTimer() >= _rechargeTime)
            {
                _containerLoot.ScrollContainer(price, _planeChance, _moneyChances, false, 0);
                PlayerPrefs.SetString("lastOpening", DateTime.Now.ToString());
                //_dayOpened++;
                //PlayerPrefsSafe.SetInt("dayOpened" + _id, _dayOpened);
            }
        }
    }

    public void StartContainer()
    {
        _playfabManager = GameObject.FindGameObjectWithTag("playfab").GetComponent<PlayfabManager>();
        if (CheckTimer() >= _rechargeTime & _playfabManager.isLogged)
        {
            _containerLoot.ScrollContainer(_price, _planeChance, _moneyChances, false, 0);
            PlayerPrefs.SetString("lastOpening", DateTime.Now.ToString());
        }
    }

    public void CreateAcceptContainerbuyingBg()
    {
        _acceptCaseBuying.CreateBuyingWindow(_price, _id);
    }

    private void CheckRecievedReward()
    {
        Image[] images = GetComponentsInChildren<Image>();
        _recievedReward = PlayerPrefsSafe.GetInt("recievedReward" + _id);
        if (_recievedReward > 0)
        {
            images[3].color = new Color(1, 1, 1, 0);
            ChangePriceText(_recievedReward + " бесплатно");
        }
        else
        {
            images[3].color = new Color(1, 1, 1, 1);
            ChangePriceText(_price.ToString());
        }
    }

    private float CheckTimer()
    {
        float timer = _rechargeTime;
        if (PlayerPrefs.HasKey("lastOpening"))
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("lastOpening"));
            timer = Convert.ToSingle(ts.TotalSeconds);
            if (timer < _rechargeTime)
            {
                ChangePriceText(FormatNumsHelper.FormatTime(_rechargeTime - timer));
            }
            else
            {
                ChangePriceText("Бесплатно");
            }
        }
        else
        {
            ChangePriceText("Бесплатно");
        }
        return timer;
    }

    public void ReloadContainer()
    {
        _dayOpened = 0;
        PlayerPrefsSafe.SetInt("dayOpened" + _id, _dayOpened);
        ChangeLimitText(_dayOpened + "/" + _dayLimit);
    }

    public void CheckLimit()
    {
        if (_dayLimit == 0)
            ChangeLimitText("");
        else
            ChangeLimitText(_dayOpened + "/" + _dayLimit);
    }

    private void Update()
    {
        if (_rechargeTime > 0)
        {
            CheckTimer();
        }
        else
        {
            CheckLimit();
            CheckRecievedReward();
        }
    }
}
