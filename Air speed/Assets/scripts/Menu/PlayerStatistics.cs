using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatistics : MonoBehaviour
{
    [SerializeField] private Text topScoresText, topDistanceText, totalGamesText, totalScoresText, totalDistanceText, totalTimeText, totalMoneyText, averageScoresText, averageDistanceText, averageSpeedText, averageTimeText, averageMoneyText, MoneyIn1MinuteText;

    private void Start()
    {
        UpdatePlayerStatistics();
    }

    public void UpdatePlayerStatistics()
    {
        topScoresText.text = "Рекорд: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("record"));
        topDistanceText.text = "Лучшая дальность: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("topDistancePlayer")) + " м";
        totalGamesText.text = "Всего полётов: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        totalScoresText.text = "Всего очков: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalScorePlayer"));
        totalDistanceText.text = "Пробег: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalDistancePlayer")) + " м";
        totalTimeText.text = "Времени в полётах: " + FormatNumsHelper.FormatTime(PlayerPrefsSafe.GetFloat("totalTimePlayer"));
        totalMoneyText.text = "Заработано денег: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalMoneyPlayer") + PlayerPrefsSafe.GetFloat("tasksTotalMoney") + PlayerPrefsSafe.GetFloat("containerTotalMoney"));
        averageScoresText.text = "Очков в среднем: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalScorePlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        averageDistanceText.text = "Дистанция в среднем: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalDistancePlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer")) + " м";
        averageSpeedText.text = "Средняя скорость: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalSpeedPlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer")) + " Км/ч";
        averageTimeText.text = "Среднее время полёта: " + FormatNumsHelper.FormatTime(PlayerPrefsSafe.GetFloat("totalTimePlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        averageMoneyText.text = "Монет в среднем: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalMoneyPlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        MoneyIn1MinuteText.text = "Монет за минуту: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalMoneyPlayer") / PlayerPrefsSafe.GetFloat("totalTimePlayer") * 60);
    }
}
