using System.Collections;
using UnityEngine;

public class StatisticsRecorder : MonoBehaviour
{
    private float _distance = 0;
    private float _time;
    private GameManager _gameManager;
    private TasksRecorder _tasksRecorder;
    private TrialsRecorder _trialsRecorder;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _tasksRecorder = GetComponent<TasksRecorder>();
        _trialsRecorder = GetComponent<TrialsRecorder>();
        StartCoroutine(StatisticsCollector());
    }

    private IEnumerator StatisticsCollector()
    {
        yield return new WaitForSeconds(0.1f);
        float speed = _gameManager.currentSpeed;
        _distance += speed / 36;
        _time += 0.1f;
        StartCoroutine(StatisticsCollector());
    }

    public float GetDistance()
    {
        return _distance;
    }

    public void FinishGame(float score, float speed, int id)
    {
        if (PlayerPrefsSafe.GetBool("isRecordingStatistics"))
        {
            float totalTime = PlayerPrefsSafe.GetFloat("totalTime" + id);
            totalTime += _time;
            PlayerPrefsSafe.SetFloat("totalTime" + id, totalTime);
            totalTime = PlayerPrefsSafe.GetFloat("totalTimePlayer");
            totalTime += _time;
            PlayerPrefsSafe.SetFloat("totalTimePlayer", totalTime);
            float totalScore = PlayerPrefsSafe.GetFloat("totalScore" + id);
            totalScore += score;
            PlayerPrefsSafe.SetFloat("totalScore" + id, totalScore);
            totalScore = PlayerPrefsSafe.GetFloat("totalScorePlayer");
            totalScore += score;
            PlayerPrefsSafe.SetFloat("totalScorePlayer", totalScore);
            float topScore = PlayerPrefsSafe.GetFloat("topScore" + id);
            if (score > topScore)
            {
                PlayerPrefsSafe.SetFloat("topScore" + id, score);
            }
            topScore = PlayerPrefsSafe.GetFloat("topScorePlayer");
            if (score > topScore)
            {
                PlayerPrefsSafe.SetFloat("topScorePlayer", score);
            }
            float totalSpeed = PlayerPrefsSafe.GetFloat("totalSpeed" + id);
            totalSpeed += speed;
            PlayerPrefsSafe.SetFloat("totalSpeed" + id, totalSpeed);
            totalSpeed = PlayerPrefsSafe.GetFloat("totalSpeedPlayer");
            totalSpeed += speed;
            PlayerPrefsSafe.SetFloat("totalSpeedPlayer", totalSpeed);
            float totalGames = PlayerPrefsSafe.GetFloat("totalGames" + id);
            totalGames++;
            PlayerPrefsSafe.SetFloat("totalGames" + id, totalGames);
            totalGames = PlayerPrefsSafe.GetFloat("totalGamesPlayer");
            totalGames++;
            PlayerPrefsSafe.SetFloat("totalGamesPlayer", totalGames);
            float totalDistance = PlayerPrefsSafe.GetFloat("totalDistance" + id);
            totalDistance += _distance;
            PlayerPrefsSafe.SetFloat("totalDistance" + id, totalDistance);
            totalDistance = PlayerPrefsSafe.GetFloat("totalDistancePlayer");
            totalDistance += _distance;
            PlayerPrefsSafe.SetFloat("totalDistancePlayer", totalDistance);
            float topDistance = PlayerPrefsSafe.GetFloat("topDistance" + id);
            if (_distance > topDistance)
            {
                PlayerPrefsSafe.SetFloat("topDistance" + id, _distance);
            }
            topDistance = PlayerPrefsSafe.GetFloat("topDistancePlayer");
            if (_distance > topDistance)
            {
                PlayerPrefsSafe.SetFloat("topDistancePlayer", _distance);
            }
            float money = score / 50;
            float totalMoney = PlayerPrefsSafe.GetFloat("totalMoney" + id);
            totalMoney += money;
            PlayerPrefsSafe.SetFloat("totalMoney" + id, totalMoney);
            totalMoney = PlayerPrefsSafe.GetFloat("totalMoneyPlayer");
            totalMoney += money;
            PlayerPrefsSafe.SetFloat("totalMoneyPlayer", totalMoney);
        }
        _tasksRecorder.FinishGame(_distance, score, _time);
        _trialsRecorder.FinishGame(_distance, score, _time);
    }
}