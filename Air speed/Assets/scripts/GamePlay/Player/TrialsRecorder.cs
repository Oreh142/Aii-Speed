using System.Collections;
using UnityEngine;

public class TrialsRecorder : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    private int[] _dayTrials = new int[4];
    private int[] _generalTrials = new int[5];
    private int[] _dayPlanesId = new int[4];
    private int[] _generalPlanesId = new int[5];

    private void Start()
    {
        for (int i = 0; i < _dayTrials.Length; i++)
        {
            _dayTrials[i] = PlayerPrefsSafe.GetInt("dayTrialId" + i);
            _dayPlanesId[i] = PlayerPrefsSafe.GetInt("dayTrialPlaneId" + i);
        }
        for (int i = 0; i < _generalTrials.Length; i++)
        {
            _generalTrials[i] = PlayerPrefsSafe.GetInt("generalTrialId" + i);
            _generalPlanesId[i] = PlayerPrefsSafe.GetInt("generalTrialPlaneId" + i);
        }
    }

    public void FinishGame(float distance, float scores, float time)
    {
        float completed;
        for (int i = 0; i < _dayTrials.Length; i++)
        {
            if (_gameManager.idPlayerPlane == _dayPlanesId[i])
            {
                completed = PlayerPrefsSafe.GetFloat("completedDayTrial" + i);
                switch (_dayTrials[i])
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
                        completed += time;
                        break;

                    case 4:
                        completed++;
                        break;

                }
                PlayerPrefsSafe.SetFloat("completedDayTrial" + i, completed);
            }
        }
        for (int j = 0; j < _generalTrials.Length; j++)
        {
            if (_gameManager.idPlayerPlane == _generalPlanesId[j])
            {
                completed = PlayerPrefsSafe.GetFloat("completedGeneralTrial" + j);
                switch (_generalTrials[j])
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
                        completed += time;
                        break;

                    case 4:
                        completed++;
                        break;

                }
                PlayerPrefsSafe.SetFloat("completedGeneralTrial" + j, completed);
            }
        }
    }
}