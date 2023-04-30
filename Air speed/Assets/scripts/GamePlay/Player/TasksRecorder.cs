using System.Collections;
using UnityEngine;

public class TasksRecorder : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private int[] _dayTasks = new int[4];
    private int[] _weekTasks = new int[4];
    private float _minSpeedDistance;
    private float _maxSpeedDistance;
    private float _minSpeedScores;
    private float _maxSpeedScores;

    private void Start()
    {
        for (int i = 0; i < _dayTasks.Length; i++)
        {
            _dayTasks[i] = PlayerPrefsSafe.GetInt("dayTaskId" + i);
        }
        for (int i = 0; i < _weekTasks.Length; i++)
        {
            _weekTasks[i] = PlayerPrefsSafe.GetInt("weekTaskId" + i);
        }
        StartCoroutine(StatsRecorderCoroutine());
    }

    public void FinishGame(float distance, float scores, float time)
    {
        StopAllCoroutines();
        float completed;
        for (int i = 0; i < _dayTasks.Length; i++)
        {
            completed = PlayerPrefsSafe.GetFloat("completedDayTask" + i);
            switch (_dayTasks[i])
            {
                case 0:
                    completed += distance;
                    break;

                case 1:
                    completed += scores;
                    break;

                case 2:
                    completed += scores / 50;
                    break;

                case 3:
                    completed += _maxSpeedDistance;
                    break;

                case 4:
                    completed += _maxSpeedScores;
                    break;

                case 6:
                    completed += time;
                    break;

                case 7:
                    completed += _minSpeedDistance;
                    break;

                case 8:
                    completed += _minSpeedScores;
                    break;

                case 9:
                    completed++;
                    break;

                case 12:
                    completed += scores / 50;
                    break;
            }
            PlayerPrefsSafe.SetFloat("completedDayTask" + i, completed);
        }
        for (int j = 0; j < _weekTasks.Length; j++)
        {
            completed = PlayerPrefsSafe.GetFloat("completedWeekTask" + j);
            switch (_weekTasks[j])
            {
                case 0:
                    completed += distance;
                    break;

                case 1:
                    completed += scores;
                    break;

                case 2:
                    completed += scores / 50;
                    break;

                case 3:
                    completed += _maxSpeedDistance;
                    break;

                case 4:
                    completed += _maxSpeedScores;
                    break;

                case 6:
                    completed += time;
                    break;

                case 7:
                    completed += _minSpeedDistance;
                    break;

                case 8:
                    completed += _minSpeedScores;
                    break;

                case 9:
                    completed++;
                    break;

                case 12:
                    completed += scores / 50;
                    break;
            }
            PlayerPrefsSafe.SetFloat("completedWeekTask" + j, completed);
        }
    }

    private IEnumerator StatsRecorderCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        float speed = _gameManager.currentSpeed;
        if (speed < 500f)
        {
            _minSpeedDistance += speed / 36;
            _minSpeedScores += _gameManager.ScoresToTasksRecorder();
        }
        else if (speed > 500f)
        {
            _maxSpeedDistance += speed / 36;
            _maxSpeedScores += _gameManager.ScoresToTasksRecorder();
        }
        StartCoroutine(StatsRecorderCoroutine());
    }
}