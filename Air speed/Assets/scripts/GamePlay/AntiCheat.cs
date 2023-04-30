using System.Collections;
using UnityEngine;

public class AntiCheat : MonoBehaviour
{
    private GameManager _gameManager;
    private float _score;
    private float _currentSpeed;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _score = _gameManager.score;
        StartCoroutine(ScoresDefender());
    }

    IEnumerator ScoresDefender()
    {
        yield return new WaitForSeconds(5);
        float currentScore = _gameManager.score;
        if (currentScore >= _score + 20000 ^ currentScore < _score)
        {
            int antiCheatTriggers = PlayerPrefsSafe.GetInt("antiCheatTriggers");
            antiCheatTriggers += 1;
            Debug.Log("anticheat detected you" + " " + antiCheatTriggers);
            PlayerPrefsSafe.SetInt("antiCheatTriggers", antiCheatTriggers);
        }
        _score = currentScore;
        StartCoroutine(ScoresDefender());
    }

}
