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
        topScoresText.text = "������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("record"));
        topDistanceText.text = "������ ���������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("topDistancePlayer")) + " �";
        totalGamesText.text = "����� ������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        totalScoresText.text = "����� �����: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalScorePlayer"));
        totalDistanceText.text = "������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalDistancePlayer")) + " �";
        totalTimeText.text = "������� � ������: " + FormatNumsHelper.FormatTime(PlayerPrefsSafe.GetFloat("totalTimePlayer"));
        totalMoneyText.text = "���������� �����: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalMoneyPlayer") + PlayerPrefsSafe.GetFloat("tasksTotalMoney") + PlayerPrefsSafe.GetFloat("containerTotalMoney"));
        averageScoresText.text = "����� � �������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalScorePlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        averageDistanceText.text = "��������� � �������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalDistancePlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer")) + " �";
        averageSpeedText.text = "������� ��������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalSpeedPlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer")) + " ��/�";
        averageTimeText.text = "������� ����� �����: " + FormatNumsHelper.FormatTime(PlayerPrefsSafe.GetFloat("totalTimePlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        averageMoneyText.text = "����� � �������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalMoneyPlayer") / PlayerPrefsSafe.GetFloat("totalGamesPlayer"));
        MoneyIn1MinuteText.text = "����� �� ������: " + FormatNumsHelper.FormatNum(PlayerPrefsSafe.GetFloat("totalMoneyPlayer") / PlayerPrefsSafe.GetFloat("totalTimePlayer") * 60);
    }
}
